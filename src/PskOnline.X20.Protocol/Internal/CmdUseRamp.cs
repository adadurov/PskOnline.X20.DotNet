namespace PskOnline.X20.Protocol.Internal
{
  using Microsoft.Extensions.Logging;
  using System;

  /// <summary>
  /// Switches the device into the ramp mode for testing
  /// </summary>
  public class CmdUseRamp : CmdBase
  {
    public CmdUseRamp(ILogger logger) : base(logger)
    {
    }

    protected override UsbStdSetupPacket GetSetupPackage()
    {
      var package = CreateDefaultSetupPackage();
      package.bRequest = (byte)X20CommandCodes.X20_USE_RAMP;
      return package;
    }

    protected override ICommandResponse ParseEmptyResponse()
    {
      return new StatusResponse { Succeeded = true };
    }

    protected override ICommandResponse ParseFailure(Exception ex)
    {
      return new StatusResponse { Succeeded = false };
    }

    protected override ICommandResponse ParseResponse(byte[] response)
    {
      return new StatusResponse { Succeeded = false };
    }
  }
}
