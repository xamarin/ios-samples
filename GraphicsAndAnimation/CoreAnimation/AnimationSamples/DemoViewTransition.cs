using System;
using UIKit;

namespace AnimationSamples
{
	public class DemoViewTransition : UIViewController
	{
		static UIStoryboard MainStoryboard =  UIStoryboard.FromName ("Main", null);

		UIView view1;
		UIView view2;

		public DemoViewTransition ()
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.White;

			view1 = new UIImageView (UIImage.FromFile ("monkey1.png")) {
				Frame = View.Frame,
				ContentMode = UIViewContentMode.ScaleAspectFit,
				UserInteractionEnabled = true
			};
			View.AddSubview (view1);

			view2 = new UIImageView (UIImage.FromFile ("monkey2.png")) {
				Frame = View.Frame,
				ContentMode = UIViewContentMode.ScaleAspectFit,
				UserInteractionEnabled = true
			};
			View.AddSubview (view1);

			view1.AddGestureRecognizer (new UITapGestureRecognizer (() => { 
				UIView.Transition (
					fromView: view1,
					toView: view2,
					duration: 2,
					options: UIViewAnimationOptions.TransitionFlipFromTop | UIViewAnimationOptions.CurveEaseInOut,
					completion: () => { Console.WriteLine ("transition complete"); });
			}));

			view2.AddGestureRecognizer (new UITapGestureRecognizer (() => {
				ViewController initalViewController = (ViewController)MainStoryboard.InstantiateViewController("InitalViewController");

				initalViewController.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;

				PresentViewController(initalViewController, true, null);
			}));
		}
	}
}