
using System;
using CoreGraphics;

using Foundation;
using UIKit;

namespace AnimationSamples
{
	public partial class DemoViewTransition : UIViewController
	{
		UIView view1;
		UIView view2;

		public DemoViewTransition () : base ("DemoViewTransition", null)
		{

		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			view1 = new UIImageView (UIImage.FromFile ("monkey1.png")) {
				Frame = View.Frame,
				ContentMode = UIViewContentMode.ScaleAspectFit
			};
			View.AddSubview (view1);
			
			view2 = new UIImageView (UIImage.FromFile ("monkey2.png")) {
				Frame = View.Frame,
				ContentMode = UIViewContentMode.ScaleAspectFit
			};

			view1.UserInteractionEnabled = true;

			view1.AddGestureRecognizer (new UITapGestureRecognizer (() => { 
				UIView.Transition (
					fromView: view1,
					toView: view2,
					duration: 2,
					options: UIViewAnimationOptions.TransitionFlipFromTop | UIViewAnimationOptions.CurveEaseInOut,
					completion: () => { Console.WriteLine ("transition complete"); });
			}));


		    
		}
	}
}

