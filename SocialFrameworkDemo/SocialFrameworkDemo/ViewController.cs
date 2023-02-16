using System;

using Accounts;
using Foundation;
using Social;
using UIKit;

namespace SocialFrameworkDemo {
	public partial class ViewController : UIViewController {
		SLComposeViewController twitterComposer = SLComposeViewController.FromService (SLServiceType.Twitter);
		ACAccount twitterAccount;

		SLComposeViewController facebookComposer = SLComposeViewController.FromService (SLServiceType.Facebook);
		ACAccount facebookAccount;

		public bool isTwitterAvailable {
			get {
				return SLComposeViewController.IsAvailable (SLServiceKind.Twitter);
			}
		}

		public SLComposeViewController TwitterComposer {
			get {
				return twitterComposer;
			}
		}

		public ACAccount TwitterAccount {
			get {
				return twitterAccount;
			}
		}

		public bool isFacebookAvailable {
			get {
				return SLComposeViewController.IsAvailable (SLServiceKind.Facebook);
			}
		}

		public SLComposeViewController FacebookComposer {
			get {
				return facebookComposer;
			}
		}

		public ACAccount FacebookAccount {
			get {
				return facebookAccount;
			}
		}

		protected ViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Update UI based on state
			SendTweet.Enabled = isTwitterAvailable;
			RequestTwitterTimeline.Enabled = false;
			PostToFacebook.Enabled = isFacebookAvailable;
			RequestFacebookTimeline.Enabled = false;

			// Initialize Twitter Account access 
			var accountStore = new ACAccountStore ();
			var accountType = accountStore.FindAccountType (ACAccountType.Twitter);

			// Request access to Twitter account
			accountStore.RequestAccess (accountType, null, (granted, error) => {
				// Allowed by user?
				if (granted) {
					// Get account
					if (accountStore.Accounts.Length == 0)
						return;

					twitterAccount = accountStore.Accounts [accountStore.Accounts.Length - 1];
					InvokeOnMainThread (() => {
						// Update UI
						RequestTwitterTimeline.Enabled = true;
					});
				}
			});

			// Initialize facebook Account access 
			var options = new AccountStoreOptions ();
			options.FacebookAppId = ""; // Enter your specific Facebook App ID here
			accountType = accountStore.FindAccountType (ACAccountType.Facebook);

			// Request access to Facebook account
			accountStore.RequestAccess (accountType, options, (granted, error) => {
				// Allowed by user?
				if (granted) {
					if (accountStore.Accounts.Length == 0)
						return;

					// Get account
					facebookAccount = accountStore.Accounts [accountStore.Accounts.Length - 1];
					InvokeOnMainThread (() => {
						// Update UI
						RequestFacebookTimeline.Enabled = true;
					});
				}
			});

		}

		partial void SendTweet_TouchUpInside (UIButton sender)
		{
			// Set initial message
			TwitterComposer.SetInitialText ("Hello Twitter!");
			TwitterComposer.AddImage (UIImage.FromFile ("Icon.png"));
			TwitterComposer.CompletionHandler += (result) => {
				InvokeOnMainThread (() => {
					DismissViewController (true, null);
					Console.WriteLine ($"Results: {result}");
				});
			};

			// Display controller
			PresentViewController (TwitterComposer, true, null);
		}

		partial void RequestTwitterTimeline_TouchUpInside (UIButton sender)
		{
			// Initialize request
			var parameters = new NSDictionary ();
			var url = new NSUrl ("https://api.twitter.com/1.1/statuses/user_timeline.json?count=10");
			var request = SLRequest.Create (SLServiceKind.Twitter, SLRequestMethod.Get, url, parameters);

			// Request data
			request.Account = TwitterAccount;
			request.PerformRequest ((data, response, error) => {
				// Was there an error?
				if (error == null) {
					// Was the request successful?
					if (response.StatusCode == 200) {
						// Yes, display it
						InvokeOnMainThread (() => {
							Results.Text = data.ToString ();
						});
					} else {
						// No, display error
						InvokeOnMainThread (() => {
							Results.Text = $"Error: {response.StatusCode}";
						});
					}
				} else {
					// No, display error
					InvokeOnMainThread (() => {
						Results.Text = $"Error: {error}";
					});
				}
			});
		}

		partial void PostToFacebook_TouchUpInside (UIButton sender)
		{
			// Set initial message
			FacebookComposer.SetInitialText ("Hello Facebook!");
			FacebookComposer.AddImage (UIImage.FromFile ("Icon.png"));
			FacebookComposer.CompletionHandler += (result) => {
				InvokeOnMainThread (() => {
					DismissViewController (true, null);
					Console.WriteLine ($"Results: {result}");
				});
			};

			// Display controller
			PresentViewController (FacebookComposer, true, null);
		}

		partial void RequestFacebookTimeline_TouchUpInside (UIButton sender)
		{
			// Initialize request
			var parameters = new NSDictionary ();
			var url = new NSUrl ("https://graph.facebook.com/283148898401104");
			var request = SLRequest.Create (SLServiceKind.Facebook, SLRequestMethod.Get, url, parameters);

			// Request data
			request.Account = FacebookAccount;
			request.PerformRequest ((data, response, error) => {
				// Was there an error?
				if (error == null) {
					// Was the request successful?
					if (response.StatusCode == 200) {
						// Yes, display it
						InvokeOnMainThread (() => {
							Results.Text = data.ToString ();
						});
					} else {
						// No, display error
						InvokeOnMainThread (() => {
							Results.Text = $"Error: {response.StatusCode}";
						});
					}
				} else {
					// No, display error
					InvokeOnMainThread (() => {
						Results.Text = $"Error: {error}";
					});
				}
			});
		}
	}
}

