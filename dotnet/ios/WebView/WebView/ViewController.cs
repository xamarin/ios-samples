using SafariServices;
using WebKit;

namespace WebView;

public partial class ViewController : UIViewController
{
    private readonly NSUrl url = new ("https://visualstudio.microsoft.com/xamarin/");

    protected ViewController (IntPtr handle) : base (handle) { }

    partial void OpenWebView (UIButton sender)
    {
        if (View is null)
            throw new NullReferenceException ();
        
        var webView = new WKWebView (View.Frame, new WKWebViewConfiguration ());
        View.AddSubview (webView);
       
        webView.LoadRequest (new NSUrlRequest (url));
    }

    partial void OpenSafari (UIButton sender)
    {
        UIApplication.SharedApplication.OpenUrl (url);
    }

    partial void OpenSafariViewController (UIButton sender)
    {
        var viewController = new SFSafariViewController (url);
        PresentViewController (viewController, true, null);
    }
}