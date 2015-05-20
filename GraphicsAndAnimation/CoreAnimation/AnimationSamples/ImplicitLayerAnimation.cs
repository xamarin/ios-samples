using System;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace AnimationSamples
{
	public class ImplicitLayerAnimation : UIViewController
	{
		static UIStoryboard MainStoryboard = UIStoryboard.FromName ("Main", null);

		CALayer layer;

		public ImplicitLayerAnimation ()
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.White;
			View.UserInteractionEnabled = true;

			layer = new CALayer {
				Bounds = new CGRect (50, 50, 50, 50),
				Position = new CGPoint (150, 50),
				Contents = UIImage.FromFile ("monkey1.png").CGImage,
				ContentsGravity = CALayer.GravityResize,
				BorderWidth = 1.5f,
				BorderColor = UIColor.Green.CGColor
			};

			View.Layer.AddSublayer (layer);

			View.AddGestureRecognizer (new UITapGestureRecognizer (() => {
				ViewController initalViewController = (ViewController)MainStoryboard.InstantiateViewController("InitalViewController");

				initalViewController.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;

				PresentViewController(initalViewController, true, null);
			}));
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			CATransaction.Begin();
			CATransaction.AnimationDuration = 10;
			layer.Position = new CGPoint (150, 550);
			layer.BorderWidth = 5.0f;
			layer.BorderColor = UIColor.Red.CGColor;
			CATransaction.Commit ();
		}
	}
}