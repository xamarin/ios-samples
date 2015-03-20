using System;

using UIKit;
using Foundation;
using AudioToolbox;

namespace SysSound
{
	public partial class MainViewController : UIViewController
	{
		SystemSound systemSound;

		public MainViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Generate the NSUrl to the sound file
			var url = NSUrl.FromFilename ("Sounds/tap.aif");

			// Generate the SystemSound instance with the NSUrl
			systemSound = new SystemSound (url);

			// This handles the playSystemButton being pressed
			// Plays the sound
			playSystemButton.TouchUpInside += (object sender, EventArgs e) => systemSound.PlaySystemSound ();

			// This handles the playAlertButton being pressed
			// PlayAlertSound Plays the sound as well as vibrates
			playAlertButton.TouchUpInside += (object sender, EventArgs e) => systemSound.PlayAlertSound ();

			// This handles the VibrateButton being pressed
			// Just vibrates the device
			VibrateButton.TouchUpInside += (object sender, EventArgs e) => SystemSound.Vibrate.PlaySystemSound (); 
		}
	}
}