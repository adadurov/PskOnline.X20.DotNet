namespace PskOnline.X20.Protocol.Internal
{
  public class CmdGetDeviceDescriptorResponse : ICommandResponse
  {
    public bool Succeeded { get; set; }

    public USBDeviceDescriptor DeviceDescriptor { get; set; }
  }
}