using System;
using System.IO;
using CoreGraphics;
using Foundation;
using UIKit;

namespace HttpClientSample
{
	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class AppDelegate : UIApplicationDelegate
	{
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			window.RootViewController = navigationController;

			button1.TouchDown += Button1TouchDown;
			TableViewSelector.Configure (stack, new [] {
				"http  - WebRequest",
				"https - WebRequest",
				"http  - NSUrlConnection",
				"http  - HttpClient",
				"https - HttpClient"
			});

			window.MakeKeyAndVisible ();

			return true;
		}

		async void Button1TouchDown (object sender, EventArgs e)
		{
			// Do not queue more than one request
			if (UIApplication.SharedApplication.NetworkActivityIndicatorVisible)
				return;

			HandlerType = null;
			button1.Enabled = false;
			switch (stack.SelectedRow ()) {
			case 0:
				new DotNet (this).HttpSample ();
				break;
			case 1:
				new DotNet (this).HttpSecureSample ();
				break;
			case 2:
				new Cocoa (this).HttpSample ();
				break;
			case 3:
				await new NetHttp (this).HttpSample (secure: false);
				break;
			case 4:
				await new NetHttp (this).HttpSample (secure: true);
			break;
			}
		}

		public Type HandlerType { get; set; }

		public void RenderStream (Stream stream)
		{
			var reader = new StreamReader (stream);

			InvokeOnMainThread (delegate {
				button1.Enabled = true;
				var view = new UIViewController ();
				var handler = new UILabel (new CGRect (20, 20, 300, 40)) {
					Text = "HttpClient is using " + HandlerType?.Name,
					Lines = 0
				};
				handler.SizeToFit ();

				var label = new UILabel (new CGRect (20, 40, 300, 80)) {
					Text = "The HTML returned by the server:"
				};
				var tv = new UITextView (new CGRect (20, 100, 300, 400)) {
					Text = reader.ReadToEnd ()
				};
				if (HandlerType != null)
					view.Add (handler);
				view.Add (label);
				view.Add (tv);

				if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) {
					view.EdgesForExtendedLayout = UIRectEdge.None;
				}

				navigationController.PushViewController (view, true);
			});
		}
	}
}