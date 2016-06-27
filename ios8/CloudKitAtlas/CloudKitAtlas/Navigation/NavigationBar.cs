using System;

using UIKit;
using Foundation;
using CloudKit;

namespace CloudKitAtlas
{
	public partial class NavigationBar : UINavigationBar
	{
		public NavigationBar (IntPtr handle) : base (handle)
		{
		}

		internal void ShowNotificationAlert (CKNotification notification)
		{
			throw new NotImplementedException ();
		}
	}
}
