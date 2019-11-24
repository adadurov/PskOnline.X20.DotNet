namespace PskOnline.X20.Protocol
{
  public interface IUsbDataPipe
  {
    /// <summary>
    /// Reads the data from the endpoint and returns the number of bytes actually read
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    int Read(byte[] buffer);
  }
}
