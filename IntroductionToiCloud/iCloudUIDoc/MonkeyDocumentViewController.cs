using System;

using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Cloud {
	public class MonkeyDocumentViewController : UIViewController {

		MonkeyDocument doc;
		UIButton saveButton;
		UITextView docText, alertText;

		public override void ViewDidLoad ()
		{	
			base.ViewDidLoad ();

			#region UI controls, you could do this in a XIB if you wanted
			saveButton = UIButton.FromType (UIButtonType.RoundedRect);
			saveButton.Frame = new CGRect (10.0, 10.0, 160.0, 50.0);
			saveButton.SetTitle ("UpdateChangeCount", UIControlState.Normal);
			saveButton.Enabled = false;
			
			alertText = new UITextView (new CGRect (175.0, 10.0, 140.0, 50.0)) {
				TextColor = UIColor.Red,
				Editable = false
			};

			docText = new UITextView (new CGRect (10.0, 70.0, 300.0, 150.0)) {
				Editable = true,
				ScrollEnabled = true,
				BackgroundColor = UIColor.FromRGB (224, 255, 255)
			};
			
			// Add the controls to the view
			Add (saveButton);
			Add (docText);
			Add (alertText);
			#endregion

			saveButton.TouchUpInside += (sender, e) => {
				// we're not checking for conflicts or anything, just saving the edited version over the top
				Console.WriteLine ("UpdateChangeCount -> hint for iCloud to save");
				doc.DocumentString = docText.Text;
				alertText.Text = string.Empty;
				doc.UpdateChangeCount (UIDocumentChangeKind.Done);
				docText.ResignFirstResponder ();
			};
			
			// listen for notifications that the document was modified via the server
			NSNotificationCenter.DefaultCenter.AddObserver (this,
				new Selector ("dataReloaded:"),
				new NSString ("monkeyDocumentModified"),
				null
			);
		}

		[Export ("dataReloaded:")]
		void DataReloaded (NSNotification notification)
		{
			doc = (MonkeyDocument)notification.Object;
			alertText.Text = string.Format ("{0} dataReloaded: notification", DateTime.Now.ToString ("H:mm:ss"));
			// we just overwrite whatever was being typed, no conflict resolution for now
			docText.Text = doc.DocumentString;
		}

		public void DisplayDocument (MonkeyDocument monkeyDoc)
		{
			alertText.Text = string.Empty;
			doc = monkeyDoc;
			docText.Text = doc.DocumentString;
			saveButton.Enabled = true;
		}
	}
}