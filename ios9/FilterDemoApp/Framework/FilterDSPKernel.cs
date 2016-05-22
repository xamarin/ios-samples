using System;
using System.Linq;
using AudioToolbox;

namespace FilterDemoFramework {
	public enum FilterParam {
		Cutoff = 0,
		Resonance = 1
	}

	public static class Extensions {
		public static float ConvertBadValuesToZero (this float x) 
		{
			var absx = Math.Abs (x);
			return (absx > 1e-15 && absx < 1e15) ? x : 0f;
		}

		public static double Squared (this double x)
		{
			return Math.Pow (x, 2);
		}

		public static float Clamp (this float input, float low, float high)
		{
			return Math.Min (Math.Max (input, low), high);
		}
	}

	public class FilterDSPKernel : DSPKernel {
		public class FilterState {
			public float X1 { get; set; }

			public float X2 { get; set; }

			public float Y1 { get; set; }

			public float Y2 { get; set; }

			public void Clear ()
			{
				X1 = X2 = Y1 = Y2 = 0;
			}

			public void ConvertBadStateValuesToZero ()
			{
				X1 = X1.ConvertBadValuesToZero ();
				X2 = X2.ConvertBadValuesToZero ();
				Y1 = Y1.ConvertBadValuesToZero ();
				Y2 = Y2.ConvertBadValuesToZero ();
			}
		}

		public class BiquadCoefficients {
			public float A1 { get; private set; }

			public float A2 { get; private set; }

			public float B0 { get; private set; }

			public float B1 { get; private set; }

			public float B2 { get; private set; }

			public void CalculateLopassParams (float frequency, float resonance)
			{
				var r = (float)Math.Pow (10, 0.05 * -resonance);

				float k = 0.5f * r * (float)Math.Sin (Math.PI * frequency);
				float c1 = (1.0f - k) / (1.0f + k);
				float c2 = (1.0f + c1) * (float)Math.Cos (Math.PI * frequency);
				float c3 = (1.0f + c1 - c2) * 0.25f;

				B0 = c3;
				B1 = 2.0f * c3;
				B2 = c3;
				A1 = -c2;
				A2 = c1;
			}

			public double GetMagnitude (double inFreq)
			{
				double zReal = Math.Cos (Math.PI * inFreq);
				double zImaginary = Math.Sin (Math.PI * inFreq);

				double numeratorReal = (B0 * (zReal.Squared () - zImaginary.Squared ())) + (B1 * zReal) + B2;
				double numeratorImaginary = (2.0 * B0 * zReal * zImaginary) + (B1 * zImaginary);
				double numeratorMagnitude = Math.Sqrt (numeratorReal.Squared () + numeratorImaginary.Squared ());

				double denominatorReal = zReal.Squared () - zImaginary.Squared () + (A1 * zReal) + A2;
				double denominatorImaginary = (2 * zReal * zImaginary) + (A1 * zImaginary);
				double denominatorMagnitude = Math.Sqrt (denominatorReal.Squared () + denominatorImaginary.Squared ());

				double response = numeratorMagnitude / denominatorMagnitude;

				return response;
			}
		}

		public ParameterRamper CutoffRamper = new ParameterRamper (400f / 44100f);
		public ParameterRamper ResonanceRamper = new ParameterRamper (20f);

		FilterState[] channelStates;
		readonly BiquadCoefficients coeffs = new BiquadCoefficients ();

		float sampleRate = 44100f;
		float nyquist;
		float inverseNyquist;

		AudioBuffers inBufferList;
		AudioBuffers outBufferList;

		public void Init (int channelCount, double inSampleRate)
		{
			channelStates = Enumerable.Range (0, channelCount)
									  .Select (i => new FilterState ())
									  .ToArray ();

			sampleRate = (float)inSampleRate;
			nyquist = 0.5f * sampleRate;
			inverseNyquist = 1f / nyquist;
		}

		public void Reset ()
		{
			foreach (var state in channelStates)
				state.Clear ();
		}

		public void SetParameter (ulong  address, float value)
		{
			switch (address) {
			case (ulong)FilterParam.Cutoff:
				CutoffRamper.Set ((value * inverseNyquist).Clamp (0f, 0.99f));
				break;
			case (ulong)FilterParam.Resonance:
				ResonanceRamper.Set (value.Clamp (-20f, 20f));
				break;
			}
		}

		public float GetParameter (nuint address)
		{
			switch (address) {
			case (ulong)FilterParam.Cutoff:
				return CutoffRamper.Goal * nyquist;
			case (ulong)FilterParam.Resonance:
				return ResonanceRamper.Goal;
			default:
				return 0.0f;
			}
		}

		public override void StartRamp (ulong address, float value, int duration)
		{
			switch (address) {
			case (ulong)FilterParam.Cutoff:
				CutoffRamper.StartRamp ((value * inverseNyquist).Clamp (0f, 0.99f), duration);
				break;
			case (ulong)FilterParam.Resonance:
				ResonanceRamper.StartRamp (value.Clamp (-20f, 20f), duration);
				break;
			}
		}

		public void SetBuffers (AudioBuffers inBufferList, AudioBuffers outBufferList)
		{
			this.inBufferList = inBufferList;
			this.outBufferList = outBufferList;
		}

		public override void Process (int frameCount, int bufferOffset)
		{
			int channelCount = channelStates.Length;

			for (int frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
				float cutoff = CutoffRamper.GetStep ();
				float resonance = ResonanceRamper.GetStep ();
				coeffs.CalculateLopassParams (cutoff, resonance);

				int frameOffset = frameIndex + bufferOffset;

				for (int channel = 0; channel < channelCount; ++channel) {
					FilterState state = channelStates [channel];

					unsafe {
						float * In = (float *)inBufferList [channel].Data + frameOffset;
						float * Out = (float *)outBufferList [channel].Data + frameOffset;

						float x0 = *In;
						float y0 = (coeffs.B0 * x0) + (coeffs.B1 * state.X1) + (coeffs.B2 * state.X2) -
							(coeffs.A1 * state.Y1) - (coeffs.A2 * state.Y2);
						*Out = y0;

						state.X2 = state.X1;
						state.X1 = x0;
						state.Y2 = state.Y1;
						state.Y1 = y0;
					}
				}
			}

			foreach (var channel in channelStates)
				channel.ConvertBadStateValuesToZero ();
		}
	}
}

