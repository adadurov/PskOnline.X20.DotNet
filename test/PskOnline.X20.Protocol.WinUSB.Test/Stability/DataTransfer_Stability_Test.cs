namespace PskOnline.X20.Protocol.WinUSB.Test.Stability
{
  using Microsoft.Extensions.Logging;
  using NUnit.Framework;
  using System;
  using System.Linq;

  [TestFixture]
  public class DataTransfer_Stability_Test
  {
    float SamplingRate => 400.0f;

    private ILogger _logger;
    private IX20Device _device; 

    [SetUp]
    public void Setup()
    {
      _logger = SerilogHelper.CreateLogger(nameof(DataTransfer_Stability_Test));
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
    public void DataTransfer_Ramp_60s()
    {
      DataTransferTestHelper.RunRampTest(_device, TimeSpan.FromSeconds(60), _logger);
    }


    [Test]
    [Explicit]
    public void DataTransfer_SamplingRate_Repeat_2x5s()
    {
      {
        var result = DataTransferTestHelper.RetrievePpgDataForPeriod(_device, TimeSpan.FromSeconds(5), _logger);
        var totalSamples = result.Packages.Sum(p => p.Samples.Length);

        // Checkpoint 3
        Assert.That(
          totalSamples,
          Is.EqualTo(SamplingRate * result.ActualRuntime.TotalSeconds)
          .Within(6)
          .Percent
          );
      }
      System.Threading.Thread.Sleep(500);

      {
        var result = DataTransferTestHelper.RetrievePpgDataForPeriod(_device, TimeSpan.FromSeconds(5), _logger);
        var totalSamples = result.Packages.Sum(p => p.Samples.Length);

        // Checkpoint 3
        Assert.That(
          totalSamples,
          Is.EqualTo(SamplingRate * result.ActualRuntime.TotalSeconds)
          .Within(6)
          .Percent
          );
      }
    }

  }
}
