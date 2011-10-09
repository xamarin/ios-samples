
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_ContentControls.Screens.iPhone.Browsers
{
	public partial class InteractiveBrowser : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public InteractiveBrowser (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public InteractiveBrowser (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public InteractiveBrowser () : base("InteractiveBrowser", null)
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

			this.btnRunScript.TouchUpInside += (s, e) => {
				this.webMain.EvaluateJavascript ("RunAction();");	
			};
				
			this.Title = "Interactivity";
			
			// can load the request directly
			string homePageUrl = NSBundle.MainBundle.BundlePath + "/Content/InteractivePages/Home.html";
			this.webMain.LoadRequest (new NSUrlRequest (new NSUrl (homePageUrl, false)));

			this.webMain.ShouldStartLoad += this.HandleStartLoad;			
		}
		
		/// <summary>
		/// In order to listen to events, we need to handle the ShouldStartLoad event. If 
		/// it's an event that we want to handle ourselves, rather than having the web view 
		/// do it, we need to return false, so that the navigation doesn't happen. in this
		/// particular case we are checking for links that have //LOCAL/Action='whateverAction'
		/// </summary>
		public bool HandleStartLoad (UIWebView webView, NSUrlRequest request
			, UIWebViewNavigationType navigationType)
		{
			Console.WriteLine (navigationType.ToString ());
			
			// first, we check to see if it's a link
			if (navigationType == UIWebViewNavigationType.LinkClicked) {
				
				// next, we check to see if it's a link with //LOCAL in it.
				if(request.Url.RelativeString.StartsWith("file://LOCAL")) {
					
					new UIAlertView ("Action!", "You clicked an action.", null, "OK", null).Show();
					// return false so that the browser doesn't try to navigate
					return false;
				}
			}
			// if we got here, it's not a link we want to handle
			return true;			
		}
	}
}

