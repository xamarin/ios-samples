using System;
using System.Diagnostics;
using Foundation;
using AudioUnit;
using CoreFoundation;
using AudioToolbox;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MultichannelMixer
{
	public class MultichannelMixerController : NSObject
	{
		class SoundBuffer
		{
			public IntPtr Data { get; set; }
			public long TotalFrames { get; set; }
			public int SampleNum { get; set; }
		}

		const double GraphSampleRate = 44100.0;

		AUGraph graph;
		CFUrl[] sourceURL;
		SoundBuffer[] soundBuffer;
		bool playing;
		AudioUnit.AudioUnit mixer;

		public MultichannelMixerController ()
		{
			// create the URLs we'll use for source A and B
			var sourceA = NSBundle.MainBundle.PathForResource ("GuitarMonoSTP", "aif");
			var sourceB = NSBundle.MainBundle.PathForResource ("DrumsMonoSTP", "aif");

			sourceURL = new [] {
				CFUrl.FromFile (sourceA),
				CFUrl.FromFile (sourceB)
			};

			soundBuffer = new SoundBuffer[sourceURL.Length];
			for (int i = 0; i < soundBuffer.Length; ++i)
				soundBuffer [i] = new SoundBuffer ();
		}

		public bool IsPlaying {
			get {
				return playing;
			}
		}

		public void InitializeAUGraph ()
		{
			Debug.Print ("Initialize");

			LoadFiles ();

			graph = new AUGraph ();

			// create two AudioComponentDescriptions for the AUs we want in the graph

			// output unit
			var outputNode = graph.AddNode (AudioComponentDescription.CreateOutput (AudioTypeOutput.Remote));

			// mixer node
			var mixerNode = graph.AddNode (AudioComponentDescription.CreateMixer (AudioTypeMixer.MultiChannel));

			// connect a node's output to a node's input
			if (graph.ConnnectNodeInput (mixerNode, 0, outputNode, 0) != AUGraphError.OK)
				throw new ApplicationException ();

			// open the graph AudioUnits are open but not initialized (no resource allocation occurs here)
			if (graph.TryOpen () != 0)
				throw new ApplicationException ();

			mixer = graph.GetNodeInfo (mixerNode);

			// set bus count
			const uint numbuses = 2;

			Debug.Print ("Set input bus count {0}", numbuses);

			if (mixer.SetElementCount (AudioUnitScopeType.Input, numbuses) != AudioUnitStatus.OK)
				throw new ApplicationException ();

			AudioStreamBasicDescription desc;

			for (uint i = 0; i < numbuses; ++i) {
				// setup render callback
				if (graph.SetNodeInputCallback (mixerNode, i, HandleRenderDelegate) != AUGraphError.OK)
					throw new ApplicationException ();

				// set input stream format to what we want
				desc = mixer.GetAudioFormat (AudioUnitScopeType.Input, i);
				//desc.ChangeNumberChannels(2, false);
				desc.SampleRate = GraphSampleRate;

				mixer.SetAudioFormat (desc, AudioUnitScopeType.Input, i);
			}

			// set output stream format to what we want
			desc = mixer.GetAudioFormat (AudioUnitScopeType.Output);

			//desc.ChangeNumberChannels(2, false);
			desc.SampleRate = GraphSampleRate;

			mixer.SetAudioFormat (desc, AudioUnitScopeType.Output);

			// now that we've set everything up we can initialize the graph, this will also validate the connections
			if (graph.Initialize () != AUGraphError.OK)
				throw new ApplicationException ();
		}			

		// load up audio data from the demo files into mSoundBuffer.data used in the render proc
		void LoadFiles ()
		{
			const int FilesCount = 2;

			for (int i = 0; i < FilesCount; i++) {
				Debug.Print ("Loading file #{0}", i);

				using (var file = ExtAudioFile.OpenUrl (sourceURL [i])) {

					var clientFormat = file.FileDataFormat;
					clientFormat.FormatFlags = AudioStreamBasicDescription.AudioFormatFlagsAudioUnitCanonical;
					clientFormat.ChannelsPerFrame = 1;
					clientFormat.FramesPerPacket = 1;
					clientFormat.BitsPerChannel = 8 * sizeof (int);
					clientFormat.BytesPerPacket =
						clientFormat.BytesPerFrame = clientFormat.ChannelsPerFrame * sizeof (int);

					file.ClientDataFormat = clientFormat;

					// set the client format to be what we want back
					double rateRatio = GraphSampleRate / clientFormat.SampleRate;

					var numFrames = file.FileLengthFrames;
					numFrames = (uint)(numFrames * rateRatio); // account for any sample rate conversion
					Debug.Print ("Number of Sample Frames after rate conversion (if any): {0}", numFrames);

					// set up our buffer
					soundBuffer[i].TotalFrames = numFrames;

					UInt32 samples = (uint) (numFrames * clientFormat.ChannelsPerFrame);
					var data_size = (int)(sizeof(uint) * samples);
					soundBuffer[i].Data = Marshal.AllocHGlobal (data_size);

					// set up a AudioBufferList to read data into
					var bufList = new AudioBuffers (1);
					bufList [0] = new AudioBuffer {
						NumberChannels = 1,
						Data = soundBuffer [i].Data,
						DataByteSize = data_size
					};

					ExtAudioFileError error;
					file.Read ((uint) numFrames, bufList, out error);
					if (error != ExtAudioFileError.OK)
						throw new ApplicationException ();
				}
			}
		}

		unsafe AudioUnitStatus HandleRenderDelegate (AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, AudioBuffers data)
		{
			var sndbuf = soundBuffer [busNumber];

			var sample = sndbuf.SampleNum;      // frame number to start from
			var bufSamples = sndbuf.TotalFrames;  // total number of frames in the sound buffer
			var input = (int*) sndbuf.Data;

			var outA = (int*) data [0].Data; // output audio buffer for L channel
			var outB = (int*) data [1].Data; // output audio buffer for R channel

			// for demonstration purposes we've configured 2 stereo input busses for the mixer unit
			// but only provide a single channel of data from each input bus when asked and silence for the other channel
			// alternating as appropriate when asked to render bus 0 or bus 1's input
			for (var i = 0; i < numberFrames; ++i) {

				if (busNumber == 1) {
					outA [i] = 0;
					outB [i] = input [sample++];
				} else {
					outA [i] = input[sample++];
					outB [i] = 0;
				}

				if (sample > bufSamples) {
					// start over from the beginning of the data, our audio simply loops
					Debug.Print ("Looping data for bus {0} after {1} source frames rendered", busNumber, sample - 1);
					sample = 0;
				}
			}

			// keep track of where we are in the source data buffer
			sndbuf.SampleNum = sample;

			return AudioUnitStatus.OK;
		}

		public void Start ()
		{
			if (graph.Start () != AUGraphError.OK)
				throw new ApplicationException ();

			playing = true;
		}

		public void Stop ()
		{
			if (graph.IsRunning) {
				if (graph.Stop () != AUGraphError.OK)
					throw new ApplicationException ();

				playing = false;
			}
		}

		public void EnableInput (int inputNum, bool isOn)
		{
			Debug.Print ("BUS {0} isOn {1}", inputNum, isOn);
			if (mixer.SetParameter (AudioUnitParameterType.MultiChannelMixerEnable, Convert.ToSingle (isOn), AudioUnitScopeType.Input, (uint) inputNum) != AudioUnitStatus.OK)
				throw new ApplicationException ();
		}

		public void SetInputVolume (int inputNum, float value)
		{
			if (mixer.SetParameter (AudioUnitParameterType.MultiChannelMixerVolume, value, AudioUnitScopeType.Input, (uint) inputNum) != AudioUnitStatus.OK)
				throw new ApplicationException ();
		}

		public void SetOutputVolume (float value)
		{
			if (mixer.SetParameter (AudioUnitParameterType.MultiChannelMixerVolume, value, AudioUnitScopeType.Output) != AudioUnitStatus.OK)
				throw new ApplicationException ();
		}
	}
}

