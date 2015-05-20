using System;
using UIKit;
using CoreGraphics;
using CoreAnimation;

namespace AnimationSamples
{
	public class ViewPropertyAnimation : UIViewController
	{
		UIStoryboard MainStoryboard = UIStoryboard.FromName ("Main", null);

		CGPoint pt;
		UIImageView imgView;
		UIImage img;
		UIButton animateButton;

		public ViewPropertyAnimation ()
		{
			
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.BackgroundColor = UIColor.White;
			View.UserInteractionEnabled = true;

			img = UIImage.FromFile ("monkey3.png");

			imgView = new UIImageView (new CGRect (50, 50, 150, 150)) {
				ContentMode = UIViewContentMode.ScaleAspectFit,
				Image = img
			};
			View.AddSubview (imgView);

			pt = imgView.Center;

			animateButton = new UIButton (UIButtonType.System) {
				Frame = new CGRect (50, 300, 300, 100)
			};
			animateButton.SetTitle ("Click me to Animate", UIControlState.Normal);

			View.AddSubview (animateButton);

			animateButton.TouchUpInside += (object sender, EventArgs e) => {
				UIView.Animate (
					duration: 2, 
					delay: 0, 
					options: UIViewAnimationOptions.CurveEaseInOut | UIViewAnimationOptions.Autoreverse,
					animation: () => {
						imgView.Center = 
							new CGPoint (UIScreen.MainScreen.Bounds.Right - imgView.Frame.Width / 2, imgView.Center.Y);},
					completion: () => {
						imgView.Center = pt; }
				);
			};

			View.AddGestureRecognizer (new UITapGestureRecognizer (() => {
				ViewController initalViewController = (ViewController)MainStoryboard.InstantiateViewController("InitalViewController");

				initalViewController.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;

				PresentViewController(initalViewController, true, null);
			}));
		}
	}
}