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
    
  }
}
