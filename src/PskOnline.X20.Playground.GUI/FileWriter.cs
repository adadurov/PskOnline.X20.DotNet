namespace PskOnline.X20.Playground.GUI
{
  using System;
  using System.IO;
  using static System.Environment;

  public sealed class FileWriter : IDisposable
  {
    private StreamWriter _writer;

    public string TargetaPath => GetFolderPath(SpecialFolder.DesktopDirectory);

    public string TargetFileName { get; }

    public FileWriter(string fileName, string deviceFirmware, string samplingRate)
    {
      TargetFileName = fileName;
      _writer = new StreamWriter(Path.Combine(TargetaPath, fileName));

      _writer.WriteLine("name, {insert}");
      _writer.WriteLine("gender, male | female");
      _writer.WriteLine("age, insert");
      _writer.WriteLine("hand, left | right");
      _writer.WriteLine("finger, thumb | index | middle | ring | pinky");
      _writer.WriteLine($"device, {deviceFirmware}");
      _writer.WriteLine($"sampling_rate, {samplingRate}");
      _writer.WriteLine("BEGIN_DATA");
    }

    internal void WriteSample(int value, int filteredValue)
    {
      _writer.WriteLine($"{value}, {filteredValue}");
    }

    public void Dispose()
    {
      _writer.Dispose();
    }
  }
}
