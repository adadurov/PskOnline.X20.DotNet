namespace PskOnline.X20.Protocol.Internal
{
  internal static class CmdHelper
  {
    internal static UsbStdSetupPacket GetCommandPackage()
    {
      return new UsbStdSetupPacket
      {
        bmRequestType = 0,
        bRequest = 0,
        wIndex = 0,
        wLength = 0,
        wValue = 0
      };
    }
  }
}
