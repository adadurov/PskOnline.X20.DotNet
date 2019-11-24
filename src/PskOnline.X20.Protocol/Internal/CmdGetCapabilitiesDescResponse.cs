namespace PskOnline.X20.Protocol.Internal
{
  public class CmdGetCapabilitiesDescriptorResponse : ICommandResponse
  {
    public bool Succeeded { get; set; }

    public InternalCapabilitiesDescriptor CapabilitiesDescriptor { get; set; }
  }
}