using System;
using Foundation;
using UIKit;

namespace Reachability {
	public partial class ViewController : UIViewController {
		private Reachability internetReachability;

		private Reachability hostReachability;

		protected ViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			summaryLabel.Hidden = true;

			// Change the host name here to change the server you want to monitor.
			const string remoteHostName = "www.google.com";
			remoteHostLabel.Text = $"Remote Host: {remoteHostName}";

			hostReachability = Reachability.ReachabilityWithHostName (remoteHostName);
			hostReachability.StartNotifier ();
			UpdateInterfaceWithReachability (hostReachability);

			internetReachability = Reachability.ReachabilityForInternetConnection ();
			internetReachability.StartNotifier ();
			UpdateInterfaceWithReachability (internetReachability);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			// Observe the kNetworkReachabilityChangedNotification. When that notification is posted, the method reachabilityChanged will be called.
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString (Reachability.ReachabilityChangedNotification), OnReachabilityChanged);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			NSNotificationCenter.DefaultCenter.RemoveObserver (this, Reachability.ReachabilityChangedNotification);
		}

		/// <summary>
		/// Called by Reachability whenever status changes.
		/// </summary>
		/// <param name="notification">Object.</param>
		private void OnReachabilityChanged (NSNotification notification)
		{
			var reachability = notification.Object as Reachability;
			UpdateInterfaceWithReachability (reachability);
		}

		private void UpdateInterfaceWithReachability (Reachability reachability)
		{
			if (reachability == hostReachability) {
				ConfigureTextField (remoteHostStatusField, remoteHostImageView, reachability);

				var networkStatus = reachability.CurrentReachabilityStatus ();
				var connectionRequired = reachability.ConnectionRequired ();
				var baseLabelText = connectionRequired ? "Cellular data network is available.\nInternet traffic will be routed through it after a connection is established."
													   : "Cellular data network is active.\nInternet traffic will be routed through it.";
				summaryLabel.Text = baseLabelText;
				summaryLabel.Hidden = networkStatus != NetworkStatus.ReachableViaWWAN;
			} else if (reachability == internetReachability) {
				ConfigureTextField (internetConnectionStatusField, internetConnectionImageView, reachability);
			}
		}

		private void ConfigureTextField (UITextField textField, UIImageView imageView, Reachability reachability)
		{
			var networkStatus = reachability.CurrentReachabilityStatus ();
			var connectionRequired = reachability.ConnectionRequired ();
			var statusString = string.Empty;

			switch (networkStatus) {
			case NetworkStatus.NotReachable:
				statusString = "Access Not Available";
				imageView.Image = UIImage.FromBundle ("stop-32.png");
				// Minor interface detail - connectionRequired may return YES even when the host is unreachable. We cover that up here...
				connectionRequired = false;
				break;

			case NetworkStatus.ReachableViaWWAN:
				statusString = "Reachable WWAN";
				imageView.Image = UIImage.FromBundle ("WWAN5.png");
				break;

			case NetworkStatus.ReachableViaWiFi:
				statusString = "Reachable WiFi";
				imageView.Image = UIImage.FromBundle ("Airport.png");
				break;
			}

			if (connectionRequired) {
				statusString = $"{statusString}, Connection Required";
			}

			textField.Text = statusString;
		}
	}
}
