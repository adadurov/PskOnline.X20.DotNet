namespace PskOnline.X20.Protocol.WinUSB.Test
{
  using Microsoft.Extensions.Logging;
  using System;
  using System.Linq;

  public static class DeviceHelper
  {
    public static IX20Device GetFirstSuitableDevice(ILoggerFactory loggerFactory)
    {
      var deviceInfo = X20DeviceEnumerator.GetDevices(loggerFactory).FirstOrDefault();
      if (deviceInfo == null)
      {
        throw new Exception("Could not find a PSK-X20 device.");
      }
      loggerFactory.CreateLogger(nameof(DeviceHelper))
        .LogInformation($"Using device with serial number '{deviceInfo.SerialNumber}'");
      return deviceInfo.CreateDevice(loggerFactory); ;
    }
  }
}
