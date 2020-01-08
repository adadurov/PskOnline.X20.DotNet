namespace PskOnline.X20.Protocol.Internal
{
  using System;
  using Microsoft.Extensions.Logging;

  public class CmdGetCapabilitiesDescriptor : ICommand
  {
    private ILogger _logger;

    public CmdGetCapabilitiesDescriptor(ILogger logger)
    {
      if (logger == null) throw new ArgumentNullException(nameof(logger));
      _logger = logger;
    }

    public ICommandResponse Execute(IUsbControlPipe controlPipe)
    {
      if (controlPipe == null) throw new ArgumentNullException(nameof(controlPipe));

      var size = GetDescriptorSize(controlPipe);
      if (size < 14)
      {
        _logger.LogError($"The reported size of the capabilities descriptor is too small ({size} bytes)");
        return new CmdGetCapabilitiesDescriptorResponse
        {
          Succeeded = false,
          CapabilitiesDescriptor = null
        };
      }
      return GetFullDescriptor(size, controlPipe, _logger);
    }

    private int GetDescriptorSize(IUsbControlPipe controlPipe)
    {
      var package = GetPartialDescriptorRequest();
      try
      {
        var response = controlPipe.ControlTransferIn(
          package.bmRequestType, package.bRequest, package.wValue, package.wIndex, package.wLength);

        return response[0] + (response[1] << 8);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error retrieving the size of the capabilities descriptor");
        return 0;
      }
    }

    private static UsbStdSetupPacket GetPartialDescriptorRequest()
    {
      // request 8 bytes initially, to find out the length of the descriptor
      var package = CmdBase.CreateDefaultInSetupPackage();
      package.bRequest = (byte)X20CommandCodes.X20_GET_CAPABILITIES_DESCRIPTOR;
      package.wLength = 8;
      return package;
    }

    private static UsbStdSetupPacket GetFullDescriptorRequest(int size)
    {
      // request the entire descriptor
      var package = CmdBase.CreateDefaultInSetupPackage();
      package.bRequest = (byte)X20CommandCodes.X20_GET_CAPABILITIES_DESCRIPTOR;
      package.wLength = (ushort)size;
      return package;
    }

    private CmdGetCapabilitiesDescriptorResponse GetFullDescriptor(int size, IUsbControlPipe controlPipe, ILogger logger)
    {
      var package = GetFullDescriptorRequest(size);
      try
      {
        var response = controlPipe.ControlTransferIn(
          package.bmRequestType, package.bRequest, package.wValue, package.wIndex, package.wLength);

        var descriptor = response.ToInternalCapabilitiesDescriptor();

        return new CmdGetCapabilitiesDescriptorResponse
        {
          Succeeded = true,
          CapabilitiesDescriptor = descriptor
        };
      }
      catch (Exception ex)
      {
        logger.LogError($"Error retrieving capabilities descriptor (requested {size} bytes)", ex);
        return new CmdGetCapabilitiesDescriptorResponse
        {
          Succeeded = false,
          CapabilitiesDescriptor = null
        };
      }
    }
  }
}
