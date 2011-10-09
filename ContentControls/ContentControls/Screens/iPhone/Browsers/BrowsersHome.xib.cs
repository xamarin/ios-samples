
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_ContentControls.Screens.iPhone.Browsers
{
	public partial class BrowsersHome : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public BrowsersHome (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public BrowsersHome (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public BrowsersHome () : base("BrowsersHome", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			this.NavigationItem.Title = "UIWebView Examples";
			
			// on web browser button click, load the web browser page
			this.btnWebBrowser.TouchUpInside += (s, e) => {
				this.NavigationController.PushViewController(new WebBrowser(), true);
			};

			// on local browser button click, load the local browser page
			this.btnLocalBrowser.TouchUpInside += (s, e) => {
				this.NavigationController.PushViewController(new LocalBrowser(), true);
			};

			// interacting with the web view
			this.btnInteractivity.TouchUpInside += (s, e) => {
				this.NavigationController.PushViewController(new InteractiveBrowser(), true);
			};
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

	}
}

