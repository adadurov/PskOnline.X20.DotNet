namespace PskOnline.X20.Protocol.WinUSB.Test.Functional
{
  using Microsoft.Extensions.Logging;
  using NUnit.Framework;
  using PskOnline.X20.Protocol.Internal;
  using Shouldly;
  using System;
  using System.Collections.Generic;

  [TestFixture]
  public class DataTransfer_Test
  {
    float SamplingRate => 400.0f;

    private ILogger _logger;
    private IX20Device _device;

    [SetUp]
    public void Setup()
    {
      _logger = SerilogHelper.CreateLogger(nameof(DataTransfer_Test));

      var fac = SerilogHelper.GetLoggerFactory();
      _device = DeviceHelper.GetFirstSuitableDevice(fac);
      var usePpgResponse = _device.UsePpgWaveform();
    }

    [TearDown]
    public void TearDown()
    {
      if (_device != null)
      {
        _device.UsePpgWaveform();
        new CmdStop(_logger).Execute(_device.GetUsbControlPipe());
        _device.Dispose();
      }
    }

    [Test]
    [Explicit]
    public void DataTransfer_Smoke_1s()
    {
      var timeLimit = TimeSpan.FromSeconds(1);

      var dataPackages = new List<PhysioDataPackage>();
      var samplesCount = 0;
      var buffer = new byte[512];

      var usePpgResponse = _device.UsePpgWaveform();
      // Checkpoint 1
      usePpgResponse.ShouldBeTrue();

      var startResponse = _device.StartMeasurement();
      // Checkpoint 2
      startResponse.ShouldBeTrue();

      // Create data retrieval thread
      var startTime = DateTime.Now;
      var t = new System.Threading.Thread(() =>
      {
        while (true)
        {
          var package = _device.GetPhysioData();
          dataPackages.Add(package);
          samplesCount += package.Samples.Length;

          _logger.LogInformation($"=====>");
          _logger.LogInformation($"{package.PackageNumber}");
          _logger.LogInformation($"{package.Samples.Length}");
          _logger.LogInformation($"{package.RingBufferDataCount}");
          _logger.LogInformation($"{package.RingBufferOverflows}");
        }
      });

      // Run the data retrieval thread
      t.Start();

      // Retrieve data during timeLimit
      while (ElapsedFrom(startTime) < timeLimit)
      {
        System.Threading.Thread.Sleep(50);
      }

      // stop the data retrieval thread
      t.Abort();

      var actualTime = ElapsedFrom(startTime);

      var stopResponse = _device.StopMeasurement();
      // Checkpoint 3
      stopResponse.ShouldBeTrue();

      _logger.LogInformation($"Ran for {actualTime.TotalSeconds} seconds");
      _logger.LogInformation($"Received {dataPackages.Count} packages");
      _logger.LogInformation($"Received {samplesCount} samples");

      // Checkpoint 4
      samplesCount.ShouldBeGreaterThan(0);
    }

    private TimeSpan ElapsedFrom(DateTime startTime)
    {
      return DateTime.Now - startTime;
    }

    [Test]
    [Explicit]
    public void DataTransfer_Ramp_5s()
    {
      _device.UseRamp();
      var packages = RetrieveDataForPeriod(TimeSpan.FromSeconds(5));
      int? lastSample = null;

      foreach (var package in packages)
      {
        _logger.LogInformation($"=====>");
        foreach (var sample in package.Samples)
        {
          _logger.LogInformation(sample.ToString());
        }
      }

      for(var pi = 0; pi < packages.Count; ++pi)
      {
        var package = packages[pi];
        for (var si = 0; si < package.Samples.Length; ++si)
        {
          var sample = package.Samples[si];
          if (lastSample.HasValue)
          {
            Assert.That(sample, Is.EqualTo(lastSample.Value + 1), 
              $"Package #{pi}, sample #{si} doesn't match the expected ramp value of {lastSample.Value}");
          }
          lastSample = sample;
        }
      }

    }

    [Test]
    [Explicit]
    public void DataTransfer_Ramp_60s()
    {
      _device.UseRamp();
      var packages = RetrieveDataForPeriod(TimeSpan.FromSeconds(60));
      int? lastSample = null;

      foreach (var package in packages)
      {
        _logger.LogInformation($"=====>");
        foreach (var sample in package.Samples)
        {
          _logger.LogInformation(sample.ToString());
        }
      }

      for (var pi = 0; pi < packages.Count; ++pi)
      {
        var package = packages[pi];
        for (var si = 0; si < package.Samples.Length; ++si)
        {
          var sample = package.Samples[si];
          if (lastSample.HasValue)
          {
            Assert.That(sample, Is.EqualTo(lastSample.Value + 1),
              $"Package #{pi}, sample #{si} doesn't match the expected ramp value of {lastSample.Value}");
          }
          lastSample = sample;
        }
      }

    }

    [Test]
    [Explicit]
    public void DataTransfer_SamplingRate_10s()
    {
      RetrieveDataForPeriod(TimeSpan.FromSeconds(10));
    }

    private List<PhysioDataPackage> RetrieveDataForPeriod(TimeSpan timeLimit)
    {
      var dataPackages = new List<PhysioDataPackage>();
      var samplesCount = 0;
      var buffer = new byte[512];

      var startResponse = _device.StartMeasurement();
      // Checkpoint 1
      startResponse.ShouldBeTrue();

      // Create data retrieval thread
      var startTime = DateTime.Now;
      var t = new System.Threading.Thread(() =>
      {
        while (true)
        {
          var package = _device.GetPhysioData();
          dataPackages.Add(package);
          samplesCount += package.Samples.Length;

          if (false)
          {
            _logger.LogInformation($"=====>");
            _logger.LogInformation($"{package.PackageNumber}");
            _logger.LogInformation($"{package.Samples.Length}");
            _logger.LogInformation($"{package.RingBufferDataCount}");
            _logger.LogInformation($"{package.RingBufferOverflows}");
          }
        }
      });

      // Run the data retrieval thread
      t.Start();

      // Retrieve data during timeLimit
      while (ElapsedFrom(startTime) < timeLimit)
      {
        System.Threading.Thread.Sleep(50);
      }

      // stop the data retrieval thread
      t.Abort();

      var actualTime = ElapsedFrom(startTime);

      var stopResponse = _device.StopMeasurement();
      // Checkpoint 2
      stopResponse.ShouldBeTrue();

      _logger.LogInformation($"Ran for {actualTime.TotalSeconds} seconds");
      _logger.LogInformation($"Received {dataPackages.Count} packages");
      _logger.LogInformation($"Received {samplesCount} samples");

      // Checkpoint 3
      Assert.That(
        samplesCount,
        Is.EqualTo((int)(SamplingRate * actualTime.TotalSeconds))
        .Within(6)
        .Percent
        );

      return dataPackages;
    }

    [Test]
    [Explicit]
    public void DataTransfer_SamplingRate_Repeat_2x5s()
    {
      RetrieveDataForPeriod(TimeSpan.FromSeconds(5));
      System.Threading.Thread.Sleep(1000);
      RetrieveDataForPeriod(TimeSpan.FromSeconds(5));
    }

  }
}
