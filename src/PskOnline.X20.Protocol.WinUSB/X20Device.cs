namespace PskOnline.X20.Protocol.WinUSB
{
  using Microsoft.Extensions.Logging;
  using PskOnline.X20.Protocol.Internal;
  using System;

  public sealed class X20Device : IX20Device
  {
    private readonly X20DeviceImplementation _deviceImpl;
    private readonly MadWizard.WinUSBNet.USBDevice _winUsbDevice;
    private readonly ILogger _logger;

    public X20Device(MadWizard.WinUSBNet.USBDeviceInfo winUsbDevice, ILoggerFactory loggerFactory)
    {
      if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
      _logger = loggerFactory.CreateLogger(nameof(X20Device));

      if (winUsbDevice == null) throw new ArgumentNullException(nameof(winUsbDevice));
      _winUsbDevice = new MadWizard.WinUSBNet.USBDevice(winUsbDevice);

      _deviceImpl = new X20DeviceImplementation(
        _logger, GetDataPipe(), GetUsbControlPipe(), _winUsbDevice.Descriptor.SerialNumber);
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
      return _deviceImpl.GetCapabilities();
    }

    public bool StartMeasurement()
    {
      return _deviceImpl.StartMeasurement();
    }

    public bool StopMeasurement()
    {
      return _deviceImpl.StopMeasurement();
    }

    public bool UsePpgWaveform()
    {
      return _deviceImpl.UsePpgWaveform();
    }

    public bool UseRamp()
    {
      return _deviceImpl.UseRamp();
    }

    public PhysioDataPackage GetPhysioData()
    {
      return _deviceImpl.GetPhysioData();
    }

    public PhysioDataPackage GetPhysioData(int timeoutMSec)
    {
      var buffer = new byte[512];
      var count = GetDataPipe().Read(buffer, timeoutMSec);
      if (count > X20Constants.PhysioPackageHeaderSize)
      {
        return buffer.UsbDataPackageFromByteArray();
      }
      return null;
    }

    public USBDeviceDescriptor GetUsbDeviceDescriptor()
    {
      return _deviceImpl.GetUsbDeviceDescriptor();
    }

  }
}
