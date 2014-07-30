using System;

using Foundation;
using UIKit;

namespace TextKitDemo
{
	/*
	 * Provides the basic interaction (respond to tapping on the apple.com url and 
	 * updating the font size in response to changes in the UI).
	 */
	public partial class BasicInteractionViewController : TextViewController
	{
		public BasicInteractionViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if (model != null)
				textView.AttributedText = model.GetAttributedText ();
			else
				textView.AttributedText = new NSAttributedString ("Model is Missing!!");

			textView.ShouldInteractWithUrl = ShouldInteractWithUrl;
		}

		bool ShouldInteractWithUrl (UITextView textView, NSUrl url, NSRange characterRange)
		{
			if (url.Host == "www.xamarin.com") {
				var webViewController = (WebViewController)Storyboard.InstantiateViewController ("WebViewController");
				PresentViewController (webViewController, true, delegate {
					webViewController.webView.LoadRequest (NSUrlRequest.FromUrl (url));
				});
				return false;
			};
			return true;
		}

		public override void PreferredContentSizeChanged ()
		{
			var descriptor = textView.Font.FontDescriptor;
			textView.Font = UIFont.GetPreferredFontForTextStyle (descriptor.FontAttributes.TextStyle);
		}
	}
}

