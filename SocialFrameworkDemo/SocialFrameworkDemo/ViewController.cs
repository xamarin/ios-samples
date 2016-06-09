	using System;
	using Foundation;
	using Social;
	using Accounts;
	using UIKit;

	namespace SocialFrameworkDemo
	{
		public partial class ViewController : UIViewController
		{
			#region Private Variables
			private SLComposeViewController _twitterComposer = SLComposeViewController.FromService (SLServiceType.Twitter);
			private ACAccount _twitterAccount;

			private SLComposeViewController _facebookComposer = SLComposeViewController.FromService (SLServiceType.Facebook);
			private ACAccount _facebookAccount;
			#endregion

			#region Computed Properties
			public bool isTwitterAvailable {
				get { return SLComposeViewController.IsAvailable (SLServiceKind.Twitter); }
			}

			public SLComposeViewController TwitterComposer {
				get { return _twitterComposer; }
			}

			public ACAccount TwitterAccount {
				get { return _twitterAccount; }
			}

			public bool isFacebookAvailable {
				get { return SLComposeViewController.IsAvailable (SLServiceKind.Facebook); }
			}

			public SLComposeViewController FacebookComposer {
				get { return _facebookComposer; }
			}

			public ACAccount FacebookAccount {
				get { return _facebookAccount; }
			}
			#endregion

			#region Constructors
			protected ViewController (IntPtr handle) : base (handle)
			{
				// Note: this .ctor should not contain any initialization logic.
			}
			#endregion

			#region Override Methods
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
				accountStore.RequestAccess (accountType, (granted, error) => {
					// Allowed by user?
					if (granted) {
						// Get account
						_twitterAccount = accountStore.Accounts [accountStore.Accounts.Length - 1];
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
						// Get account
						_facebookAccount = accountStore.Accounts [accountStore.Accounts.Length - 1];
						InvokeOnMainThread (() => {
							// Update UI
							RequestFacebookTimeline.Enabled = true;
						});
					}
				});

			}
			#endregion

			#region Actions
			partial void SendTweet_TouchUpInside (UIButton sender)
			{
				// Set initial message
				TwitterComposer.SetInitialText ("Hello Twitter!");
				TwitterComposer.AddImage (UIImage.FromFile ("Icon.png"));
				TwitterComposer.CompletionHandler += (result) => {
					InvokeOnMainThread (() => {
						DismissViewController (true, null);
						Console.WriteLine ("Results: {0}", result);
					});
				};

				// Display controller
				PresentViewController (TwitterComposer, true, null);
			}

			partial void RequestTwitterTimeline_TouchUpInside (UIButton sender)
			{
				// Initialize request
				var parameters = new NSDictionary ();
				var url = new NSUrl("https://api.twitter.com/1.1/statuses/user_timeline.json?count=10");
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
								Results.Text = string.Format ("Error: {0}", response.StatusCode);
							});
						}
					} else {
						// No, display error
						InvokeOnMainThread (() => {
							Results.Text = string.Format ("Error: {0}", error);
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
						Console.WriteLine ("Results: {0}", result);
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
								Results.Text = string.Format ("Error: {0}", response.StatusCode);
							});
						}
					} else {
						// No, display error
						InvokeOnMainThread (() => {
							Results.Text = string.Format ("Error: {0}", error);
						});
					}
				});
			}
			#endregion
		}
	}

