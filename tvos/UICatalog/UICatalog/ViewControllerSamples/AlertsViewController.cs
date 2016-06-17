using System;

using Foundation;
using UIKit;

namespace UICatalog {
	public partial class AlertsViewController : UIViewController {

		const string title = "A Short Title is Best";
		const string message = "A message should be a short, complete sentence.";
		const string acceptButtonTitle = "OK";
		const string cancelButtonTitle = "Cancel";
		const string deleteButtonTitle = "Delete";

		[Export ("initWithCoder:")]
		public AlertsViewController (NSCoder coder): base (coder)
		{
		}

		partial void ShowSimpleAlert (NSObject sender)
		{
			var alertController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);

			// Create the action.
			var acceptAction = UIAlertAction.Create (acceptButtonTitle, UIAlertActionStyle.Default, _ => 
				Console.WriteLine ("The simple alert's accept action occurred.")
			);

			// Add the action.
			alertController.AddAction (acceptAction);
			PresentViewController (alertController, true, null);
		}

		partial void ShowOkCancelAlert (NSObject sender)
		{
			var alertController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);

			// Create the action.
			var acceptAction = UIAlertAction.Create (acceptButtonTitle, UIAlertActionStyle.Default, _ => 
				Console.WriteLine ("The \"OK/Cancel\" alert's other action occurred.")
			);

			var cancelAction = UIAlertAction.Create (cancelButtonTitle, UIAlertActionStyle.Cancel, _ => 
				Console.WriteLine ("The \"OK/Cancel\" alert's other action occurred.")
			);

			// Add the actions.
			alertController.AddAction (acceptAction);
			alertController.AddAction (cancelAction);
			PresentViewController (alertController, true, null);
		}

		partial void ShowDestructiveAlert (NSObject sender)
		{
			var alertController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);

			var cancelAction = UIAlertAction.Create (cancelButtonTitle, UIAlertActionStyle.Cancel, _ => 
				Console.WriteLine ("The \"Other\" alert's other action occurred.")
			);

			var deleteAction = UIAlertAction.Create (deleteButtonTitle, UIAlertActionStyle.Destructive, _ => 
				Console.WriteLine ("The \"Other\" alert's other action occurred.")
			);

			// Add the actions.
			alertController.AddAction (cancelAction);
			alertController.AddAction (deleteAction);
			PresentViewController (alertController, true, null);
		}
	}
}
