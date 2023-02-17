using System;
using System.Diagnostics;
using System.IO;
using AVFoundation;
using Foundation;
using UIKit;

namespace Sound {
	public partial class ViewController : UIViewController {
		private NSObject observer;

		private AVPlayer player;

		private NSUrl audioFilePath;

		private AVAudioRecorder recorder;

		private Stopwatch stopwatch;

		private Status status;

		protected ViewController (IntPtr handle) : base (handle) { }

		protected Status Status {
			get {
				return status;
			}

			set {
				if (status != value) {
					status = value;
					UpdateUserInterface ();
				}
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			playButton.Enabled = false;
			stopButton.Enabled = false;
			statusLabel.Text = string.Empty;
			lenghtButton.Text = string.Empty;

			observer = AVPlayerItem.Notifications.ObserveDidPlayToEndTime (OnDidPlayToEndTime);
		}

		private void UpdateUserInterface ()
		{
			InvokeOnMainThread (() => {
				switch (status) {
				case Status.PreparingError:
					statusLabel.Text = "Error preparing";
					lenghtButton.Text = string.Empty;
					startButton.Enabled = true;
					stopButton.Enabled = false;
					playButton.Enabled = false;
					break;

				case Status.Playing:
					statusLabel.Text = "Playing";
					startButton.Enabled = false;
					stopButton.Enabled = false;
					playButton.Enabled = false;
					break;

				case Status.Recording:
					lenghtButton.Text = string.Empty;
					statusLabel.Text = "Recording";
					startButton.Enabled = false;
					stopButton.Enabled = true;
					playButton.Enabled = false;
					break;

				case Status.Recorded:
					lenghtButton.Text = string.Format ("{0:hh\\:mm\\:ss}", stopwatch.Elapsed);
					statusLabel.Text = string.Empty;
					startButton.Enabled = true;
					stopButton.Enabled = false;
					playButton.Enabled = true;
					break;
				}
			}

		}

		partial void StartRecording (UIButton sender)
		{
			Console.WriteLine ("Begin Recording");

			var session = AVAudioSession.SharedInstance ();
			session.RequestRecordPermission ((granted) => {
				Console.WriteLine ($"Audio Permission: {granted}");

				if (granted) {
					session.SetCategory (AVAudioSession.CategoryRecord, out NSError error);
					if (error == null) {
						session.SetActive (true, out error);
						if (error != null) {
							Status = Status.PreparingError;
						} else {
							var isPrepared = PrepareAudioRecording () && recorder.Record ();
							if (isPrepared) {
								stopwatch = new Stopwatch ();
								stopwatch.Start ();
								Status = Status.Recording;
							} else {
								Status = Status.PreparingError;
							}
						}
					} else {
						Console.WriteLine (error.LocalizedDescription);
					}
				} else {
					Console.WriteLine ("YOU MUST ENABLE MICROPHONE PERMISSION");
				}
			});
		}

		partial void StopRecording (UIButton sender)
		{
			if (recorder != null) {
				recorder.Stop ();
				stopwatch?.Stop ();

				Status = Status.Recorded;
			}
		}

		private NSUrl CreateOutputUrl ()
		{
			var fileName = $"Myfile-{DateTime.Now.ToString ("yyyyMMddHHmmss")}.aac";
			var tempRecording = Path.Combine (Path.GetTempPath (), fileName);

			return NSUrl.FromFilename (tempRecording);
		}

		private void OnDidPlayToEndTime (object sender, NSNotificationEventArgs e)
		{
			player.Dispose ();
			player = null;

			Status = Status.Recorded;
		}

		partial void PlayRecorded (UIButton sender)
		{
			Console.WriteLine ($"Playing Back Recording {audioFilePath}");

			// The following line prevents the audio from stopping
			// when the device autolocks. will also make sure that it plays, even
			// if the device is in mute
			AVAudioSession.SharedInstance ().SetCategory (AVAudioSession.CategoryPlayback, out NSError error);
			if (error == null) {
				Status = Status.Playing;
				player = new AVPlayer (audioFilePath);
				player.Play ();
			} else {
				Status = Status.Recorded;
				Console.WriteLine (error.LocalizedDescription);
			}
		}

		private bool PrepareAudioRecording ()
		{
			var result = false;

			audioFilePath = CreateOutputUrl ();

			var audioSettings = new AudioSettings {
				SampleRate = 44100,
				NumberChannels = 1,
				AudioQuality = AVAudioQuality.High,
				Format = AudioToolbox.AudioFormatType.MPEG4AAC,
			};

			// Set recorder parameters
			recorder = AVAudioRecorder.Create (audioFilePath, audioSettings, out NSError error);
			if (error == null) {
				// Set Recorder to Prepare To Record
				if (!recorder.PrepareToRecord ()) {
					recorder.Dispose ();
					recorder = null;
				} else {
					recorder.FinishedRecording += OnFinishedRecording;
					result = true;
				}
			} else {
				Console.WriteLine (error.LocalizedDescription);
			}

			return result;
		}

		private void OnFinishedRecording (object sender, AVStatusEventArgs e)
		{
			recorder.Dispose ();
			recorder = null;

			Console.WriteLine ($"Done Recording (status: {e.Status})");
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (observer != null) {
				observer.Dispose ();
				observer = null;
			}

			if (player != null) {
				player.Dispose ();
				player = null;
			}

			if (recorder != null) {
				recorder.Dispose ();
				recorder = null;
			}

			if (audioFilePath != null) {
				audioFilePath.Dispose ();
				audioFilePath = null;
			}
		}
	}

	public enum Status {
		Unknown,
		PreparingError,
		Recording,
		Recorded,
		Playing,
	}
}
