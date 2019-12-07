namespace PskOnline.X20.Protocol.WinUSB.Test
{
  using Microsoft.Extensions.Logging;
  using NUnit.Framework;
  using Shouldly;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public static class DataTransferTestHelper
  {
    public static PhysioDataPackage RetrieveDataOnceOrFail(IX20Device _device, ILogger _logger, TimeSpan timeout)
    {
      var dataPackages = new List<PhysioDataPackage>();

      var startResponse = _device.StartMeasurement();
      // Checkpoint 1
      startResponse.ShouldBeTrue();

      // Create data retrieval thread
      var startTime = DateTime.Now;
      bool completed = false;
      var t = new System.Threading.Thread(() =>
      {
        var package = _device.GetPhysioData();
      });

      // Run the data retrieval thread
      t.Start();

      // Retrieve data during timeLimit
      while (!completed && ElapsedFrom(startTime) < timeout)
      {
        System.Threading.Thread.Sleep(50);
      }

      // stop the data retrieval thread
      t.Abort();


      return dataPackages.FirstOrDefault();
    }

    public static List<PhysioDataPackage> RetrieveDataForPeriod(IX20Device _device, ILogger _logger, TimeSpan timeLimit)
    {
      var SamplingRate = _device.GetCapabilities().SamplingRate;
      var dataPackages = new List<PhysioDataPackage>();
      var samplesCount = 0;

      var startResponse = _device.StartMeasurement();
      // Checkpoint 1
      startResponse.ShouldBeTrue();

      // Create data retrieval thread
      var startTime = DateTime.Now;
      var t = new System.Threading.Thread(() =>
      {
        while (true)
        {
          var package = _device.GetPhysioData();
          dataPackages.Add(package);
          samplesCount += package.Samples.Length;

          if (false)
          {
            _logger.LogInformation($"=====>");
            _logger.LogInformation($"{package.PackageNumber}");
            _logger.LogInformation($"{package.Samples.Length}");
            _logger.LogInformation($"{package.RingBufferDataCount}");
            _logger.LogInformation($"{package.RingBufferOverflows}");
          }
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

      var actualTime = ElapsedFrom(startTime);

      var stopResponse = _device.StopMeasurement();
      // Checkpoint 2
      stopResponse.ShouldBeTrue();

      _logger.LogInformation($"Ran for {actualTime.TotalSeconds} seconds");
      _logger.LogInformation($"Received {dataPackages.Count} packages");
      _logger.LogInformation($"Received {samplesCount} samples");

      // Checkpoint 3
      Assert.That(
        samplesCount,
        Is.EqualTo((int)(SamplingRate * actualTime.TotalSeconds))
        .Within(6)
        .Percent
        );

      return dataPackages;
    }

    private static TimeSpan ElapsedFrom(DateTime startTime)
    {
      return DateTime.Now - startTime;
    }

  }
}
