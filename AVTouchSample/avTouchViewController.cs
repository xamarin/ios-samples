
using System;
using System.IO;
using System.Collections;
using UIKit;
using Foundation;
using AVFoundation;

namespace avTouch
{
	public partial class avTouchViewController : UIViewController
	{

		public avTouchViewController (IntPtr handle) : base (handle)
		{
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations.
			return toInterfaceOrientation == UIInterfaceOrientation.Portrait;
		}

	}
	
	public partial class avTouchController : NSObject {
		TimeSpan SkipTime = TimeSpan.FromSeconds (0.5);
		double SkipTimeSeconds = 1;
		TimeSpan SkipInterval = TimeSpan.FromSeconds (0.2);
		
		NSTimer update_timer;
		UIImage playBtnBg, pauseBtnBg;
		AVAudioPlayer player;
		NSTimer rewTimer, ffwTimer;
		
		public override void AwakeFromNib ()
		{
			playBtnBg = UIImage.FromFile ("images/play.png").StretchableImage (12, 0);
			pauseBtnBg = UIImage.FromFile ("images/pause.png").StretchableImage (12, 0);
			_playButton.SetImage (playBtnBg, UIControlState.Normal);
			
			_duration.AdjustsFontSizeToFitWidth = true;
			_currentTime.AdjustsFontSizeToFitWidth = true;
			_progressBar.MinValue = 0;
			
			var fileUrl = NSBundle.MainBundle.PathForResource ("sample", "m4a");
			player = AVAudioPlayer.FromUrl (new NSUrl (fileUrl, false));
			
			player.FinishedPlaying += delegate(object sender, AVStatusEventArgs e) {
				if (!e.Status)
					Console.WriteLine ("Did not complete successfully");
				    
				player.CurrentTime = 0;
				UpdateViewForPlayerState ();
			};
			player.DecoderError += delegate(object sender, AVErrorEventArgs e) {
				Console.WriteLine ("Decoder error: {0}", e.Error.LocalizedDescription);
			};
			player.BeginInterruption += delegate {
				UpdateViewForPlayerState ();
			};
			player.EndInterruption += delegate {
				StartPlayback ();
			};
			_fileName.Text = String.Format ("Mono {0} ({1} ch)", Path.GetFileName (player.Url.RelativePath), player.NumberOfChannels);
			UpdateViewForPlayerInfo ();
			UpdateViewForPlayerState ();
		}
 
		public void UpdateCurrentTime ()
		{
			_currentTime.Text = String.Format ("{0}:{1:2}", (int) player.CurrentTime/60, (int) player.CurrentTime % 60);
			_progressBar.Value = (float) player.CurrentTime;
		}
	
		public void UpdateViewForPlayerState ()
		{
			UpdateCurrentTime ();
			
			if (update_timer != null)
				update_timer.Invalidate ();
			
			if (player.Playing){
				_playButton.SetImage (pauseBtnBg, UIControlState.Normal);
				_lvlMeter_in.Player = player;
				update_timer = NSTimer.CreateRepeatingScheduledTimer (TimeSpan.FromSeconds (0.01), delegate {
					UpdateCurrentTime ();
				});
			} else {
				_playButton.SetImage (playBtnBg, UIControlState.Normal);
				_lvlMeter_in.Player = null;
				update_timer = null;
			}
		}
		
		void UpdateViewForPlayerInfo ()
		{
			_duration.Text = String.Format ("{0}:{1:2}", player.Duration / 60, player.Duration % 60);
			_progressBar.MaxValue = (float) player.Duration;
			_volumeSlider.Value = player.Volume;
		}
		
		void PausePlayback ()
		{
			player.Pause ();
			UpdateViewForPlayerState ();
		}
		
		void StartPlayback ()
		{
			if (player.Play ())
				UpdateViewForPlayerState();
			else
				Console.WriteLine ("Could not play the file {0}", player.Url);
		}
			
		partial void ffwButtonPressed (UIButton sender)
		{
			if (ffwTimer != null)
				return;
			
			ffwTimer = NSTimer.CreateRepeatingScheduledTimer (SkipTime, delegate {
				player.CurrentTime += SkipTimeSeconds;
				UpdateCurrentTime ();
			});
		}

		partial void ffwButtonReleased (UIButton sender)
		{
			if (ffwTimer == null)
				return;			
			ffwTimer.Invalidate ();
			ffwTimer = null;
		}


		partial void volumeSliderMoved (UISlider sender)
		{
			player.Volume = sender.Value;	
		}

		partial void rewButtonReleased (UIButton sender)
		{
			if (rewTimer == null)
				return;			
			rewTimer.Invalidate ();
			rewTimer = null;
		}

		partial void rewButtonPressed (UIButton sender)
		{
			if (rewTimer != null)
				return;
			
			rewTimer = NSTimer.CreateRepeatingScheduledTimer (SkipTime, delegate {
				player.CurrentTime -= SkipTimeSeconds;
				UpdateCurrentTime ();
			});
		}

		partial void progressSliderMoved (UISlider sender)
		{
			player.CurrentTime = sender.Value;
			UpdateCurrentTime ();
		}

		partial void playButtonPressed (UIButton sender)
		{
			if (player.Playing)
				PausePlayback ();
			else
				StartPlayback ();
		}

	}
}

namespace UIKit {
	public class UICustomObject : NSObject {
	}
}
