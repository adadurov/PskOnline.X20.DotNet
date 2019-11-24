namespace PskOnline.X20.Protocol.WinUSB.Test
{
  using Microsoft.Extensions.Logging;
  using NUnit.Framework;
  using PskOnline.X20.Protocol.Internal;
  using Shouldly;
  using System;
  using System.Linq;

  [TestFixture]
  public class CmdUsePpgWaveForm_Test
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
    public void UsePpgWaveForm_Smoke()
    {
      var response = new CmdUsePpgWaveform(_logger).Execute(_device.GetUsbControlPipe());
      response.Succeeded.ShouldBe(true);
    }

  }
}
