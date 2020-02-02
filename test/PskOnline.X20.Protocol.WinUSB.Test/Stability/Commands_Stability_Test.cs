namespace PskOnline.X20.Protocol.WinUSB.Test.Stability
{
  using Microsoft.Extensions.Logging;
  using NUnit.Framework;
  using PskOnline.X20.Protocol.Internal;
  using Shouldly;

  [TestFixture]
  public class Commands_Stability_Test
  {
    private ILogger _logger;
    private IX20Device _device; 

    [SetUp]
    public void Setup()
    {
      _logger = SerilogHelper.CreateLogger(nameof(Commands_Stability_Test));
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
    public void Start_Long()
    {
      int count = 0;

      try
      {
        while (count < 1000)
        {
          count++;
          var response = _device.StartMeasurement();
          response.ShouldBe(true);
        }
      }
      finally
      {
        _logger.LogInformation($"START executed {count} times.");
      }
    }

    [Test]
    [Explicit]
    public void Stop_Long()
    {
      int count = 0;

      while (count < 1000)
      {
        count++;
        var response = new CmdStop(_logger).Execute(_device.GetUsbControlPipe());
        response.Succeeded.ShouldBe(true);

        _logger.LogInformation($"STOP executed {count} times.");
      }
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
