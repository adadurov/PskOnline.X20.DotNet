namespace PskOnline.X20.Playground.Console
{
  using System;
  using System.Linq;

  using PskOnline.X20.Protocol;
  using PskOnline.X20.Protocol.Internal;
  using PskOnline.X20.Protocol.WinUSB;

  class Program
  {
    static void Main(string[] args)
    {
      var guid = Guid.Parse("{CC745879-91C7-4E8B-8F66-0FA69748909B}");

      Func<byte[], int> readPipeWinUsb = GetWinUsbPhysioPipeReader(guid);

      DumpUsbPackages(readPipeWinUsb);
    }


    private static Func<byte[], int> GetWinUsbPhysioPipeReader(Guid guid)
    {
      var logger = SerilogHelper.CreateLogger(nameof(Program));
      var devices = X20DeviceEnumerator.GetDevices(SerilogHelper.GetLoggerFactory());

      if (devices.Count() == 0)
      {
        Console.WriteLine("No matching WinUSB devices found, waiting to connect...");
        do
        {
          System.Threading.Thread.Sleep(1000);
          devices = X20DeviceEnumerator.GetDevices(SerilogHelper.GetLoggerFactory());
        }
        while (devices.Count() == 0);
      }
      var dev = devices.First().CreateDevice(SerilogHelper.GetLoggerFactory());
      var controlPipe = dev.GetUsbControlPipe();

      // send 'get capabilities' command
      var cd = (CmdGetCapabilitiesDescriptorResponse)
                  new CmdGetCapabilitiesDescriptor(logger).Execute(controlPipe);

      // send 'start' command
      new CmdStart(logger).Execute(controlPipe);

      var thePipe = dev.GetDataPipe();

      // read pipe function for WinUSB device
      Func<byte[], int> readPipe = b => thePipe.Read(b);
      return readPipe;
    }

    private static void DumpUsbPackages(Func<byte[], int> pipeReader)
    {
      var buffer = new byte[512];

      while (true)
      {
        for (int i = 0; i < buffer.Length; ++i) buffer[i] = 0;
        try
        {
          var numRead = pipeReader(buffer);

          var usbPackage = buffer.UsbDataPackageFromByteArray();
          Console.WriteLine($"=====> {numRead}");
          Console.WriteLine(usbPackage.PackageNumber);
          Console.WriteLine(usbPackage.Samples.Length);
          Console.WriteLine(usbPackage.RingBufferDataCount);
          Console.WriteLine(usbPackage.RingBufferOverflows);
          //foreach (int sample in usbPackage.samples)
          //{
          //  Console.WriteLine(sample);
          //}
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          Console.WriteLine(ex.InnerException?.Message ?? "");
        }
      }
    }
  }
}
