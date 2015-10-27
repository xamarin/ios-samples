using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace UIKit
{
	/// <summary>
	/// Alert view controller is a reusable helper class that makes working with <c>UIAlertViewController</c> alerts
	/// easier in a tvOS app.
	/// </summary>
	public class AlertViewController
	{
		#region Static Methods
		public static UIAlertController PresentOKAlert(string title, string description, UIViewController controller) {
			// No, inform the user that they must create a home first
			UIAlertController alert = UIAlertController.Create(title, description, UIAlertControllerStyle.Alert);

			// Configure the alert
			alert.AddAction(UIAlertAction.Create("OK",UIAlertActionStyle.Default,(action) => {}));

			// Display the alert
			controller.PresentViewController(alert,true,null);

			// Return created controller
			return alert;
		}

		public static UIAlertController PresentOKCancelAlert(string title, string description, UIViewController controller, AlertOKCancelDelegate action) {
			// No, inform the user that they must create a home first
			UIAlertController alert = UIAlertController.Create(title, description, UIAlertControllerStyle.Alert);

			// Add cancel button
			alert.AddAction(UIAlertAction.Create("Cancel",UIAlertActionStyle.Cancel,(actionCancel) => {
				// Any action?
				if (action!=null) {
					action(false);
				}
			}));

			// Add ok button
			alert.AddAction(UIAlertAction.Create("OK",UIAlertActionStyle.Default,(actionOK) => {
				// Any action?
				if (action!=null) {
					action(true);
				}
			}));

			// Display the alert
			controller.PresentViewController(alert,true,null);

			// Return created controller
			return alert;
		}

		public static UIAlertController PresentDestructiveAlert(string title, string description, string destructiveAction, UIViewController controller, AlertOKCancelDelegate action) {
			// No, inform the user that they must create a home first
			UIAlertController alert = UIAlertController.Create(title, description, UIAlertControllerStyle.Alert);

			// Add cancel button
			alert.AddAction(UIAlertAction.Create("Cancel",UIAlertActionStyle.Cancel,(actionCancel) => {
				// Any action?
				if (action!=null) {
					action(false);
				}
			}));

			// Add ok button
			alert.AddAction(UIAlertAction.Create(destructiveAction,UIAlertActionStyle.Destructive,(actionOK) => {
				// Any action?
				if (action!=null) {
					action(true);
				}
			}));

			// Display the alert
			controller.PresentViewController(alert,true,null);

			// Return created controller
			return alert;
		}

		public static UIAlertController PresentTextInputAlert(string title, string description, string placeholder, string text, UIViewController controller, AlertTextInputDelegate action) {
			// No, inform the user that they must create a home first
			UIAlertController alert = UIAlertController.Create(title, description, UIAlertControllerStyle.Alert);
			UITextField field = null;

			// Add and configure text field
			alert.AddTextField ((textField) => {
				// Save the field
				field = textField;

				// Initialize field
				field.Placeholder = placeholder;
				field.Text = text;
				field.AutocorrectionType = UITextAutocorrectionType.No;
				field.KeyboardType = UIKeyboardType.Default;
				field.ReturnKeyType = UIReturnKeyType.Done;
				field.ClearButtonMode = UITextFieldViewMode.WhileEditing;

			});

			// Add cancel button
			alert.AddAction(UIAlertAction.Create("Cancel",UIAlertActionStyle.Cancel,(actionCancel) => {
				// Any action?
				if (action!=null) {
					action(false,"");
				}
			}));

			// Add ok button
			alert.AddAction(UIAlertAction.Create("OK",UIAlertActionStyle.Default,(actionOK) => {
				// Any action?
				if (action!=null && field !=null) {
					action(true, field.Text);
				}
			}));

			// Display the alert
			controller.PresentViewController(alert,true,null);

			// Return created controller
			return alert;
		}
		#endregion

		#region Delegates
		public delegate void AlertOKCancelDelegate(bool OK);
		public delegate void AlertTextInputDelegate(bool OK, string text);
		#endregion
	}
}

