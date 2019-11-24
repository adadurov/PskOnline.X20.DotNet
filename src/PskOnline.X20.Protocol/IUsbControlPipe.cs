namespace PskOnline.X20.Protocol
{
  public interface IUsbControlPipe
  {
    /// <summary>
    /// performs a control request without data stage
    /// </summary>
    /// <param name="bmRequestType"></param>
    /// <param name="bRequest"></param>
    /// <param name="wValue"></param>
    /// <param name="wIndex"></param>
    /// <returns></returns>
    void ControlTransfer(byte bmRequestType, byte bRequest, ushort wValue, ushort wIndex);

    /// <summary>
    ///     Initiates a control transfer over the default control endpoint. The request should
    ///     have an IN direction (specified by the highest bit of the requestType parameter).
    ///     A buffer to receive the data is automatically created by this method.
    /// </summary>
    /// <param name="bmRequestType"></param>
    /// <param name="bRequest"></param>
    /// <param name="wValue"></param>
    /// <param name="wIndex"></param>
    /// <param name="buffer"></param>
    /// <param name="wLength"></param>
    byte[] ControlTransferIn(byte bmRequestType, byte bRequest, ushort wValue, ushort wIndex, ushort wLength);
  }
}
