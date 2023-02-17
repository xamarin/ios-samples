/*
See LICENSE folder for this sample’s licensing information.

Abstract:
The root view controller of the app. Contains a label displaying text, and a button to present a sheet for editing the text.
*/

using System;
using Foundation;
using UIKit;

namespace DisablingPullingDownASheet {
	public partial class RootViewController : UIViewController, IEditViewControllerDelegate {
		public RootViewController (IntPtr handle) : base (handle)
		{
		}

		#region Model

		string text = "Hello World!";
		public string Text {
			get => text;
			set {
				text = value;
				ViewIfLoaded.SetNeedsLayout ();
			}
		}

		#endregion

		#region View

		public override void ViewWillLayoutSubviews ()
		{
			// Ensure textView.text is up to date
			TextView.Text = Text;
		}

		#endregion

		#region Segue

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			switch (segue.Identifier) {
			case "Edit":
				// Get the presented navigationController and the editViewController it contains
				var navigationController = segue.DestinationViewController as UINavigationController;
				var editViewController = navigationController.TopViewController as EditViewController;

				// Set the editViewController to be the delegate of the PresentationController for this presentation,
				// so that editViewController can respond to attempted dismissals
				navigationController.PresentationController.Delegate = editViewController;

				// Set ourself as the delegate of editViewController, so we can respond to editViewController cancelling or finishing
				editViewController.Delegate = this;

				// Pass our model to the editViewController
				editViewController.OriginalText = text;
				break;
			default:
				break;
			}
		}

		#endregion

		#region EditViewControllerDelegate

		public void EditViewControllerDidCancel (EditViewController editViewController) => DismissViewController (true, null);

		public void EditViewControllerDidFinish (EditViewController editViewController)
		{
			Text = editViewController.EditedText;
			DismissViewController (true, null);
		}

		#endregion
	}
}

