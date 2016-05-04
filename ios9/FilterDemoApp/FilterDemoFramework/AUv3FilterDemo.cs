using System;
using System.Collections.Generic;
using System.Linq;
using AudioToolbox;
using AudioUnit;
using AVFoundation;
using Foundation;

namespace FilterDemoFramework {
	public class AUv3FilterDemo : AUAudioUnit {

		readonly AUAudioUnitBus outputBus;
		BufferedInputBus inputBus = new BufferedInputBus ();

		public FilterDSPKernel Kernel { get; } = new FilterDSPKernel ();

		public override AUParameterTree ParameterTree { get; set; }

		AUAudioUnitBusArray inputBusArray;
		public override AUAudioUnitBusArray InputBusses {
			get {
				return inputBusArray;
			}
		}

		AUAudioUnitBusArray outputBusArray;
		public override AUAudioUnitBusArray OutputBusses {
			get {
				return outputBusArray;
			}
		}

		public override AUInternalRenderBlock InternalRenderBlock {
			get {
				return InternalRenderBlockProc;
			}
		}

		public AUv3FilterDemo (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithComponentDescription:options:error:")]
		public AUv3FilterDemo (AudioComponentDescription description, AudioComponentInstantiationOptions options, out NSError error) :
			base (description, options, out error)
		{
			var defaultFormat = new AVAudioFormat (44100.0, 2);

			Kernel.Init ((int)defaultFormat.ChannelCount, defaultFormat.SampleRate);

			AUParameter cutoffParam = AUParameterTree.CreateParameter (
				"cutoff", "Cutoff", 0, 12, 2000,
				AudioUnitParameterUnit.Hertz, null,
				0, null, null
			);

			AUParameter resonanceParam = AUParameterTree.CreateParameter (
				"resonance", "Resonance", 1, -20, 20,
				AudioUnitParameterUnit.Decibels, null,
				0, null, null
			);

			cutoffParam.Value = 400f;
			resonanceParam.Value = -5.0f;
			Kernel.SetParameter ((ulong)FilterParam.Cutoff, cutoffParam.Value);
			Kernel.SetParameter ((ulong)FilterParam.Resonance, resonanceParam.Value);

			ParameterTree = AUParameterTree.CreateTree (
				new [] {
					cutoffParam,
					resonanceParam
				}
			);

			inputBus.Init (defaultFormat, 8);

			NSError err;
			outputBus = new AUAudioUnitBus (defaultFormat, out err);

			inputBusArray = new AUAudioUnitBusArray (this, AUAudioUnitBusType.Input, new [] { inputBus.Bus });
			outputBusArray = new AUAudioUnitBusArray (this, AUAudioUnitBusType.Output, new [] { outputBus });

			var filterKernel = Kernel;

			ParameterTree.ImplementorValueObserver = (param, value) => filterKernel.SetParameter (param.Address, value);
			ParameterTree.ImplementorValueProvider = param => filterKernel.GetParameter ((nuint)param.Address);
			ParameterTree.ImplementorStringFromValueCallback = (AUParameter param, ref float? value) => {
				switch (param.Address) {
				case (ulong)FilterParam.Cutoff:
					return param.Value.ToString ();
				case (ulong)FilterParam.Resonance:
					return param.Value.ToString ();
				default:
					return "?";
				}
			};

			MaximumFramesToRender = 512;
		}

		public override bool AllocateRenderResources (out NSError outError)
		{
			if (!base.AllocateRenderResources (out outError))
				return false;

			if (outputBus.Format.ChannelCount != inputBus.Bus.Format.ChannelCount) {
				if (outError != null)
					outError = NSError.FromDomain (NSError.OsStatusErrorDomain, (int)AudioUnitStatus.FailedInitialization);

				return false;
			}

			inputBus.AllocateRenderResources (MaximumFramesToRender);

			Kernel.Init ((int)outputBus.Format.ChannelCount, outputBus.Format.SampleRate);
			Kernel.Reset ();

			var scheduleParameter = ScheduleParameterBlock; 
			var rampTime = 0.02 * outputBus.Format.SampleRate;

			ParameterTree.ImplementorValueObserver = (param, val) =>
				scheduleParameter (AUEventSampleTime.Immediate, (uint)rampTime, param.Address, val);

			return true;
		}

		public override void DeallocateRenderResources ()
		{
			base.DeallocateRenderResources ();

			inputBus.DeallocateRenderResources ();
			var filterKernel = Kernel;

			ParameterTree.ImplementorValueObserver = (param, val) => filterKernel.SetParameter (param.Address, val);
		}

		public AudioUnitStatus InternalRenderBlockProc (ref AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timestamp, uint frameCount, nint outputBusNumber, AudioBuffers outputData, AURenderEventEnumerator realtimeEventListHead, AURenderPullInputBlock pullInputBlock)
		{
			var transportStateFlags = (AUHostTransportStateFlags)0;

			double currentSamplePosition = 0;
			double cycleStartBeatPosition = 0;
			double cycleEndBeatPosition = 0;

			var callBack = TransportStateBlock;
			if(callBack != null)
				callBack (ref transportStateFlags, ref currentSamplePosition, ref cycleStartBeatPosition, ref cycleEndBeatPosition);

			var state = Kernel;
			var input = inputBus;

			var pullFlags = (AudioUnitRenderActionFlags)0;
			AudioUnitStatus err = input.PullInput (ref pullFlags, timestamp, frameCount, 0, pullInputBlock);
			if (err != AudioUnitStatus.NoError)
				return err;

			AudioBuffers inAudioBufferList = input.MutableAudioBufferList;

			if (outputData [0].Data == IntPtr.Zero) {
				for (int i = 0; i < outputData.Count; i++)
					outputData.SetData (i, inAudioBufferList [i].Data);
			}

			state.SetBuffers (inAudioBufferList, outputData);
			state.ProcessWithEvents (timestamp, (int)frameCount, realtimeEventListHead);

			return AudioUnitStatus.NoError;
		}

		public double[] GetMagnitudes (double[] frequencies)
		{
			var coefficients = new FilterDSPKernel.BiquadCoefficients ();
			coefficients.CalculateLopassParams (Kernel.CutoffRamper.Goal, Kernel.ResonanceRamper.Goal);

			double inverseNyquist = 2.0 / outputBus.Format.SampleRate;
			return frequencies.Select (f => coefficients.GetMagnitude (f * inverseNyquist))
				              .ToArray ();
		}
	}
}

