using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace HandlingRotation.Screens.iPad.Method3SwapViews
{
	public class Controller : UIViewController
	{
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			switch (InterfaceOrientation) {
			case UIInterfaceOrientation.LandscapeLeft:
			case UIInterfaceOrientation.LandscapeRight:
				NSBundle.MainBundle.LoadNib ("LandscapeView", this, null);
				break;
			case UIInterfaceOrientation.Portrait:
			case UIInterfaceOrientation.PortraitUpsideDown:
				NSBundle.MainBundle.LoadNib ("PortraitView", this, null);
				break;
			}
		}
	}
}

