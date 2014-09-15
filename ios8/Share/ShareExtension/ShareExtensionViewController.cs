using System;
using System.Drawing;

using UIKit;
using Social;
using Foundation;
using CoreFoundation;

namespace ShareExtension
{
	public partial class ShareExtensionViewController : SLComposeServiceViewController
	{
		public ShareExtensionViewController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Do any additional setup after loading the view.
		}

		public override bool IsContentValid ()
		{
			// Do validation of contentText and/or NSExtensionContext attachments here
			return true;
		}

		public override void DidSelectPost ()
		{
			// This is called after the user selects Post. Do the upload of contentText and/or NSExtensionContext attachments.
			UIAlertController alert = UIAlertController.Create ("Share extension", "This is the step where you should post the ContentText value: \"" + ContentText + "\" to your targeted service.", UIAlertControllerStyle.Alert);
			PresentViewController (alert, true, () => {
				DispatchQueue.MainQueue.DispatchAfter (new DispatchTime (DispatchTime.Now, 5000000000), () => {
					ExtensionContext.CompleteRequest (null, null);
				});
			});
		}

		public override SLComposeSheetConfigurationItem[] GetConfigurationItems ()
		{
			// To add configuration options via table cells at the bottom of the sheet, return an array of SLComposeSheetConfigurationItem here.
			return new SLComposeSheetConfigurationItem[0];
		}
	}
}

