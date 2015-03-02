using System;
using Foundation;
using UIKit;
using System.Net;
using AudioToolbox;
using System.IO;
using System.Threading;
using System.Diagnostics;
using AVFoundation;

namespace StreamingAudio
{
	public partial class PlayerViewController : UIViewController
	{
		NSTimer updatingTimer;
		StreamingPlayback player;

		public event EventHandler<ErrorArg> ErrorOccurred;

		public string SourceUrl { get; private set; }

		public PlayerOption PlayerOption { get; private set; }

		public bool IsPlaying { get; private set; }

		public PlayerViewController (PlayerOption playerOption, string sourceUrl) : base ("PlayerViewController", null)
		{
			PlayerOption = playerOption;
			SourceUrl = sourceUrl;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.View = View;
			volumeSlider.TouchUpInside += SetVolume;
			playPauseButton.TouchUpInside += PlayPauseButtonClickHandler;
		}

		void SetVolume (object sender, EventArgs e)
		{
			if (player == null)
				return;

			player.Volume = volumeSlider.Value;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			Title = PlayerOption == PlayerOption.Stream ? "Stream " : "Stream & Save";
			playPauseButton.TitleLabel.Text = "Pause";
			timeLabel.Text = string.Empty;

			// Create a shared intance session & check
			var session = AVAudioSession.SharedInstance ();
			if (session == null) {
				var alert = new UIAlertView ("Playback error", "Unable to playback stream", null, "Cancel");
				alert.Show ();
				alert.Clicked += (object sender, UIButtonEventArgs e) => alert.DismissWithClickedButtonIndex (0, true);
			} else {
				StartPlayback ();
				IsPlaying = true;

				// Set up the session for playback category
				NSError error;
				session.SetCategory (AVAudioSessionCategory.Playback, AVAudioSessionCategoryOptions.DefaultToSpeaker);
				session.OverrideOutputAudioPort (AVAudioSessionPortOverride.Speaker, out error);
			}
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

			if (updatingTimer != null)
				updatingTimer.Invalidate ();

			if (player != null) {
				player.FlushAndClose ();
				player = null;
			}
		}

		void PlayPauseButtonClickHandler (object sender, EventArgs e)
		{
			if (player == null)
				return;

			if (IsPlaying)
				player.Pause ();
			else
				player.Play ();

			var title = IsPlaying ? "Play" : "Pause";
			playPauseButton.SetTitle (title, UIControlState.Normal);
			playPauseButton.SetTitle (title, UIControlState.Selected);
			IsPlaying = !IsPlaying;
		}

		void StartPlayback ()
		{
			try {
				var request = (HttpWebRequest)WebRequest.Create (SourceUrl);
				request.BeginGetResponse (StreamDownloadedHandler, request);
			} catch (Exception e) {
				string.Format ("Error: {0}", e.ToString ());
			}
		}

		void RaiseErrorOccurredEvent (string message)
		{
			var handler = ErrorOccurred;
			if (handler != null)
				handler (this, new ErrorArg { Description = message });
		}

		void StreamDownloadedHandler (IAsyncResult result)
		{
			var buffer = new byte [8192];
			int l = 0;
			int inputStreamLength;
			double sampleRate = 0;

			Stream inputStream;
			AudioQueueTimeline timeline = null;

			var request = result.AsyncState as HttpWebRequest;
			try {
				var response = request.EndGetResponse (result);
				var responseStream = response.GetResponseStream ();

				if (PlayerOption == PlayerOption.StreamAndSave)
					inputStream = GetQueueStream (responseStream);
				else
					inputStream = responseStream;

				using (player = new StreamingPlayback ()) {
					player.OutputReady += delegate {
						timeline = player.OutputQueue.CreateTimeline ();
						sampleRate = player.OutputQueue.SampleRate;
					};

					InvokeOnMainThread (delegate {
						if (updatingTimer != null)
							updatingTimer.Invalidate ();

						updatingTimer = NSTimer.CreateRepeatingScheduledTimer (0.5, (timer) => RepeatingAction (timeline, sampleRate));
					});

					while ((inputStreamLength = inputStream.Read (buffer, 0, buffer.Length)) != 0 && player != null) {
						l += inputStreamLength;
						player.ParseBytes (buffer, inputStreamLength, false, l == (int)response.ContentLength);

						InvokeOnMainThread (delegate {
							progressBar.Progress = l / (float)response.ContentLength;
						});
					}
				}

			} catch (Exception e) {
				RaiseErrorOccurredEvent ("Error fetching response stream\n" + e);
				Debug.WriteLine (e);
				InvokeOnMainThread (delegate {
					if (NavigationController != null)
						NavigationController.PopToRootViewController (true);
				});
			}
		}

		void RepeatingAction (AudioQueueTimeline timeline, double sampleRate)
		{
			var queue = player.OutputQueue;
			if (queue == null || timeline == null)
				return;

			bool disc = false;
			var time = new AudioTimeStamp ();
			queue.GetCurrentTime (timeline, ref time, ref disc);

			playbackTime.Text = FormatTime (time.SampleTime / sampleRate);
		}

		string FormatTime (double time)
		{
			double minutes = time / 60;
			double seconds = time % 60;

			return String.Format ("{0}:{1:D2}", (int)minutes, (int)seconds);
		}

		Stream GetQueueStream (Stream responseStream)
		{
			var queueStream = new QueueStream (Environment.GetFolderPath (Environment.SpecialFolder.Personal) + "/copy.mp3");
			var t = new Thread ((x) => {
				var tbuf = new byte [8192];
				int count;

				while ((count = responseStream.Read (tbuf, 0, tbuf.Length)) != 0)
					queueStream.Push (tbuf, 0, count);

			});
			t.Start ();
			return queueStream;
		}
	}
}

