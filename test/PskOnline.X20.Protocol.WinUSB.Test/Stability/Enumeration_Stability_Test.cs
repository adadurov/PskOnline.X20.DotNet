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
      var fwRevision = "";
      var fwBuildDate = "";

      for ( int i = 0; i < 1000; ++i)
      {
        try
        {
          using (var device = DeviceHelper.GetFirstSuitableDevice(fac))
          {
            var caps = device.GetCapabilities();
            var serialNumber = caps.SerialNumber;
            
            if (string.IsNullOrEmpty(fwRevision))
            {
              fwRevision = caps.RevisionInfo;
            }
            if (string.IsNullOrEmpty(fwBuildDate))
            {
              fwBuildDate = caps.FirmwareBuildDate;
            }
          }
        }
        catch
        {
          _logger.LogInformation($"Failed after {i} successful attempts");
          _logger.LogInformation($"Last FW revision was:   {fwRevision}");
          _logger.LogInformation($"Last FW build date was: {fwBuildDate}");
          throw;
        }
      }
    }

  }
}
