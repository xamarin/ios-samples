using System;
using UIKit;
using NotificationCenter;
using CoreGraphics;
using Foundation;

namespace DayCountExtension
{
	[Register ("CodeViewController")]
	public class CodeViewController : UIViewController, INCWidgetProviding
	{
		public CodeViewController ()
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var v = new UIView (new CGRect (0, 0, UIScreen.MainScreen.Bounds.Width, 55f));
			v.BackgroundColor = UIColor.Blue;
			this.PreferredContentSize = new CGSize (UIScreen.MainScreen.Bounds.Width, 55f);
			this.View = v;
		}
	}
}

