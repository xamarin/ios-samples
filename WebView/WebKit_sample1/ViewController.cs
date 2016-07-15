using System;
using Foundation;
using SafariServices;
using UIKit;
using WebKit;

namespace WebView
{
	public partial class ViewController : UIViewController
	{
		WKWebView webView;
		NSUrl url = new NSUrl ("https://xamarin.com");

		protected ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			webviewButton.TouchUpInside += (sender, e) => {
				webView = new WKWebView (View.Frame, new WKWebViewConfiguration ());
				View.AddSubview (webView);

				var request = new NSUrlRequest (url);
				webView.LoadRequest (request);
			};

			safariButton.TouchUpInside += (sender, e) => {

				var sfViewController = new SFSafariViewController (url);

				PresentViewController (sfViewController, true, null);
			};

			openSafari.TouchUpInside += (sender, e) => {

				UIApplication.SharedApplication.OpenUrl (url);
			};
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
