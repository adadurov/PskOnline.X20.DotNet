namespace PskOnline.X20.Protocol.Internal
{
  using Microsoft.Extensions.Logging;
  using System;

  public abstract class CmdBase : ICommand
  {
    public CmdBase(ILogger logger)
    {
      if (logger == null) throw new ArgumentNullException(nameof(logger));
      Logger = logger;
    }

    protected ILogger Logger { get; set; }

    internal static UsbStdSetupPacket CreateDefaultInSetupPackage()
    {
      return new UsbStdSetupPacket {
        bmRequestType = (int)(UsbConstants.UsbSetupDataStageIn |
                        UsbConstants.UsbReqTypeClass |
                        UsbConstants.UsbReqRecipientInterface)
      };
    }
    internal static UsbStdSetupPacket CreateDefaultSetupPackage()
    {
      return new UsbStdSetupPacket
      {
        bmRequestType = (int)(
                        UsbConstants.UsbReqTypeClass |
                        UsbConstants.UsbReqRecipientInterface)
      };
    }

    public ICommandResponse Execute(IUsbControlPipe controlPipe)
    {
      if (controlPipe == null) throw new ArgumentNullException(nameof(controlPipe));

      var package = GetSetupPackage();
      if (package.wLength == 0)
      {
        try
        {
          controlPipe.ControlTransfer(
            package.bmRequestType, package.bRequest, package.wValue, package.wIndex);
          return ParseEmptyResponse();
        }
        catch (Exception ex)
        {
          Logger.LogError($"Error executing command {GetType().Name}", ex);
          return ParseFailure(ex);
        }
      }
      else
      {
        try
        {
          var response = controlPipe.ControlTransferIn(
            package.bmRequestType, package.bRequest, package.wValue, package.wIndex, package.wLength);
          return ParseResponse(response);
        }
        catch (Exception ex)
        {
          Logger.LogError($"Error executing command {GetType().Name}", ex);
          return ParseFailure(ex);
        }
      }
    }

    protected abstract UsbStdSetupPacket GetSetupPackage();

    protected abstract ICommandResponse ParseEmptyResponse();

    protected abstract ICommandResponse ParseResponse(byte[] response);

    protected abstract ICommandResponse ParseFailure(Exception ex);

  }
}
