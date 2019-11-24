namespace PskOnline.X20.Protocol.Internal
{
  internal enum X20CommandCodes : byte
  {
    // returns the capabilities descriptor defined by
    // x20_capabilities_descriptor
    X20_GET_CAPABILITIES_DESCRIPTOR = 0x20,

    X20_USE_PPG = 0x30,

    X20_USE_RAMP = 0x31,

    X20_START = 0x50,

    X20_STOP = 0x51
  }
}
