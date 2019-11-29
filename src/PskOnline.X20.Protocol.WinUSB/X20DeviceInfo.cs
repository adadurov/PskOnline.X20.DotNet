namespace PskOnline.X20.Protocol.WinUSB
{
  using MadWizard.WinUSBNet;
  using Microsoft.Extensions.Logging;

  public class X20DeviceInfo : IX20DeviceInfo
  {
    private readonly USBDeviceInfo _d;
    private readonly string _serialNumber;

    internal X20DeviceInfo(USBDeviceInfo d, string serialNumber)
    {
      _d = d;
      _serialNumber = serialNumber;
    }

    public int VID => _d.VID;

    public int PID => _d.PID;

    public string Manufacturer => _d.Manufacturer;

    public string DeviceDescription => _d.DeviceDescription;

    public string DevicePath => _d.DevicePath;

    public string SerialNumber => _serialNumber;

    public IX20Device CreateDevice(ILoggerFactory loggerFactory)
    {
      return new X20Device(_d, loggerFactory);
    }
  }
}