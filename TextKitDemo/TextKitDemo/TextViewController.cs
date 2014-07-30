using System;
using CoreGraphics;

using Foundation;
using UIKit;

namespace TextKitDemo
{
	public class TextViewController : UIViewController
	{
		public DemoModel model;
		NSObject notification;

		public TextViewController (IntPtr handle) : base (handle)
		{
		}

		public TextViewController () : base ()
		{
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);


			if (notification != null)
				notification.Dispose ();
			notification = UIApplication.Notifications.ObserveContentSizeCategoryChanged (delegate {
				PreferredContentSizeChanged ();
			});
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

			if (notification != null) {
				notification.Dispose ();
				notification = null;
			}
		}

		public virtual void PreferredContentSizeChanged ()
		{
		}
	}
}

