
//
// This is a sample program that streams audio from an HTTP server using
// Mono's HTTP stack, AudioStreamFile to parse partial audio streams and
// AudioQueue/OutputAudioQueue to generate the output.
//
// MIT X11
//
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Net;
using MonoTouch.AudioToolbox;

namespace StreamingAudio
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}

	public partial class AppDelegate : UIApplicationDelegate
	{
		public override void OnActivated (UIApplication application)
		{
		}
		
		NSTimer updatingTimer;
		StreamingPlayback player = null;
		QueueStream queueStream;
		bool paused;
		bool saveCopy;
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window.AddSubview (viewController.View);
			AudioSession.Initialize ();
			
			// Nice creative commons source.
			entry.Text = "http://ccmixter.org/content/bradstanfield/bradstanfield_-_People_Let_s_Stop_The_War.mp3";
			entry.EditingDidEnd += delegate {
				entry.ResignFirstResponder ();
			};
			
			window.MakeKeyAndVisible ();
			
			return true;
		}

		// Action hooked up on Interface Builder
		partial void playControlClicked (UIButton sender)
		{
			paused = !paused;
					
			if (player == null)
				return;
			
			if (paused)
				player.Pause ();
			else
				player.Play ();
			
			string title = paused ? "Play" : "Pause";
			button.SetTitle (title, UIControlState.Normal);
			button.SetTitle (title, UIControlState.Selected);
		}

		// Action hooked up on Interface Builder
		partial void volumeSet (UISlider sender)
		{
			if (player == null)
				return;
			
			player.Volume = sender.Value;
		}

		void PreparePlayback ()
		{
			status.Text = "Starting HTTP request";
			
			// The following line prevents the audio from stopping 
			// when the device autolocks.
			AudioSession.Category = AudioSessionCategory.MediaPlayback;
			AudioSession.RoutingOverride = AudioSessionRoutingOverride.Speaker;
			
			button.TitleLabel.Text = "Pause";			
		}
		
		// Action hooked up on Interface Builder
		partial void startPlayback (UIButton sender)
		{
			saveCopy = false;
			StartPlayback ();
		}
		
		// Action hooked up on Interface Builder
		partial void startPlaybackAndSave (UIButton sender)
		{
			saveCopy = true;
			StartPlayback ();
		}

		void StartPlayback ()
		{
			PreparePlayback ();
			try {
				var request = (HttpWebRequest) WebRequest.Create (entry.Text);
				request.BeginGetResponse (StreamDownloaded, request);
			} catch (Exception e){
				status.Text = "Error: " + e.ToString ();
			}
		}
		
		void StreamDownloaded (IAsyncResult result)
		{
			var request = result.AsyncState as HttpWebRequest;
			bool pushed = false;
			try {
				var response = request.EndGetResponse (result);
				var responseStream = response.GetResponseStream ();
				Stream inputStream;
				var buffer = new byte [8192];
				int l = 0, n;
				
				InvokeOnMainThread (delegate {
					viewController.PushViewController (playController, true);
				});
				
				pushed = true;
				
				if (saveCopy)
					inputStream = MakeQueueStream (responseStream);
				else
					inputStream = responseStream;
				
				// 
				// Create StreamingPlayer, the using statement will automatically
				// force the resources to be disposed and the playback to stop.
				//
				using (player = new StreamingPlayback ()){
					AudioQueueTimeline timeline = null;
					double sampleRate = 0;
					
					player.OutputReady += delegate {
						timeline = player.OutputQueue.CreateTimeline ();
						sampleRate = player.OutputQueue.SampleRate;
					};
					InvokeOnMainThread (delegate {
						if (updatingTimer != null)
							updatingTimer.Invalidate ();
								
						updatingTimer = NSTimer.CreateRepeatingScheduledTimer (0.5, delegate {
							var queue = player.OutputQueue;
							if (queue == null || timeline == null)
								return;
							bool disc;
							AudioTimeStamp time;
							queue.GetCurrentTime (timeline, ref time, ref disc);
							
							playbackTime.Text = FormatTime (time.SampleTime / sampleRate);
						});
					});
					while ((n = inputStream.Read (buffer, 0, buffer.Length)) != 0){
						l += n;
						player.ParseBytes (buffer, n, false, l == (int)response.ContentLength);
						
						InvokeOnMainThread (delegate {
							progress.Progress = l / (float) response.ContentLength;
						});
					}
					
				}
			} catch (Exception e){
				InvokeOnMainThread (delegate {
					if (pushed){
						viewController.PopToRootViewController (true);
						pushed = false;
					}
					status.Text = "Error fetching response stream\n" + e;
					Console.WriteLine (e);
				});
			}
	
			//
			// Restore the default AudioSession, this allows the iPhone
			// to go to sleep now that we are done playing the audio
			//
			AudioSession.Category = AudioSessionCategory.MediaPlayback;
			if (pushed){
				viewController.PopToRootViewController (true);
				status.Text = "Finished playback";
			}
		}
		
		static string FormatTime (double time)
		{
			double minutes = time / 60;
			double seconds = time % 60;
			
			return String.Format ("{0}:{1:D2}", (int) minutes, (int) seconds);
		}
		
		//
		// Launches a thread that reads the network stream
		// and queues it for use by our audio thread
		//
		Stream MakeQueueStream (Stream networkStream)
		{
			queueStream = new QueueStream (Environment.GetFolderPath (Environment.SpecialFolder.Personal) + "copy.mp3");
			var t = new Thread ((x) => {
				var tbuf = new byte [8192];
				int count;
				
				while ((count = networkStream.Read (tbuf, 0, tbuf.Length)) != 0){
					queueStream.Push (tbuf, 0, count);
				}
				
			});
			t.Start ();
			return queueStream;
		}
		
	}
}
