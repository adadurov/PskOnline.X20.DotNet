﻿namespace PskOnline.X20.Protocol.WinUSB.Test.Functional
{
  using Microsoft.Extensions.Logging;
  using NUnit.Framework;
  using PskOnline.X20.Protocol.Internal;
  using Shouldly;

  [TestFixture]
  public class CmdStart_Test
  {
    private ILogger _logger;
    private IX20Device _device;

    [SetUp]
    public void Setup()
    {
      _logger = SerilogHelper.CreateLogger(nameof(CmdStart_Test));

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
    public void Start_Smoke()
    {
      var response = new CmdStart(_logger).Execute(_device.GetUsbControlPipe());
      response.Succeeded.ShouldBe(true);
    }

  }
}
