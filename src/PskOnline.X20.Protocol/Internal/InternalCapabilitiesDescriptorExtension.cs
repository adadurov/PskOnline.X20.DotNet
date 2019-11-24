namespace PskOnline.X20.Protocol.Internal
{
  using System;
  using System.Runtime.InteropServices;

  public static class InternalCapabilitiesDescriptorExtension
  {
    internal static InternalCapabilitiesDescriptor ToInternalCapabilitiesDescriptor(this byte[] bytes)
    {
      if (bytes == null) throw new ArgumentNullException(nameof(bytes));

      var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
      try
      {
        var s = (InternalCapabilitiesDescriptor)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(InternalCapabilitiesDescriptor));

        return new InternalCapabilitiesDescriptor
        {
          size = s.size,
          bitsPerSample = s.bitsPerSample,
          bytesPerPhysioTransfer = s.bytesPerPhysioTransfer,
          revisionInfoStringDescriptorIndex = s.revisionInfoStringDescriptorIndex,
          firmwareBuildDateStringDescriptorIndex = s.firmwareBuildDateStringDescriptorIndex,
          generation = s.generation,
          samplingRate = s.samplingRate
        };
      }
      finally
      {
        handle.Free();
      }
    }

    private static string StringFromAsciiBytes(byte[] firmwareBuildDate)
    {
      return System.Text.Encoding.ASCII.GetString(firmwareBuildDate);
    }
  }

}
