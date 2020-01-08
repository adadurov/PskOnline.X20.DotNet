namespace PskOnline.X20.Protocol.WinUSB
{
  using MadWizard.WinUSBNet;
  using Microsoft.Extensions.Logging;

  public class X20DeviceInfo : IX20DeviceInfo
  {
    private readonly USBDeviceInfo _d;

    internal X20DeviceInfo(USBDeviceInfo d, string serialNumber)
    {
      _d = d;
      SerialNumber = serialNumber;
      DeviceDescription = _d.DeviceDescription;
    }

    public int VID => _d.VID;

    public int PID => _d.PID;

    public string Manufacturer => _d.Manufacturer;

    public string DeviceDescription { get; set; }

    public string DevicePath => _d.DevicePath;

    public string SerialNumber { get; }

    public IX20Device CreateDevice(ILoggerFactory loggerFactory)
    {
      return new X20Device(_d, loggerFactory);
    }
  }
}
