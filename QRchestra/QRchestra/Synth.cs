using System;
using UIKit;
using AudioUnit;
using Foundation;
using CoreFoundation;
using AVFoundation;
using CoreMidi;
using AudioToolbox;

namespace QRchestra
{
	public class Synth : UIViewController
	{
		enum MIDIMessage {
			NoteOn = 0x9,
			NoteOff = 0x8
		}

		const int LowNote = 48;
		const int HighNote = 72;
		const int MidNote = 60;

		double graphSampleRate;
		AUGraph processingGraph;
		AudioUnit.AudioUnit samplerUnit;
		AudioUnit.AudioUnit ioUnit;

		public Synth ()
		{
			bool audioSessionActivated = setupAudioSession ();
			if (!audioSessionActivated)
				throw new Exception ("Unable to set up audio session");

			createAUGraph ();
			configureAndStartAudioProcessingGraph (processingGraph);
		}

		bool createAUGraph ()
		{
			AUGraphError result = 0;
			int samplerNode, ioNode;

			var cd = new AudioComponentDescription () {
				ComponentManufacturer = AudioComponentManufacturerType.Apple,
				ComponentFlags = 0,
				ComponentFlagsMask = 0
			};

			processingGraph = new AUGraph ();

			cd.ComponentType = AudioComponentType.MusicDevice;
			cd.ComponentSubType = (int)AudioTypeMusicDevice.Sampler; //0x73616d70;
		
			samplerNode = processingGraph.AddNode (cd);

			cd.ComponentType = AudioComponentType.Output;
			cd.ComponentSubType = (int)AudioTypeOutput.Remote; //0x72696f63;

			ioNode = processingGraph.AddNode (cd);
		
			processingGraph.Open ();

			result = processingGraph.ConnnectNodeInput (samplerNode, 0, ioNode, 0);
			if (result != AUGraphError.OK)
				throw new Exception ("Unable to open the audio processing graph.  Error code: " + result);
			samplerUnit = processingGraph.GetNodeInfo (samplerNode);
			ioUnit = processingGraph.GetNodeInfo (ioNode);

			return true;
		}

		void configureAndStartAudioProcessingGraph (AUGraph graph)
		{
			int result = 0;
			uint framesPerSlice = 0;

			result = ioUnit.Initialize ();
			if (result != 0)
				throw new Exception ("Unable to Initialize the I/O unit.  Error code: " + result);

			var status = ioUnit.SetSampleRate (graphSampleRate, AudioUnitScopeType.Output);
			if (status != AudioUnitStatus.NoError)
				throw new Exception ("AudioUnitSetProperty (set Sample output stream sample rate).  Error code: " + (int)status);

			framesPerSlice = ioUnit.GetMaximumFramesPerSlice (AudioUnitScopeType.Global);

			samplerUnit.SetSampleRate (graphSampleRate, AudioUnitScopeType.Output);
			samplerUnit.SetMaximumFramesPerSlice (framesPerSlice, AudioUnitScopeType.Global);

			if (graph != null) {
				result = (int)graph.Initialize ();
				if (result != (int)AUGraphError.OK)
					throw new Exception ("Unable to initialize AUGraph object.  Error code: " + result);

				result = (int)graph.Start ();
				if (result != (int)AUGraphError.OK)
					throw new Exception ("Unable to start audio processing graph.  Error code: " + result);
		
//				TODO: CAShow
				Console.WriteLine (graph);
			}
		}

		public void LoadPreset (NSObject sender)
		{
			string presetPath = NSBundle.MainBundle.PathForResource ("Vibraphone", "aupreset");
			NSUrl presetUrl = new NSUrl (presetPath, false);
			if (presetUrl != null)
				Console.WriteLine ("Attempting to load preset + " + presetUrl.Description);
			else
				Console.WriteLine ("COULD NOT GET PRESET PATH!");

			loadSynth (presetUrl);
		}

		int loadSynth (NSUrl presetUrl)
		{
			var dict = NSDictionary.FromUrl (presetUrl);
			var preset = new ClassInfoDictionary (dict);
			var error = samplerUnit.SetClassInfo (preset);

			return (int)error;
		}

		bool setupAudioSession ()
		{
			var session = AVAudioSession.SharedInstance ();
			session.WeakDelegate = this;
			AVAudioSession.Notifications.ObserveInterruption (Interruption);

			NSError audioSessionError = null;
			session.SetCategory (AVAudioSession.CategoryPlayback, out audioSessionError);

			if (audioSessionError != null) {
				Console.WriteLine ("Error setting audio session category.");
				return false;
			}

			graphSampleRate = 44100f;
			session.SetPreferredSampleRate (graphSampleRate, out audioSessionError);
			if (audioSessionError != null) {
				Console.WriteLine ("Error setting preferred hardware sample rate.");
				return false;
			}

			session.SetActive (true, out audioSessionError);
			if (audioSessionError != null) {
				Console.WriteLine ("Error activating the audio session.");
				return false;
			}

			graphSampleRate = session.SampleRate;

			return true;
		}

		public void StartPlayNoteNumber (int noteNum)
		{
			uint onVelocity = 127;
			uint noteCommand = (int)MIDIMessage.NoteOn << 4 | 0;

			var result = samplerUnit.MusicDeviceMIDIEvent (noteCommand, (uint)noteNum, onVelocity, 0);
			if (result != AudioUnitStatus.NoError)
				Console.WriteLine ("Unable to start playing the mid note.  Error code: {0}", (int)result);

		}

		public void StopPlayNoteNumber (int noteNum)
		{
			uint noteCommand = (int)MIDIMessage.NoteOff << 4 | 0;

			var result = samplerUnit.MusicDeviceMIDIEvent (noteCommand, (uint)noteNum, 0, 0);
			if (result != AudioUnitStatus.NoError)
				Console.WriteLine ("Unable to stop playing the mid note.  Error code: {0}", (int)result);
		}

		void stopAudioProcessingGraph ()
		{
			AUGraphError result = AUGraphError.OK;
			if (processingGraph != null)
				result = processingGraph.Stop ();

			if (result != AUGraphError.OK)
				throw new Exception ("Unable to restart the audio processing graph.  Error code: " + result);
		}

		void restartAudioProcessingGraph ()
		{
			AUGraphError result = AUGraphError.OK;
			if (processingGraph != null)
				result = processingGraph.Start ();

			if (result != AUGraphError.OK)
				throw new Exception ("Unable to restart the audio processing graph.  Error code: " + result);
		}

//		[Export("beginInterruption")]
		void Interruption (object sender, AVAudioSessionInterruptionEventArgs e)
		{
			if (e.InterruptionType == AVAudioSessionInterruptionType.Began)
				stopAudioProcessingGraph ();
			else if (e.InterruptionType == AVAudioSessionInterruptionType.Ended) {
				NSError error = null;
				AVAudioSession.SharedInstance ().SetActive (true, out error);

				if (error != null) {
					Console.WriteLine ("Unable to reactivate the audio session");
					return;
				}

				if (e.Option == AVAudioSessionInterruptionOptions.ShouldResume)
					restartAudioProcessingGraph ();
			}
		}

		[Export("beginInterruption")]
		void beginInterruption ()
		{
			stopAudioProcessingGraph ();
		}
//
		[Export("endInterruptionWithFlags:")]
		void endInterruption (AVAudioSessionInterruptionFlags flags)
		{
			NSError error = null;
			AVAudioSession.SharedInstance ().SetActive (true, out error);

			if (error != null) {
				Console.WriteLine ("Unable to reactivate the audio session");
				return;
			}

			if ((flags & AVAudioSessionInterruptionFlags.ShouldResume) != null)
				restartAudioProcessingGraph ();
		}

		[Export("inputIsAvailableChanged:")]
		public void InputIsAvailableChanged (bool isInputAvailable)
		{

		}

		void registerForUIApplicationNotifications ()
		{
			NSNotificationCenter notificationCenter = NSNotificationCenter.DefaultCenter;

			notificationCenter.AddObserver (UIApplication.WillResignActiveNotification,
			                                handleResigningActive,
			                                UIApplication.SharedApplication);

			notificationCenter.AddObserver (UIApplication.DidBecomeActiveNotification,
			                                handleBecomingActive,
			                                UIApplication.SharedApplication);
		}

		void handleResigningActive (NSNotification notification)
		{
			stopAudioProcessingGraph ();
		}

		void handleBecomingActive (NSNotification notification)
		{
			restartAudioProcessingGraph ();
		}
	}
}