using System;

using Foundation;
using UIKit;

namespace UICatalog
{
	[Register ("WebViewController")]
	public class WebViewController : UIViewController
	{
		[Outlet]
		UIWebView WebView { get; set; }

		[Outlet]
		UITextField AddressTextField { get; set; }

		public WebViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ConfigureWebView ();
			LoadAddressURL ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			if (UIApplication.SharedApplication.NetworkActivityIndicatorVisible)
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}

		void LoadAddressURL ()
		{
			var requestURL = NSUrl.FromString (AddressTextField.Text);
			var request = new NSUrlRequest (requestURL);
			WebView.LoadRequest (request);
		}

		void ConfigureWebView ()
		{
			WebView.BackgroundColor = UIColor.White;
			WebView.ScalesPageToFit = true;
			WebView.DataDetectorTypes = UIDataDetectorType.All;
		}

		#region UIWebViewDelegate

		[Export ("webViewDidStartLoad:")]
		void WebViewDidStartLoad (UIWebView webView)
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
		}

		[Export ("webViewDidFinishLoad:")]
		void WebViewDidFinishLoad (UIWebView webView)
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}

		[Export ("webView:didFailLoadWithError:")]
		void OnLoadFailed (UIWebView webView, NSError error)
		{
			// Report the error inside the web view.
			var localizedErrorMessage = "An error occured:".Localize ();
			var errorHTML = string.Format ("<!doctype html><html><body><div style=\"width: 100%%; text-align: center; font-size: 36pt;\">{0} {1}</div></body></html>", localizedErrorMessage, error.Description);

			webView.LoadHtmlString (errorHTML, null);
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}

		#endregion

		#region UITextFieldDelegate

		// This helps dismiss the keyboard when the "Done" button is clicked.
		[Export ("textFieldShouldReturn:")]
		bool TextFieldShouldReturn (UITextField textField)
		{
			textField.ResignFirstResponder ();
			LoadAddressURL ();

			return true;
		}

		#endregion
	}
}
