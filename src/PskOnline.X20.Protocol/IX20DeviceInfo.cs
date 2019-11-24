namespace PskOnline.X20.Protocol
{
  using Microsoft.Extensions.Logging;

  public interface IX20DeviceInfo
  {
    IX20Device CreateDevice(ILoggerFactory loggerFactory);

    /// <summary>
    /// Vendor ID (VID) of the USB device
    /// </summary>
    int VID { get; }

    /// <summary>
    /// Product ID (VID) of the USB device
    /// </summary>
    int PID { get; }

    /// <summary>
    /// Manufacturer of the device, as specified in the INF file (not the device descriptor)
    /// </summary>
    string Manufacturer { get; }

    /// <summary>
    /// Description of the device, as specified in the INF file(not the device descriptor)
    /// </summary>
    string DeviceDescription { get; }

    /// <summary>
    /// Serial number
    /// </summary>
    string SerialNumber { get; }


    /// <summary>
    /// Device pathname (opaque value)
    /// </summary>
    string DevicePath { get; }
  }
}
