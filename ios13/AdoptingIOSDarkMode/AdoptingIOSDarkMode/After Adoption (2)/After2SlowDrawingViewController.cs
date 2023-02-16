/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A view controller demonstrating doing time-consuming drawing on a background thread.
 This is the code after dark mode adoption has happened.
*/

using System;

using CoreFoundation;
using CoreGraphics;
using UIKit;

namespace AdoptingIOSDarkMode {
	// This view controller will draw an image, off of the main thread,
	// and display the results in resultImageView.
	// While it's drawing, it shows an activity indicator (spinner).
	public partial class After2SlowDrawingViewController : UIViewController {
		public After2SlowDrawingViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// DARK MODE ADOPTION: Changed to use a new iOS 13 activity
			// indicator style which appears correct in light and dark modes.
			activityIndicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Large;
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			// After our view has appeared, start performing the
			// slow drawing operation.
			PerformSlowDrawing ();
		}

		void PerformSlowDrawing ()
		{
			// On the main queue:
			// Start the spinner.
			activityIndicator.StartAnimating ();

			// Determine how big the image should be.
			var size = resultImageView.Bounds.Size;

			// Specify what colors to draw.
			UIColor [] colors = { UIColor.White, UIColor.LightGray, UIColor.Gray, UIColor.FromName ("HeaderColor") };

#if DEBUG
			UIApplication.CheckForIllegalCrossThreadCalls = false;
#endif

			DispatchQueue.GetGlobalQueue (DispatchQueuePriority.High).DispatchAsync (() => {
				var image = CreateImage (size, colors);

				// And go back to the main queue to update the UI.
				DispatchQueue.MainQueue.DispatchAsync (() => {
					resultImageView.Image = image;
					activityIndicator.StopAnimating ();

#if DEBUG
					UIApplication.CheckForIllegalCrossThreadCalls = true;
#endif
				});
			});
		}

		UIImage CreateImage (CGSize size, UIColor [] colors)
		{
			// Perform some time-consuming drawing, using some colors of our choice.
			// This example just draws a lot of random circles.
			var renderer = new UIGraphicsImageRenderer (size);
			return renderer.CreateImage (context => {
				var random = new Random (DateTime.Now.Millisecond);
				for (int i = 0; i <= 25000; i++) {
					var colorIndex = random.Next (0, colors.Length);
					var color = colors [colorIndex];

					color.SetFill ();

					var rectX = random.NextDouble () * size.Width;
					var rectY = random.NextDouble () * size.Height;
					var s = random.NextDouble () * 80;

					var rect = new CGRect (rectX, rectY, s, s);
					var path = UIBezierPath.FromOval (rect);

					path.Fill ();
				}
			});
		}
	}
}

