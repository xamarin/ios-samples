using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace UICatalog
{
	public partial class AlertControllerViewController : UITableViewController
	{
		private UIAlertAction secureTextAlertAction;

		public AlertControllerViewController (IntPtr handle) : base (handle)
		{
		}

		// Show an alert with an "Okay" button.
		private void ShowSimpleAlert()
		{
			var title = "A Short Title is Best".Localize ();
			var message = "A message should be a short, complete sentence.".Localize ();
			var cancelButtonTitle = "OK".Localize ();

			var alertController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);

			// Create the action.
			var cancelAction = UIAlertAction.Create (cancelButtonTitle, UIAlertActionStyle.Cancel, alertAction => {
				Console.WriteLine ("The simple alert's cancel action occured.");
			});

			// Add the action.
			alertController.AddAction (cancelAction);
			PresentViewController (alertController, true, null);
		}

		// Show an alert with an "Okay" and "Cancel" button.
		private void ShowOkayCancelAlert()
		{
			var title = "A Short Title is Best".Localize ();
			var message = "A message should be a short, complete sentence.".Localize ();
			var cancelButtonTitle = "Cancel".Localize ();
			var otherButtonTitle = "OK".Localize ();

			var alertCotroller = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);

			// Create the actions.
			var cancelAction = UIAlertAction.Create (cancelButtonTitle, UIAlertActionStyle.Cancel, alertAction => {
				Console.WriteLine ("The 'Okay/Cancel' alert's cancel action occured.");
			});

			var otherAction = UIAlertAction.Create (otherButtonTitle, UIAlertActionStyle.Default, alertAction => {
				Console.WriteLine ("The 'Okay/Cancel' alert's other action occured.");
			});

			// Add the actions.
			alertCotroller.AddAction (cancelAction);
			alertCotroller.AddAction (otherAction);

			PresentViewController (alertCotroller, true, null);
		}

		// Show an alert with two custom buttons.
		private void ShowOtherAlert()
		{
			var title = "A Short Title is Best".Localize ();
			var message = "A message should be a short, complete sentence.".Localize ();
			var cancelButtonTitle = "Cancel".Localize ();
			var otherButtonTitleOne = "Coice One".Localize ();
			var otherButtonTitleTwo = "Choice Two".Localize ();

			var alertController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);

				// Create the actions.
			var cancelAction = UIAlertAction.Create (cancelButtonTitle, UIAlertActionStyle.Cancel, alertAction => {
				Console.WriteLine ("The \"Other\" alert's cancel action occured.");
			});
			var otherButtonOneAction = UIAlertAction.Create (otherButtonTitleOne, UIAlertActionStyle.Default, alertAction => {
				Console.WriteLine ("The \"Other\" alert's other button one action occured.");
			});

			var otherButtonTwoAction = UIAlertAction.Create (otherButtonTitleTwo, UIAlertActionStyle.Default, alertAction => {
				Console.WriteLine ("The \"Other\" alert's other button two action occured.");
			});

			// Add the actions.
			alertController.AddAction (cancelAction);
			alertController.AddAction (otherButtonOneAction);
			alertController.AddAction (otherButtonTwoAction);

			PresentViewController (alertController, true, null);
		}

		// Show a text entry alert with two custom buttons.
		private void ShowTextEntryAlert()
		{
			var title = "A Short Title is Best".Localize ();
			var message = "A message should be a short, complete sentence.".Localize ();
			var cancelButtonTitle = "Cancel".Localize ();
			var otherButtonTitle = "OK".Localize ();

			var alertController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);

			// Add the text field for text entry.
			alertController.AddTextField (textField => {
				// If you need to customize the text field, you can do so here.
			});

			// Create the actions.
			var cancelAction = UIAlertAction.Create (cancelButtonTitle, UIAlertActionStyle.Cancel, alertAction => {
				Console.WriteLine ("The \"Text Entry\" alert's cancel action occured.");
			});

			var otherAction = UIAlertAction.Create (otherButtonTitle, UIAlertActionStyle.Default, alertAction => {
				Console.WriteLine ("The \"Text Entry\" alert's other action occured.");
			});

			// Add the actions.
			alertController.AddAction (cancelAction);
			alertController.AddAction (otherAction);

			PresentViewController (alertController, true, null);
		}

		// Show a secure text entry alert with two custom buttons.
		private void ShowSecureTextEntryAlert()
		{
			var title = "A Short Title is Best".Localize ();
			var message = "A message should be a short, complete sentence.".Localize ();
			var cancelButtonTitle = "Cancel".Localize ();
			var otherButtonTitle = "OK".Localize ();
			
			var alertController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);

			NSObject observer = null;
			// Add the text field for the secure text entry.
			alertController.AddTextField(textField => {
				// Listen for changes to the text field's text so that we can toggle the current
				// action's enabled property based on whether the user has entered a sufficiently
				// secure entry.
				observer = NSNotificationCenter.DefaultCenter.AddObserver(UITextField.TextFieldTextDidChangeNotification, HandleTextFieldTextDidChangeNotification);
				textField.SecureTextEntry = true;
			});

			// Stop listening for text change notifications on the text field. This func will be called in the two action handlers.
			Action removeTextFieldObserver = () => {
				NSNotificationCenter.DefaultCenter.RemoveObserver(observer);
			};

			// Create the actions.
			var cancelAction = UIAlertAction.Create (cancelButtonTitle, UIAlertActionStyle.Cancel, alertAction => {
				Console.WriteLine ("The \"Secure Text Entry\" alert's cancel action occured.");
				removeTextFieldObserver();
			});
				
			var otherAction = UIAlertAction.Create (otherButtonTitle, UIAlertActionStyle.Default, alertAction => {
				Console.WriteLine ("The \"Secure Text Entry\" alert's other action occured.");
				removeTextFieldObserver ();
			}); 

			// The text field initially has no text in the text field, so we'll disable it.
			otherAction.Enabled = false;

			// Hold onto the secure text alert action to toggle the enabled/disabled state when the text changed.
			secureTextAlertAction = otherAction;

				// Add the actions.
			alertController.AddAction (cancelAction);
			alertController.AddAction (otherAction);

			PresentViewController (alertController, true, null);
		}

		// Show a dialog with an "Okay" and "Cancel" button.
		private void ShowOkayCancelActionSheet()
		{
			var cancelButtonTitle = "Cancel".Localize ();
			var destructiveButtonTitle = "OK".Localize ();

			var alertController = UIAlertController.Create (null, null, UIAlertControllerStyle.ActionSheet);

			// Create the actions.
			var cancelAction = UIAlertAction.Create (cancelButtonTitle, UIAlertActionStyle.Cancel, _ => {
				Console.WriteLine ("The \"Okay/Cancel\" alert action sheet's cancel action occured.");
			});

			var destructiveAction = UIAlertAction.Create (destructiveButtonTitle, UIAlertActionStyle.Destructive, _ => {
				Console.WriteLine ("The \"Okay/Cancel\" alert action sheet's destructive action occured.");
			});

			// Add the actions.
			alertController.AddAction (cancelAction);
			alertController.AddAction (destructiveAction);

			PresentViewController (alertController, true, null);
		}

		// Show a dialog with two custom buttons.
		private void ShowOtherActionSheet()
		{
			var destructiveButtonTitle = "Destructive Choice".Localize ();
			var otherButtonTitle = "Safe Choice".Localize ();

			var alertController = UIAlertController.Create (null, null, UIAlertControllerStyle.ActionSheet);

			// Create the actions.
			var destructiveAction = UIAlertAction.Create (destructiveButtonTitle, UIAlertActionStyle.Destructive, _ => {
				Console.WriteLine ("The \"Other\" alert action sheet's destructive action occured.");
			});

			var otherAction = UIAlertAction.Create (otherButtonTitle, UIAlertActionStyle.Default, _ => {
				Console.WriteLine ("The \"Other\" alert action sheet's other action occured.");
			});

			// Add the actions.
			alertController.AddAction (destructiveAction);
			alertController.AddAction (otherAction);

			PresentViewController (alertController, true, null);
		}

		private void HandleTextFieldTextDidChangeNotification(NSNotification notification)
		{
			var textField = notification.Object as UITextField;

			// Enforce a minimum length of >= 5 for secure text alerts.
			if (secureTextAlertAction == null)
				throw new InvalidOperationException ();

			secureTextAlertAction.Enabled = textField.Text.Length >= 5;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			// A matrix of closures that should be invoked based on which table view cell is
			// tapped (index by section, row).
			Action[][] actionMap = new Action[][] {
				// Alert style alerts.
				new Action[] {
					ShowSimpleAlert,
					ShowOkayCancelAlert,
					ShowOtherAlert,
					ShowTextEntryAlert,
					ShowSecureTextEntryAlert
				},
				// Action sheet style alerts.
				new Action[] {
					ShowOkayCancelActionSheet,
					ShowOtherActionSheet
				}
			};

			var action = actionMap [indexPath.Section] [indexPath.Row];
			action ();
			tableView.DeselectRow (indexPath, animated: true);
		}
	}
}
