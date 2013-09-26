using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Diagnostics;

namespace CalabashSample.iOS
{
	public partial class BonjourBrowser : NSNetServiceBrowserDelegate
	{
		public Action successCallback { get; set; }

		public override void FoundService (NSNetServiceBrowser sender, NSNetService service, bool moreComing)
		{
			Debug.WriteLine (String.Format("Found {0}", service.Name));

			if (service.Name.Equals ("Calabash Server"))
			if (successCallback != null)
				successCallback ();
		}
	}

	public partial class MainViewController : UIViewController
	{
		BonjourBrowser _browserDelegate;
		NSNetServiceBrowser _browser;

		public MainViewController () : base ("MainViewController", null) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			_browserDelegate = new BonjourBrowser ();
			_browserDelegate.successCallback += ()=> {
				statusLabel.Text = "Calabash is running!";
				_browser.Stop();
			};

			_browser = new NSNetServiceBrowser ();
			_browser.Delegate = _browserDelegate;
			_browser.SearchForServices ("_http._tcp.", "local.");
		}
	}
}

