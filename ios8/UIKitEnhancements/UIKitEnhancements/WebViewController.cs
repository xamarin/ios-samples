using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace UIKitEnhancements
{
	partial class WebViewController : UIViewController
	{
		#region Computed Properties
		public string URL { get; set; }
		#endregion

		#region Constructors
		public WebViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Shows the busy indicator
		/// </summary>
		private void ShowBusy() {

			//Define Animation
			UIView.BeginAnimations("Show");
			UIView.SetAnimationDuration(1.0f);

			Loading.Alpha = 0.5f;

			//Execute Animation
			UIView.CommitAnimations();
		}

		/// <summary>
		/// Hides the busy.
		/// </summary>
		private void HideBusy() {

			//Define Animation
			UIView.BeginAnimations("Hide");
			UIView.SetAnimationDuration(1.0f);

			Loading.Alpha = 0f;

			//Execute Animation
			UIView.CommitAnimations();
		}
		#endregion

		#region Override Methods
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Hide Loading Indicator
			Loading.Alpha = 0.0f;

			// Display the give webpage
			WebView.LoadRequest(new NSUrlRequest(NSUrl.FromString(URL)));

			// Wireup Webview notifications
			WebView.LoadStarted += (sender, e) => {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
				ShowBusy();
			};

			WebView.LoadFinished += (sender, e) => {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
				HideBusy();
			};
		}
		#endregion
	}
}
