using System;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace AnimationSamples
{
	public class ImplicitLayerAnimation : UIViewController
	{

		CALayer layer;
		public static UIStoryboard Storyboard = UIStoryboard.FromName ("Main", null);


		public ImplicitLayerAnimation ()
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.White;

			layer = new CALayer ();
			layer.Bounds = new CGRect (50, 50, 50, 50);
			layer.Position = new CGPoint (150, 50);
			layer.Contents = UIImage.FromFile ("monkey1.png").CGImage;
			layer.ContentsGravity = CALayer.GravityResize;
			layer.BorderWidth = 1.5f;
			layer.BorderColor = UIColor.Green.CGColor;

			View.Layer.AddSublayer (layer);

			View.UserInteractionEnabled = true;

			View.AddGestureRecognizer (new UITapGestureRecognizer (() => {

				ViewController initalViewController = (ViewController)Storyboard.InstantiateViewController("InitalViewController");

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

