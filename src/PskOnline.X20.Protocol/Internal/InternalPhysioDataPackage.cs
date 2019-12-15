namespace PskOnline.X20.Protocol.Internal
{
  using System;
  using System.Runtime.InteropServices;

#pragma warning disable CA1812 // Class is never instantiated

  [StructLayout(LayoutKind.Explicit)]
  internal class InternalPhysioDataPackage
  {
    [FieldOffset(0)]
    public UInt32 package_number;

    [FieldOffset(4)]
    public UInt32 flags;

    [FieldOffset(8)]
    public UInt32 reserved;

    [FieldOffset(12)]
    public Int32 ring_buffer_overflows;

    [FieldOffset(16)]
    public UInt16 ring_buffer_data_count;

    [FieldOffset(18)]
    public UInt16 num_samples;
  }

#pragma warning restore CA1812 // Class is never instantiated

}
