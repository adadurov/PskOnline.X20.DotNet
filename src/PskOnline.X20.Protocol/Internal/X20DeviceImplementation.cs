namespace PskOnline.X20.Protocol.Internal
{
  using Microsoft.Extensions.Logging;
  using System;
  using System.Text;

  public sealed class X20DeviceImplementation
  {
    private readonly ILogger _logger;
    private readonly IUsbDataPipe _usbDataPipe;
    private readonly IUsbControlPipe _usbControlPipe;
    private readonly Capabilities _capabilities;
    private readonly string _serialNumber;

    public X20DeviceImplementation(ILogger logger, IUsbDataPipe usbDataPipe, IUsbControlPipe usbControlPipe, string serialNumber)
    {
      _logger = logger;
      _usbDataPipe = usbDataPipe;
      _usbControlPipe = usbControlPipe;
      _serialNumber = serialNumber;

      _capabilities = RetrieveCapabilities();

      LogDeviceInformation(_logger, _capabilities);
    }

    private Capabilities RetrieveCapabilities()
    {
      var getCapDesc = new CmdGetCapabilitiesDescriptor(_logger);

      var getCapDescResponse = (CmdGetCapabilitiesDescriptorResponse)getCapDesc.Execute(GetUsbControlPipe());
      if (!getCapDescResponse.Succeeded)
      {
        return null;
      }

      var capDesc = getCapDescResponse.CapabilitiesDescriptor;

      var fwBuildDate = GetStringDescriptor(capDesc.firmwareBuildDateStringDescriptorIndex);
      var fwVersionInfo = GetStringDescriptor(capDesc.revisionInfoStringDescriptorIndex);

      return new Capabilities
      {
        Size = capDesc.size,
        SerialNumber = _serialNumber,
        Generation = capDesc.generation,
        BitsPerSample = capDesc.bitsPerSample,
        BytesPerPhysioTransfer = capDesc.bytesPerPhysioTransfer,
        FirmwareBuildDate = fwBuildDate,
        RevisionInfo = fwVersionInfo,
        SamplingRate = capDesc.samplingRate
      };
    }

    private string GetStringDescriptor(ushort stringDescriptorIndex)
    {
      var packet = new UsbStdSetupPacket
      {
        bmRequestType = 0x80,
        bRequest = 6, // GET_DESCRIPTOR
        wValue = (ushort)((3 << 8) + stringDescriptorIndex), // 3 => STRING
        wIndex = 0x0409,
        wLength = 256
      };

      var buffer = _usbControlPipe.ControlTransferIn(packet.bmRequestType, packet.bRequest, packet.wValue, packet.wIndex, packet.wLength);

      // this is required due to an issue in WinUSB.NET
      var realLength = Math.Max(Math.Min(buffer[0], buffer.Length) - 2, 0);

      return Encoding.Unicode.GetString(buffer, 2, realLength);
    }

    private static void LogDeviceInformation(ILogger logger, Capabilities cap)
    {
      var sb = new StringBuilder("PSK-X20: ", 300);

      sb.Append($" S/N: {cap.SerialNumber} //");
      sb.Append($" Generation: {cap.Generation} //");
      sb.Append($" FW Revision: {cap.RevisionInfo} //");
      sb.Append($" FW Date: {cap.FirmwareBuildDate} //");
      sb.Append($" Sampling rate: {cap.SamplingRate} //");

      logger.LogInformation(sb.ToString());
    }

    public Capabilities GetCapabilities()
    {
      return _capabilities;
    }

    public IUsbDataPipe GetDataPipe()
    {
      return _usbDataPipe;
    }

    public PhysioDataPackage GetPhysioData()
    {
      var buffer = new byte[512];
      var count = GetDataPipe().Read(buffer);
      if (count > X20Constants.PhysioPackageHeaderSize)
      {
        return buffer.UsbDataPackageFromByteArray();
      }
      return null;
    }

    public IUsbControlPipe GetUsbControlPipe()
    {
      return _usbControlPipe;
    }

    public bool StartMeasurement()
    {
      var result = new CmdStart(_logger).Execute(GetUsbControlPipe());
      return result.Succeeded;
    }

    public bool StopMeasurement()
    {
      var result = new CmdStop(_logger).Execute(GetUsbControlPipe());
      return result.Succeeded;
    }

    public bool UsePpgWaveform()
    {
      var result = new CmdUsePpgWaveform(_logger).Execute(GetUsbControlPipe());
      return result.Succeeded;
    }

    public bool UseRamp()
    {
      var result = new CmdUseRamp(_logger).Execute(GetUsbControlPipe());
      return result.Succeeded;
    }
  }
}
