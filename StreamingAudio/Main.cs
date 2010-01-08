
//
// This is a sample program that streams audio from an HTTP server using
// Mono's HTTP stack, AudioStreamFile to parse partial audio streams and
// AudioQueue/OutputAudioQueue to generate the output.
//
// MIT X11
//
using System;
using System.Collections.Generic;
using System.Linq;
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
		
		StreamingPlayback player = null;
		bool paused;
		
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

		partial void volumeSet (UISlider sender)
		{
			if (player == null)
				return;
			
			player.OutputQueue.Volume = sender.Value;
		}

		partial void startPlayback (UIButton sender)
		{
			status.Text = "Starting HTTP request";
			AudioSession.Category = AudioSessionCategory.MediaPlayback;
			AudioSession.RoutingOverride = AudioSessionRoutingOverride.Speaker;
			
			button.TitleLabel.Text = "Pause";
			
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
				var stream = response.GetResponseStream ();
				var buffer = new byte [8192];
				int l = 0, n;
				
				InvokeOnMainThread (delegate {
					viewController.PushViewController (playController, true);
				});
				
				pushed = true;
							
				// The following line prevents the audio from stopping 
				// when the device autolocks.
				AudioSession.Category = AudioSessionCategory.MediaPlayback;

				// 
				// Create StreamingPlayer, the using statement will automatically
				// force the resources to be disposed and the playback to stop.
				//
				using (player = new StreamingPlayback ()){
					while ((n = stream.Read (buffer, 0, buffer.Length)) != 0){
						l += n;
						player.ParseBytes (buffer, n, false);
						
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
			AudioSession.Category = AudioSessionCategory.SoloAmbientSound;
			if (pushed){
				viewController.PopToRootViewController (true);
				status.Text = "Finished playback";
			}
		}
	}
}
