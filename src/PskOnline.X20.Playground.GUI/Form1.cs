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

    OxyPlot.Series.LineSeries _lineSeries;
    StdDevStabiizer _signalNormalizer;

    IX20Device _device;

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
      _lineSeries = new OxyPlot.Series.LineSeries();
      plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
      {
        Position = OxyPlot.Axes.AxisPosition.Bottom,
        Minimum = 0,
        Maximum = 5.0
      });

      plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
      {
        Position = OxyPlot.Axes.AxisPosition.Left,
        Minimum = -600,
        Maximum = 600
      });

      plotModel.Series.Add(_lineSeries);
      plotView1.Model = plotModel;

      _analysisQueue = new ConcurrentQueue<PhysioDataPackage>();

      _thread = new Thread(DataRetrievingThread);
      _thread.Start();
      timer1.Start();
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

    private void UpdatePlotData()
    {
      // take all new packages from the queue
      var packages = new List<PhysioDataPackage>(5);

      PhysioDataPackage package;
      while (_analysisQueue.TryDequeue(out package))
      {
        // Normalize samples using normalizer
        _signalNormalizer.NormalizeDataInPlace(package.Samples);

        packages.Add(package);
      }

      // find out which samples must be visible
      UpdateVisibleSamples(packages);

      var c = 0;
      var points = new OxyPlot.DataPoint[_visibleSamples.Count];
      foreach (var value in _visibleSamples)
      {
        points[c++] = new OxyPlot.DataPoint(c * 1.0 / SamplingRate, value);
      }

      _lineSeries.Points.Clear();
      _lineSeries.Points.AddRange(points);
      plotView1.InvalidatePlot(true);
    }

    /// <summary>
    /// Put to _inWindowSamples only those samples that must be visible
    /// </summary>
    /// <param name="packages"></param>
    private void UpdateVisibleSamples(List<PhysioDataPackage> packages)
    {
      var invertedNewSamples = packages.SelectMany(p => p.Samples).Select(p => -p);
      var allSamples = _visibleSamples.Concat(invertedNewSamples).ToArray();

      var screensToSkip = allSamples.Count() / MaxNumberOfSamplesOnScreen;
      var samplesToSkip = screensToSkip * MaxNumberOfSamplesOnScreen;

      _visibleSamples.Clear();
      _visibleSamples.AddRange(allSamples.Skip(samplesToSkip));
    }
  }
}
