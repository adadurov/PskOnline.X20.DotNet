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

  delegate void ClearDeviceInfoDelegate();

  public partial class Form1 : Form
  {
    ConcurrentQueue<PhysioDataPackage> _analysisQueue;
    List<int> _visibleFilteredSamples;
    List<int> _visibleUnfilteredSamples;
    List<int> _newUnfilteredSamplesBuffer;
    List<int> _newFilteredSamplesBuffer;

    OxyPlot.Series.LineSeries _filteredSeries;
    OxyPlot.Series.LineSeries _unfilteredSeries;

    OxyPlot.Axes.Axis _filteredAxis;
    OxyPlot.Axes.Axis _unfilteredAxis;

    StdDevStabilizer _signalNormalizer;

    LowPassFilter _lpFilter;

    IX20Device _device;
    FileWriter _fileWriter;

    double SamplingRate { get; set; } = 400.0;

    int MaxNumberOfSamplesOnScreen => (int)(5 * SamplingRate);

    Thread _thread;

    public Form1()
    {
      _visibleUnfilteredSamples = new List<int>(2 * MaxNumberOfSamplesOnScreen);
      _visibleFilteredSamples = new List<int>(2 * MaxNumberOfSamplesOnScreen);
      _newUnfilteredSamplesBuffer = new List<int>(2 * MaxNumberOfSamplesOnScreen);
      _newFilteredSamplesBuffer = new List<int>(2 * MaxNumberOfSamplesOnScreen);

      _signalNormalizer = new StdDevStabilizer(
        SamplingRate,
        new StdDevStabilizerParams { MinGain = 200, MaxGain = 13000, DSN = 3337 });

      _lpFilter = new LowPassFilter();
      InitializeComponent();

      ClearDeviceDetails();
    }

    private void Form1_FormClosed(object sender, FormClosedEventArgs e)
    {
      _thread?.Abort();
      _device?.StopMeasurement();
      _device?.Dispose();
      _fileWriter?.Dispose();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      var plotModel = new OxyPlot.PlotModel
      {
        PlotType = OxyPlot.PlotType.XY,
      };
      var X = new OxyPlot.Axes.LinearAxis
      {
        Position = OxyPlot.Axes.AxisPosition.Bottom,
        Minimum = 0,
        Maximum = 5.0
      };

      _filteredAxis = new OxyPlot.Axes.LinearAxis
      {
        Key = "Filtered",
        Position = OxyPlot.Axes.AxisPosition.Left,
        Minimum = -600,
        Maximum = 600
      };

      _unfilteredAxis = new OxyPlot.Axes.LinearAxis
      {
        Key = "Unfiltered",
        Position = OxyPlot.Axes.AxisPosition.Right,
        Minimum = -40000,
        Maximum = -20000
      };

      plotModel.Axes.Add(X);
      plotModel.Axes.Add(_filteredAxis);
      plotModel.Axes.Add(_unfilteredAxis);

      _filteredSeries = new OxyPlot.Series.LineSeries
      {
        XAxisKey = X.Key,
        YAxisKey = _filteredAxis.Key,
        Color = OxyPlot.OxyColor.FromRgb(0, 0, 255)
      };

      _unfilteredSeries = new OxyPlot.Series.LineSeries
      {
        XAxisKey = X.Key,
        YAxisKey = _unfilteredAxis.Key,
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
      _newUnfilteredSamplesBuffer.Clear();
      _newFilteredSamplesBuffer.Clear();
      PhysioDataPackage package;
      while (_analysisQueue.TryDequeue(out package))
      {
        for (var i = 0; i < package.Samples.Length; ++i)
        {
          package.Samples[i] = (1 << 17) - package.Samples[i];
        }
        _newUnfilteredSamplesBuffer.AddRange(package.Samples);
        _newFilteredSamplesBuffer.AddRange(package.Samples);
      }

      _signalNormalizer.NormalizeDataInPlace(_newFilteredSamplesBuffer);
      _lpFilter.FilterInPlace(_newFilteredSamplesBuffer);

      WriteSamples(_fileWriter, _newFilteredSamplesBuffer, _newUnfilteredSamplesBuffer);

      // find out which samples must be visible
      var visibleUnfilteredSamples = GetVisibleSamplesAndCache(_newUnfilteredSamplesBuffer, _visibleUnfilteredSamples);

      var visibleFilteredSamples = GetVisibleSamplesAndCache(_newFilteredSamplesBuffer, _visibleFilteredSamples);

      var count = visibleUnfilteredSamples.Count;

      var filteredPoints = new OxyPlot.DataPoint[count];
      var unfilteredPoints = new OxyPlot.DataPoint[count];
      for (var c = 0; c < count; ++c)
      {
        var time = c * 1.0 / SamplingRate;

        var value = visibleUnfilteredSamples[c];
        var filteredValue = visibleFilteredSamples[c];

        unfilteredPoints[c] = new OxyPlot.DataPoint(time, value);
        filteredPoints[c] = new OxyPlot.DataPoint(time, filteredValue);
      }

      _unfilteredSeries.Points.Clear();
      _unfilteredSeries.Points.AddRange(unfilteredPoints);
      _filteredSeries.Points.Clear();
      _filteredSeries.Points.AddRange(filteredPoints);

      plotView1.InvalidatePlot(true);
    }

    private void WriteSamples(
      FileWriter fileWriter,
      List<int> newFilteredSamples, 
      List<int> newUnfilteredSamples)
    {
      if (checkBoxDoRecord.Checked)
      {
        for (var i = 0; i < newFilteredSamples.Count; ++i)
        {
          fileWriter?.WriteSample(newUnfilteredSamples[i], newFilteredSamples[i]);
        }
      }
    }

    /// <summary>
    /// Put to _inWindowSamples only those samples that must be visible
    /// </summary>
    /// <param name="packages"></param>
    private List<int> GetVisibleSamplesAndCache(List<int> newSamples, List<int> buffer)
    {
      var allSamples = buffer.Concat(newSamples).ToArray();

      var samplesToSkip = allSamples.Length - allSamples.Length % MaxNumberOfSamplesOnScreen;

      buffer.Clear();
      buffer.AddRange(allSamples.Skip(samplesToSkip));
      return buffer;
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
          DisposeDeviceAndWaitForNewConnection();
        }
      }
    }

    private void DisposeDeviceAndWaitForNewConnection()
    {
      try
      {
        _device?.Dispose();
        _device = null;
      }
      catch
      {
      }
      finally
      {
        _device = null;

        ClearDeviceInfoDelegate d = () => ClearDeviceDetails();
        Invoke(d);
      }
    }

    private void ClearDeviceDetails()
    {
      labelBuildDate.Text = "";
      labelSerialNumber.Text = "";
      labelRevision.Text = "";
    }

    private void DisplayDeviceDetails()
    {
      var caps = _device.GetCapabilities();
      labelBuildDate.Text = caps.FirmwareBuildDate;
      labelSerialNumber.Text = caps.SerialNumber;
      labelRevision.Text = caps.RevisionInfo;
    }

    private IX20Device GetConnectedDevice()
    {
      var devices = X20DeviceEnumerator.GetDevices(SerilogHelper.GetLoggerFactory());

      if (devices.Count() == 0)
      {
        return null;
      }

      try
      {
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
      catch (Exception ex)
      {
        Debug.WriteLine("Warning: " + ex.Message);
      }
      return null;
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      if (_device == null)
      {
        _device = GetConnectedDevice();
        if (_device != null)
        {
          DisplayDeviceDetails();
        }
      }
      if (_device != null)
      {
        try
        {
          UpdatePlotData();
        }
        catch (Exception ex)
        {
          Debug.WriteLine(ex.Message);
        }
      }
    }

    private void useRamp_CheckedChanged(object sender, EventArgs e)
    {
      if (useRamp.Checked)
      {
        _device?.UseRamp();
      }
      else
      {
        _device?.UsePpgWaveform();
      }
    }

    private void findPlotsButton_Click(object sender, EventArgs e)
    {
      if (_visibleFilteredSamples.Count > 0)
      {
        var minFiltered = _visibleFilteredSamples.Min();
        var maxFiltered = _visibleFilteredSamples.Max();
        var range = maxFiltered - minFiltered;
        _filteredAxis.Zoom(
          minFiltered - range,
          maxFiltered + range);
      }

      if (_visibleUnfilteredSamples.Count > 0)
      {
        var minUnfiltered = _visibleUnfilteredSamples.Min();
        var maxUnfiltered = _visibleUnfilteredSamples.Max();
        var range = maxUnfiltered - minUnfiltered;
        _unfilteredAxis.Zoom(
          minUnfiltered - range,
          maxUnfiltered + range);
      }
    }

    private void checkBoxDoRecord_CheckedChanged(object sender, EventArgs e)
    {
      if (checkBoxDoRecord.Checked)
      {
        _fileWriter?.Dispose();
        _fileWriter = CreateFileWriter();
      }
      else
      {
        _fileWriter?.Dispose();
        _fileWriter = null;
      }
    }

    private FileWriter CreateFileWriter()
    {
      string timePart = DateTime.Now.ToString("yyyyMMdd_HHmm");
      return new FileWriter
        ("psk_x20" + textBoxFileNamePrefix.Text + "_" + timePart + ".csv",
        _device?.GetCapabilities().RevisionInfo,
        _device?.GetCapabilities().SamplingRate.ToString());
    }
  }
}
