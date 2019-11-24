namespace PskOnline.X20.Protocol.Internal
{
  public class UsbStdSetupPacket
  {
    public byte bmRequestType { get; set; }

    public byte bRequest { get; set; }

    public ushort wValue { get; set; }

    public ushort wIndex { get; set; }

    public ushort wLength { get; set; }
  }
}
