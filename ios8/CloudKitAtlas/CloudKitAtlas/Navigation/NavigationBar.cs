using System;

using UIKit;
using Foundation;
using CloudKit;

using static UIKit.NSLayoutAttribute;
using static UIKit.NSLayoutRelation;

namespace CloudKitAtlas
{
	public partial class NavigationBar : UINavigationBar
	{
		NotificationBar NotificationBar { get; set; }

		public NavigationBar (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public NavigationBar (NSCoder coder)
			: base (coder)
		{
			NotificationBar = new NotificationBar (coder);

			BarStyle = UIBarStyle.Black;
			BarTintColor = new UIColor (0.25f, 0.29f, 0.36f, 1);
			TintColor = UIColor.White;

			AddSubview (NotificationBar);

			var leftConstraint = NSLayoutConstraint.Create (null, Leading, Equal, NotificationBar, Leading, 1, 0);
			AddConstraint (leftConstraint);

			var rightConstraint = NSLayoutConstraint.Create (null, Trailing, Equal, NotificationBar, Trailing, 1, 0);
			AddConstraint (rightConstraint);

			var topConstraint = NSLayoutConstraint.Create (this, Top, Equal, NotificationBar, Top, 1, 0);
			AddConstraint (topConstraint);
		}

		public void ShowNotificationAlert (CKNotification notification)
		{
			if (NotificationBar.Notification == null) {
				NotificationBar.Notification = notification;
				BringSubviewToFront (NotificationBar);
				NotificationBar.Show ();
			}
		}
	}
}
