namespace PskOnline.X20.Protocol.WinUSB.Test
{
  using Microsoft.Extensions.Logging;
  using NUnit.Framework;
  using Shouldly;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;

  public class DataTransferResult
  {
    public IList<PhysioDataPackage> Packages { get; set; }

    public TimeSpan ActualRuntime { get; set; }
  }

  public static class DataTransferTestHelper
  {
    public static PhysioDataPackage TryRetrieveDataPackage(IX20Device _device, TimeSpan timeout)
    {
      PhysioDataPackage dataPackage = null;

      var startResponse = _device.StartMeasurement();
      // Checkpoint 1
      startResponse.ShouldBeTrue();

      var startTime = DateTime.Now;
      // Retrieve data during timeLimit
      while ((dataPackage == null) && ElapsedFrom(startTime) < timeout)
      {
        dataPackage = _device.GetPhysioData(200);
        if (dataPackage != null)
        {
          break;
        }
      }

      return dataPackage;
    }

    /// <summary>
    /// </summary>
    /// <param name="device"></param>
    /// <param name="_logger"></param>
    /// <param name="timeLimit"></param>
    /// <returns></returns>
    public static DataTransferResult RetrievePpgDataForPeriod(IX20Device device, TimeSpan timeLimit)
    {
      device.UsePpgWaveform();

      return RetrieveDataForPeriod(device, timeLimit);
    }

    public static DataTransferResult RetrieveDataForPeriod(IX20Device device, TimeSpan timeLimit)
    {
      var SamplingRate = device.GetCapabilities().SamplingRate;
      var dataPackages = new List<PhysioDataPackage>();

      var startResponse = device.StartMeasurement();
      // Checkpoint 1
      startResponse.ShouldBeTrue();

      var startTime = DateTime.Now;

      // Retrieve data during timeLimit
      while (ElapsedFrom(startTime) < timeLimit)
      {
        var package = device.GetPhysioData(200);
        if (package != null)
        {
          dataPackages.Add(package);
        }
      }

      var actualTime = ElapsedFrom(startTime);

      var stopResponse = device.StopMeasurement();
      // Checkpoint 2
      stopResponse.ShouldBeTrue();

      return new DataTransferResult
      {
        Packages = dataPackages,
        ActualRuntime = actualTime
      };
    }

    public static void RunRampTest(IX20Device device, TimeSpan timeLimit, ILogger logger)
    {
      // stop possibly continued transfer and skip a package 
      // that might have been queued for transmission before,
      // but doesn't contain the ramp data
      device.StopMeasurement();
      RetrieveDataForPeriod(device, TimeSpan.FromMilliseconds(500));

      device.UseRamp();
      
      var result = RetrieveDataForPeriod(device, timeLimit);
      
      var totalSamples = result.Packages.Sum(p => p.Samples.Length);

      var SamplingRate = device.GetCapabilities().SamplingRate;
      // Checkpoint 3
      Assert.That(
        totalSamples,
        Is.EqualTo(SamplingRate * result.ActualRuntime.TotalSeconds)
        .Within(6)
        .Percent
        );

      int? lastSample = null;

      for (var pi = 0; pi < result.Packages.Count; ++pi)
      {
        logger.LogInformation($">>>>>>");
        var package = result.Packages[pi];
        for (var si = 0; si < package.Samples.Length; ++si)
        {
          var sample = package.Samples[si];
          logger.LogInformation(sample.ToString());
        }
      }
      logger.LogInformation($"<<<<<<");


      for (var pi = 0; pi < result.Packages.Count; ++pi)
      {
        var package = result.Packages[pi];
        for (var si = 0; si < package.Samples.Length; ++si)
        {
          var sample = package.Samples[si];
          if (lastSample.HasValue)
          {
            var expected = lastSample.Value + 1;
            Assert.That(sample, Is.EqualTo(expected),
              $"Package #{pi}, sample #{si} doesn't match the expected ramp value of {expected}");
          }
          lastSample = sample;
        }
      }
    }



    private static TimeSpan ElapsedFrom(DateTime startTime)
    {
      return DateTime.Now - startTime;
    }

  }
}
