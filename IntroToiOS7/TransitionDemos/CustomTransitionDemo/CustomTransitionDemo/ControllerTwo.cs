using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace CustomTransitionDemo
{
	public partial class ControllerTwo : UIViewController
	{
		UIButton showOne;

		public ControllerTwo ()
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.BackgroundColor = UIColor.LightGray;

			showOne = UIButton.FromType (UIButtonType.System);
			showOne.Frame = new CGRect (0, 0, 150, 100);
			showOne.SetTitle ("Show Controller One", UIControlState.Normal);

			showOne.TouchUpInside += (object sender, EventArgs e) => {

				this.DismissViewController (true, null);
			};

			View.AddSubview (showOne);
		}
	}
}

