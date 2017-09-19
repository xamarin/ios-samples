using System;
using CoreFoundation;
using Foundation;
using UIKit;

namespace PlacingObjects
{
	public partial class ViewController : IUIPopoverPresentationControllerDelegate
	{
		static class SegueIdentifier
		{
			public static readonly NSString ShowSettings = new NSString("showSettings");
			public static readonly NSString ShowObjects = new NSString("showObjects");
		}

		[Export("adaptivePresentationStyleForPresentationController:")]
		public UIModalPresentationStyle GetAdaptivePresentationStyle(UIPresentationController forPresentationController)
		{
			return UIModalPresentationStyle.None;
		}

		[Export("adaptivePresentationStyleForPresentationController:traitCollection:")]
		public UIModalPresentationStyle GetAdaptivePresentationStyle(UIPresentationController controller, UITraitCollection traitCollection)
		{
			return UIModalPresentationStyle.None;
		}

		[Action("RestartExperience:")]
		public void RestartExperience(NSObject sender)
		{
			if (!RestartExperienceButton.Enabled || IsLoadingObject) 
			{
				return;
			}

			RestartExperienceButton.Enabled = false;

			UserFeedback.CancelAllScheduledMessages();
			UserFeedback.DismissPresentedAlert();
			UserFeedback.ShowMessage("STARTING A NEW SESSION");

			virtualObjectManager.RemoveAllVirtualObjects();
			AddObjectButton.SetImage(UIImage.FromBundle("add"), UIControlState.Normal);
			AddObjectButton.SetImage(UIImage.FromBundle("addPressed"), UIControlState.Highlighted);
			if (FocusSquare != null)
			{
				FocusSquare.Hidden = true;
			}
			ResetTracking();

			RestartExperienceButton.SetImage(UIImage.FromBundle("restart"), UIControlState.Normal);

			// Disable Restart button for a second in order to give the session enough time to restart.
			var when = new DispatchTime(DispatchTime.Now, new TimeSpan(0, 0, 1));
			DispatchQueue.MainQueue.DispatchAfter(when, () => SetupFocusSquare());
		}

		[Action("chooseObject:")]
		public void ChooseObject(UIButton button)
		{
			// Abort if we are about to load another object to avoid concurrent modifications of the scene.
			if (IsLoadingObject)
			{
				return;
			}

			UserFeedback.CancelScheduledMessage(MessageType.ContentPlacement);
			PerformSegue(SegueIdentifier.ShowObjects, button);
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			// All popover segues should be popovers even on iPhone.
			var popoverController = segue.DestinationViewController?.PopoverPresentationController;
			if (popoverController == null)
			{
				return;
			}
			var button = (UIButton)sender;
			popoverController.Delegate = this;
			popoverController.SourceRect = button.Bounds;
			var identifier = segue.Identifier;
			if (identifier == SegueIdentifier.ShowObjects)
			{
				var objectsViewController = segue.DestinationViewController as VirtualObjectSelectionViewController;
				objectsViewController.Delegate = this;
			}
		}
	}
}
