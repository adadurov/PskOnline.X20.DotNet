namespace PskOnline.X20.Protocol.WinUSB
{
  using MadWizard.WinUSBNet;
  using Microsoft.Extensions.Logging;
  using PskOnline.X20.Protocol.Internal;
  using System;

  public sealed class X20Device : IX20Device
  {
    private readonly USBDevice _winUsbDevice;
    private readonly ILogger _logger;

    public X20Device(USBDeviceInfo winUsbDevice, ILoggerFactory loggerFactory)
    {
      if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
      _logger = loggerFactory.CreateLogger(nameof(X20Device));

      if (winUsbDevice == null) throw new ArgumentNullException(nameof(winUsbDevice));
      _winUsbDevice = new USBDevice(winUsbDevice);
    }

    public IUsbDataPipe GetDataPipe()
    {
      return new WinUsbDataPipe(_winUsbDevice);
    }

    public IUsbControlPipe GetUsbControlPipe()
    {
      return new WinUsbControlPipe(_winUsbDevice);
    }

    public void Dispose()
    {
      _winUsbDevice?.Dispose();
    }

    public Capabilities GetCapabilities()
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

      var buffer = _winUsbDevice.ControlIn(packet.bmRequestType, packet.bRequest, packet.wValue, packet.wIndex, packet.wLength);

      var realLength = Math.Min(buffer[1], buffer.Length - 2);

      return System.Text.Encoding.Unicode.GetString(buffer, 2, realLength);
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

    public bool UseRamp()
    {
      var result = new CmdUseRamp(_logger).Execute(GetUsbControlPipe());
      return result.Succeeded;
    }

    public bool UsePpgWaveform()
    {
      var result = new CmdUsePpgWaveform(_logger).Execute(GetUsbControlPipe());
      return result.Succeeded;
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
  }
}
