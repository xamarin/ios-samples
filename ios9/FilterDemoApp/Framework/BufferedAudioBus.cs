using AudioToolbox;
using AudioUnit;
using AVFoundation;
using Foundation;

namespace FilterDemoFramework {
	public class BufferedAudioBus {
		AVAudioPcmBuffer pcmBuffer;

		protected AudioBuffers OriginalAudioBufferList { get; private set; }

		public AUAudioUnitBus Bus { get; private set; }

		protected uint MaxFrames { get; private set; }

		public AudioBuffers MutableAudioBufferList { get; set; }

		public void Init (AVAudioFormat defaultFormat, uint maxChannels)
		{
			MaxFrames = 0;
			pcmBuffer = null;
			OriginalAudioBufferList = null;
			MutableAudioBufferList = null;
			NSError error;

			Bus = new AUAudioUnitBus (defaultFormat, out error) {
				MaximumChannelCount = maxChannels
			};
		}

		public void AllocateRenderResources (uint inMaxFrames)
		{
			MaxFrames = inMaxFrames;
			pcmBuffer = new AVAudioPcmBuffer (Bus.Format, MaxFrames);

			OriginalAudioBufferList = pcmBuffer.AudioBufferList;
			MutableAudioBufferList = pcmBuffer.MutableAudioBufferList;
		}

		public void DeallocateRenderResources ()
		{
			pcmBuffer = null;
			OriginalAudioBufferList = null;
			MutableAudioBufferList = null;
		}
	}
}

