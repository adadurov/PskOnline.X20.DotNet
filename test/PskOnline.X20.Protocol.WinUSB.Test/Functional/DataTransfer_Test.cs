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

      var packages = DataTransferTestHelper.RetrieveDataForPeriod(_device, _logger, timeLimit);

      // Checkpoint
      packages.Count.ShouldBeGreaterThan(0);
    }

    [Test]
    [Explicit]
    public void DataTransfer_Ramp_5s()
    {
      _device.UseRamp();
      var packages = DataTransferTestHelper.RetrieveDataForPeriod(_device, _logger, TimeSpan.FromSeconds(5));
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
    public void DataTransfer_SamplingRate_10s()
    {
      DataTransferTestHelper.RetrieveDataForPeriod(_device, _logger, TimeSpan.FromSeconds(10));
    }

    [Test]
    [Explicit]
    public void DataTransfer_SamplingRate_Repeat_2x5s()
    {
      DataTransferTestHelper.RetrieveDataForPeriod(_device, _logger, TimeSpan.FromSeconds(5));
      System.Threading.Thread.Sleep(1000);
      DataTransferTestHelper.RetrieveDataForPeriod(_device, _logger, TimeSpan.FromSeconds(5));
    }

  }
}
