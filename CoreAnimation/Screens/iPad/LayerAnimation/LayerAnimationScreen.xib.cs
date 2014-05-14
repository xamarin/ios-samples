
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using CoreAnimation;
using CoreGraphics;

namespace Example_CoreAnimation.Screens.iPad.LayerAnimation
{
	public partial class LayerAnimationScreen : UIViewController, IDetailView
	{
		public event EventHandler ContentsButtonClicked;

		private CGPath animationPath;
		private UIImageView backgroundImage;
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			animationPath = new CGPath ();
			backgroundImage = new UIImageView (View.Frame);
			View.AddSubview (backgroundImage);

			CreatePath ();

			btnContents.TouchUpInside += (sender, e) => {
				if(ContentsButtonClicked != null)
					ContentsButtonClicked(sender, e);
			};

			btnAnimate.TouchUpInside += (s, e) => {
				
				// create a keyframe animation
				var keyFrameAnimation = (CAKeyFrameAnimation)CAKeyFrameAnimation.FromKeyPath ("position");
				keyFrameAnimation.Path =  animationPath;
				keyFrameAnimation.Duration = 3;
				
				keyFrameAnimation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
				
				imgToAnimate.Layer.AddAnimation (keyFrameAnimation, "MoveImage");
				imgToAnimate.Layer.Position = new CGPoint (700f, 900f);
			};
		}
		
		// Creates the path that we'll use to animate on. Once the path is created, it calls
		// DrawPathAsBackground to draw the path on the screen.
		protected void CreatePath()
		{
			// define our path
			var curve1StartPoint = new CGPoint (56f, 104f);
			var curve1ControlPoint1 = new CGPoint (50f, 250f);
			var curve1ControlPoint2 = new CGPoint (220f, 450f);
			var curve1EndPoint = new CGPoint (384f, 450f);
			var curve2ControlPoint1 = new CGPoint (500f, 450f);
			var curve2ControlPoint2 = new CGPoint (700f, 650f);
			var curve2EndPoint = new CGPoint (700f, 900f);
			animationPath.MoveToPoint (curve1StartPoint.X, curve1StartPoint.Y);
			animationPath.AddCurveToPoint (curve1ControlPoint1.X, curve1ControlPoint1.Y,
										   curve1ControlPoint2.X, curve1ControlPoint2.Y,
										   curve1EndPoint.X, curve1EndPoint.Y);

			animationPath.AddCurveToPoint (curve2ControlPoint1.X, curve2ControlPoint1.Y,
										   curve2ControlPoint2.X, curve2ControlPoint2.Y,
										   curve2EndPoint.X, curve2EndPoint.Y);

			DrawPathAsBackground ();
		}
		
		// Draws our animation path on the background image, just to show it
		protected void DrawPathAsBackground ()
		{
			// create our offscreen bitmap context
			var bitmapSize = new CGSize (View.Frame.Size);
			using (var context = new CGBitmapContext (
				       IntPtr.Zero,
				       (int)bitmapSize.Width, (int)bitmapSize.Height, 8,
				       (int)(4 * bitmapSize.Width), CGColorSpace.CreateDeviceRGB (),
				       CGImageAlphaInfo.PremultipliedFirst)) {
				
				// convert to View space
				var affineTransform = CGAffineTransform.MakeIdentity ();
				// invert the y axis
				affineTransform.Scale (1f, -1f);
				// move the y axis up
				affineTransform.Translate (0, View.Frame.Height);
				context.ConcatCTM (affineTransform);

				// actually draw the path
				context.AddPath (animationPath);
				context.SetStrokeColor (UIColor.LightGray.CGColor);
				context.SetLineWidth (3f);
				context.StrokePath ();
				
				// set what we've drawn as the backgound image
				backgroundImage.Image = UIImage.FromImage (context.ToImage());
			}
		}
	}
}

