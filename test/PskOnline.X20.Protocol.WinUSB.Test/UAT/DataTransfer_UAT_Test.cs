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

    private TimeSpan ElapsedFrom(DateTime startTime)
    {
      return DateTime.Now - startTime;
    }

    private DataTransferResult RetrieveDataForPeriod(
      TimeSpan timeLimit, 
      string testName, 
      Action<IX20Device> waveformModeSelector,
      Action<DataTransferResult> resultValidator)
    {
      var fac = SerilogHelper.GetLoggerFactory();
      var logger = fac.CreateLogger(testName);
      using (var device = DeviceHelper.GetFirstSuitableDevice(fac))
      {

        var capabilities = device.GetCapabilities();

        // Checkpoint
        capabilities.SamplingRate.ShouldBe((int)SamplingRate);

        waveformModeSelector(device);
        var result = DataTransferTestHelper.RetrieveDataForPeriod(device, timeLimit, logger);

        var samplesCount = result.Packages.Sum(p => p.Samples.Length);

        _logger.LogInformation($"Time:     {result.ActualRuntime.TotalSeconds} seconds");
        _logger.LogInformation($"Packages: {result.Packages.Count}");
        _logger.LogInformation($"Samples:  {samplesCount}");

        // Checkpoints

        resultValidator?.Invoke(result);

        // Checkpoint
        result.Packages.Count.ShouldBeGreaterThan(0);

        for (var i = 0; i < result.Packages.Count; ++i)
        {
          var p = result.Packages[i];
          _logger.LogInformation(
            $"Package [{i}]. Number:{p.PackageNumber} / remaining FIFO data: {p.RingBufferDataCount} / overflows: {p.RingBufferOverflows}");

          // Checkpoint
          p.RingBufferOverflows.ShouldBe(0);
        }

        // Checkpoint
        // Check for sequential package numbers
        var numbers = result.Packages.Select(p => p.PackageNumber);
        var nextNumbers = numbers.Skip(1);
        var counter = 0;

        nextNumbers.Zip(numbers, (next, current) =>
        {
          var delta = (int)(next - current);
          delta.ShouldBe(1, $"Non-sequential package numbers ({current}, {next}) for #{counter}: delta = {delta}");
          counter++;
          return delta;
        }).ToList();

        // Checkpoint
        Assert.That(
          samplesCount,
          Is.EqualTo((int)(SamplingRate * result.ActualRuntime.TotalSeconds))
          .Within(5)
          .Percent
          );

        return result;
      }
    }

    [Test]
    [Explicit]
    public void DataTransfer_Repeat_10x6min()
    {
      for (var i = 0; i < 10; ++i)
      {
        RetrieveDataForPeriod(
          TimeSpan.FromMinutes(6),
          nameof(DataTransfer_Repeat_10x6min),
          device => device.UsePpgWaveform(),
          null);
        System.Threading.Thread.Sleep(2000);
      }
    }

    [Test]
    [Explicit]
    public void DataTransfer_Repeat_40x4min()
    {
      for (var i = 0; i < 40; ++i)
      {
        RetrieveDataForPeriod(
          TimeSpan.FromSeconds(3 * 60 + 55), 
          nameof(DataTransfer_Repeat_10x30sec),
          device => device.UsePpgWaveform(),
          null);
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
          nameof(DataTransfer_Repeat_10x30sec),
          device => device.UsePpgWaveform(),
          null);
        System.Threading.Thread.Sleep(1000);
      }
    }

    [Test]
    [Explicit]
    public void DataTransfer_Ramp_10x30sec()
    {
      for (var i = 0; i < 10; ++i)
      {
        var rampErrors = new List<string>();
        try
        {
          RetrieveDataForPeriod(
            TimeSpan.FromSeconds(29),
            nameof(DataTransfer_Repeat_10x30sec),
            device => device.UseRamp(),
            (r) =>
            {
              // validate that all samples are sequential

              int? lastValue = null;
              for (var k = 0; k < r.Packages.Count; ++k)
              {
                var p = r.Packages[k];
                for (var j = 0; j < p.Samples.Length; ++j)
                {
                  if( lastValue.HasValue)
                  {
                    var next = p.Samples[j];
                    var delta = next - lastValue;
                    if (delta != 1)
                    {
                      rampErrors.Add(
                        $"Non-sequential samples in the ramp. Package: {k}, sample {j}: {lastValue.Value}, {next}. " +
                        $"FIFO first pointer: {p.Reserved}");
                    }
                  }
                  lastValue = p.Samples[j];
                }
              }
            });
        }

        finally
        {
          foreach (string s in rampErrors)
          {
            _logger.LogError(s);
          }
        }

        System.Threading.Thread.Sleep(1000);
      }
    }


    [Test]
    [Explicit]
    public void DataTransfer_Ramp_10x10s()
    {
      var timeLimit = TimeSpan.FromSeconds(10);

      var fac = SerilogHelper.GetLoggerFactory();

      using (var device = DeviceHelper.GetFirstSuitableDevice(fac))
      {
        for (var i = 0; i < 10; ++i)
        {
          _logger.LogInformation($"Started iteration {i}");
          DataTransferTestHelper.RunRampTest(device, timeLimit, _logger);
          System.Threading.Thread.Sleep(2000);

          //DataTransferTestHelper.RetrievePpgDataForPeriod(device, TimeSpan.FromSeconds(1), _logger);
          //_logger.LogInformation($"Completed iteration {i}");
          //System.Threading.Thread.Sleep(200);
        }
      }
    }

  }
}
