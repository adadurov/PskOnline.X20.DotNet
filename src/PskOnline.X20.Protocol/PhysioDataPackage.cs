namespace PskOnline.X20.Protocol
{
  using System;

  public class PhysioDataPackage
  {
    public UInt32 PackageNumber { get; set; }

    public UInt32 Flags { get; set; }

    public UInt32 Reserved { get; set; }

    public Int32 RingBufferOverflows { get; set; }

    public Int32 RingBufferDataCount { get; set; }

    public Int32[] Samples { get; set; }
  }
}
