namespace PskOnline.X20.Protocol.WinUSB
{
  using Microsoft.Extensions.Logging;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public static class X20DeviceEnumerator
  {
    private static readonly Guid InterfaceGuid = Guid.Parse("{CC745879-91C7-4E8B-8F66-0FA69748909B}");

    public static IEnumerable<IX20DeviceInfo> GetDevices(ILoggerFactory loggerFactory)
    {
      if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));

      var logger = loggerFactory.CreateLogger(nameof(X20DeviceEnumerator));
      var devices = MadWizard.WinUSBNet.USBDevice.GetDevices(InterfaceGuid);

      return devices.Select(d => {
        var serialNumber = "";
        try
        {
          using( var dev = new MadWizard.WinUSBNet.USBDevice(d))
          {
            serialNumber = dev.Descriptor.SerialNumber;
          }
        }
        catch(Exception ex)
        {
          logger.LogError("Cannot read device serial number", ex);
        }
        return new X20DeviceInfo(d, serialNumber);
      });
    }
  }
}
