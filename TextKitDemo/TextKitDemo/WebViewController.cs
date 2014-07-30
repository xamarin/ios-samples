using System;

using Foundation;
using UIKit;

namespace TextKitDemo
{
	public partial class WebViewController : UIViewController
	{
		public WebViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			toolBar.BarTintColor = UIColor.White;

			webView.LoadStarted += (sender, e) => {
				backwardButton.Enabled = webView.CanGoBack;
				forwardButton.Enabled = webView.CanGoForward;
			};

			webView.LoadFinished += (sender, e) => {
				backwardButton.Enabled = webView.CanGoBack;
				forwardButton.Enabled = webView.CanGoForward;
			};

			doneButton.Clicked += (sender, e) => {
				DismissViewController(true,null);
			};

			toolBar.Delegate = new MyUIToolbarDelegate ();
		}
	}

	public class MyUIToolbarDelegate : UIToolbarDelegate
	{
		public MyUIToolbarDelegate () : base ()
		{
		}

		public override UIBarPosition GetPositionForBar (IUIBarPositioning barPositioning)
		{
			return UIBarPosition.TopAttached;
		}
	}
}

