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
			this.Title = "Browser";
			
			// wire up event handlers
			this.btnBack.TouchUpInside += (s, e) => { if (this.webMain.CanGoBack) { this.webMain.GoBack (); } };
			this.btnForward.TouchUpInside += (s, e) => { if (this.webMain.CanGoForward) { this.webMain.GoForward (); } };
			this.btnStop.TouchUpInside += (s, e) => { this.webMain.StopLoading (); };
			this.btnGo.TouchUpInside += (s, e) => { NavigateToUrl (); };
			this.txtAddress.ShouldReturn += HandleEditingDone;
			this.webMain.LoadStarted += LoadStarted;
			this.webMain.LoadFinished += LoadingFinished;
			this.webMain.LoadError += LoadError;
			
			// disable our buttons to start
			this.btnBack.Enabled = false;
			this.btnForward.Enabled = false;
			this.btnStop.Enabled = false;
			
			// navigate to google
			this.txtAddress.Text = "google.com";
			this.NavigateToUrl ();

		}

		protected void NavigateToUrl ()
		{
			string url = this.txtAddress.Text;
			
			// make sure it's prefixed with either https:// or http://
			if (!(url.StartsWith ("http://") || url.StartsWith ("https://")))
				url = "http://" + url;
			
			this.webMain.LoadRequest (new NSUrlRequest (new NSUrl (url)));
		}

		protected void SetBackAndForwardEnable ()
		{
			this.btnBack.Enabled = this.webMain.CanGoBack;
			this.btnForward.Enabled = this.webMain.CanGoForward;
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
			this.btnStop.Enabled = true;
			this.SetBackAndForwardEnable ();
			this.imgBusy.StartAnimating ();
		}
			
		public void LoadingFinished (object source, EventArgs e)
		{
			this.SetBackAndForwardEnable ();
			this.btnStop.Enabled = false;
			this.imgBusy.StopAnimating ();
		}
		
		public void LoadError (object sender, UIWebErrorArgs e)
		{
			this.imgBusy.StopAnimating ();
			this.btnStop.Enabled = false;
			this.SetBackAndForwardEnable ();
			// show the error
			UIAlertView alert = new UIAlertView ("Browse Error"
				, "Web page failed to load: " + e.Error.ToString ()
				, null, "OK", null);
			alert.Show ();
		}
	
		#endregion

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	}
}

