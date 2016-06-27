using System;

using UIKit;
using CloudKit;
using Foundation;

using static UIKit.NSLayoutAttribute;
using static UIKit.NSLayoutRelation;
using System.Linq;

namespace CloudKitAtlas
{
	public class NotificationBar : UIView
	{
		NSLayoutConstraint heightConstraint;
		UILabel label;
		UIButton button;

		CKNotification notification;
		public CKNotification Notification {
			get {
				return notification;
			}
			set {
				notification = value;
				var navigationController = Window.RootViewController as UINavigationController;
				if (navigationController != null) {
					foreach (var viewController in navigationController.ViewControllers)
						viewController.NavigationItem.HidesBackButton = notification != null;
				}

				SetNeedsLayout ();
				Superview.LayoutIfNeeded ();
			}
		}

		public NotificationBar (NSCoder coder)
			: base (coder)
		{
			heightConstraint = NSLayoutConstraint.Create (this, Height, Equal, null, NoAttribute, 1, 0);
			TranslatesAutoresizingMaskIntoConstraints = false;
			BackgroundColor = UIColor.Black;

			AddConstraint (heightConstraint);
			label = new UILabel {
				Text = "You have a new CloudKit notification!",
				TextColor = UIColor.White,
				TextAlignment = UITextAlignment.Center,
				TranslatesAutoresizingMaskIntoConstraints = false,
				Hidden = true,
				UserInteractionEnabled = true
			};
			AddSubview (label);

			button = new UIButton ();
			button.SetTitle ("✕", UIControlState.Normal);
			button.AddTarget (Close, UIControlEvent.TouchDown);
			button.TranslatesAutoresizingMaskIntoConstraints = false;
			button.Hidden = true;

			AddSubview (button);

			var rightConstraint = NSLayoutConstraint.Create (this, RightMargin, Equal, button, Right, 1, 0);
			AddConstraint (rightConstraint);

			var centerConstraint = NSLayoutConstraint.Create (this, CenterY, Equal, button, CenterY, 1, 0);
			AddConstraint (centerConstraint);

			var leftConstraint = NSLayoutConstraint.Create (this, LeftMargin, Equal, label, Left, 1, 0);
			AddConstraint (leftConstraint);

			var rightLabelConstraint = NSLayoutConstraint.Create (button, Left, Equal, label, Right, 1, 8);
			AddConstraint (rightLabelConstraint);

			var centerLabelConstraint = NSLayoutConstraint.Create (null, CenterY, Equal, label, CenterY, 1, 0);
			AddConstraint (centerLabelConstraint);

			var tapGestureRecognizer = new UITapGestureRecognizer (ShowNotification);
			label.AddGestureRecognizer (tapGestureRecognizer);
		}

		void Close (object sender, EventArgs e)
		{
			Close ();
		}

		void Close ()
		{
			Animate (0.4, () => {
				label.Hidden = true;
				button.Hidden = true;
				heightConstraint.Constant = 0;
				notification = null;
			});
		}

		public void Show ()
		{
			Animate (0.4, () => {
				heightConstraint.Constant = Superview.Frame.Size.Height;
				label.Hidden = false;
				button.Hidden = false;
				Superview.LayoutIfNeeded ();
			});
		}

		void ShowNotification ()
		{
			if (Notification == null)
				return;

			var navigationController = Window.RootViewController as NavigationController;
			if (navigationController == null)
				return;

			var mainMenuViewController = navigationController.ViewControllers [0] as MainMenuTableViewController;
			if (mainMenuViewController == null)
				return;

			Close ();
			var topViewController = navigationController.TopViewController as CodeSampleViewController;
			if (topViewController?.SelectedCodeSample is MarkNotificationsReadSample) {
				topViewController.RunCode (topViewController.RunButton);
			} else {
				var notificationSample = mainMenuViewController.CodeSampleGroups.Last ().CodeSamples [0];
				navigationController.PerformSegue ("ShowLoadingView", new SegueArg { Sample = notificationSample });
			}
		}
	}
}
