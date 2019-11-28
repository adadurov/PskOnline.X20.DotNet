namespace PskOnline.X20.Math
{
  using System;
  using System.Collections.Generic;

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

      O = new long[WindowSize];
      InitWindow();
    }

    public int Normalize(int data)
    {
      return NormalizeSingleValue(data);
    }

    public void NormalizeDataInPlace(List<int> data)
    {
      for (int i = 0; i < data.Count; ++i)
      {
        data[i] = NormalizeSingleValue(data[i]);
      }
    }

    public void NormalizeDataInPlace(int[] data)
    {
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

      // update the sum of inputs over the history window
      INsum -= Aold;                         
      INsum += Ain;
      IN[_idx] = Ain;

      // average of the inputs (sum(inputs) / number)
      var MA = INsum / WindowSize;
      // store the current average in the history window
      IM[_idx] = MA;

      // gain -- proportional to StdDev
      var gain = 51.0 * Math.Sqrt(DDsum / WindowSize);

      // the input value at 1/2 of the window back
      var A2 = IN[IM2];
      // deviation of the current input from Mean(input)
      var DS1 = A2 - MA;
      // squared
      DS1 *= DS1;

      // Update the sum of the squared deviations over the window
      DDsum -= DD[_idx];
      DDsum += DS1;
      // store the latest sum of the deivations
      DD[_idx] = DS1;

      // Prevent amplification of the noise
      gain = Math.Min(Math.Max(gain, GainMin), GainMax);

      // Produce the output value
      // Aold - the oldest value in the window
      // МА2 - Mean(A) at 1/2 of the window earlier
      var MA2 = IM[IM2];                  // 
      var output = TargetStdDev * (Aold - MA2) / gain;

      // center the output at 0
      Osum -= O[_idx];
      Osum += (long)output;
      O[_idx] = (long)output;

      return (int) (output - Osum / WindowSize);
    }

    private void InitWindow()
    {
      if (_historyEmpty)
      {
        // reset history
        for (var i = 0; i < WindowSize; ++i)
        {
          IN[i] = 0;
          IM[i] = 0;
          DD[i] = 0;
          O[i] = 0;
        }
        _historyEmpty = false;
      }
    }

    private double GainMin { get; set; } = 400;

    private double GainMax { get; set; } = 13000;

  }
}
