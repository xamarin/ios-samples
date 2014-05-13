//
// Web sample in C#v
//

using System;
using UIKit;
using Foundation;
using CoreGraphics;

namespace MonoCatalog {
	
	public partial class WebViewController : UIViewController {
		UIWebView web;
		
		// Load our definition from the NIB file
		public WebViewController () : base ("WebViewController", null)
		{
		}
	
		public override void ViewWillDisappear (bool animated)
		{
			web.StopLoading ();
			web.Delegate = null;
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
		
		public override void ViewDidLoad ()
		{
			Title = "Web";
			NavigationController.NavigationBar.Translucent = false;
			var webFrame = UIScreen.MainScreen.ApplicationFrame;
			webFrame.Y += 25f;
			webFrame.Height -= 40f;
	
			web = new UIWebView (webFrame) {
				BackgroundColor = UIColor.White,
				ScalesPageToFit = true,
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight
			};
			web.LoadStarted += delegate {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			};
			web.LoadFinished += delegate {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			};
			web.LoadError += (webview, args) => {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
				web.LoadHtmlString (String.Format ("<html><center><font size=+5 color='red'>An error occurred:<br>{0}</font></center></html>", args.Error.LocalizedDescription), null);
			};
			View.AddSubview (web);
	
			// Delegate = new 
			var urlField = new UITextField (new CGRect (20f, 10f, View.Bounds.Width - (20f * 2f), 30f)){
				BorderStyle = UITextBorderStyle.Bezel,
				TextColor = UIColor.Black,
				Placeholder = "<enter a URL>",
				Text = "http://ios.xamarin.com/",
				BackgroundColor = UIColor.White,
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
				ReturnKeyType = UIReturnKeyType.Go,
				KeyboardType = UIKeyboardType.Url,
				AutocapitalizationType = UITextAutocapitalizationType.None,
				AutocorrectionType = UITextAutocorrectionType.No,
				ClearButtonMode = UITextFieldViewMode.Always
			};
	
			urlField.ShouldReturn = delegate (UITextField field){
				field.ResignFirstResponder ();
				web.LoadRequest (NSUrlRequest.FromUrl (new NSUrl (field.Text)));
	
				return true;
			};
	
			View.AddSubview (urlField);
			
			web.LoadRequest (NSUrlRequest.FromUrl (new NSUrl ("http://ios.xamarin.com/")));
		}
	}
}
