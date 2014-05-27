
using System;
using CoreGraphics;

using Foundation;
using UIKit;

using CoreAnimation;

namespace AnimationSamples
{
	public partial class ViewAnimation : UIViewController
	{
		CGPoint pt;
		UIImageView imgView;
		UIImage img;

		public ViewAnimation () : base ("ViewAnimation", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			imgView = new UIImageView (new CGRect (0, 0, 100, 100));
			imgView.ContentMode = UIViewContentMode.ScaleAspectFit;
			img = UIImage.FromFile ("monkey3.png");
			imgView.Image = img;
			View.AddSubview (imgView);

			pt = imgView.Center;

			UIView.Animate (
				duration: 2, 
				delay: 0, 
				options: UIViewAnimationOptions.CurveEaseInOut | UIViewAnimationOptions.Autoreverse,
				animation: () => {
					imgView.Center = new CGPoint (View.Bounds.GetMaxX () - imgView.Frame.Width / 2, pt.Y);},
				completion: () => {
					imgView.Center = pt; }
			);
		}
	}
}

