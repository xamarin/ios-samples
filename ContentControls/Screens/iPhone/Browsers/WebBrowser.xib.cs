using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_ContentControls.Screens.iPhone.Browsers
{
	public partial class WebBrowser : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public WebBrowser (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public WebBrowser (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public WebBrowser () : base("WebBrowser", null)
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
			
			// set the title
			Title = "Browser";
			
			// wire up event handlers
			btnBack.TouchUpInside += (s, e) => { if (webMain.CanGoBack) { webMain.GoBack (); } };
			btnForward.TouchUpInside += (s, e) => { if (webMain.CanGoForward) { webMain.GoForward (); } };
			btnStop.TouchUpInside += (s, e) => { webMain.StopLoading (); };
			btnGo.TouchUpInside += (s, e) => { NavigateToUrl (); };
			txtAddress.ShouldReturn += HandleEditingDone;
			webMain.LoadStarted += LoadStarted;
			webMain.LoadFinished += LoadingFinished;
			webMain.LoadError += LoadError;
			
			// disable our buttons to start
			btnBack.Enabled = false;
			btnForward.Enabled = false;
			btnStop.Enabled = false;
			
			// navigate to google
			txtAddress.Text = "google.com";
			NavigateToUrl ();

		}

		protected void NavigateToUrl ()
		{
			string url = txtAddress.Text;
			
			// make sure it's prefixed with either https:// or http://
			if (!(url.StartsWith ("http://") || url.StartsWith ("https://")))
				url = "http://" + url;
			
			webMain.LoadRequest (new NSUrlRequest (new NSUrl (url)));
		}

		protected void SetBackAndForwardEnable ()
		{
			btnBack.Enabled = webMain.CanGoBack;
			btnForward.Enabled = webMain.CanGoForward;
		}
		
		#region event handlers

		protected bool HandleEditingDone (UITextField textBox)
		{
			textBox.ResignFirstResponder ();
			NavigateToUrl ();
			return true;
		}
				
		public void LoadStarted (object source, EventArgs e)
		{
			btnStop.Enabled = true;
			SetBackAndForwardEnable ();
			imgBusy.StartAnimating ();
		}
			
		public void LoadingFinished (object source, EventArgs e)
		{
			SetBackAndForwardEnable ();
			btnStop.Enabled = false;
			imgBusy.StopAnimating ();
		}
		
		public void LoadError (object sender, UIWebErrorArgs e)
		{
			imgBusy.StopAnimating ();
			btnStop.Enabled = false;
			SetBackAndForwardEnable ();
			// show the error
			UIAlertView alert = new UIAlertView ("Browse Error",
							     "Web page failed to load: " + e.Error.ToString (),
							     null, "OK", null);
			alert.Show ();
		}
	
		#endregion

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	}
}

