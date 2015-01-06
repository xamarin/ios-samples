using System;

using Foundation;
using AVFoundation;
using MediaToolbox;
using AudioToolbox;

namespace AudioTapProcessor
{
	struct AVAudioTapProcessorContext
	{
		public bool SupportedTapProcessingFormat;

		public bool IsNonInterleaved;

		public double SampleRate;

		public double SampleCount;

		public float LeftChannelVolume;

		public float RightChannelVolume;

		public AudioUnit.AudioUnit AudioUnit;
	}
}