namespace PskOnline.X20.Protocol
{
  using System;

  public interface IX20Device : IDisposable
  {
    IUsbDataPipe GetDataPipe();

    IUsbControlPipe GetUsbControlPipe();

    Capabilities GetCapabilities();

    bool StartMeasurement();

    bool StopMeasurement();

    bool UseRamp();

    bool UsePpgWaveform();

    PhysioDataPackage GetPhysioData();

    PhysioDataPackage GetPhysioData(int timeoutMsec);

  }
}
