using System;
using System.Linq;

using Foundation;
using UIKit;

namespace UICatalog {
	public partial class AlertFormViewController : UIViewController {

		const string title = "A Short Title is Best";
		const string message = "A message should be a short, complete sentence.";
		const string acceptButtonTitle = "OK";
		const string cancelButtonTitle = "Cancel";

		UIAlertAction secureTextAlertAction;

		[Export ("initWithCoder:")]
		public AlertFormViewController (NSCoder coder): base (coder)
		{
		}

		partial void ShowSecureEntry (NSObject sender)
		{
			var alertController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);
			alertController.AddTextField (textField => {
				textField.Placeholder = "Password";
				textField.SecureTextEntry = true;
				textField.InputAccessoryView = new CustomInputAccessoryView ("Enter at least 5 characters");
				textField.EditingChanged += HandleTextFieldTextDidChangeNotification;
			});

			var acceptAction = UIAlertAction.Create (acceptButtonTitle, UIAlertActionStyle.Default, _ =>
				Console.WriteLine ("The \"Secure Text Entry\" alert's other action occured.")
			);

			var cancelAction = UIAlertAction.Create (cancelButtonTitle, UIAlertActionStyle.Cancel, _ =>
				Console.WriteLine ("The \"Text Entry\" alert's cancel action occured.")
			);

			acceptAction.Enabled = false;
			secureTextAlertAction = acceptAction;

			// Add the actions.
			alertController.AddAction (acceptAction);
			alertController.AddAction (cancelAction);

			PresentViewController (alertController, true, null);
		}

		partial void ShowSimpleForm (NSObject sender)
		{
			var alertController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);

			alertController.AddTextField (textField => {
				textField.Placeholder = "Name";
				textField.InputAccessoryView = new CustomInputAccessoryView ("Enter your name");
			});
				
			alertController.AddTextField (textField => {
				textField.KeyboardType = UIKeyboardType.EmailAddress;
				textField.Placeholder = "example@example.com";
				textField.InputAccessoryView = new CustomInputAccessoryView ("Enter your email address");
			});

			var acceptAction = UIAlertAction.Create (acceptButtonTitle, UIAlertActionStyle.Default, _ => {
				Console.WriteLine ("The \"Text Entry\" alert's other action occured.");
				string enteredText = alertController.TextFields?.First ()?.Text;

				if (string.IsNullOrEmpty (enteredText))
					Console.WriteLine ("The text entered into the \"Text Entry\" alert's text field was \"{0}\"", enteredText);
			});

			var cancelAction = UIAlertAction.Create (cancelButtonTitle, UIAlertActionStyle.Cancel, _ =>
				Console.WriteLine ("The \"Text Entry\" alert's cancel action occured.")
			);

			// Add the actions.
			alertController.AddAction (acceptAction);
			alertController.AddAction (cancelAction);

			PresentViewController (alertController, true, null);
		}

		void HandleTextFieldTextDidChangeNotification (object sender, EventArgs e)
		{
			if (secureTextAlertAction == null)
				throw new Exception ("secureTextAlertAction has not been set");
			var textField = (UITextField)sender;

			var text = textField.Text ?? string.Empty;
			secureTextAlertAction.Enabled = text.Length >= 5;
		}
	}
}
