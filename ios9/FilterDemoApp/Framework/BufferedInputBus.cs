using AudioUnit;
using AudioToolbox;

namespace FilterDemoFramework {
	public class BufferedInputBus : BufferedAudioBus {
		public AudioUnitStatus PullInput (ref AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timestamp, uint frameCount, int inputBusNumber, AURenderPullInputBlock pullInputBlock)
		{
			if (pullInputBlock == null)
				return AudioUnitStatus.NoConnection;

			PrepareInputBufferList ();
			AudioUnitStatus s = pullInputBlock (ref actionFlags, ref timestamp, frameCount, inputBusNumber, MutableAudioBufferList);
			return s;
		}

		void PrepareInputBufferList ()
		{
			uint byteSize = MaxFrames * sizeof(float);

			MutableAudioBufferList = new AudioBuffers (OriginalAudioBufferList.Count);

			for (int i = 0; i < OriginalAudioBufferList.Count; ++i) {
				MutableAudioBufferList[i] = new AudioBuffer {
					Data = OriginalAudioBufferList [i].Data,
					DataByteSize = (int)byteSize,
					NumberChannels = OriginalAudioBufferList [i].NumberChannels
				};
			}
		}
	}
}

