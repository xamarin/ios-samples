
using System;
using CoreGraphics;

using Foundation;
using UIKit;
using CoreAnimation;

namespace AnimationSamples
{
	public partial class ImplicitLayerAnimation : UIViewController
	{
		CALayer layer;

		public ImplicitLayerAnimation () : base ("ImplicitLayerAnimation", null)
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			layer = new CALayer ();
			layer.Bounds = new CGRect (0, 0, 50, 50);
			layer.Position = new CGPoint (50, 50);
			layer.Contents = UIImage.FromFile ("monkey2.png").CGImage;
			layer.ContentsGravity = CALayer.GravityResize;
			layer.BorderWidth = 1.5f;
			layer.BorderColor = UIColor.Green.CGColor;

			View.Layer.AddSublayer (layer);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			CATransaction.Begin ();
			CATransaction.AnimationDuration = 10;
			layer.Position = new CGPoint (50, 400);
			layer.BorderWidth = 5.0f;
			layer.BorderColor = UIColor.Red.CGColor;
			CATransaction.Commit ();
		}
	}
}

