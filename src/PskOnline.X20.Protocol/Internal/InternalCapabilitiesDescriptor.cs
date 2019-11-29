namespace PskOnline.X20.Protocol.Internal
{
  using System.Runtime.InteropServices;

#pragma warning disable CA1051 // Do not declare visible instance fields

  [StructLayout(LayoutKind.Sequential, Pack = 2)]
  public class InternalCapabilitiesDescriptor
  {
    /// <summary>
    /// the size of the structure (in bytes)
    /// </summary>
    public ushort size;

    /// <summary>
    /// The generation number for the firmware & device.
    /// The clients may choose which generations they do support.
    /// </summary>
    public ushort generation;

    /// <summary>
    /// bits per sample
    /// </summary>
    public ushort bitsPerSample;

    /// <summary>
    /// sampling rate (Hz)
    /// </summary>
    public ushort samplingRate;

    /// <summary>
    /// Bytes per transfer (possibly a multi-packet transfer)
    /// over PHYSIO_EPIN_ADDR endpoint
    /// </summary>
    public ushort bytesPerPhysioTransfer;

    /// <summary>
    /// The index of the USB string descriptor representing the device's firmware build date.
    /// </summary>
    public ushort firmwareBuildDateStringDescriptorIndex;

    /// <summary>
    /// The index of the USB string descriptor representing the device's firmware version.
    /// </summary>
    public ushort revisionInfoStringDescriptorIndex;
  }

#pragma warning restore CA1051 // Do not declare visible instance fields

}
