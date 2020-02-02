namespace PskOnline.X20.Protocol.WinUSB.Test.Stability
{
  using Microsoft.Extensions.Logging;
  using NUnit.Framework;
  using System;
  using System.Linq;

  [TestFixture]
  public class Enumeration_Stability_Test
  {
    float SamplingRate => 400.0f;

    private ILogger _logger;

    [SetUp]
    public void Setup()
    {
      _logger = SerilogHelper.CreateLogger(nameof(Enumeration_Stability_Test));
    }

    [TearDown]
    public void TearDown()
    {
    }


    [Test]
    [Explicit]
    public void Init_Retry_1000()
    {
      var fac = SerilogHelper.GetLoggerFactory();

      for( int i = 0; i < 1000; ++i)
      {
        using (var device = DeviceHelper.GetFirstSuitableDevice(fac))
        {
          var serialNumber = device.GetCapabilities().SerialNumber;
          _logger.LogInformation($"Enumerated device {serialNumber}, attempt {i}");
        }
      }
    }

  }
}
