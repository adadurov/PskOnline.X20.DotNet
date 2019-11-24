namespace PskOnline.X20.Protocol
{
  public class Capabilities
  {
    /// <summary>
    /// The size of the original structure provided by the device
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    /// Bits per sample
    /// </summary>
    public int BitsPerSample { get; set; }

    /// <summary>
    /// Sampling rate (Hz)
    /// </summary>
    public int SamplingRate { get; set; }

    /// <summary>
    /// Bytes per (possibly a multi-packet) transfer
    /// over PHYSIO_EPIN_ADDR endpoint
    /// </summary>
    public int BytesPerPhysioTransfer { get; set; }

    /// <summary>
    /// The generation number for the firmware & device.
    /// The clients may choose which generations they do support.
    /// </summary>
    public int Generation { get; set; }

    /// <summary>
    /// Firmware build date, e.g. 2019-08-18T22:00:22.000
    /// </summary>
    public string FirmwareBuildDate { get; set; }

    /// <summary>
    /// A textual description of the device's firmware version
    /// A containing only numbers, Latin letters, dots and parentheses.
    /// For example: '1.2.3.0-alpha-29ab2fd9'
    /// </summary>
    public string RevisionInfo { get; set; }
  }

}
