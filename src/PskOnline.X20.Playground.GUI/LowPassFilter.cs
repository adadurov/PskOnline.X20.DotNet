using System;
using System.Linq;

namespace PskOnline.X20.Playground.GUI
{

  class LowPassFilter
  {
    MathNet.Filtering.FIR.OnlineFirFilter _filter = new MathNet.Filtering.FIR.OnlineFirFilter(filter_taps);

    public void FilterInPlace(int[] data)
    {
      var input = data.Select(p => (double)p).ToArray();
      var output = _filter.ProcessSamples(input);

      for (int i = 0; i < data.Length; ++i)
      {
        data[i] = (int)output[i];
      }
    }

    internal double Filter(int value)
    {
      return _filter.ProcessSample(value);
    }



    /*

    FIR filter designed with
    http://t-filter.appspot.com

    sampling frequency: 400 Hz

    * 0 Hz - 20 Hz
      gain = 1
      desired ripple = 5 dB
      actual ripple = 3.203267930807167 dB

    * 41 Hz - 200 Hz
      gain = 0
      desired attenuation = -40 dB
      actual attenuation = -42.235838602177395 dB

    */

    static double[] filter_taps = new double[] {
    -0.007226912808506824,
    -0.008166064975747522,
    -0.010342216999519595,
    -0.01033155447363545,
    -0.0066872445276336175,
    0.0017717716033504266,
    0.015656596947454714,
    0.03469213789109646,
    0.05763521979266151,
    0.0823329541305081,
    0.10598456198644052,
    0.12562643680600383,
    0.1386306339648464,
    0.1431804233883157,
    0.1386306339648464,
    0.12562643680600383,
    0.10598456198644052,
    0.0823329541305081,
    0.05763521979266151,
    0.03469213789109646,
    0.015656596947454714,
    0.0017717716033504266,
    -0.0066872445276336175,
    -0.01033155447363545,
    -0.010342216999519595,
    -0.008166064975747522,
    -0.007226912808506824
  };

  }
}
