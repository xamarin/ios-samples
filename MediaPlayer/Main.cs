
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MediaPlayer;

namespace MediaPlayer
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}

	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class MoviePlayerAppDelegate : UIApplicationDelegate
	{
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// If you have defined a view, add it here:
			window.AddSubview (tabBarController.View);
			window.MakeKeyAndVisible ();
			
			// Register to receive movie notifications
			var center = NSNotificationCenter.DefaultCenter;
			center.AddObserver ("MPMoviePlayerContentPreloadDidFinishNotification",
			                    (notify) => { Console.WriteLine ("Content Preload Finished"); notify.Dispose (); });
			center.AddObserver ("MPMoviePlayerPlaybackDidFinishNotification",
			                    (notify) => { Console.WriteLine ("Playback finished"); notify.Dispose (); });
			center.AddObserver ("MPMoviePlayerScalingModeDidChangeNotification",
			                    (notify) => { Console.WriteLine ("ScalingMode changed"); notify.Dispose (); });
			                                                
			return true;
		}

		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
		
		MPMoviePlayerController moviePlayer;
		
		public void InitAndPlayMovie (NSUrl url)
		{
			if (url == null){
				Console.WriteLine ("The url was null");
				return;
			}
			
			// MPMoviePlayerController is a weird singleton class, and if we let the GC collect the object
			// it causes unexpected behaviours, as such we explicitly dispose our previous instance if we 
			// have one
			if (moviePlayer != null) {
				moviePlayer.Dispose ();
				moviePlayer = null;
			}
			moviePlayer = new MPMoviePlayerController (url);
			SetMoviePlayerUserSettings ();
			moviePlayer.Play ();
		}
		
		void SetMoviePlayerUserSettings ()
		{
			
		}
	}
	
	public partial class MyMovieViewController : UIViewController {
		public MyMovieViewController (IntPtr handle) : base (handle) { }
		
		// This method was wired up from InterfaceBuilder
		partial void playMovieButtonPressed (UIButton sender)
		{
			PlayMovie ();
		}
		
		public void PlayMovie ()
		{
			var appDelegate = UIApplication.SharedApplication.Delegate as MoviePlayerAppDelegate;
			
			appDelegate.InitAndPlayMovie (LocalMovieUrl);
			
			// Now show the overloay
			var windows = UIApplication.SharedApplication.Windows;
			if (windows.Length > 1){
				// Locate the movie player window
				var moviePlayer = UIApplication.SharedApplication.KeyWindow;
				moviePlayer.AddSubview (overlayView);
			}
				
		}
		
		// Button pressed on the overlay
		partial void overlayViewButtonPress (UIButton sender)
		{
			Console.WriteLine ("Overlay button pressed");
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

		
		NSUrl movieUrl;
		
		NSUrl LocalMovieUrl {
			get {
				if (movieUrl == null){
					var path = NSBundle.MainBundle.PathForResource ("Movie", "m4v");
					Console.WriteLine (path);
					if (path != null)
						movieUrl = new NSUrl (path, false);
				}
				return movieUrl;
			}
		}
	}
	
	public partial class MyImageView : UIImageView {
		public MyImageView (IntPtr handle) : base (handle) {}
			
		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			var touch = touches.AnyObject as UITouch;
			if (touch.Phase == UITouchPhase.Began){
				(viewController as MyMovieViewController).PlayMovie ();
			}
		}

	}
}
