using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace CustomTransitionDemo
{
	public partial class ControllerOne : UIViewController
	{
		UIButton showTwo;
		ControllerTwo controllerTwo;
		TransitioningDelegate transitioningDelegate;

		public ControllerOne ()
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.BackgroundColor = UIColor.White;
			
			showTwo = UIButton.FromType (UIButtonType.System);
			showTwo.Frame = new CGRect (View.Frame.Width / 2 - 100, View.Frame.Height / 2 - 25, 200, 50);
			showTwo.SetTitle ("Show Controller Two", UIControlState.Normal);

			showTwo.TouchUpInside += (object sender, EventArgs e) => {

				controllerTwo = new ControllerTwo ();
				controllerTwo.ModalPresentationStyle = UIModalPresentationStyle.Custom;

				transitioningDelegate = new TransitioningDelegate ();
				controllerTwo.TransitioningDelegate = transitioningDelegate;

				this.PresentViewController (controllerTwo, true, null);
			};

			View.AddSubview (showTwo);
		}
	}
}

