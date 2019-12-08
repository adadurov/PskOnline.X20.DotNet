namespace PskOnline.X20.Protocol
{
  public interface IUsbDataPipe
  {
    /// <summary>
    /// Reads the data from the endpoint and returns the number of bytes actually read.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    /// <remarks>Doesn't return until a package is received from the hardware device.</remarks>
    int Read(byte[] buffer);

    int Read(byte[] buffer, int timeoutMSec);
  }
}
