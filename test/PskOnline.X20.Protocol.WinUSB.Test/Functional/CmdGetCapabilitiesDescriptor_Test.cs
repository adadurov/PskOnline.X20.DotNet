namespace PskOnline.X20.Protocol.WinUSB.Test.Functional
{
  using Microsoft.Extensions.Logging;
  using NUnit.Framework;
  using PskOnline.X20.Protocol.Internal;
  using Shouldly;

  [TestFixture]
  public class CmdGetCapabilitiesDescriptor_Test
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
      _device?.Dispose();
    }

    [Test]
    [Explicit]
    public void GetCapabilitiesDescriptor_Smoke()
    {
      var response = (CmdGetCapabilitiesDescriptorResponse)
        new CmdGetCapabilitiesDescriptor(_logger).Execute(_device.GetUsbControlPipe());

      response.Succeeded.ShouldBe(true);

      ((int)response.CapabilitiesDescriptor.firmwareBuildDateStringDescriptorIndex).ShouldNotBe(0);
      ((int)response.CapabilitiesDescriptor.revisionInfoStringDescriptorIndex).ShouldNotBe(0);

      _logger.LogInformation(
        $"Build date                      = {response.CapabilitiesDescriptor.firmwareBuildDateStringDescriptorIndex}"
        );
      _logger.LogInformation(
        $"Revision info descriptor index  = {response.CapabilitiesDescriptor.revisionInfoStringDescriptorIndex}"
        );
      _logger.LogInformation(
        $"Bytes per physio transfer       = {response.CapabilitiesDescriptor.bytesPerPhysioTransfer}"
        );
      _logger.LogInformation(
        $"Sampling rate                   = {response.CapabilitiesDescriptor.samplingRate}"
        );
      _logger.LogInformation(
        $"Bits per sample                 = {response.CapabilitiesDescriptor.bitsPerSample}"
        );
    }

    [Test]
    [Explicit]
    public void GetCapabilitiesDescriptor_Should_Return_Generation_0()
    {
      var response = (CmdGetCapabilitiesDescriptorResponse)
        new CmdGetCapabilitiesDescriptor(_logger).Execute(_device.GetUsbControlPipe());
      
      response.Succeeded.ShouldBe(true);

      ((int)response.CapabilitiesDescriptor.generation).ShouldBe(0);
    }


    [Test]
    [Explicit]
    public void GetCapabilitiesDescriptor_Long()
    {
      int count = 0;

      while (count < 1000)
      {
        count++;
        var response = (CmdGetCapabilitiesDescriptorResponse)
        new CmdGetCapabilitiesDescriptor(_logger).Execute(_device.GetUsbControlPipe());
        response.Succeeded.ShouldBe(true);

        _logger.LogInformation($"GET_CAPABILITIES_DESCRIPTOR executed {count} times.");
      }
    }

  }
}
