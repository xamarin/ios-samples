using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace HomeKitIntro
{
	public class AlertView
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

		#endregion
	}
}

