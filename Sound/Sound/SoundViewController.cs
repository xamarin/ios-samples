using System;
using System.IO;
using System.Diagnostics;

using UIKit;
using Foundation;
using AVFoundation;

namespace Sound
{
	public partial class SoundViewController : UIViewController
	{
		AVAudioRecorder recorder;
		AVPlayer player;
		Stopwatch stopwatch;
		NSUrl audioFilePath;
		NSObject observer;

		public SoundViewController (IntPtr handle)
			: base(handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			RecordingStatusLabel.Text = string.Empty;
			LengthOfRecordingLabel.Text = string.Empty;

			StartRecordingButton.TouchUpInside += OnStartRecording;
			StopRecordingButton.TouchUpInside += OnStopRecording;
			PlayRecordedSoundButton.TouchUpInside += OnPlayRecordedSound;

			observer = AVPlayerItem.Notifications.ObserveDidPlayToEndTime (OnDidPlayToEndTime);
		}

		void OnStopRecording (object sender, EventArgs e)
		{
			recorder.Stop ();
			stopwatch.Stop ();

			LengthOfRecordingLabel.Text = string.Format ("{0:hh\\:mm\\:ss}", stopwatch.Elapsed);
			RecordingStatusLabel.Text = "";
			StartRecordingButton.Enabled = true;
			StopRecordingButton.Enabled = false;
			PlayRecordedSoundButton.Enabled = true;
		}

		void OnStartRecording (object sender, EventArgs e)
		{
			Console.WriteLine ("Begin Recording");

			var session = AVAudioSession.SharedInstance ();

			NSError error = null;
			session.SetCategory (AVAudioSession.CategoryRecord, out error);
			if (error != null) {
				Console.WriteLine (error);
				return;
			}

			session.SetActive (true, out error);
			if (error != null) {
				Console.WriteLine (error);
				return;
			}

			if (!PrepareAudioRecording ()) {
				RecordingStatusLabel.Text = "Error preparing";
				return;
			}

			if (!recorder.Record ()) {
				RecordingStatusLabel.Text = "Error preparing";
				return;
			}

			stopwatch = new Stopwatch ();
			stopwatch.Start ();

			LengthOfRecordingLabel.Text = "";
			RecordingStatusLabel.Text = "Recording";
			StartRecordingButton.Enabled = false;
			StopRecordingButton.Enabled = true;
			PlayRecordedSoundButton.Enabled = false;
		}

		NSUrl CreateOutputUrl ()
		{
			string fileName = string.Format ("Myfile{0}.aac", DateTime.Now.ToString ("yyyyMMddHHmmss"));
			string tempRecording = Path.Combine (Path.GetTempPath (), fileName);

			return NSUrl.FromFilename (tempRecording);
		}

		void OnDidPlayToEndTime (object sender, NSNotificationEventArgs e)
		{
			player.Dispose ();
			player = null;
		}

		void OnPlayRecordedSound (object sender, EventArgs e)
		{
			try {
				Console.WriteLine ("Playing Back Recording {0}", audioFilePath);

				// The following line prevents the audio from stopping
				// when the device autolocks. will also make sure that it plays, even
				// if the device is in mute
				NSError error = null;
				AVAudioSession.SharedInstance ().SetCategory (AVAudioSession.CategoryPlayback, out error);
				if (error != null)
					throw new Exception (error.DebugDescription);

				player = new AVPlayer (audioFilePath);
				player.Play ();
			} catch (Exception ex) {
				Console.WriteLine ("There was a problem playing back audio: ");
				Console.WriteLine (ex.Message);
			}
		}

		bool PrepareAudioRecording ()
		{
			audioFilePath = CreateOutputUrl ();

			var audioSettings = new AudioSettings {
				SampleRate = 44100,
				Format = AudioToolbox.AudioFormatType.MPEG4AAC,
				NumberChannels = 1,
				AudioQuality = AVAudioQuality.High
			};

			//Set recorder parameters
			NSError error;
			recorder = AVAudioRecorder.Create (audioFilePath, audioSettings, out error);
			if (error != null) {
				Console.WriteLine (error);
				return false;
			}

			//Set Recorder to Prepare To Record
			if (!recorder.PrepareToRecord ()) {
				recorder.Dispose ();
				recorder = null;
				return false;
			}

			recorder.FinishedRecording += OnFinishedRecording;

			return true;
		}

		void OnFinishedRecording (object sender, AVStatusEventArgs e)
		{
			recorder.Dispose ();
			recorder = null;
			Console.WriteLine ("Done Recording (status: {0})", e.Status);
		}

		protected override void Dispose (bool disposing)
		{
			observer.Dispose ();
			base.Dispose (disposing);
		}
	}
}