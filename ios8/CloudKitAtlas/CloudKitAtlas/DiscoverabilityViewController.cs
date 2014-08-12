using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CloudKit;

namespace CloudKitAtlas
{
	public partial class DiscoverabilityViewController : UIViewController, ICloudViewController
	{
		public CloudManager CloudManager { get; set; }

		public DiscoverabilityViewController (IntPtr handle) : base (handle)
		{
		}

		public override async void ViewDidAppear (bool animated)
		{
			if (await CloudManager.RequestDiscoverabilityPermissionAsync ()) {
				var userInfo = await CloudManager.DiscoverUserInfoAsync ();
				DiscoveredUserInfo (userInfo);
			} else {
				var alert = UIAlertController.Create ("CloudKitAtlas", "Getting your name using Discoverability requires permission",
					UIAlertControllerStyle.Alert);

				alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, (act) => {
					DismissViewController (true, null);
				}));

				PresentViewController (alert, true, null);
			}
		}

		private void DiscoveredUserInfo (CKDiscoveredUserInfo user)
		{
			if (user == null)
				name.Text = "Anonymous";
			else
				name.Text = string.Format ("{0} {1}", user.FirstName, user.LastName);
		}
	}
}
