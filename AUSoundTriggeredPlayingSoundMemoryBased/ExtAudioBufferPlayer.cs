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
        const int PlayingDuration = SampleRate * 2;
        // 2sec
        const int Threshold = 100000;
        readonly CFUrl url;
        const int SampleRate = 44100;

        AudioComponent audioComponent;
		AudioUnit.AudioUnit audioUnit;
        ExtAudioFile extAudioFile;
        AudioBuffers buffer;
        AudioStreamBasicDescription srcFormat;
        AudioStreamBasicDescription dstFormat;
        long totalFrames;
        uint currentFrame;
        int numberOfChannels;
        int triggered;
        float signalLevel;

        public long TotalFrames
        {
            get { return totalFrames; }
        }

        public long CurrentPosition
        {
            set
            {
                long frame = value;
                frame = Math.Min(frame, totalFrames);
                frame = Math.Max(frame, 0);
				currentFrame = (uint)frame;
            }
            get
            {
                return currentFrame;
            }
        }

        public float SignalLevel
        {
			get { return signalLevel; }
        }

        public ExtAudioBufferPlayer(CFUrl url)
        {
			this.url = url;

			AudioSession.Initialize ();
			AudioSession.Category = AudioSessionCategory.PlayAndRecord; // TODO: we need to play only here
			AudioSession.Resumed += OnAudioSessionResumed;
			AudioSession.PreferredHardwareIOBufferDuration = 0.005f;

			// All iPhones and iPads have microphones, but early iPod touches did not
			if (!AudioSession.AudioInputAvailable) {
				var noInputAlert = new UIAlertView ("No audio input", "No audio input device is currently attached", null, "Ok");
				noInputAlert.Show ();
				return;
			}

			// Getting the hardware sample rate
			double hardwareSampleRate = AudioSession.CurrentHardwareSampleRate;
			Console.WriteLine ("hardwareSampleRate = {0}", hardwareSampleRate);

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
				SampleRate = hardwareSampleRate,
				Format = AudioFormatType.LinearPCM,
//				FormatFlags = AudioFormatFlags.IsSignedInteger,
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

			audioUnit.Initialize ();
			audioUnit.Start ();

            PrepareExtAudioFile();
//            PrepareAudioUnit();
        }
			
		void OnAudioSessionResumed (object sender, EventArgs e)
		{
			AudioSession.SetActive (true);
			audioUnit.Initialize ();
			audioUnit.Start ();
		}

        AudioUnitStatus RenderCallback(AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, AudioBuffers data)
		{
			audioUnit.Render (ref actionFlags, timeStamp, 1, numberFrames, data);

//			int bytesPerChannel = dstFormat.BytesPerFrame / dstFormat.ChannelsPerFrame;
//			for (int bufCount = 0; bufCount < data.Count; bufCount++) {
//				AudioBuffer buf = data [bufCount];
//
//				int currentFrame = 0;
//				while (currentFrame < numberFrames) {
//					// Copy sample to buffer, across all channels
//					for (int currentChannel = 0; currentChannel < buf.NumberChannels; currentChannel++) {
//						int offset = currentFrame * dstFormat.BytesPerFrame + currentChannel * bytesPerChannel;
//
//						short sample = buf.ReadInt16 (offset);
//						double tetha = state.SinPhase * Math.PI * 2;
//						sample = (Int16)(Math.Sin (tetha) * sample);
//						buf.WriteInt16 (offset, sample);
//					}
//
//					currentFrame++;
//				}
//			}

			return AudioUnitStatus.OK;
			/*
			// getting microphone input signal
			audioUnit.Render (ref actionFlags,
				timeStamp,
				1, // Remote input
				numberFrames,
				data);

			// Getting a pointer to a buffer to be filled
			IntPtr outL = data [0].Data;
			IntPtr outR = data [1].Data;

			// Getting signal level and trigger detection
			unsafe {
				var outLPtr = (int*)outL.ToPointer ();
				for (int i = 0; i < numberFrames; i++) {
					// LPF
					float diff = Math.Abs (*outLPtr) - signalLevel;
					if (diff > 0)
						signalLevel += diff / 1000f;
					else
						signalLevel += diff / 10000f;

					diff = Math.Abs (diff);

					// sound triger detection
					Console.WriteLine ("triggered {0}", triggered);
					Console.WriteLine ("diff > Threshold {0}", (int)diff > Threshold);
//					if (triggered <= 0 && diff > Threshold) {
						Console.WriteLine ("triggered = PlayingDuration");
						triggered = PlayingDuration;
//					}
				}
			}

			// playing sound
			unsafe {
				var outLPtr = (int*)outL.ToPointer ();
				var outRPtr = (int*)outR.ToPointer ();

				for (int i = 0; i < numberFrames; i++) {
//					triggered = Math.Max (0, triggered - 1);
//
//					if (triggered <= 0) {
//						// 0-filling
//						*outLPtr++ = 0;
//						*outRPtr++ = 0;
//					} else {
						var buf0 = (int*)buffer [0].Data;
						var buf1 = (numberOfChannels == 2) ? (int*)buffer [1].Data : buf0;

						if (currentFrame >= totalFrames)
							currentFrame = 0;

						++currentFrame;
						*outLPtr++ = buf0 [currentFrame];
						*outRPtr++ = buf1 [currentFrame];
//					}
				}
			}

			return AudioUnitStatus.NoError;
			*/
		}

        void PrepareExtAudioFile()
        {
			extAudioFile = ExtAudioFile.OpenUrl(url);
			CheckValue (extAudioFile, "ExtAudioFile.OpenUrl failed");

			srcFormat = extAudioFile.FileDataFormat;

            // Setting the channel number of the output format same to the input format
//			dstFormat = AudioStreamBasicDescription.CreateLinearPCM(channelsPerFrame: (uint)srcFormat.ChannelsPerFrame, bitsPerChannel: 16);
//            dstFormat.FormatFlags |= AudioFormatFlags.IsNonInterleaved;

//			Console.WriteLine (dstFormat);
//			dstFormat = new AudioStreamBasicDescription {
//				SampleRate = AudioSession.CurrentHardwareSampleRate,
//				Format = AudioFormatType.LinearPCM,
//				FormatFlags = AudioFormatFlags.IsSignedInteger | AudioFormatFlags.IsNonInterleaved,
//				BytesPerPacket = 4,
//				FramesPerPacket = 1,
//				BytesPerFrame = 4,
//				ChannelsPerFrame = 2,
//				BitsPerChannel = 16
//			};
//			Console.WriteLine (dstFormat.SampleRate);

			// This is how you say,“When you convert the data, this is the format I’d like to receive.”
			// The client data format must be PCM. In other words, you can’t use a single ExtAudioFile to convert between two compressed formats.
            extAudioFile.ClientDataFormat = dstFormat;

            // getting total frame
			totalFrames = extAudioFile.FileLengthFrames;

            // Allocating AudioBufferList
			buffer = new AudioBuffers(srcFormat.ChannelsPerFrame);
            for (int i = 0; i < buffer.Count; ++i)
            {
                int size = (int)(sizeof(int) * totalFrames);
                buffer.SetData(i, Marshal.AllocHGlobal(size), size);
            }
			numberOfChannels = srcFormat.ChannelsPerFrame;

            // Reading all frame into the buffer
            ExtAudioFileError status;
            extAudioFile.Read((uint)totalFrames, buffer, out status);
            if (status != ExtAudioFileError.OK)
                throw new ApplicationException();
        }

        void PrepareAudioUnit()
        {
            // Updated for deprecated AudioSession
            NSError error;
			AudioUnitStatus status;

//			error = session.SetCategory(AVAudioSessionCategory.PlayAndRecord);
//			CheckError (error);
//
//			session.SetActive(true);
//			CheckError (error);

            // Getting AudioComponent Remote output
			audioComponent = AudioComponent.FindComponent(AudioTypeOutput.Remote);
			CheckValue (audioComponent);

            // creating an audio unit instance
			audioUnit = new AudioUnit.AudioUnit(audioComponent);

            // turning on microphone
			status = audioUnit.SetEnableIO(true,
                AudioUnitScopeType.Input,
                1 // Remote Input
            );
			CheckStatus (status);

			status = audioUnit.SetEnableIO(true,
				AudioUnitScopeType.Output,
				0 // Remote Input
			);
			CheckStatus (status);


            // setting audio format
            audioUnit.SetAudioFormat(dstFormat,
				AudioUnitScopeType.Output,
                1 // Remote Output
            );
			CheckStatus (status);

//			AudioStreamBasicDescription format = AudioStreamBasicDescription.CreateLinearPCM(SampleRate, bitsPerChannel: 32);
//            format.FormatFlags = AudioStreamBasicDescription.AudioFormatFlagsNativeFloat;
			audioUnit.SetAudioFormat(dstFormat, AudioUnitScopeType.Output, 1);

            // setting callback method
			status = audioUnit.SetRenderCallback(RenderCallback, AudioUnitScopeType.Input, 0);
			CheckStatus (status);

			status = (AudioUnitStatus)audioUnit.Initialize();
			CheckStatus (status);
            audioUnit.Start();
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
