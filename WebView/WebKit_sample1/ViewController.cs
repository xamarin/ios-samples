using Foundation;
using SafariServices;
using System;
using UIKit;
using WebKit;

namespace WebView
{
    public partial class ViewController : UIViewController
    {
        private NSUrl url = new NSUrl("https://xamarin.com");

        protected ViewController(IntPtr handle) : base(handle) { }

        partial void OpenWebView(UIButton sender)
        {
            var webView = new WKWebView(View.Frame, new WKWebViewConfiguration());
            View.AddSubview(webView);

            webView.LoadRequest(new NSUrlRequest(url));
        }

        partial void OpenSafari(UIButton sender)
        {
            UIApplication.SharedApplication.OpenUrl(url);
        }

        partial void OpenSafariViewController(UIButton sender)
        {
            var viewController = new SFSafariViewController(url);
            PresentViewController(viewController, true, null);
        }
    }
}