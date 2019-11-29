﻿namespace PskOnline.X20.Protocol.Internal
{
  using System;
  using System.Runtime.InteropServices;

  public static class InternalPhysioDataPackageExtensions
  {
    public static PhysioDataPackage UsbDataPackageFromByteArray(this byte[] bytes)
    {
      var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
      try
      {
        var s = (InternalPhysioDataPackage)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(InternalPhysioDataPackage));

        // use s.num_samples to convert remaining part of the package to integer samples
        var start = 20;
        var end = s.num_samples * sizeof(UInt32);
        var samples = new int[s.num_samples];
        for (int i = 0; i < s.num_samples; ++i)
        {
          samples[i] = BitConverter.ToInt32(bytes, start + i * sizeof(int));
        }

        float dieTemperature = s.die_temperature >> 8;
        dieTemperature += 0.0625f * (s.die_temperature & 0x0f);

        return new PhysioDataPackage
        {
          PackageNumber = s.package_number,
          DieTemperature = dieTemperature,
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
  }
}
