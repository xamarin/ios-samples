using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace UIKitEnhancements
{
	partial class AlertViewController : UIViewController
	{
		public AlertViewController (IntPtr handle) : base (handle)
		{
		}

		partial void OKAlert_TouchUpInside (UIButton sender)
		{
			// Present alert
			var okAlertController = UIAlertController.Create("OK Alert","This is a sample alert with an OK button.", UIAlertControllerStyle.Alert);

			okAlertController.AddAction(UIAlertAction.Create ("Ok", UIAlertActionStyle.Default, null));

			PresentViewController(okAlertController, true, null);
		}

		partial void OKCancelAlert_TouchUpInside (UIButton sender)
		{
			var okCancelAlertController = UIAlertController.Create("OK / Cancel Alert", "This is a sample alert with an OK / Cancel Button", UIAlertControllerStyle.Alert);

			okCancelAlertController.AddAction(UIAlertAction.Create("Okay", UIAlertActionStyle.Default, alert => Console.WriteLine ("Okay was clicked")));

			okCancelAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alert => Console.WriteLine ("Cancel was clicked")));

			PresentViewController(okCancelAlertController, true, null);
		}

		partial void TextInputAlert_TouchUpInside (UIButton sender)
		{

			var textInputAlertController = UIAlertController.Create("Text Input Alert", "Hey, input some text", UIAlertControllerStyle.Alert);

			textInputAlertController.AddTextField(textField => {
			});
				

			var cancelAction = UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, alertAction => Console.WriteLine ("Cancel was Pressed"));

			var okayAction = UIAlertAction.Create ("Okay", UIAlertActionStyle.Default, alertAction => Console.WriteLine ("The user entered '{0}'", textInputAlertController.TextFields[0].Text));

			textInputAlertController.AddAction(cancelAction);
			textInputAlertController.AddAction(okayAction);

			PresentViewController(textInputAlertController, true, null);
				
		}

		partial void ActionSheet_TouchUpInside (UIButton sender)
		{
			// Create a new Alert Controller
			UIAlertController alert = UIAlertController.Create("Action Sheet", "Select an item from below", UIAlertControllerStyle.ActionSheet);

			// Add items
			alert.AddAction(UIAlertAction.Create("Item One",UIAlertActionStyle.Default,(action) => {
				Console.WriteLine("Item One pressed.");
			}));

			alert.AddAction(UIAlertAction.Create("Item Two",UIAlertActionStyle.Default,(action) => {
				Console.WriteLine("Item Two pressed.");
			}));

			alert.AddAction(UIAlertAction.Create("Item Three",UIAlertActionStyle.Default,(action) => {
				Console.WriteLine("Item Three pressed.");
			}));

			alert.AddAction(UIAlertAction.Create("Cancel",UIAlertActionStyle.Cancel,(action) => {
				Console.WriteLine("Cancel button pressed.");
			}));

			// Required for iPad - You must specify a source for the Action Sheet since it is
			// displayed as a popover
			UIPopoverPresentationController presentationPopover = alert.PopoverPresentationController;
			if (presentationPopover!=null) {
				presentationPopover.SourceView = this.View;
				presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Up;
				presentationPopover.SourceRect = ActionSheet.Frame;
			}

			// Display the alert
			this.PresentViewController(alert,true,null);
		}
	}
}
