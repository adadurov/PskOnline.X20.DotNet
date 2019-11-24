namespace PskOnline.X20.Protocol.WinUSB
{
  using PskOnline.X20.Protocol.Internal;
  using System;
  using System.Linq;

  internal class WinUsbDataPipe : IUsbDataPipe
  {
    private readonly MadWizard.WinUSBNet.USBPipe _pipe;

    public WinUsbDataPipe(MadWizard.WinUSBNet.USBDevice device)
    {
      if (device == null)
      {
        throw new ArgumentNullException(nameof(device));
      }

      var pipeAddress = X20Constants.PhysioPipeAddress;
      _pipe = device.Pipes.FirstOrDefault(p => p.Address == pipeAddress);
      if (_pipe == null)
      {
        throw new Exception($"The device {device.Descriptor.PathName} has no pipe with address {pipeAddress}");
      }
    }

    public int Read(byte[] buffer)
    {
      return _pipe.Read(buffer);
    }
  }
}
