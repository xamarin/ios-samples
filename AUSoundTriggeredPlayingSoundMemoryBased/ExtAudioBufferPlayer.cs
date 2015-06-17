using System;

using AudioToolbox;
using CoreFoundation;
using AudioUnit;
using System.Runtime.InteropServices;
using AVFoundation;
using UIKit;
using Foundation;

namespace AUSoundTriggeredPlayingSoundMemoryBased
{
    class ExtAudioBufferPlayer : IDisposable
    {
		const float PlayDurarion = 0.5f;
		const int FramesToPlay = (int) (SampleRate * PlayDurarion);
		const int Threshold = 30;
        const int SampleRate = 44100;
		readonly CFUrl url;

        AudioComponent audioComponent;
		AudioUnit.AudioUnit audioUnit;
        ExtAudioFile extAudioFile;
        AudioBuffers buffer;
        AudioStreamBasicDescription srcFormat;
        AudioStreamBasicDescription dstFormat;
        int numberOfChannels;
        int triggered;

		public long TotalFrames { get; private set; }

		long currentFrame;
        public long CurrentFrame
        {
            set
            {
                long frame = value;
                frame = Math.Max(frame, 0);
				currentFrame = frame % TotalFrames;
            }
            get
            {
                return currentFrame;
            }
        }

		public double SignalLevel { get; private set; }

		public ExtAudioBufferPlayer(CFUrl url)
		{
			this.url = url;

			AudioSession.Initialize ();
			AudioSession.Category = AudioSessionCategory.PlayAndRecord; // TODO: we need to play only here
			AudioSession.Resumed += OnAudioSessionResumed;

			PrepareAudioUnit ();
			PrepareExtAudioFile();

			audioUnit.Initialize ();
			audioUnit.Start ();
		}

		void OnAudioSessionResumed (object sender, EventArgs e)
		{
			AudioSession.SetActive (true);
			audioUnit.Initialize ();
			audioUnit.Start ();
		}

        AudioUnitStatus RenderCallback(AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, AudioBuffers data)
		{
			// getting microphone input signal
			audioUnit.Render (ref actionFlags, timeStamp, 1, numberFrames, data);

			// Getting a pointer to a buffer to be filled
			IntPtr outL = data [0].Data;
			IntPtr outR = data [1].Data;

			// Getting signal level
			// https://en.wikipedia.org/wiki/Root_mean_square
			float sqrSum = 0;
			for (int j = 0;  j < numberFrames; j++) {
				float v = Marshal.ReadInt16(outL, j * sizeof(Int16));
				sqrSum += (v * v);
			}
			SignalLevel = (float)Math.Sqrt (sqrSum / numberFrames);

			if (triggered <= 0 && SignalLevel > Threshold)
				triggered = FramesToPlay;

			// playing sound
			unsafe {
				var outLPtr = (int*)outL.ToPointer ();
				var outRPtr = (int*)outR.ToPointer ();

				for (int i = 0; i < numberFrames; i++) {
					triggered = Math.Max (0, triggered - 1);

					if (triggered > 0) {
						var buf0 = (int*)buffer [0].Data;
						var buf1 = (int*)buffer [numberOfChannels - 1].Data;

						++CurrentFrame;
						*outLPtr++ = buf0 [currentFrame];
						*outRPtr++ = buf1 [currentFrame];
					} else {
						// 0-filling
						*outLPtr++ = 0;
						*outRPtr++ = 0;
					}
				}
			}

			return AudioUnitStatus.NoError;
		}

        void PrepareExtAudioFile()
        {
			extAudioFile = ExtAudioFile.OpenUrl(url);
			CheckValue (extAudioFile, "ExtAudioFile.OpenUrl failed");

			srcFormat = extAudioFile.FileDataFormat;

			// This is how you say,“When you convert the data, this is the format I’d like to receive.”
			// The client data format must be PCM. In other words, you can’t use a single ExtAudioFile to convert between two compressed formats.
            extAudioFile.ClientDataFormat = dstFormat;

            // getting total frame
			TotalFrames = extAudioFile.FileLengthFrames;

            // Allocating AudioBufferList
			buffer = new AudioBuffers(srcFormat.ChannelsPerFrame);
            for (int i = 0; i < buffer.Count; ++i)
            {
                int size = (int)(sizeof(int) * TotalFrames);
                buffer.SetData(i, Marshal.AllocHGlobal(size), size);
            }
			numberOfChannels = srcFormat.ChannelsPerFrame;

            // Reading all frame into the buffer
            ExtAudioFileError status;
            extAudioFile.Read((uint)TotalFrames, buffer, out status);
            if (status != ExtAudioFileError.OK)
                throw new ApplicationException();
        }

		void PrepareAudioUnit()
		{
			// All iPhones and iPads have microphones, but early iPod touches did not
			if (!AudioSession.AudioInputAvailable) {
				var noInputAlert = new UIAlertView ("No audio input", "No audio input device is currently attached", null, "Ok");
				noInputAlert.Show ();
				return;
			}

			// Getting AudioComponent Remote output
			audioComponent = AudioComponent.FindComponent(AudioTypeOutput.Remote);
			CheckValue (audioComponent);

			// creating an audio unit instance
			audioUnit = new AudioUnit.AudioUnit(audioComponent);

			AudioUnitStatus status;
			status = audioUnit.SetEnableIO(true, AudioUnitScopeType.Input, 1);
			CheckStatus (status);
			status = audioUnit.SetEnableIO(true, AudioUnitScopeType.Output, 0);
			CheckStatus (status);

			dstFormat = new AudioStreamBasicDescription {
				SampleRate = AudioSession.CurrentHardwareSampleRate,
				Format = AudioFormatType.LinearPCM,
				FormatFlags = AudioFormatFlags.IsSignedInteger | AudioFormatFlags.IsNonInterleaved,
				BytesPerPacket = 4,
				FramesPerPacket = 1,
				BytesPerFrame = 4,
				ChannelsPerFrame = 2,
				BitsPerChannel = 16
			};

			audioUnit.SetAudioFormat(dstFormat, AudioUnitScopeType.Input, 0);
			audioUnit.SetAudioFormat(dstFormat, AudioUnitScopeType.Output, 1);

			status = audioUnit.SetRenderCallback(RenderCallback, AudioUnitScopeType.Input, 0);
			CheckStatus (status);
		}

		void CheckValue (object value, string message = "")
		{
			if (value == null)
				throw new InvalidProgramException (message);
		}

		void CheckError (NSError error)
		{
			if (error != null)
				throw new InvalidProgramException (error.Description);
		}

		void CheckStatus (AudioUnitStatus status)
		{
			if (status != AudioUnitStatus.NoError)
				throw new InvalidProgramException ();
		}

        public void Dispose()
        {
            audioUnit.Stop();
            audioUnit.Dispose();
			extAudioFile.Dispose();
        }
    }
}
