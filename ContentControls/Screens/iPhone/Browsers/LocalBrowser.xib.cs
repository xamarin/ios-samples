
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_ContentControls.Screens.iPhone.Browsers
{
	public partial class LocalBrowser : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public LocalBrowser (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public LocalBrowser (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public LocalBrowser () : base("LocalBrowser", null)
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
		
			this.Title = "My Content";

			// can load the request directly
			string homePageUrl = NSBundle.MainBundle.BundlePath + "/Content/Home.html";
			this.webMain.LoadRequest (new NSUrlRequest (new NSUrl (homePageUrl, false)));
			
			// can also manually create html
			string contentDirectoryPath = NSBundle.MainBundle.BundlePath + "/Content/";
			this.webMain.LoadHtmlString ("<html><a href=\"Home.html\">Click Me</a>", new NSUrl (contentDirectoryPath, true));
		
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}


	}
}

