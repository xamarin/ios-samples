using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;
using UIKit;
using Foundation;
using ObjCRuntime;

namespace Cloud
{
	public class MonkeyDocumentViewController : UIViewController
	{
		MonkeyDocument doc; // the UIDocument

		UIButton saveButton;
		UITextView docText, alertText;

		public override void ViewDidLoad ()
		{	
			base.ViewDidLoad ();

			#region UI controls, you could do this in a XIB if you wanted
			saveButton = UIButton.FromType(UIButtonType.RoundedRect);
			saveButton.Frame = new CGRect(10,10,160,50);
			saveButton.SetTitle("UpdateChangeCount", UIControlState.Normal);
			saveButton.Enabled = false;
			
			alertText = new UITextView(new CGRect(175, 10, 140, 50));
			alertText.TextColor = UIColor.Red;
			alertText.Editable = false;

			docText = new UITextView(new CGRect(10, 70, 300, 150));
			docText.Editable = true;
			docText.ScrollEnabled = true;
			docText.BackgroundColor = UIColor.FromRGB(224,255,255);
			
			// Add the controls to the view
			this.Add(saveButton);
			this.Add(docText);
			this.Add (alertText);
			#endregion

			saveButton.TouchUpInside += (sender, e) => {
				// we're not checking for conflicts or anything, just saving the edited version over the top
				Console.WriteLine ("UpdateChangeCount -> hint for iCloud to save");
				doc.DocumentString = docText.Text;
				alertText.Text = "";
				doc.UpdateChangeCount (UIDocumentChangeKind.Done);
				docText.ResignFirstResponder();
			};
			
			// listen for notifications that the document was modified via the server
			NSNotificationCenter.DefaultCenter.AddObserver (this
				, new Selector("dataReloaded:")
				, new NSString("monkeyDocumentModified"),
				null);
		}
		
		[Export("dataReloaded:")]
		void DataReloaded (NSNotification notification) {
			doc = (MonkeyDocument)notification.Object;
			alertText.Text = DateTime.Now.ToString ("H:mm:ss") + " dataReloaded: notification";
			// we just overwrite whatever was being typed, no conflict resolution for now
			docText.Text = doc.DocumentString;
		}

		public void DisplayDocument (MonkeyDocument monkeyDoc) {
			alertText.Text = "";
			doc = monkeyDoc;
			docText.Text = doc.DocumentString;
			saveButton.Enabled = true;
		}
	}
}