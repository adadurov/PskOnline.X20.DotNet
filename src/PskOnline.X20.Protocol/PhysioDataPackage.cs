namespace PskOnline.X20.Protocol
{
  using System;

  public class PhysioDataPackage
  {
    public UInt32 PackageNumber { get; set; }

    public UInt32 Flags { get; set; }

    public float DieTemperature { get; set; }

    public UInt16 Reserved { get; set; }

    public UInt32 RingBufferOverflows { get; set; }

    public UInt16 RingBufferDataCount { get; set; }

    public Int32[] Samples { get; set; }
  }
}
