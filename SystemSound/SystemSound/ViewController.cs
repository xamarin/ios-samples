using Foundation;
using System;
using UIKit;

namespace SystemSound {
	public partial class ViewController : UIViewController {
		private AudioToolbox.SystemSound systemSound;

		protected ViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Generate the SystemSound instance with the NSUrl
			systemSound = new AudioToolbox.SystemSound (NSUrl.FromFilename ("Sounds/tap.aif"));
		}

		partial void Vibrate (UIButton sender)
		{
			// Just vibrates the device
			AudioToolbox.SystemSound.Vibrate.PlaySystemSound ();
		}

		partial void PlayAlert (UIButton sender)
		{
			// Plays the sound as well as vibrates
			systemSound.PlayAlertSound ();
		}

		partial void PlaySystem (UIButton sender)
		{
			// Plays the sound
			systemSound.PlaySystemSound ();
		}
	}
}
