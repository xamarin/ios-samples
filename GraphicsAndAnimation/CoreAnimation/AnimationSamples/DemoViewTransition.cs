using System;
using UIKit;

namespace AnimationSamples
{
	public class DemoViewTransition : UIViewController
	{
		static readonly UIStoryboard MainStoryboard =  UIStoryboard.FromName ("Main", null);

		UIView fromView;
		UIView toView;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.White;

			fromView = new UIImageView (UIImage.FromFile ("monkey1.png")) {
				Frame = View.Frame,
				ContentMode = UIViewContentMode.ScaleAspectFit,
				UserInteractionEnabled = true
			};
			View.AddSubview (fromView);

			toView = new UIImageView (UIImage.FromFile ("monkey2.png")) {
				Frame = View.Frame,
				ContentMode = UIViewContentMode.ScaleAspectFit,
				UserInteractionEnabled = true
			};
			View.AddSubview (fromView);

			var options = UIViewAnimationOptions.TransitionFlipFromTop | UIViewAnimationOptions.CurveEaseInOut;
			fromView.AddGestureRecognizer (new UITapGestureRecognizer (() => UIView.Transition (fromView, toView, 2, options, () => Console.WriteLine ("transition complete"))));

			toView.AddGestureRecognizer (new UITapGestureRecognizer (() => {
				var initalViewController = (ViewController)MainStoryboard.InstantiateViewController("InitalViewController");
				initalViewController.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
				PresentViewController(initalViewController, true, null);
			}));
		}
	}
}