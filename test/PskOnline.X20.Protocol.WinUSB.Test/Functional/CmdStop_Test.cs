namespace PskOnline.X20.Protocol.WinUSB.Test.Functional
{
  using Microsoft.Extensions.Logging;
  using NUnit.Framework;
  using PskOnline.X20.Protocol.Internal;
  using Shouldly;

  [TestFixture]
  public class CmdStop_Test
  {
    private ILogger _logger;
    private IX20Device _device;

    [SetUp]
    public void Setup()
    {
      _logger = SerilogHelper.CreateLogger(nameof(CmdStop_Test));

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
    public void Stop_Smoke()
    {
      var response = new CmdStop(_logger).Execute(_device.GetUsbControlPipe());
      response.Succeeded.ShouldBe(true);
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

  }
}
