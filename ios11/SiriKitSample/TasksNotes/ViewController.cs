using System;
using Intents;
using UIKit;

namespace TasksNotes {
	public partial class ViewController : UIViewController {
		protected ViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			// Request access to Siri
			INPreferences.RequestSiriAuthorization ((INSiriAuthorizationStatus status) => {
				// Respond to returned status
				switch (status) {
				case INSiriAuthorizationStatus.Authorized:
					Console.WriteLine ("SiriKit Authorized");
					break;
				case INSiriAuthorizationStatus.Denied:
					Console.WriteLine ("SiriKit Denied");
					break;
				case INSiriAuthorizationStatus.NotDetermined:
					Console.WriteLine ("SiriKit Not Determined");
					break;
				case INSiriAuthorizationStatus.Restricted:
					Console.WriteLine ("SiriKit Restricted");
					break;
				}

				// Save status
				AppDelegate.SiriAuthorizationStatus = status;
			});
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
