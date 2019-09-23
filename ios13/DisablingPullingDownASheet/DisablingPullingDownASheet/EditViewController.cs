/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A view controller for editing saved text. Allows cancellation and saving with standard bar button items, as well as with the pull to dismiss gesture.
*/

using System;

using Foundation;
using UIKit;

namespace DisablingPullingDownASheet {
	public partial class EditViewController : UIViewController, IUITextViewDelegate, IUIAdaptivePresentationControllerDelegate {
		public EditViewController (IntPtr handle) : base (handle)
		{
		}

		#region Model

		string originalText;
		public string OriginalText {
			get => originalText;
			set {
				EditedText = value;
				originalText = value;
			}
		}

		string editedText;
		public string EditedText {
			get => editedText;
			set {
				editedText = value;
				ViewIfLoaded?.SetNeedsLayout ();
			}
		}

		public bool HasChanges { get => OriginalText != EditedText; }

		#endregion

		#region Delegate

		[Weak] IEditViewControllerDelegate _delegate;
		public IEditViewControllerDelegate Delegate {
			get => _delegate;
			set => _delegate = value;
		}

		void SendDidCancel () => Delegate?.EditViewControllerDidCancel (this);
		void SendDidFinish () => Delegate?.EditViewControllerDidFinish (this);

		#endregion

		#region View

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// For user convenience, present the keyboard as soon as we begin appearing
			textView.BecomeFirstResponder ();
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

			// Ensure textView.text is up to date
			textView.Text = editedText;

			// If our model has unsaved changes, prevent pull to dismiss and enable the save button
			var hasChanges = HasChanges;
			ModalInPresentation = hasChanges;
			saveButton.Enabled = hasChanges;
		}

		#endregion

		#region Events

		[Export ("textViewDidChange:")]
		public void Changed (UITextView textView) => EditedText = textView.Text;

		partial void Cancel (NSObject sender)
		{
			if (HasChanges)
				// The user tapped Cancel with unsaved changes
				// Confirm that they really mean to cancel
				ConfirmCancel (false);
			else
				// No unsaved changes, so dismiss immediately
				SendDidCancel ();
		}

		partial void Save (NSObject sender) => SendDidFinish ();

		[Export ("presentationControllerDidAttemptToDismiss:")]
		public void DidAttemptToDismiss (UIPresentationController presentationController)
		{
			// The user pulled down with unsaved changes
			// Clarify the user's intent by asking whether they intended to cancel or save
			ConfirmCancel (true);
		}

		#endregion

		#region Cancellation Confirmation

		void ConfirmCancel (bool showingSave)
		{
			// Present an action sheet, which in a regular width environment appears as a popover
			var alert = UIAlertController.Create (null, null, UIAlertControllerStyle.ActionSheet);

			// Only ask if the user intended to save if they attempted to pull to dismiss, not if they tap Cancel
			if (showingSave)
				alert.AddAction (UIAlertAction.Create ("Save", UIAlertActionStyle.Default, obj => Delegate?.EditViewControllerDidFinish (this)));

			alert.AddAction (UIAlertAction.Create ("Discard Changes", UIAlertActionStyle.Destructive, obj => Delegate?.EditViewControllerDidCancel (this)));
			alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));

			// The popover should point at the Cancel button
			if (alert.PopoverPresentationController != null)
				// The popover should point at the Cancel button
				alert.PopoverPresentationController.BarButtonItem = cancelButton;

			PresentViewController (alert, true, null);
		}

		#endregion
	}
}

