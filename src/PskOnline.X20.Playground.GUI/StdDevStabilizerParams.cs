namespace PskOnline.X20.Math
{
  public class StdDevStabilizerParams
  {
    public StdDevStabilizerParams() { }

    internal StdDevStabilizerParams(long DSN, int minGain, int maxGain)
    {
      this.DSN = DSN;
      MinGain = minGain;
      MaxGain = maxGain;
    }

    public double DSN { get; set; }

    public double MinGain { get; set; }

    public double MaxGain { get; set; }
  }
}
