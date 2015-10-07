using System;

using UIKit;

namespace HomeKitCatalog
{
	public static class Alert
	{
		/// <summary>
		/// A simple factory which creates `UIAlertController` that prompts for a name, then runs a completion block passing in the name.
		/// </summary>
		/// <param name="attributeType">The type of object that will be named</param>
		/// <param name="completionHandler">A block to call, passing in the provided text</param>
		public static UIAlertController Create (string attributeType, Action<string> completionHandler, string placeholder, string shortType)
		{
			if (string.IsNullOrWhiteSpace(placeholder))
				placeholder = attributeType;
			if (string.IsNullOrWhiteSpace (shortType))
				shortType = attributeType;

			string title = string.Format ("New {0}", attributeType);
			const string message = "Enter a name.";
			var alert = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);
			alert.AddTextField (textField => {
				textField.Placeholder = placeholder;
				textField.AutocapitalizationType = UITextAutocapitalizationType.Words;
			});

			var cancelAction = UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, action => alert.DismissViewController (true, null));

			string addString = string.Format ("Add {0}", shortType);
			var addNewObject = UIAlertAction.Create(addString, UIAlertActionStyle.Default, action => {
				var textFields = alert.TextFields;
				if(textFields != null && textFields.Length > 0) {
					var textField = textFields[0];
					if(textField != null) {
						var trimmedName = textField.Text.Trim();
						completionHandler(trimmedName);
					}
				}
				alert.DismissViewController(true, null);
			});

			alert.AddAction (cancelAction);
			alert.AddAction (addNewObject);

			return alert;
		}

		public static UIAlertController Create(string title, string body)
		{
			var alert = UIAlertController.Create (title, body, UIAlertControllerStyle.Alert);
			var okayAction = UIAlertAction.Create ("Okay", UIAlertActionStyle.Default, action => alert.DismissViewController (true, null));
			alert.AddAction (okayAction);

			return alert;
		}
	}
}