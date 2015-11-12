using System;

using Foundation;
using UIKit;
using NotificationCenter;
using WebKit;

namespace ExtensionsDemo {
	public partial class ExtensionsDemoViewController : UIViewController {
		WKWebView webView;

		public ExtensionsDemoViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			var controller = NCWidgetController.GetWidgetController ();
			controller.SetHasContent (true, "com.xamarin.ExtensionsDemo.EvolveCountdownWidget");
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			webView = new WKWebView (View.Frame, new WKWebViewConfiguration ());
			View.AddSubview (webView);

			var url = new NSUrl ("https://evolve.xamarin.com");
			var request = new NSUrlRequest (url);
			webView.LoadRequest (request);
		}
	}
}

