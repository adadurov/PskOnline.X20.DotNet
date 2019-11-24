namespace PskOnline.X20.Protocol.WinUSB.Test
{
  using Microsoft.Extensions.Logging;
  using NUnit.Framework;
  using Shouldly;
  using System;
  using System.Collections.Generic;

  [TestFixture]
  public class X20Device_Test
  {
    private ILogger _logger;
    private IX20Device _device;

    [SetUp]
    public void Setup()
    {
      _logger = SerilogHelper.CreateLogger(nameof(CmdGetCapabilitiesDescriptor_Test));
      var fac = SerilogHelper.GetLoggerFactory();
      _device = DeviceHelper.GetFirstSuitableDevice(fac);
    }

    [TearDown]
    public void TearDown()
    {
      try
      {
        _device?.StopMeasurement();
        _device?.Dispose();
      }
      catch
      {
      }
    }

    [Test]
    [Explicit]
    public void GetCapabilities_Smoke()
    {
      var response = _device.GetCapabilities();

      response.ShouldNotBeNull();
      response.FirmwareBuildDate.ShouldNotBeNull();
      response.FirmwareBuildDate.Length.ShouldBeLessThan(100);

      response.RevisionInfo.ShouldNotBeNull();
      response.BytesPerPhysioTransfer.ShouldBeGreaterThan(64);
      response.SamplingRate.ShouldBe(400);
      response.Generation.ShouldBe(0);
      response.BitsPerSample.ShouldBe(18);

      _logger.LogInformation(
        $"Build date                = {response.FirmwareBuildDate}"
        );
      _logger.LogInformation(
        $"Revision info             = {response.RevisionInfo}"
        );
      _logger.LogInformation(
        $"Bytes per physio transfer = {response.BytesPerPhysioTransfer}"
        );
      _logger.LogInformation(
        $"Sampling rate             = {response.SamplingRate}"
        );
      _logger.LogInformation(
        $"Bits per sample           = {response.BitsPerSample}"
        );
    }

    [Test]
    [Explicit]
    public void GetCapabilitiesDescriptor_Long()
    {
      int count = 0;

      while (count < 500)
      {
        count++;
        var response = _device.GetCapabilities();
        response.ShouldNotBeNull();

        _logger.LogInformation($"Capabilities retrieved {count} times.");
      }
    }

    [Test]
    [Explicit]
    public void GetPhysioData_Smoke()
    {
      var usePpgResponse = _device.UsePpgWaveform();
      // Checkpoint 1
      usePpgResponse.ShouldBeTrue();

      var startResponse = _device.StartMeasurement();
      // Checkpoint 2
      startResponse.ShouldBeTrue();

      var package = _device.GetPhysioData();

      package.ShouldNotBeNull();

      if (package != null)
      {
        _logger.LogInformation($"=====>");
        _logger.LogInformation($"{package.PackageNumber}");
        _logger.LogInformation($"{package.Samples.Length}");
        _logger.LogInformation($"{package.RingBufferDataCount}");
        _logger.LogInformation($"{package.RingBufferOverflows}");
      }

      // Checkpoints...
      package.Samples.Length.ShouldBeGreaterThan(0);
    }

    private TimeSpan ElapsedFrom(DateTime startTime)
    {
      return DateTime.Now - startTime;
    }

  }
}
