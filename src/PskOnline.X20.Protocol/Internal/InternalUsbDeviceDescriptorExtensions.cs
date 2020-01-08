namespace PskOnline.X20.Protocol.Internal
{
  using System;
  using System.Collections.Generic;
  using System.Runtime.InteropServices;
  using System.Text;

  public static class InternalUsbDeviceDescriptorExtensions
  {
    internal static USBDeviceDescriptor ToDeviceDescriptor(this byte[] bytes)
    {
      if (bytes == null) throw new ArgumentNullException(nameof(bytes));

      var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
      try
      {
        var d = (InternalUSBDeviceDescriptor)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(InternalUSBDeviceDescriptor));

        return new USBDeviceDescriptor
        {
          DeviceClass = d.bDeviceClass,
          iManufacturer = d.iManufacturer,
          iProduct = d.iProduct,
          iSerialumber = d.iSerialumber,
          PID = d.PID,
          VID = d.VID,
          DeviceProtocol = d.bDeviceProtocol,
          DeviceSubClass = d.bDeviceSubclass
        };
      }
      finally
      {
        handle.Free();
      }
    }
  }
}
