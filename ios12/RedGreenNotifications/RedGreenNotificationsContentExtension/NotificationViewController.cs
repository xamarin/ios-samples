using System;
using UIKit;
using UserNotifications;
using UserNotificationsUI;
using CoreGraphics;
using System.Drawing;
using Foundation;

namespace RedGreenNotificationsContentExtension {
	public partial class NotificationViewController : UIViewController, IUNNotificationContentExtension {
		int rotationButtonTaps = 0;
		UNNotification notification;

		protected NotificationViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}


		public void DidReceiveNotification (UNNotification notification)
		{
			this.notification = notification;
			if (notification.Request.Content.CategoryIdentifier == "red-category") {
				View.BackgroundColor = new UIColor (red: 0.55f, green: 0.00f, blue: 0.00f, alpha: 1.0f);
			} else {
				View.BackgroundColor = new UIColor (red: 0.40f, green: 0.47f, blue: 0.22f, alpha: 1.0f);
			}
		}

		[Export ("didReceiveNotificationResponse:completionHandler:")]
		public void DidReceiveNotificationResponse (UNNotificationResponse response, Action<UNNotificationContentExtensionResponseOption> completionHandler)
		{
			var rotationAction = ExtensionContext.GetNotificationActions () [0];

			if (response.ActionIdentifier == "rotate-twenty-degrees-action") {
				rotationButtonTaps += 1;

				double radians = (20 * rotationButtonTaps) * (2 * Math.PI / 360.0);
				Xamagon.Transform = CGAffineTransform.MakeRotation ((float) radians);

				// 9 rotations * 20 degrees = 180 degrees. No reason to
				// show the reset rotation button when the image is half
				// or fully rotated.
				if (rotationButtonTaps % 9 == 0) {
					ExtensionContext.SetNotificationActions (new UNNotificationAction [] { rotationAction });
				} else if (rotationButtonTaps % 9 == 1) {
					var resetRotationAction = UNNotificationAction.FromIdentifier ("reset-rotation-action", "Reset rotation", UNNotificationActionOptions.None);
					ExtensionContext.SetNotificationActions (new UNNotificationAction [] { rotationAction, resetRotationAction });
				}
			}

			if (response.ActionIdentifier == "reset-rotation-action") {
				rotationButtonTaps = 0;

				double radians = (20 * rotationButtonTaps) * (2 * Math.PI / 360.0);
				Xamagon.Transform = CGAffineTransform.MakeRotation ((float) radians);

				ExtensionContext.SetNotificationActions (new UNNotificationAction [] { rotationAction });
			}

			completionHandler (UNNotificationContentExtensionResponseOption.DoNotDismiss);
		}

		partial void HandleLaunchAppButtonTap (UIButton sender)
		{
			ExtensionContext.PerformNotificationDefaultAction ();
		}

		partial void HandleDismissNotificationButtonTap (UIButton sender)
		{
			ExtensionContext.DismissNotificationContentExtension ();
		}

		partial void HandleRemoveNotificationButtonTap (UIButton sender)
		{
			ExtensionContext.DismissNotificationContentExtension ();
			UNUserNotificationCenter.Current.RemoveDeliveredNotifications (new string [] { notification.Request.Identifier });
		}

		partial void HandleSliderValueChanged (UISlider sender)
		{
			Xamagon.Alpha = sender.Value;
		}
	}
}
