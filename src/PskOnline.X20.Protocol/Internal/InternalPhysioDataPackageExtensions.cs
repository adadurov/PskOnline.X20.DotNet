namespace PskOnline.X20.Protocol.Internal
{
  using System;
  using System.Runtime.InteropServices;

  public static class InternalPhysioDataPackageExtensions
  {
    public static PhysioDataPackage UsbDataPackageFromByteArray(this byte[] bytes)
    {
      if (bytes == null) return null;

      var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
      try
      {
        var s = (InternalPhysioDataPackage)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(InternalPhysioDataPackage));

        // use s.num_samples to convert remaining part of the package to integer samples
        int[] samples = ConvertToSamples(bytes, s);

        return new PhysioDataPackage
        {
          PackageNumber = s.package_number,
          Flags = s.flags,
          Reserved = s.reserved,
          RingBufferOverflows = s.ring_buffer_overflows,
          RingBufferDataCount = s.num_samples,
          Samples = samples
        };
      }
      finally
      {
        handle.Free();
      }
    }

    private static int[] ConvertToSamples(byte[] bytes, InternalPhysioDataPackage s)
    {
      const int sampleSize = 3;
      var samples = new int[s.num_samples];
      for (int i = 0; i < s.num_samples; ++i)
      {
        // Generation 0
        samples[i] =
          bytes[sampleSize * i] +
          (bytes[sampleSize * i + 1] << 8) +
          (bytes[sampleSize * i + 2] << 16);

        // Generation 1
        // BitConverter.ToInt32(bytes, start + i * sizeof(int));
      }

      return samples;
    }
  }
}
