using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace UIKit {
	/// <summary>
	/// Alert view controller is a reusable helper class that makes working with <c>UIAlertViewController</c> alerts
	/// easier in a tvOS app.
	/// </summary>
	public class AlertViewController {
		#region Static Methods
		/// <summary>
		/// Presents an alert containing only the OK button.
		/// </summary>
		/// <returns>The <c>UIAlertController</c> for the alert.</returns>
		/// <param name="title">The alert's title.</param>
		/// <param name="description">The alert's description.</param>
		/// <param name="controller">The View Controller that will present the alert.</param>
		public static UIAlertController PresentOKAlert (string title, string description, UIViewController controller)
		{
			// No, inform the user that they must create a home first
			UIAlertController alert = UIAlertController.Create (title, description, UIAlertControllerStyle.Alert);

			// Configure the alert
			alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, (action) => { }));

			// Display the alert
			controller.PresentViewController (alert, true, null);

			// Return created controller
			return alert;
		}

		/// <summary>
		/// Presents an alert with an OK and a Cancel button.
		/// </summary>
		/// <returns>The <c>UIAlertController</c> for the alert.</returns>
		/// <param name="title">The alert's title.</param>
		/// <param name="description">The alert's Description.</param>
		/// <param name="controller">The Vinew Controller that will present the alert.</param>
		/// <param name="action">The <c>AlertOKCancelDelegate</c> use to respond to the user's action.</param>
		public static UIAlertController PresentOKCancelAlert (string title, string description, UIViewController controller, AlertOKCancelDelegate action)
		{
			// No, inform the user that they must create a home first
			UIAlertController alert = UIAlertController.Create (title, description, UIAlertControllerStyle.Alert);

			// Add cancel button
			alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, (actionCancel) => {
				// Any action?
				if (action != null) {
					action (false);
				}
			}));

			// Add ok button
			alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, (actionOK) => {
				// Any action?
				if (action != null) {
					action (true);
				}
			}));

			// Display the alert
			controller.PresentViewController (alert, true, null);

			// Return created controller
			return alert;
		}

		/// <summary>
		/// Presents a destructive alert (such as delete a file).
		/// </summary>
		/// <returns>The <c>UIAlertController</c> for the alert.</returns>
		/// <param name="title">The alert's title.</param>
		/// <param name="description">The alert's description.</param>
		/// <param name="destructiveAction">The title for the destructive action's button (such as delete).</param>
		/// <param name="controller">The View Controller that will present the alert.</param>
		/// <param name="action">The <c>AlertOKCancelDelegate</c> use to respond to the user's action.</param>
		public static UIAlertController PresentDestructiveAlert (string title, string description, string destructiveAction, UIViewController controller, AlertOKCancelDelegate action)
		{
			// No, inform the user that they must create a home first
			UIAlertController alert = UIAlertController.Create (title, description, UIAlertControllerStyle.Alert);

			// Add cancel button
			alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, (actionCancel) => {
				// Any action?
				if (action != null) {
					action (false);
				}
			}));

			// Add ok button
			alert.AddAction (UIAlertAction.Create (destructiveAction, UIAlertActionStyle.Destructive, (actionOK) => {
				// Any action?
				if (action != null) {
					action (true);
				}
			}));

			// Display the alert
			controller.PresentViewController (alert, true, null);

			// Return created controller
			return alert;
		}

		/// <summary>
		/// Presents an alert that allows the user to input a single line of text.
		/// </summary>
		/// <returns>The <c>UIAlertController</c> for the alert.</returns>
		/// <param name="title">The alert's title.</param>
		/// <param name="description">The alert's description.</param>
		/// <param name="placeholder">The placholder text that will be displayed when the text field is empty.</param>
		/// <param name="text">The initial value for the text field.</param>
		/// <param name="controller">The View Controller that will present the alert.</param>
		/// <param name="action">The <c>AlertTextInputDelegate</c> that will respond to the user's action.</param>
		public static UIAlertController PresentTextInputAlert (string title, string description, string placeholder, string text, UIViewController controller, AlertTextInputDelegate action)
		{
			// No, inform the user that they must create a home first
			UIAlertController alert = UIAlertController.Create (title, description, UIAlertControllerStyle.Alert);
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
			alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, (actionCancel) => {
				// Any action?
				if (action != null) {
					action (false, "");
				}
			}));

			// Add ok button
			alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, (actionOK) => {
				// Any action?
				if (action != null && field != null) {
					action (true, field.Text);
				}
			}));

			// Display the alert
			controller.PresentViewController (alert, true, null);

			// Return created controller
			return alert;
		}
		#endregion

		#region Delegates
		/// <summary>
		/// Delegate to handle the user's selection on an OK/Cancel alert.
		/// </summary>
		public delegate void AlertOKCancelDelegate (bool OK);

		/// <summary>
		/// Delegate to handle the user's selection on an single line text entry alert.
		/// </summary>
		public delegate void AlertTextInputDelegate (bool OK, string text);
		#endregion
	}
}
