using System;
using System.Drawing;

using Foundation;
using UIKit;
using Social;

namespace SocialFrameworkDemo
{
	public partial class SocialFrameworkDemoViewController : UIViewController
	{
		SLComposeViewController slComposer;

		public SocialFrameworkDemoViewController () : base ("SocialFrameworkDemoViewController", null)
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			twitterButton.TouchUpInside += delegate {

				if (SLComposeViewController.IsAvailable (SLServiceKind.Twitter)) {
				
					slComposer = SLComposeViewController.FromService (SLServiceType.Twitter);

					slComposer.SetInitialText ("test");
					slComposer.AddImage (UIImage.FromFile ("monkey.png"));

					slComposer.CompletionHandler += (result) => {
						InvokeOnMainThread (() => {
							DismissViewController (true, null);
							resultsTextView.Text = result.ToString ();
						});
					};

					PresentViewController (slComposer, true, null);
				} else {
					resultsTextView.Text = "Twitter Account not added";
				}
			};

			facebookButton.TouchUpInside += delegate {
				
				if (SLComposeViewController.IsAvailable (SLServiceKind.Facebook)) {				

					slComposer = SLComposeViewController.FromService (SLServiceType.Facebook);

					slComposer.SetInitialText ("test2");
					slComposer.AddImage (UIImage.FromFile ("monkey.png"));
					slComposer.AddUrl (new NSUrl ("http://xamarin.com"));
					
					slComposer.CompletionHandler += (result) => {
						InvokeOnMainThread (() => {
							DismissViewController (true, null);
							resultsTextView.Text = result.ToString ();
						});
					};
					
					PresentViewController (slComposer, true, null);
				} else {
					resultsTextView.Text = "Facebook Account not added";
				}				
			};

			twitterRequestButton.TouchUpInside += delegate {

				if (SLComposeViewController.IsAvailable (SLServiceKind.Twitter)) {

					var parameters = new NSDictionary ();
					var request = SLRequest.Create (SLServiceKind.Twitter,
					                                SLRequestMethod.Get,
					                                new NSUrl ("http://api.twitter.com/1/statuses/public_timeline.json"),
					                                parameters);
				
					request.PerformRequest ((data, response, error) => {
					
						if (response.StatusCode == 200) {
							InvokeOnMainThread (() => {
								resultsTextView.Text = data.ToString (); });
						
						} else {
							InvokeOnMainThread (() => {
								resultsTextView.Text = "Error: " + response.StatusCode.ToString (); });
						}
					});
				} else {
					resultsTextView.Text = "Twitter Account not added";
				}
			};

			facebookRequestButton.TouchUpInside += delegate {

				if (SLComposeViewController.IsAvailable (SLServiceKind.Facebook)) {
					
					var parameters = new NSDictionary ();
					var request = SLRequest.Create (SLServiceKind.Facebook,
					                                SLRequestMethod.Get,
					                                new NSUrl ("https://graph.facebook.com/283148898401104"),
					                                parameters);
					
					request.PerformRequest ((data, response, error) => {
						
						if (response.StatusCode == 200) {
							InvokeOnMainThread (() => {
								resultsTextView.Text = data.ToString (); });
							
						} else {
							InvokeOnMainThread (() => {
								resultsTextView.Text = "Error: " + response.StatusCode.ToString (); });
						}
					});
				} else {
					resultsTextView.Text = "Facebook Account not added";
				}

			};

		}

	}
}

