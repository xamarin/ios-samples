using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace MonkeyBrowse
{
	public partial class FourthViewController : UIViewController
	{
		#region Computed Properties
		/// <summary>
		/// Gets a value indicating whether this <see cref="MonkeyBrowse.FirstViewController"/> user interface idiom is phone.
		/// </summary>
		/// <value><c>true</c> if user interface idiom is phone; otherwise, <c>false</c>.</value>
		public bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		/// <summary>
		/// Returns the delegate of the current running application
		/// </summary>
		/// <value>The this app.</value>
		public AppDelegate ThisApp {
			get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
		}

		/// <summary>
		/// Gets or sets the user activity.
		/// </summary>
		/// <value>The user activity.</value>
		public NSUserActivity UserActivity { get; set; }
		#endregion

		#region Constructors
		public FourthViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Navigates the Webview to the given URL string.
		/// </summary>
		/// <param name="url">URL.</param>
		private void NavigateToURL(string url) {

			// Properly formatted?
			if (!url.StartsWith ("http://")) {
				// Add web
				url = "http://" + url;
			}

			// Display the give webpage
			WebView.LoadRequest(new NSUrlRequest(NSUrl.FromString(url)));

			// Invalidate existing Activity
			if (UserActivity != null) {
				UserActivity.Invalidate();
				UserActivity = null;
			}

			// Create a new user Activity to support this tab
			UserActivity = new NSUserActivity (ThisApp.UserActivityTab4);
			UserActivity.Title = "Coffee Break Tab";

			// Update the activity when the tab's URL changes
			var userInfo = new NSMutableDictionary ();
			userInfo.Add (new NSString ("Url"), new NSString (url));
			UserActivity.AddUserInfoEntries (userInfo);

			// Inform Activity that it has been updated
			UserActivity.BecomeCurrent ();

			// Log User Activity
			Console.WriteLine ("Creating User Activity: {0} - {1}", UserActivity.Title, url);
		}

		/// <summary>
		/// Shows the busy indicator
		/// </summary>
		/// <param name="reason">Reason.</param>
		private void ShowBusy(string reason) {

			// Display reason
			BusyText.Text = reason;

			//Define Animation
			UIView.BeginAnimations("Show");
			UIView.SetAnimationDuration(1.0f);

			Handoff.Alpha = 0.5f;

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

			Handoff.Alpha = 0f;

			//Execute Animation
			UIView.CommitAnimations();
		}
		#endregion

		#region Public Methods
		public void PreparingToHandoff() {
			// Inform caller
			ShowBusy ("Continuing Activity...");
		}

		public void PerformHandoff(NSUserActivity activity) {

			// Hide busy indicator
			HideBusy ();

			// Extract URL from dictionary
			var url = activity.UserInfo ["Url"].ToString ();

			// Display value
			URL.Text = url;

			// Display the give webpage
			WebView.LoadRequest(new NSUrlRequest(NSUrl.FromString(url)));

			// Save activity
			UserActivity = activity;
			UserActivity.BecomeCurrent ();

		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Views the did load.
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Hide Handoff notification
			Handoff.Alpha = 0f;

			// Attach to the App Delegate
			ThisApp.Tab4 = this;

			// Wireup Webview notifications
			WebView.LoadStarted += (sender, e) => {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
				ShowBusy(string .Format("Loading {0}...",URL.Text));
			};

			WebView.LoadFinished += (sender, e) => {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
				HideBusy();
			};

			// Configure URL entry field
			URL.Placeholder = "(enter url)";

			// Wire-up URL field
			URL.ShouldReturn = delegate (UITextField field){
				field.ResignFirstResponder ();
				NavigateToURL(field.Text);

				return true;
			};

			// Wire-up the Go Button
			GoButton.Clicked += (sender, e) => {
				NavigateToURL(URL.Text);
			};

		}

		/// <summary>
		/// Restores the state of the user activity.
		/// </summary>
		/// <param name="activity">Activity.</param>
		public override void RestoreUserActivityState (NSUserActivity activity)
		{
			base.RestoreUserActivityState (activity);

			// Log activity
			Console.WriteLine ("Restoring Activity {0}", activity.Title);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}
		#endregion
	}
}
