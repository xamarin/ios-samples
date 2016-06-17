using CoreGraphics;
using CoreAnimation;
using UIKit;

namespace AnimationSamples
{
	public class ExplicitLayerAnimation : UIViewController
	{
		static UIStoryboard MainStoryboard = UIStoryboard.FromName ("Main", null);

		CALayer layer;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.White;
			View.UserInteractionEnabled = true;

			layer = new CALayer {
				Bounds = new CGRect (0, 0, 50, 50),
				Position = new CGPoint (50, 50),
				Contents = UIImage.FromFile ("monkey2.png").CGImage,
				ContentsGravity = CALayer.GravityResize,
				BorderWidth = 1.5f,
				BorderColor = UIColor.Green.CGColor
			};

			View.Layer.AddSublayer (layer);

			View.AddGestureRecognizer (new UITapGestureRecognizer (() => {
				var initalViewController = (ViewController)MainStoryboard.InstantiateViewController("InitalViewController");
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
			var path = new CGPath ();
			path.AddLines (new []{ fromPt, new CGPoint (50, 300), new CGPoint (200, 50), new CGPoint (200, 300) });

			// create a keyframe animation for the position using the path
			CAKeyFrameAnimation animPosition = CAKeyFrameAnimation.FromKeyPath ("position");
			animPosition.Path = path;
			animPosition.Duration = 2;

			/*  Add the animation to the layer.
			The "position" key is used to overwrite the implicit animation created
			 when the layer positino is set above*/
			layer.AddAnimation (animPosition, "position");
		}
	}
}