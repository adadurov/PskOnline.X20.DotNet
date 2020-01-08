namespace PskOnline.X20.Protocol
{
  public class USBDeviceDescriptor
  {
    public int VID { get; set; }

    public int PID { get; set; }

    public byte DeviceClass { get; set; }

    public byte DeviceSubClass { get; set; }

    public byte DeviceProtocol { get; set; }

    public ushort iManufacturer { get; set; }

    public string Manufacturer { get; set; }

    public ushort iProduct { get; set; }

    public string Product { get; set; }

    public ushort iSerialumber { get; set; }

    public string SerialNumber { get; set; }
  }
}
