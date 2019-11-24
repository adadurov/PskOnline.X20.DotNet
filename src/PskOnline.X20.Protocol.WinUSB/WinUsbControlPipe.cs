namespace PskOnline.X20.Protocol.WinUSB
{
  using PskOnline.X20.Protocol;
  internal class WinUsbControlPipe : IUsbControlPipe
  {
    private readonly MadWizard.WinUSBNet.USBDevice _device;

    public WinUsbControlPipe(MadWizard.WinUSBNet.USBDevice device)
    {
      _device = device;
    }

    public void ControlTransfer(byte bmRequestType, byte bRequest, ushort wValue, ushort wIndex)
    {
      _device.ControlTransfer(bmRequestType, bRequest, wValue, wIndex);
    }

    public byte[] ControlTransferIn(byte bmRequestType, byte bRequest, ushort wValue, ushort wIndex, ushort wLength)
    {
      return _device.ControlIn(bmRequestType, bRequest, wValue, wIndex, wLength);
    }
  }
}
