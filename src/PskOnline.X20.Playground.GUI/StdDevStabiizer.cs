namespace PskOnline.X20.Math
{
  using System;

  public class StdDevStabiizer
  {
    /// <summary>
    /// Current index in the window
    /// </summary>
    int _idx = 0;

    bool _historyEmpty = true;

    /// <summary>
    /// The target StdDev
    /// </summary>
    readonly double TargetStdDev = 2073;

    /// <summary>
    /// Number of points in the sliding history windows
    /// </summary>
    readonly int WindowSize;

    /// <summary>
    /// History of the input values
    /// </summary>
    readonly long[] IN;

    /// <summary>
    /// Sum of the inputs window
    /// </summary>
    long INsum = 0;

    /// <summary>
    /// Series of the sums of the squared deviations
    /// </summary>
    readonly long[] DD;

    /// <summary>
    /// sum of the squared deviations over the window
    /// </summary>
    long DDsum = 0;

    /// <summary>
    /// History of the outputs
    /// </summary>
    readonly long[] O;

    /// <summary>
    /// Sum of the outputs
    /// </summary>
    long Osum = 0;

    /// <summary>
    /// History of the Standard Deviation values
    /// </summary>
    readonly long[] StdDev;

    /// <summary>
    /// History of the Mean(_I) values
    /// </summary>
    readonly long[] IM;


    /// <summary>
    /// </summary>
    /// <param name="samplingRate"></param>
    public StdDevStabiizer(double samplingRate, StdDevStabilizerParams para)
    {
      TargetStdDev = para.DSN;
      GainMin = para.MinGain;
      GainMax = para.MaxGain;

      WindowSize = (int)( samplingRate * 2.25 );

      IN = new long[WindowSize];
      IM = new long[WindowSize];
      DD = new long[WindowSize];

      StdDev = new long[WindowSize];
      O = new long[WindowSize];
    }

    public void NormalizeDataInPlace(int[] data)
    {
      InitWindow(data);

      for (int i = 0; i < data.Length; ++i)
      {
        data[i] = NormalizeSingleValue(data[i]);
      }
    }

    private int NormalizeSingleValue(long Ain)
    {
      _idx = (_idx + 1) % WindowSize;               // The current index in the window buffer

      var IM2 = (_idx + WindowSize / 2) % WindowSize;       // This index corresponids to 1/2 of the window back in time

      var Aold = IN[_idx];                  // The oldest input

      // update the sum of inputs over the analysis window
      INsum -= Aold;                         
      INsum += Ain;
      IN[_idx] = Ain;                       // remember the sum of the inputs

      var MA = INsum / WindowSize;          // average of the inputs (sum(inputs) / number)
      IM[_idx] = MA;                        // remember the current sum

      var gain = 51.0 * Math.Sqrt(DDsum / WindowSize);  // gain -- proportional to StdDev

      StdDev[_idx] = (long)gain;            // store the StdDev value in the analysis window

      var A2 = IN[IM2];                     // the input value at 1/2 of the window back
      var DS1 = A2 - MA;                    // deviation of the current input from Mean(input)
      DS1 *= DS1;                           // squared

      // Update the sum of the squared deviations over the window
      DDsum -= DD[_idx];
      DDsum += DS1;
      DD[_idx] = DS1;                       // remember the latest sum of the deivations

      // Prevent amplification of the noise
      gain = Math.Min(Math.Max(gain, GainMin), GainMax);

      // Produce the output value
      // Aold - the oldest value in the window
      // МА2 - MA at 1/2 of the window earlier
      // КА - gain
      // DSN - target StdDev
      var MA2 = IM[IM2];                  // 
      var output = TargetStdDev * (Aold - MA2) / gain;

      // center the output at 0
      Osum -= O[_idx];
      Osum += (long)output;
      O[_idx] = (long)output;

      return (int) (output - Osum / WindowSize);
    }

    private void InitWindow(int[] data)
    {
      if( _historyEmpty && (data.Length > 0) )
      {
        // reset history
        for (var i = 0; i < WindowSize; ++i)
        {
          IN[i] = 0;
          IM[i] = 0;
          DD[i] = 0;
          StdDev[i] = 0;
          O[i] = 0;
        }
        _historyEmpty = false;
      }
    }

    private double GainMin { get; set; } = 400;

    private double GainMax { get; set; } = 13000;

  }
}
