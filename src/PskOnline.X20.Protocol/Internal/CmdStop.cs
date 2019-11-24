namespace PskOnline.X20.Protocol.Internal
{
  using Microsoft.Extensions.Logging;
  using System;

  /// <summary>
  /// Stops transmitting the PPG waveform data 
  /// (or the ramp data)
  /// </summary>
  public class CmdStop : CmdBase
  {
    public CmdStop(ILogger logger) : base(logger)
    {
    }

    protected override UsbStdSetupPacket GetSetupPackage()
    {
      var package = CreateDefaultSetupPackage();
      package.bRequest = (byte)X20CommandCodes.X20_STOP;
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
