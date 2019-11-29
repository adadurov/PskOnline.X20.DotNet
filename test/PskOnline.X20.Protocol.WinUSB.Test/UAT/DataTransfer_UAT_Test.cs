namespace PskOnline.X20.Protocol.WinUSB.Test.UAT
{
  using Microsoft.Extensions.Logging;
  using NUnit.Framework;
  using Shouldly;
  using System;
  using System.Collections.Generic;

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

    private TimeSpan ElapsedFrom(DateTime startTime)
    {
      return DateTime.Now - startTime;
    }

    private void RetrieveDataForPeriod(TimeSpan timeLimit)
    {
      var fac = SerilogHelper.GetLoggerFactory();
      using (var device = DeviceHelper.GetFirstSuitableDevice(fac))
      {

        var capabilities = device.GetCapabilities();

        // Checkpoint 0
        capabilities.SamplingRate.ShouldBe((int)SamplingRate);

        var dataPackages = new List<PhysioDataPackage>();
        var samplesCount = 0;
        var buffer = new byte[512];

        var usePpgResponse = device.UsePpgWaveform();
        // Checkpoint 1
        usePpgResponse.ShouldBeTrue();

        var startResponse = device.StartMeasurement();
        // Checkpoint 2
        startResponse.ShouldBeTrue();

        // Create data retrieval thread
        var startTime = DateTime.Now;
        var t = new System.Threading.Thread(() =>
        {
          while (true)
          {
            var package = device.GetPhysioData();
            dataPackages.Add(package);
            samplesCount += package.Samples.Length;
          }
        });

        // Run the data retrieval thread
        t.Start();

        // Retrieve data during timeLimit
        while (ElapsedFrom(startTime) < timeLimit)
        {
          System.Threading.Thread.Sleep(50);
        }

        // stop the data retrieval thread
        t.Abort();
        device.StopMeasurement();

        var actualTime = ElapsedFrom(startTime);

        var stopResponse = device.StopMeasurement();
        // Checkpoint 3
        stopResponse.ShouldBeTrue();

        _logger.LogInformation($"Ran for {actualTime.TotalSeconds} seconds");
        _logger.LogInformation($"Received {dataPackages.Count} packages");
        _logger.LogInformation($"Received {samplesCount} samples");

        // Checkpoint 4
        Assert.That(
          samplesCount,
          Is.EqualTo((int)(SamplingRate * actualTime.TotalSeconds))
          .Within(5)
          .Percent
          );
      }
    }

    [Test]
    [Explicit]
    public void DataTransfer_Repeat_10x6min()
    {
      for (var i = 0; i < 10; ++i)
      {
        RetrieveDataForPeriod(TimeSpan.FromMinutes(6));
        System.Threading.Thread.Sleep(2000);
      }
    }

    [Test]
    [Explicit]
    public void DataTransfer_Repeat_40x4min()
    {
      for (var i = 0; i < 40; ++i)
      {
        RetrieveDataForPeriod(TimeSpan.FromMinutes(3.5));
        System.Threading.Thread.Sleep(30000);
      }
    }

    [Test]
    [Explicit]
    public void DataTransfer_Repeat_10x30sec()
    {
      for (var i = 0; i < 10; ++i)
      {
        RetrieveDataForPeriod(TimeSpan.FromSeconds(30));
        System.Threading.Thread.Sleep(1000);
      }
    }

  }
}
