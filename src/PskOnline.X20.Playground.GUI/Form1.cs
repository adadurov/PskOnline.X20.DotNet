namespace PskOnline.X20.Playground.GUI
{
  using PskOnline.X20.Protocol.WinUSB;
  using System;
  using System.Linq;
  using System.Windows.Forms;
  using PskOnline.X20.Protocol;
  using System.Threading;
  using System.Collections.Concurrent;
  using System.Diagnostics;
  using System.Collections.Generic;
  using PskOnline.X20.Math;

  public partial class Form1 : Form
  {
    Microsoft.Extensions.Logging.ILogger _logger;

    ConcurrentQueue<PhysioDataPackage> _analysisQueue;
    List<int> _visibleSamples;

    OxyPlot.Series.LineSeries _filteredSeries;
    OxyPlot.Series.LineSeries _unfilteredSeries;

    StdDevStabiizer _signalNormalizer;

    LowPassFilter _lpFilter;

    IX20Device _device;

    double globalCounter = 0;

    double SamplingRate { get; set; } = 400.0;

    int MaxNumberOfSamplesOnScreen => (int)(5 * SamplingRate);

    Thread _thread;


    public Form1()
    {
      _logger = SerilogHelper.CreateLogger(nameof(Program));
      _visibleSamples = new List<int>(2 * MaxNumberOfSamplesOnScreen);
      _signalNormalizer = new StdDevStabiizer(
        SamplingRate,
        new StdDevStabilizerParams { MinGain = 200, MaxGain = 13000, DSN = 3337 });

      _lpFilter = new LowPassFilter();
      InitializeComponent();
    }

    private void Form1_FormClosed(object sender, FormClosedEventArgs e)
    {
      _thread?.Abort();
      _device?.StopMeasurement();
      _device?.Dispose();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      var plotModel = new OxyPlot.PlotModel
      {
        PlotType = OxyPlot.PlotType.XY
      };
      var X = new OxyPlot.Axes.LinearAxis
      {
        Position = OxyPlot.Axes.AxisPosition.Bottom,
        Minimum = 0,
        Maximum = 5.0
      };

      var Y_left = new OxyPlot.Axes.LinearAxis
      {
        Key = "Filtered",
        Position = OxyPlot.Axes.AxisPosition.Left,
        Minimum = -600,
        Maximum = 600
      };

      var Y_right = new OxyPlot.Axes.LinearAxis
      {
        Key = "Unfiltered",
        Position = OxyPlot.Axes.AxisPosition.Right,
        Minimum = -40000,
        Maximum = -20000
      };

      plotModel.Axes.Add(X);
      plotModel.Axes.Add(Y_left);
      plotModel.Axes.Add(Y_right);

      _filteredSeries = new OxyPlot.Series.LineSeries
      {
        XAxisKey = X.Key,
        YAxisKey = Y_left.Key,
        Color = OxyPlot.OxyColor.FromRgb(0, 0, 255)
      };

      _unfilteredSeries = new OxyPlot.Series.LineSeries
      {
        XAxisKey = X.Key,
        YAxisKey = Y_right.Key,
        Color = OxyPlot.OxyColor.FromRgb(255, 0, 0)
      };


      plotModel.Series.Add(_unfilteredSeries);
      plotModel.Series.Add(_filteredSeries);
      plotView1.Model = plotModel;

      _analysisQueue = new ConcurrentQueue<PhysioDataPackage>();

      _thread = new Thread(DataRetrievingThread);
      _thread.Start();
      timer1.Start();
    }

    private void UpdatePlotData()
    {
      // take all new packages from the queue
      var packages = new List<PhysioDataPackage>(5);

      PhysioDataPackage package;
      while (_analysisQueue.TryDequeue(out package))
      {
        for (var i = 0; i < package.Samples.Length; ++i)
        {
          package.Samples[i] = 32767 - package.Samples[i];
        }
        packages.Add(package);
      }

      // find out which samples must be visible
      var visibleSamples = GetVisibleSamplesAndCache(packages).ToArray();

      var unfilteredSamples = visibleSamples.ToArray();
      var filteredSamples = visibleSamples.ToArray();
      _signalNormalizer.NormalizeDataInPlace(filteredSamples);
      _lpFilter.FilterInPlace(filteredSamples);

      var filteredPoints = new OxyPlot.DataPoint[unfilteredSamples.Length];
      var unfilteredPoints = new OxyPlot.DataPoint[unfilteredSamples.Length];
      for (var c = 0; c < unfilteredSamples.Length; ++c)
      {
        var time = c * 1.0 / SamplingRate;

        var value = unfilteredSamples[c];
        var filteredValue = filteredSamples[c];

        unfilteredPoints[c] = new OxyPlot.DataPoint(time, value);
        filteredPoints[c] =   new OxyPlot.DataPoint(time, filteredValue);
      }

      _unfilteredSeries.Points.Clear();
      _unfilteredSeries.Points.AddRange(unfilteredPoints);
      _filteredSeries.Points.Clear();
      _filteredSeries.Points.AddRange(filteredPoints);

      plotView1.InvalidatePlot(true);
    }

    /// <summary>
    /// Put to _inWindowSamples only those samples that must be visible
    /// </summary>
    /// <param name="packages"></param>
    private List<int> GetVisibleSamplesAndCache(List<PhysioDataPackage> packages)
    {
      var invertedNewSamples = packages.SelectMany(p => p.Samples);
      var allSamples = _visibleSamples.Concat(invertedNewSamples).ToArray();

      var samplesToSkip = allSamples.Length - allSamples.Length % MaxNumberOfSamplesOnScreen;

      _visibleSamples.Clear();
      _visibleSamples.AddRange(allSamples.Skip(samplesToSkip));
      return _visibleSamples;
    }

    private void DataRetrievingThread()
    {
      while (true)
      {
        try
        {
          while (true)
          {
            var package = _device?.GetPhysioData();
            if (package != null)
            {
              _analysisQueue.Enqueue(package);
            }
          }
        }
        catch (ThreadAbortException)
        {
          Debug.WriteLine("Data retrieval thread was aborted.");
        }
        catch (Exception ex)
        {
          Debug.WriteLine("Unable to read from device. " + ex.Message);
        }
      }
    }

    private IX20Device GetWinUsbPhysioPipeReader()
    {
      var devices = X20DeviceEnumerator.GetDevices(SerilogHelper.GetLoggerFactory());

      if (devices.Count() == 0)
      {
        return null;
      }

      var dev = devices.First().CreateDevice(SerilogHelper.GetLoggerFactory());

      // send 'get capabilities' command
      var capabilities = dev.GetCapabilities();
      SamplingRate = capabilities.SamplingRate;

      dev.UsePpgWaveform();
//      dev.UseRamp();

      // send 'start' command
      dev.StartMeasurement();

      return dev;
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      if (_device == null)
      {
        _device = GetWinUsbPhysioPipeReader();
      }
      if (_device != null)
      {
        try
        {
          UpdatePlotData();
        }
        catch
        {
        }
      }
    }
  }
}
