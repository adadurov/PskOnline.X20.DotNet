namespace PskOnline.X20.Protocol.WinUSB.Test.Functional
{
  using Microsoft.Extensions.Logging;
  using NUnit.Framework;
  using Shouldly;
  using System;
  using System.Threading;

  [TestFixture]
  public class X20Device_Test
  {
    private ILogger _logger;
    private IX20Device _device;

    [SetUp]
    public void Setup()
    {
      _logger = SerilogHelper.CreateLogger(nameof(X20Device_Test));
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

        response.FirmwareBuildDate.Length.ShouldBeGreaterThan(5);
        response.RevisionInfo.Length.ShouldBeGreaterThan(2);
        response.SerialNumber.Length.ShouldBeGreaterThan(5);

        _logger.LogInformation($"Capabilities retrieved {count} times.");
      }
    }

    [Test]
    [Explicit]
    public void GetPhysioData_Smoke()
    {
      var package = DataTransferTestHelper.TryRetrieveDataPackage(_device, TimeSpan.FromSeconds(1));
      
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

    [Test]
    [Explicit]
    public void OverflowFlag_Smoke()
    {
      var startResponse = _device.StartMeasurement();
      startResponse.ShouldBeTrue();

      // the pause before overflow occurs
      // depends on the size of the buffer in the firmware
      Thread.Sleep(5000);

      var package1 = _device.GetPhysioData();
      var package2 = _device.GetPhysioData();

      // checkpoint...
      package1.ShouldNotBeNull();
      package2.ShouldNotBeNull();

      DumpPackageHeader(package1);
      DumpPackageHeader(package2);

      // checkpoints...
      Assert.That(
        package1.RingBufferOverflows > 0 ||
        package2.RingBufferOverflows > 0);
    }

    private void DumpPackageHeader(PhysioDataPackage package)
    {
      _logger.LogInformation($"=====>");
      _logger.LogInformation($"Number:      {package.PackageNumber}");
      _logger.LogInformation($"Samples:     {package.Samples.Length}");
      _logger.LogInformation($"Ring buffer: {package.RingBufferDataCount}");
      _logger.LogInformation($"Overflows:   {package.RingBufferOverflows}");
    }
  }
}
