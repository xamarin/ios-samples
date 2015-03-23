using System;
using Foundation;
using UIKit;

namespace PrivacyPrompts {

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate {

		// This is used to respond to Notifications permissions (see NotificationsPrivacyController.cs)
		public event Action<UIUserNotificationSettings> NotificationsRegistered;

		public override UIWindow Window {
			get;
			set;
		}

		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}

		public override void DidRegisterUserNotificationSettings (UIApplication application, UIUserNotificationSettings notificationSettings)
		{
			var handler = NotificationsRegistered;
			if(handler != null)
				handler (notificationSettings);
		}
	}
}
