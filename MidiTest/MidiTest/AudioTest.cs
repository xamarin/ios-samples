using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using AudioUnit;
using CoreMidi;
using AudioToolbox;

using Foundation;
using CoreFoundation;
using UIKit;

namespace MidiTest
{
	public class AudioTest
	{
		AUGraph processingGraph;
		AudioUnit.AudioUnit samplerUnit;

		MidiClient virtualMidi;
		MidiEndpoint virtualEndpoint;
		UILabel label;

		bool CreateAUGraph ()
		{
			processingGraph = new AUGraph ();

			int samplerNode, ioNode;

			var musicSampler = new AudioComponentDescription () {
				ComponentManufacturer = AudioComponentManufacturerType.Apple,
				ComponentType = AudioComponentType.MusicDevice,
				ComponentSubType = (int)AudioTypeMusicDevice.Sampler
			};
			samplerNode = processingGraph.AddNode (musicSampler);

			var remoteOutput = new AudioComponentDescription () {
				ComponentManufacturer = AudioComponentManufacturerType.Apple,
				ComponentType = AudioComponentType.Output,
				ComponentSubType = (int)AudioTypeOutput.Remote
			};
			ioNode = processingGraph.AddNode (remoteOutput);

			processingGraph.Open ();

			processingGraph.ConnnectNodeInput (
				sourceNode: samplerNode,
				sourceOutputNumber: 0,
				destNode: ioNode,
				destInputNumber: 0);

			samplerUnit = processingGraph.GetNodeInfo (samplerNode);


			return true;
		}

		void ConfigureAndStartAudioProcessingGraph (AUGraph graph)
		{
			if (graph == null)
				return;

			var error = graph.Initialize ();
			if (error != AUGraphError.OK)
				throw new Exception ("Unable to initialize AUGraph object.  Error code: " + error);

			error = graph.Start ();
			if (error != AUGraphError.OK)
				throw new Exception ("Unable to start audio processing graph.  Error code: " + error);
		}

		void MidiMessageReceived (object sender, MidiPacketsEventArgs midiPacketArgs)
		{
			var packets = midiPacketArgs.Packets;

			for (int i = 0; i < packets.Length; i++) {
				var packet = packets [i];
				byte[] data = new byte[packet.Length];
				Marshal.Copy (packet.Bytes, data, 0, packet.Length);
				var midiStatus = data [0];
				var midiCommand = midiStatus >> 4;

				if (midiCommand == 0x09) {
					var note = data [1] & 0x7F;
					var velocity = data [2] & 0x7F;

					int noteNumber = ((int)note) % 12;
					string noteType;
					switch (noteNumber) {
					case 0:
						noteType = "C";
						break;
					case 1:
						noteType = "C#";
						break;
					case 2:
						noteType = "D";
						break;
					case 3:
						noteType = "D#";
						break;
					case 4:
						noteType = "E";
						break;
					case 5:
						noteType = "F";
						break;
					case 6:
						noteType = "F#";
						break;
					case 7:
						noteType = "G";
						break;
					case 8:
						noteType = "G#";
						break;
					case 9:
						noteType = "A";
						break;
					case 10:
						noteType = "Bb";
						break;
					case 11:
						noteType = "B";
						break;
					default:
						throw new NotImplementedException ();
					}

					samplerUnit.MusicDeviceMIDIEvent ((uint)midiStatus, (uint)note, (uint)velocity);

					label.InvokeOnMainThread (delegate {
						label.Text = String.Format ("Playing: {0}: {1}", noteType, noteNumber);
					});
				}
			}
		}

		AudioUnitStatus LoadFromDLSOrSoundFont (CFUrl bankUrl, int presetNumber)
		{
			var instrumentData = new SamplerInstrumentData (bankUrl, InstrumentType.SF2Preset) {
				PresetID = (byte)presetNumber,
				BankMSB = SamplerInstrumentData.DefaultMelodicBankMSB,
				BankLSB = SamplerInstrumentData.DefaultBankLSB
			};

			var result = samplerUnit.LoadInstrument (instrumentData, AudioUnitScopeType.Global, 0);
			if (result != AudioUnitStatus.NoError)
				Console.WriteLine (result.ToString ());

			return result;
		}

		public async void MidiTest (UILabel label)
		{
			this.label = label;
			CreateAUGraph ();
			ConfigureAndStartAudioProcessingGraph (processingGraph);

			virtualMidi = new MidiClient ("VirtualClient");
			virtualMidi.IOError += (object sender, IOErrorEventArgs e) => {
				Console.WriteLine ("IO Error, messageId={0}", e.ErrorCode);
			};

			virtualMidi.PropertyChanged += (object sender, ObjectPropertyChangedEventArgs e) => {
				Console.WriteLine ("Property changed: " + e.MidiObject + ", " + e.PropertyName);
			};

			MidiError error;
			virtualEndpoint = virtualMidi.CreateVirtualDestination ("Virtual Destination", out error);

			if (error != MidiError.Ok)
				throw new Exception ("Error creating virtual destination: " + error);
			virtualEndpoint.MessageReceived += MidiMessageReceived;

			var sequence = new MusicSequence ();

			var midiFilePath = NSBundle.MainBundle.PathForResource ("simpletest", "mid");
			var midiFileurl = NSUrl.FromFilename (midiFilePath);

			sequence.LoadFile (midiFileurl, MusicSequenceFileTypeID.Midi);

			var player = new MusicPlayer ();

			sequence.SetMidiEndpoint (virtualEndpoint);

			var presetUrl = CFUrl.FromFile (NSBundle.MainBundle.PathForResource ("Gorts_Filters", "sf2"));

			LoadFromDLSOrSoundFont (presetUrl, 10);

			player.MusicSequence = sequence;
			player.Preroll ();
			player.Start ();

			MusicTrack track;
			track = sequence.GetTrack (1);
			var length = track.TrackLength;

			while (true) {
				await Task.Delay (TimeSpan.FromSeconds (3));
				double now = player.Time;
				if (now > length)
					break;
			}

			player.Stop ();
			sequence.Dispose ();
			player.Dispose ();
			label.Text = "Done";
		}
	}
}

