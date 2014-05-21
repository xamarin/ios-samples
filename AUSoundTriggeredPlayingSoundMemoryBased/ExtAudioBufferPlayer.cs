using System;
using System.Collections.Generic;
using System.Text;

using AudioToolbox;
using CoreFoundation;
using AudioUnit;
using System.Runtime.InteropServices;

namespace AUSoundTriggeredPlayingSoundMemoryBased
{
	class ExtAudioBufferPlayer : IDisposable
	{
		const int _playingDuration = _sampleRate * 2; // 2sec
		const int _threshold = 100000;
		readonly CFUrl _url;
		const int _sampleRate = 44100;

		AudioComponent _audioComponent;
		AudioUnit.AudioUnit _audioUnit;
		ExtAudioFile _extAudioFile;
		AudioBuffers _buffer;
		AudioStreamBasicDescription _srcFormat;
		AudioStreamBasicDescription _dstFormat;
		long _totalFrames;
		uint _currentFrame;
		int  _numberOfChannels;
		int  _triggered;
		float _signalLevel;

		public long TotalFrames {
			get { return _totalFrames; }
		}

		public long CurrentPosition {
			set {
				long frame = value;
				frame = Math.Min (frame, _totalFrames);
				frame = Math.Max (frame, 0);
				_currentFrame = (uint)frame;                
			}
			get {
				return _currentFrame;
			}
		}

		public float SignalLevel {
			get { return _signalLevel; }
		}

		public ExtAudioBufferPlayer (CFUrl url)
		{
			_url = url;

			prepareExtAudioFile ();
			prepareAudioUnit ();
		}

		AudioUnitStatus _audioUnit_RenderCallback (AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, AudioBuffers data)
		{
			// getting microphone input signal
			_audioUnit.Render (ref actionFlags,
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
					float diff = Math.Abs (*outLPtr) - _signalLevel;
					if (diff > 0)
						_signalLevel += diff / 1000f;
					else
						_signalLevel += diff / 10000f;
                    
					diff = Math.Abs (diff);
                    
					// sound triger detection
					if (_triggered <= 0 && diff > _threshold) {
						_triggered = _playingDuration;
					}
				}
			}                        

			// playing sound
			unsafe {
				var outLPtr = (int*)outL.ToPointer ();
				var outRPtr = (int*)outR.ToPointer ();                
                
				for (int i = 0; i < numberFrames; i++) {                    
					_triggered = Math.Max (0, _triggered - 1);

					if (_triggered <= 0) {
						// 0-filling
						*outLPtr++ = 0;
						*outRPtr++ = 0;
					} else {
						var buf0 = (int*)_buffer [0].Data;
						var buf1 = (_numberOfChannels == 2) ? (int*)_buffer [1].Data : buf0;

						if (_currentFrame >= _totalFrames) {
							_currentFrame = 0;
						}
                        
						++_currentFrame;
						*outLPtr++ = buf0 [_currentFrame];
						*outRPtr++ = buf1 [_currentFrame];
					}
				}
			}

			return AudioUnitStatus.NoError;
		}

		void prepareExtAudioFile ()
		{
			// Opening Audio File
			_extAudioFile = ExtAudioFile.OpenUrl (_url);

			// Getting file data format
			_srcFormat = _extAudioFile.FileDataFormat;

			// Setting the channel number of the output format same to the input format
			_dstFormat = AudioStreamBasicDescription.CreateLinearPCM (channelsPerFrame: (uint)_srcFormat.ChannelsPerFrame, bitsPerChannel: 32);
			_dstFormat.FormatFlags |= AudioFormatFlags.IsNonInterleaved;

			// setting reading format as audio unit cannonical format
			_extAudioFile.ClientDataFormat = _dstFormat;

			// getting total frame
			_totalFrames = _extAudioFile.FileLengthFrames;

			// Allocating AudioBufferList
			_buffer = new AudioBuffers (_srcFormat.ChannelsPerFrame);
			for (int i = 0; i < _buffer.Count; ++i) {
				int size = (int)(sizeof(uint) * _totalFrames);
				_buffer.SetData (i, Marshal.AllocHGlobal (size), size);
			}
			_numberOfChannels = _srcFormat.ChannelsPerFrame;

			// Reading all frame into the buffer
			ExtAudioFileError status;
			_extAudioFile.Read ((uint)_totalFrames, _buffer, out status);
			if (status != ExtAudioFileError.OK)
				throw new ApplicationException ();
		}

		void prepareAudioUnit ()
		{
			// AudioSession
			AudioSession.Initialize ();
			AudioSession.SetActive (true);
			AudioSession.Category = AudioSessionCategory.PlayAndRecord;
			AudioSession.PreferredHardwareIOBufferDuration = 0.005f;            

			// Getting AudioComponent Remote output 
			_audioComponent = AudioComponent.FindComponent (AudioTypeOutput.Remote);

			// creating an audio unit instance
			_audioUnit = new AudioUnit.AudioUnit (_audioComponent);

			// turning on microphone
			_audioUnit.SetEnableIO (true,
                AudioUnitScopeType.Input,
                1 // Remote Input
			);

			// setting audio format
			_audioUnit.SetAudioFormat (_dstFormat, 
                AudioUnitScopeType.Input, 
                0 // Remote Output
			);  

			var format = AudioStreamBasicDescription.CreateLinearPCM (_sampleRate, bitsPerChannel: 32);
			format.FormatFlags = AudioStreamBasicDescription.AudioFormatFlagsAudioUnitCanonical;
			_audioUnit.SetAudioFormat (format, AudioUnitScopeType.Output, 1);

			// setting callback method
			_audioUnit.SetRenderCallback (_audioUnit_RenderCallback, AudioUnitScopeType.Global);

			_audioUnit.Initialize ();
			_audioUnit.Start ();
		}

		public void Dispose ()
		{
			_audioUnit.Stop (); 
			_audioUnit.Dispose ();
			_extAudioFile.Dispose ();            
		}
	}
}
