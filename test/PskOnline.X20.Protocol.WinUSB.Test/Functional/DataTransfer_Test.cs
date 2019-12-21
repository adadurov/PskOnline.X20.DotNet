namespace PskOnline.X20.Protocol.WinUSB.Test.Functional
{
  using Microsoft.Extensions.Logging;
  using NUnit.Framework;
  using PskOnline.X20.Protocol.Internal;
  using Shouldly;
  using System;
  using System.Linq;

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

      var result = DataTransferTestHelper.RetrievePpgDataForPeriod(_device, timeLimit, _logger);

      // Checkpoint
      result.Packages.Count.ShouldBeGreaterThan(0);
    }

    [Test]
    [Explicit]
    public void DataTransfer_Ramp_Smoke_1s()
    {
      var timeLimit = TimeSpan.FromSeconds(1);

      DataTransferTestHelper.RunRampTest(_device, timeLimit, _logger);
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
      var result = DataTransferTestHelper.RetrievePpgDataForPeriod(_device, TimeSpan.FromSeconds(10), _logger);

      var totalSamples = result.Packages.Sum(p => p.Samples.Length);

      // Checkpoint 3
      Assert.That(
        totalSamples,
        Is.EqualTo(SamplingRate * result.ActualRuntime.TotalSeconds)
        .Within(6)
        .Percent
        );
    }

    [Test]
    [Explicit]
    public void DataTransfer_Ramp_10s()
    {
      var time = TimeSpan.FromSeconds(10);

      DataTransferTestHelper.RunRampTest(_device, time, _logger);
    }

    [Test]
    [Explicit]
    public void DataTransfer_SamplingRate_Repeat_2x5s()
    {
      {
        var result = DataTransferTestHelper.RetrievePpgDataForPeriod(_device, TimeSpan.FromSeconds(5), _logger);
        var totalSamples = result.Packages.Sum(p => p.Samples.Length);

        // Checkpoint 3
        Assert.That(
          totalSamples,
          Is.EqualTo(SamplingRate * result.ActualRuntime.TotalSeconds)
          .Within(6)
          .Percent
          );
      }
      System.Threading.Thread.Sleep(500);

      {
        var result = DataTransferTestHelper.RetrievePpgDataForPeriod(_device, TimeSpan.FromSeconds(5), _logger);
        var totalSamples = result.Packages.Sum(p => p.Samples.Length);

        // Checkpoint 3
        Assert.That(
          totalSamples,
          Is.EqualTo(SamplingRate * result.ActualRuntime.TotalSeconds)
          .Within(6)
          .Percent
          );
      }
    }

  }
}
