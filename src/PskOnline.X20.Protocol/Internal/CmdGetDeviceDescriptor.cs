namespace PskOnline.X20.Protocol.Internal
{
  using Microsoft.Extensions.Logging;
  using System;

  public class CmdGetDeviceDescriptor : ICommand
  {
    private ILogger _logger;

    public CmdGetDeviceDescriptor(ILogger logger)
    {
      if (logger == null) throw new ArgumentNullException(nameof(logger));
      _logger = logger;
    }

    public ICommandResponse Execute(IUsbControlPipe controlPipe)
    {
      if (controlPipe == null) throw new ArgumentNullException(nameof(controlPipe));

      var size = GetDeviceDescriptorSize(controlPipe);
      if (size < 8)
      {
        _logger.LogError($"The reported size of the device descriptor is too small ({size} bytes)");
        return new CmdGetDeviceDescriptorResponse
        {
          Succeeded = false,
          DeviceDescriptor = null
        };
      }
      return GetFullDescriptor(size, controlPipe, _logger);
    }

    private int GetDeviceDescriptorSize(IUsbControlPipe controlPipe)
    {
      var package = GetPartialDescriptorRequest();
      try
      {
        var response = controlPipe.ControlTransferIn(
          package.bmRequestType, package.bRequest, package.wValue, package.wIndex, package.wLength);

        return response[0];
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error retrieving the size of the device descriptor");
        return 0;
      }
    }

    private static UsbStdSetupPacket GetPartialDescriptorRequest()
    {
      // request 8 bytes initially, to find out the length of the descriptor
      var package = new UsbStdSetupPacket {
        bmRequestType = (byte)(UsbConstants.UsbSetupDataStageIn |
                        UsbConstants.UsbReqRecipientDevice |
                        UsbConstants.UsbReqTypeStandard),
        bRequest = 6,     // GET_DESCRIPTOR
        wLength = 8,
        wIndex = 0,
        wValue = 1 << 8   // Device
      };
      return package;
    }

    private static UsbStdSetupPacket GetFullDescriptorRequest(int size)
    {
      // request 8 bytes initially, to find out the length of the descriptor
      var package = new UsbStdSetupPacket
      {
        bmRequestType = (byte)(UsbConstants.UsbSetupDataStageIn |
                        UsbConstants.UsbReqRecipientDevice |
                        UsbConstants.UsbReqTypeStandard),
        bRequest = 6,     // GET_DESCRIPTOR
        wLength = (ushort)size,
        wIndex = 0,
        wValue = 1 << 8   // Device
      };
      return package;
    }

    private CmdGetDeviceDescriptorResponse GetFullDescriptor(int size, IUsbControlPipe controlPipe, ILogger logger)
    {
      var package = GetFullDescriptorRequest(size);
      try
      {
        var response = controlPipe.ControlTransferIn(
          package.bmRequestType, package.bRequest, package.wValue, package.wIndex, package.wLength);

        var descriptor = response.ToDeviceDescriptor();

        return new CmdGetDeviceDescriptorResponse
        {
          Succeeded = true,
          DeviceDescriptor = descriptor
        };
      }
      catch (Exception ex)
      {
        logger.LogError($"Error retrieving device descriptor (requested {size} bytes)", ex);
        return new CmdGetDeviceDescriptorResponse
        {
          Succeeded = false,
          DeviceDescriptor = null
        };
      }
    }

  }
}
