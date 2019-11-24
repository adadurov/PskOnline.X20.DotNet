namespace PskOnline.X20.Protocol.Internal
{
  using System;

  [Flags]
  public enum UsbConstants
  {
    UsbSetupDataStageIn = 0x80,

    UsbReqTypeStandard = 0x00,
    UsbReqTypeClass = 0x20,
    UsbReqTypeVendor = 0x40,
    UsbReqTypeMask = 0x60,

    UsbReqRecipientDevice = 0x00,
    UsbReqRecipientInterface = 0x01,
    UsbReqRecipientEndpoint = 0x02,
    UsbReqRecipientMask = 0x03
  }
}
