namespace PskOnline.X20.Protocol.Internal
{
  using System.Runtime.InteropServices;

#pragma warning disable CA1051 // Do not declare visible instance fields

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public class InternalUSBDeviceDescriptor
  {
    public byte Length;

    public byte bDescriptorType;

    public ushort bcdUSB;

    public byte bDeviceClass;

    public byte bDeviceSubclass;

    public byte bDeviceProtocol;

    public byte bMaxPacketSize0;

    public ushort VID;

    public ushort PID;

    public ushort bcdDevice;

    public byte iManufacturer;

    public byte iProduct;

    public byte iSerialumber;

    public byte bNumConfiguration;
  }

#pragma warning restore CA1051 // Do not declare visible instance fields

}
