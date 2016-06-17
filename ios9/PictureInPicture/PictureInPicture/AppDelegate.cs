using AVFoundation;
using Foundation;
using UIKit;

namespace PictureInPicture
{
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Setup audio session for picture in picture playback.
			// Application has to be configured correctly to be able to initiate picture in picture.
			// This configuration involves:
			//
			// 1. Setting UIBackgroundMode to audio under the project settings.
			// 2. Setting audio session category to AVAudioSessionCategoryPlayback or AVAudioSessionCategoryPlayAndRecord (as appropriate)
			//
			// If an application is not configured correctly, AVPictureInPictureController.PictureInPicturePossible returns false.
			var audioSession = AVAudioSession.SharedInstance ();
			NSError error = audioSession.SetCategory (AVAudioSessionCategory.Playback);
			if(error != null)
				System.Console.WriteLine ("Audio session SetCategory failed");

			return true;
		}
	}
}