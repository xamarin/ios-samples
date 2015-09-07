/*
 * This controller displays the Glance. It demonstrates passing information, via Handoff, to the WatchKit app 
 * to route the wearer to the appropriate controller once the app is launched. 
 * Tapping on the Glance will launch the WatchKit app.
*/

using System;

using UIKit;
using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	public partial class GlanceController : WKInterfaceController
	{
		public GlanceController (IntPtr handle) : base (handle)
		{
		}

		public override void Awake (NSObject context)
		{
			// // Load image inside WatchKit Extension
			// using (var image = UIImage.FromBundle ("Walkway"))
			// using (var png = image.AsPNG ())
			// 	glanceImage.SetImage (png);
		}

		public override void WillActivate ()
		{
			// This method is called when the controller is about to be visible to the wearer.
			Console.WriteLine ("{0} will activate", this);

			// using (var d = new NSDictionary ("controllerName", "imageDetailController", 
			// 	"detailInfo", "This is some more detailed information to pass.")) {
			// 	// Use Handoff to route the wearer to the image detail controller when the Glance is tapped.
			// 	UpdateUserActivity ("com.example.apple-samplecode.WatchKit-Catalog", d, null);
			// }
		}

		public override void DidDeactivate ()
		{
			// This method is called when the controller is no longer visible.
			Console.WriteLine ("{0} did deactivate", this);
		}
	}
}