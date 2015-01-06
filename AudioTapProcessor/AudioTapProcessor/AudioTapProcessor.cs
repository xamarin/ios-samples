using System;
using System.Runtime.InteropServices;

using Foundation;
using AVFoundation;
using MediaToolbox;
using AudioToolbox;
using AudioUnit;
using CoreMedia;
using CoreFoundation;


namespace AudioTapProcessor
{
	public class AudioTapProcessor : NSObject
	{
		AVAssetTrack audioAssetTrack;
		MTAudioProcessingTap audioProcessingTap;
		AVAudioTapProcessorContext context;

		public MainViewController Controller { get; set; }

		public bool IsBandpassFilterEnabled { get; set; }

		float centerFrequency;

		public float CenterFrequency {
			get {
				return centerFrequency;
			}
			set {
				if (centerFrequency != value)
					UpdateCenterFrequency (centerFrequency = value);
			}
		}

		float bandwidth;

		public float Bandwidth {
			get {
				return bandwidth;
			}
			set {
				if (bandwidth != value)
					UpdateBandwidth (bandwidth = value);
			}
		}

		AVAudioMix audioMix;

		public AVAudioMix AudioMix {
			get {
				return audioMix = audioMix ?? CreateAudioMix ();
			}
		}

		public AudioTapProcessor (AVAssetTrack audioTrack)
		{
			if (audioTrack == null)
				throw new ArgumentNullException ("audioTrack");

			if (audioTrack.MediaType != AVMediaType.Audio)
				throw new ArithmeticException ("MediaType is not AVMediaType.Audio");

			audioAssetTrack = audioTrack;
			centerFrequency = 4980f / 23980f; // equals 5000 Hz (assuming sample rate is 48k)
			bandwidth = 500f / 11900f; // equals 600 Cents
		}

		unsafe AVAudioMix CreateAudioMix ()
		{
			AVMutableAudioMix audioMix = AVMutableAudioMix.Create ();
			AVMutableAudioMixInputParameters audioMixInputParameters = AVMutableAudioMixInputParameters.FromTrack (audioAssetTrack);
			var callbacks = new MTAudioProcessingTapCallbacks (TapProcess) {
				Initialize = TapInitialization,
				Finalize = Finalaze,
				Prepare = TapPrepare,
				Unprepare = Unprepare,
			};

			audioProcessingTap = new MTAudioProcessingTap (callbacks, MTAudioProcessingTapCreationFlags.PreEffects);
			audioMixInputParameters.AudioTapProcessor = audioProcessingTap;

			audioMix.InputParameters = new AVAudioMixInputParameters[] { audioMixInputParameters };

			return audioMix;
		}

		#region Update parameters

		#region CenterFrequency

		unsafe void UpdateCenterFrequency (float frequency)
		{
			// Update center frequency of bandpass filter Audio Unit.
			float newFrequency = CalcNewCenterFrequency ((float)context.SampleRate, frequency);
			SetCenterFrequency (newFrequency);
		}

		float CalcNewCenterFrequency (float sampleRate, float oldCenterFrequency)
		{
			// Global, Hz, 20->(SampleRate/2), 5000
			return 20 + ((sampleRate * 0.5f) - 20) * centerFrequency;
		}

		void SetCenterFrequency (float newFrequency)
		{
			if (context.AudioUnit == null)
				return;

			AudioUnitStatus status = context.AudioUnit.SetParameter (AudioUnitParameterType.BandpassCenterFrequency, newFrequency, AudioUnitScopeType.Global);
			if (status != AudioUnitStatus.NoError)
				Console.WriteLine ("AudioUnit.SetParameter(AudioUnitParameterType.BandpassCenterFrequency): {0}", status);
		}

		#endregion

		#region Bandwidth

		void UpdateBandwidth (float bandwidth)
		{
			// Update bandwidth of bandpass filter Audio Unit.
			float newBandwidth = CalcNewBandwidth (bandwidth);
			SetBandwidth (newBandwidth);
		}

		float CalcNewBandwidth (float bandwidth)
		{
			// Global, Cents, 100->12000, 600
			return 100 + 11900 * bandwidth;
		}

		void SetBandwidth (float newBandwidth)
		{
			if (context.AudioUnit == null)
				return;

			// Console.WriteLine ("newBandwidth={0}", newBandwidth);
			AudioUnitStatus status = context.AudioUnit.SetParameter (AudioUnitParameterType.BandpassBandwidth, newBandwidth, AudioUnitScopeType.Global);
			if (status != AudioUnitStatus.NoError)
				Console.WriteLine ("AudioUnit.SetParameter(AudioUnitParameterType.BandpassBandwidth): {0}", status);
		}

		#endregion

		#endregion

		#region MTAudioProcessingTap Callbacks

		unsafe void TapProcess (MTAudioProcessingTap tap, nint numberFrames, MTAudioProcessingTapFlags flags,
		                        AudioBuffers bufferList,
		                        out nint numberFramesOut,
		                        out MTAudioProcessingTapFlags flagsOut)
		{
			numberFramesOut = 0;
			flagsOut = (MTAudioProcessingTapFlags)0;

			// Skip processing when format not supported.
			if (!context.SupportedTapProcessingFormat) {
				Console.WriteLine ("Unsupported tap processing format.");
				return;
			}

			if (IsBandpassFilterEnabled) {
				// Apply bandpass filter Audio Unit.
				if (context.AudioUnit != null) {
					var audioTimeStamp = new AudioTimeStamp {
						SampleTime = context.SampleCount,
						Flags = AudioTimeStamp.AtsFlags.SampleTimeValid
					};

					var f = (AudioUnitRenderActionFlags)0;
					var status = context.AudioUnit.Render (ref f, audioTimeStamp, 0, (uint)numberFrames, bufferList);
					if (status != AudioUnitStatus.NoError) {
						Console.WriteLine ("AudioUnitRender(): {0}", status);
						return;
					}

					// Increment sample count for audio unit.
					context.SampleCount += numberFrames;

					// Set number of frames out.
					numberFramesOut = numberFrames;
				}
			} else {
				// Get actual audio buffers from MTAudioProcessingTap (AudioUnitRender() will fill bufferListInOut otherwise).
				CMTimeRange tr;
				MTAudioProcessingTapError status = tap.GetSourceAudio (numberFrames, bufferList, out flagsOut, out tr, out numberFramesOut);
				if (status != MTAudioProcessingTapError.None) {
					Console.WriteLine ("MTAudioProcessingTapGetSourceAudio: {0}", status);
					return;
				}
			}

			UpdateVolumes (bufferList, numberFrames);
		}

		unsafe void TapInitialization (MTAudioProcessingTap tap, out void* tapStorage)
		{
			context = new AVAudioTapProcessorContext {
				SupportedTapProcessingFormat = false,
				IsNonInterleaved = false,
				SampleRate = double.NaN,
				SampleCount = 0,
				LeftChannelVolume = 0,
				RightChannelVolume = 0
			};

			// We don't use tapStorage we store all data within context field
			tapStorage = (void*)IntPtr.Zero;
		}

		unsafe void Finalaze (MTAudioProcessingTap tap)
		{
		}

		unsafe void TapPrepare (MTAudioProcessingTap tap, nint maxFrames, ref AudioStreamBasicDescription processingFormat)
		{
			// Store sample rate for CenterFrequency property
			context.SampleRate = processingFormat.SampleRate;

			/* Verify processing format (this is not needed for Audio Unit, but for RMS calculation). */
			VerifyProcessingFormat (processingFormat);

			if (processingFormat.FormatFlags.HasFlag (AudioFormatFlags.IsNonInterleaved))
				context.IsNonInterleaved = true;

			/* Create bandpass filter Audio Unit */

			var audioComponentDescription = AudioComponentDescription.CreateEffect (AudioTypeEffect.BandPassFilter);
			// TODO: https://trello.com/c/GZUGUyH0
			var audioComponent = AudioComponent.FindNextComponent (null, ref audioComponentDescription);
			if (audioComponent == null)
				return;

			AudioUnitStatus error = AudioUnitStatus.NoError;
			AudioUnit.AudioUnit audioUnit = audioComponent.CreateAudioUnit ();
			try {
				audioUnit.SetAudioFormat (processingFormat, AudioUnitScopeType.Input);
				audioUnit.SetAudioFormat (processingFormat, AudioUnitScopeType.Output);
			} catch (AudioUnitException) {
				error = AudioUnitStatus.FormatNotSupported;
			}

			if (error == AudioUnitStatus.NoError)
				error = audioUnit.SetRenderCallback (Render, AudioUnitScopeType.Input);

			if (error == AudioUnitStatus.NoError)
				error = audioUnit.SetMaximumFramesPerSlice ((uint)maxFrames, AudioUnitScopeType.Global);

			if (error == AudioUnitStatus.NoError)
				error = (AudioUnitStatus)audioUnit.Initialize ();

			if (error != AudioUnitStatus.NoError) {
				audioUnit.Dispose ();
				audioUnit = null;
			}

			context.AudioUnit = audioUnit;
		}

		void VerifyProcessingFormat (AudioStreamBasicDescription processingFormat)
		{
			context.SupportedTapProcessingFormat = true;

			if (processingFormat.Format != AudioFormatType.LinearPCM) {
				Console.WriteLine ("Unsupported audio format for AudioProcessingTap. LinearPCM only.");
				context.SupportedTapProcessingFormat = false;
			}

			if (!processingFormat.FormatFlags.HasFlag (AudioFormatFlags.IsFloat)) {
				Console.WriteLine ("Unsupported audio format flag for AudioProcessingTap. Float only.");
				context.SupportedTapProcessingFormat = false;
			}
		}

		unsafe void Unprepare (MTAudioProcessingTap tap)
		{
			/* Release bandpass filter Audio Unit */

			if (context.AudioUnit == null)
				return;

			context.AudioUnit.Dispose ();
		}

		AudioUnitStatus Render (AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, AudioBuffers data)
		{
			// Just return audio buffers from MTAudioProcessingTap.
			MTAudioProcessingTapFlags flags;
			CMTimeRange range;
			nint n;
			var error = (AudioUnitStatus)(int)audioProcessingTap.GetSourceAudio ((nint)numberFrames, data, out flags, out range, out n);
			if (error != AudioUnitStatus.NoError)
				Console.WriteLine ("{0} audioProcessingTap.GetSourceAudio failed", error);
			return error;
		}

		#endregion

		unsafe void UpdateVolumes (AudioBuffers bufferList, nint numberFrames)
		{
			// Calculate root mean square (RMS) for left and right audio channel.
			// http://en.wikipedia.org/wiki/Root_mean_square
			for (int i = 0; i < bufferList.Count; i++) {
				AudioBuffer pBuffer = bufferList [i];
				long cSamples = numberFrames * (context.IsNonInterleaved ? 1 : pBuffer.NumberChannels);

				float* pData = (float*)(void*)pBuffer.Data;

				float rms = 0;
				for (int j = 0; j < cSamples; j++)
					rms += pData [j] * pData [j];

				if (cSamples > 0)
					rms = (float)Math.Sqrt (rms / cSamples);

				if (i == 0)
					context.LeftChannelVolume = rms;
				if (i == 1 || (i == 1 && bufferList.Count == 1))
					context.RightChannelVolume = rms;
			}

			// Pass calculated left and right channel volume to VU meters.
			UpdateVolumes (context.LeftChannelVolume, context.RightChannelVolume);
		}

		void UpdateVolumes (float leftVolume, float rightVolume)
		{
			// Forward left and right channel volume to Controller
			DispatchQueue.MainQueue.DispatchAsync (() => Controller.OnNewLeftRightChanelValue (this, leftVolume, rightVolume));
		}
	}
}