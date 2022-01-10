using Foundation;
using System;
using UIKit;

namespace UICatalog
{
    public partial class AlertControllerViewController : UITableViewController
    {
        // Alert style alerts.
        private readonly Action[] alertActions;

        // Action sheet style alerts.
        private readonly Action[] sheetActions;

        public AlertControllerViewController(IntPtr handle) : base(handle)
        {
            alertActions = new Action[]
            {
                ShowSimpleAlert,
                ShowOkayCancelAlert,
                ShowOtherAlert,
                ShowTextEntryAlert,
                ShowSecureTextEntryAlert
            };

            sheetActions = new Action[]
            {
                ShowOkayCancelActionSheet,
                ShowOtherActionSheet
            };
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
            {
                alertActions[indexPath.Row].Invoke();
            }
            else
            {
                sheetActions[indexPath.Row].Invoke();
            }

            tableView.DeselectRow(indexPath, true);
        }

        #region actions

        /// <summary>
        /// Show an alert with an "Okay" button.
        /// </summary>
        private void ShowSimpleAlert()
        {
            var title = "A Short Title is Best";
            var message = "A message should be a short, complete sentence.";
            var cancelButtonTitle = "OK";

            const string msg = "The simple alert's cancel action occured.";
            var alertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            var cancelAction = UIAlertAction.Create(cancelButtonTitle, UIAlertActionStyle.Cancel, alertAction => Console.WriteLine(msg));

            // Add the action.
            alertController.AddAction(cancelAction);
            PresentViewController(alertController, true, null);
        }

        /// <summary>
        /// Show an alert with an "Okay" and "Cancel" button.
        /// </summary>
        private void ShowOkayCancelAlert()
        {
            const string title = "A Short Title is Best";
            const string message = "A message should be a short, complete sentence.";

            var alertCotroller = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            // Create the actions.
            var cancelAction = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alertAction => Console.WriteLine("The 'Cancel' alert's cancel action occured."));

            var otherAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, alertAction => Console.WriteLine("The 'OK' alert's other action occured."));

            // Add the actions.
            alertCotroller.AddAction(cancelAction);
            alertCotroller.AddAction(otherAction);

            PresentViewController(alertCotroller, true, null);
        }

        /// <summary>
        /// Show an alert with two custom buttons.
        /// </summary>
        private void ShowOtherAlert()
        {
            const string title = "A Short Title is Best";
            const string message = "A message should be a short, complete sentence.";
            var alertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

            // Create the actions.
            var cancelAction = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alertAction => Console.WriteLine("The 'Other' alert's cancel action occured."));
            var otherButtonOneAction = UIAlertAction.Create("Choice One", UIAlertActionStyle.Default, alertAction => Console.WriteLine("The 'Other' alert's other button one action occured."));
            var otherButtonTwoAction = UIAlertAction.Create("Choice Two", UIAlertActionStyle.Default, alertAction => Console.WriteLine("The 'Other' alert's other button two action occured."));

            // Add the actions.
            alertController.AddAction(cancelAction);
            alertController.AddAction(otherButtonOneAction);
            alertController.AddAction(otherButtonTwoAction);

            PresentViewController(alertController, true, null);
        }

        /// <summary>
        /// Show a text entry alert with two custom buttons.
        /// </summary>
        private void ShowTextEntryAlert()
        {
            const string title = "A Short Title is Best";
            const string message = "A message should be a short, complete sentence.";

            var alertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

            // Add the text field for text entry.
            alertController.AddTextField(textField => { /* If you need to customize the text field, you can do so here. */ });

            // Create the actions.
            var cancelAction = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alertAction => Console.WriteLine("The 'Text Entry' alert's cancel action occured."));
            var otherAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, alertAction => Console.WriteLine("The 'Text Entry' alert's other action occured."));

            // Add the actions.
            alertController.AddAction(cancelAction);
            alertController.AddAction(otherAction);

            PresentViewController(alertController, true, null);
        }

        /// <summary>
        /// Show a secure text entry alert with two custom buttons.
        /// </summary>
        private void ShowSecureTextEntryAlert()
        {
            NSObject observer = null;
            const string title = "A Short Title is Best";
            const string message = "A message should be a short, complete sentence.";

            var alertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

            // Create the actions.
            var cancelAction = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alertAction =>
            {
                Console.WriteLine("The 'Secure Text Entry' alert's cancel action occured.");
                RemoveObserver();
            });

            var otherAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, alertAction =>
            {
                Console.WriteLine("The 'Secure Text Entry' alert's other action occured.");
                RemoveObserver();
            });

            // The text field initially has no text in the text field, so we'll disable it.
            otherAction.Enabled = false;

            // Add the text field for the secure text entry.
            alertController.AddTextField(textField =>
            {
                // Listen for changes to the text field's text so that we can toggle the current
                // action's enabled property based on whether the user has entered a sufficiently
                // secure entry.
                observer = UITextField.Notifications.ObserveTextFieldTextDidChange(textField, (sender, e) =>
                {
                    otherAction.Enabled = textField.Text.Length >= 5;
                });
                textField.SecureTextEntry = true;
            });

            // Add the actions.
            alertController.AddAction(cancelAction);
            alertController.AddAction(otherAction);

            PresentViewController(alertController, true, null);

            void RemoveObserver()
            {
                // Stop listening for text change notifications on the text field. 
                NSNotificationCenter.DefaultCenter.RemoveObserver(observer);
                observer.Dispose();
                observer = null;
            }
        }

        #endregion

        #region sheet actions

        /// <summary>
        /// Show a dialog with an "Okay" and "Cancel" button.
        /// </summary>
        private void ShowOkayCancelActionSheet()
        {
            var alertController = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);

            // Create the actions.
            var cancelAction = UIAlertAction.Create("Cancel", UIAlertActionStyle.Default, _ => Console.WriteLine("The 'Okay-Cancel' alert action sheet's cancel action occured."));
            var destructiveAction = UIAlertAction.Create("OK", UIAlertActionStyle.Destructive, _ => Console.WriteLine("The 'Okay-Cancel' alert action sheet's destructive action occured."));

            // Add the actions.
            alertController.AddAction(cancelAction);
            alertController.AddAction(destructiveAction);

            SetupPopover(alertController, okayActionSheetCell);
            PresentViewController(alertController, true, null);
        }

        /// <summary>
        /// Show a dialog with two custom buttons.
        /// </summary>
        private void ShowOtherActionSheet()
        {
            var alertController = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);

            // Create the actions.
            var destructiveAction = UIAlertAction.Create("Destructive Choice", UIAlertActionStyle.Destructive, _ => Console.WriteLine("The 'Other' alert action sheet's destructive action occured."));
            var otherAction = UIAlertAction.Create("Safe Choice", UIAlertActionStyle.Default, _ => Console.WriteLine("The 'Other' alert action sheet's other action occured."));

            // Add the actions.
            alertController.AddAction(destructiveAction);
            alertController.AddAction(otherAction);

            SetupPopover(alertController, otherActionSheetCell);
            PresentViewController(alertController, true, null);
        }

        private static void SetupPopover(UIAlertController alertController, UIView sourceView)
        {
            var popover = alertController.PopoverPresentationController;
            if (popover != null)
            {
                popover.SourceView = sourceView;
                popover.SourceRect = sourceView.Bounds;
            }
        }

        #endregion
    }
}