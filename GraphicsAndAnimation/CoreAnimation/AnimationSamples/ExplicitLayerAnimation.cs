using System;
using CoreGraphics;
using CoreAnimation;
using UIKit;

namespace AnimationSamples
{
	public class ExplicitLayerAnimation : UIViewController
	{

		CALayer layer;
		public static UIStoryboard MainStoryboard = UIStoryboard.FromName ("Main", null);

		public ExplicitLayerAnimation ()
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.White;

			layer = new CALayer ();
			layer.Bounds = new CGRect (0, 0, 50, 50);
			layer.Position = new CGPoint (50, 50);
			layer.Contents = UIImage.FromFile ("monkey2.png").CGImage;
			layer.ContentsGravity = CALayer.GravityResize;
			layer.BorderWidth = 1.5f;
			layer.BorderColor = UIColor.Green.CGColor;

			View.Layer.AddSublayer (layer);

			View.UserInteractionEnabled = true;

			View.AddGestureRecognizer (new UITapGestureRecognizer (() => {

				ViewController initalViewController = (ViewController)MainStoryboard.InstantiateViewController("InitalViewController");

				initalViewController.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;

				PresentViewController(initalViewController, true, null);
			}));
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			// get the initial value to start the animation from
			CGPoint fromPt = layer.Position;

			/* set the position to coincide with the final animation value
			to prevent it from snapping back to the starting position
			after the animation completes */
			layer.Position = new CGPoint (200, 300);

			//Create a path for the animation to follow
			CGPath path = new CGPath ();
			path.AddLines (new CGPoint[]{ fromPt, new CGPoint (50, 300), new CGPoint (200, 50), new CGPoint (200, 300) });

			// create a keyframe animation for the position using the path
			CAKeyFrameAnimation animPosition = (CAKeyFrameAnimation)CAKeyFrameAnimation.FromKeyPath ("position");
			animPosition.Path = path;
			animPosition.Duration = 2;

			/*  Add the animation to the layer.
			The "position" key is used to overwrite the implicit animation created
			 when the layer positino is set above*/
			layer.AddAnimation (animPosition, "position");

		}


	}
}

