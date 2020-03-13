namespace PskOnline.X20.Protocol.WinUSB.Test.UAT
{
  using Microsoft.Extensions.Logging;
  using NUnit.Framework;
  using Shouldly;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  [TestFixture]
  public class DataTransfer_UAT_Test
  {
    float SamplingRate => 400.0f;

    private ILogger _logger;

    [SetUp]
    public void Setup()
    {
      _logger = SerilogHelper.CreateLogger(nameof(DataTransfer_UAT_Test));
    }

    [TearDown]
    public void TearDown()
    {
    }

    private DataTransferResult RetrieveDataForPeriod(
      TimeSpan timeLimit, 
      string testName)
    {
      var fac = SerilogHelper.GetLoggerFactory();
      var logger = fac.CreateLogger(testName);

      using (var device = DeviceHelper.GetFirstSuitableDevice(fac))
      {
        var result = DataTransferTestHelper.RetrievePpgDataForPeriod(device, timeLimit, logger);

        var samplesCount = result.Packages.Sum(p => p.Samples.Length);

        _logger.LogInformation($"Time:     {result.ActualRuntime.TotalSeconds} seconds");
        _logger.LogInformation($"Packages: {result.Packages.Count}");
        _logger.LogInformation($"Samples:  {samplesCount}");

        var actualSamplingRate = samplesCount / result.ActualRuntime.TotalSeconds;

        actualSamplingRate.ShouldBe(400, 400 * 0.01);

        // Checkpoints
        return result;
      }
    }

    [Test]
    [Explicit]
    public void DataTransfer_Repeat_20x4min()
    {
      for (var i = 0; i < 20; ++i)
      {
        RetrieveDataForPeriod(
          TimeSpan.FromSeconds(3 * 60 + 55), 
          nameof(DataTransfer_Repeat_10x30sec));
        System.Threading.Thread.Sleep(5000);
      }
    }

    [Test]
    [Explicit]
    public void DataTransfer_Repeat_10x30sec()
    {
      for (var i = 0; i < 10; ++i)
      {
        RetrieveDataForPeriod(
          TimeSpan.FromSeconds(29), 
          nameof(DataTransfer_Repeat_10x30sec));
        System.Threading.Thread.Sleep(1000);
      }
    }

    [Test]
    [Explicit]
    public void DataTransfer_Ramp_10x30sec()
    {
      var timeLimit = TimeSpan.FromSeconds(29);

      var fac = SerilogHelper.GetLoggerFactory();
      using (var device = DeviceHelper.GetFirstSuitableDevice(fac))
      {
        for (var i = 0; i < 10; ++i)
        {
          _logger.LogInformation($"Started iteration {i}");
          DataTransferTestHelper.RunRampTest(device, timeLimit, _logger);
          System.Threading.Thread.Sleep(1000);
        }
      }
    }
  }
}
