using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace PrivacyPrompts {

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate {

		public override UIWindow Window {
			get;
			set;
		}

		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}

#region Notifications 
		//This is used to respond to Notifications permissions (see NotificationsPrivacyController.cs)
		public event Action<UIUserNotificationSettings> NotificationsRegistered;

		public override void DidRegisterUserNotificationSettings (UIApplication application, UIUserNotificationSettings notificationSettings)
		{
			NotificationsRegistered (notificationSettings);
		}

#endregion
	}
}