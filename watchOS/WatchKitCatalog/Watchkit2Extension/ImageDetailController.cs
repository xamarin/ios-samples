/*
 * This controller displays images, static and animated. It demonstrates using the image cache 
 * to send images from the WatchKit app extension bundle to be stored and used in the WatchKit app bundle. 
 * It also demonstrates how to use screenBounds to use the most appropriate sized image for the device at runtime. 
 * Finally, this controller demonstrates loading images from the WatchKit Extension bundle and from the WatchKit app bundle.
*/

using System;

using UIKit;
using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	public partial class ImageDetailController : WKInterfaceController
	{
		public ImageDetailController ()
		{
		}

		public override void Awake (NSObject context)
		{
			// Log the context passed in, if the wearer arrived at this controller via the sample's Glance.
			Console.WriteLine ("Passed in context: {0}", context);

			// var device = WKInterfaceDevice.CurrentDevice;

			// using (var image = UIImage.FromBundle ("Bumblebee")) {
			// 	if (!device.AddCachedImage (image, "Bumblebee")) {
			// 		Console.WriteLine ("Image cache full.");
			// 	} else {
			// 		cachedImage.SetImage ("Bumblebee");
			// 	}
			// }

			// // Log what's currently residing in the image cache.
			// Console.WriteLine ("Currently cached images: {0}", WKInterfaceDevice.CurrentDevice.WeakCachedImages);

			// // Uses image inside WatchKit Extension bundle.
			// using (var image = UIImage.FromBundle ("Walkway"))
			// using (var png = image.AsPNG ())
			// 	staticImage.SetImage (png);
		}

		public override void WillActivate ()
		{
			// This method is called when the controller is about to be visible to the wearer.
			Console.WriteLine ("{0} will activate", this);
		}

		public override void DidDeactivate ()
		{
			// This method is called when the controller is no longer visible.
			Console.WriteLine ("{0} did deactivate", this);
		}

		partial void PlayAnimation (NSObject obj)
		{
			// animatedImage.SetImage ("Bus");
			// animatedImage.StartAnimating ();

			// // Animate with a specific range, duration, and repeat count.
			// //animatedImage.StartAnimating (new NSRange (0, 4), 2.0, 3);
		}

		partial void StopAnimation (NSObject obj)
		{
			// animatedImage.StopAnimating ();
		}
	}
}

