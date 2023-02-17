using Foundation;
using System;
using UIKit;
using WebKit;

namespace UICatalog {
	public partial class WebViewController : UIViewController, IUITextFieldDelegate, IWKNavigationDelegate {
		public WebViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			webView.NavigationDelegate = this;
			LoadAddressUrl ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}

		private void LoadAddressUrl ()
		{
			var requestURL = NSUrl.FromString (addressTextField.Text);
			var request = new NSUrlRequest (requestURL);
			var navigation = webView.LoadRequest (request);
		}

		#region IUIWebViewDelegate

		[Export ("webView:didCommitNavigation:")]
		public void DidCommitNavigation (WKWebView webView, WKNavigation navigation)
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
		}

		[Export ("webView:didFinishNavigation:")]
		public void DidFinishNavigation (WKWebView webView, WKNavigation navigation)
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}

		[Export ("webView:didFailNavigation:withError:")]
		public void DidFailNavigation (WKWebView webView, WKNavigation navigation, NSError error)
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;

			// Report the error inside the web view.
			var localizedErrorMessage = "An error occured:";
			var errorHTML = string.Format ("<!doctype html><html><body><div style=\"width: 100%%; text-align: center; font-size: 36pt;\">{0} {1}</div></body></html>", localizedErrorMessage, error.Description);

			webView.LoadHtmlString (errorHTML, null);
		}

		#endregion

		#region IUITextFieldDelegate

		[Export ("textFieldShouldReturn:")]
		public bool ShouldReturn (UITextField textField)
		{
			textField.ResignFirstResponder ();
			LoadAddressUrl ();

			return true;
		}

		#endregion
	}
}
