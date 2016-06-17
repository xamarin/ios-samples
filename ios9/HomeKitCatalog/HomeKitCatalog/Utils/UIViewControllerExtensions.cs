using System;
using System.Collections.Generic;

using CoreFoundation;
using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	public static class UIViewControllerExtensions
	{
		public static void PresentAddAlertWithAttributeType (this UIViewController vc, string type, string placeholder, string shortType, Action<string> completion)
		{
			var alertController = Alert.Create (type, completion, placeholder, shortType);
			vc.PresentViewController (alertController, true, null);
		}

		// Displays a collection of errors, separated by newlines.
		public static void DisplayErrors (this UIViewController self, IEnumerable<NSError> errors)
		{
			var messages = new List<string> ();
			foreach (var error in errors) {
				var errorCode = (HMError)(int)error.Code;
				if (self.PresentedViewController != null || errorCode == HMError.OperationCancelled || errorCode == HMError.UserDeclinedAddingUser)
					Console.WriteLine (error.LocalizedDescription);
				else
					messages.Add (error.LocalizedDescription);
			}

			if (messages.Count > 0) {
				// There were errors in the list, reduce the messages into a single one.
				string collectedMessage = string.Join ("\n", messages);
				self.DisplayErrorMessage (collectedMessage);
			}
		}

		static void DisplayErrorMessage (this UIViewController self, string message)
		{
			self.DisplayMessage ("Error", message);
		}

		// Displays a `UIAlertController` with the passed-in text and an 'Okay' button.
		static void DisplayMessage (this UIViewController self, string title, string message)
		{
			DispatchQueue.MainQueue.DispatchAsync (() => {
				var alert = Alert.Create (title, message);
				self.PresentViewController (alert, true, null);
			});
		}
	}
}