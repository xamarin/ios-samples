using System;
using Foundation;
using UIKit;
using CoreGraphics;
using SceneKit;
using ARKit;

namespace PlacingObjects
{
	public class TextManager : NSObject
	{
		private NSTimer MessageHideTimer;
		private NSTimer FocusSquareMessageTimer;
		private NSTimer PlaneEstimationMessageTimer;
		private NSTimer ContentPlacementMessageTimer;
		private NSTimer TrackingStateFeedbackMessageTimer;
		private bool SchedulingMessageBlocked = false;
		private NSTimer ScheduleMessageTimer;
		private UIAlertController AlertController;
		private nint BlurEffectViewTag = 100;

		public ViewController Controller { get; set; }

		public TextManager(ViewController controller)
		{
			// Initialize
			Controller = controller;
		}

		public void ShowMessage(string text, bool autoHide = true)
		{
			InvokeOnMainThread(() =>
			{
				// Cancel previous hide timer
				MessageHideTimer?.Invalidate();

				// Set text and display status
				InvokeOnMainThread(() =>
								  Controller.MessageLabel.Text = text);
				ShowHideMessage(false, true);

				// Autohide?
				if (autoHide)
				{
					// Compute an appropriate amount of time to display the on screen message.
					// According to https://en.wikipedia.org/wiki/Words_per_minute, adults read
					// about 200 words per minute and the average English word is 5 characters
					// long. So 1000 characters per minute / 60 = 15 characters per second.
					// We limit the duration to a range of 1-10 seconds.
					var charCount = text.Length;
					var displayDuration = Math.Min(10, (double)charCount / 15.0 + 1.0);
					MessageHideTimer = NSTimer.CreateScheduledTimer(displayDuration, false, (obj) =>
					{
						ShowHideMessage(true, true);
					});
				}
			});
		}

		public void ScheduleMessage(string text, double displayDuration, MessageType messageType)
		{
			// Can messages be scheduled?
			if (SchedulingMessageBlocked) 
			{
				return;
			}

			// Get timer based on the message type
			switch (messageType)
			{
				case MessageType.ContentPlacement:
					ScheduleMessageTimer = ContentPlacementMessageTimer;
					break;
				case MessageType.FocusSquare:
					ScheduleMessageTimer = FocusSquareMessageTimer;
					break;
				case MessageType.PlaneEstimation:
					ScheduleMessageTimer = PlaneEstimationMessageTimer;
					break;
				case MessageType.TrackingStateEscalation:
					ScheduleMessageTimer = TrackingStateFeedbackMessageTimer;
					break;
			}

			// Stop any running timers
			ScheduleMessageTimer?.Invalidate();

			// Create a new timer
			ScheduleMessageTimer = NSTimer.CreateScheduledTimer(displayDuration, false, (obj) =>
			{
				ShowMessage(text, true);
				ScheduleMessageTimer?.Invalidate();
			});

			// Save timer based on the message type
			switch (messageType)
			{
				case MessageType.ContentPlacement:
					ContentPlacementMessageTimer = ScheduleMessageTimer;
					break;
				case MessageType.FocusSquare:
					FocusSquareMessageTimer = ScheduleMessageTimer;
					break;
				case MessageType.PlaneEstimation:
					PlaneEstimationMessageTimer = ScheduleMessageTimer;
					break;
				case MessageType.TrackingStateEscalation:
					TrackingStateFeedbackMessageTimer = ScheduleMessageTimer;
					break;
			}
		}

		public void ShowTrackingQualityInfo(ARTrackingState trackingState, ARTrackingStateReason reason, bool autoHide)
		{
			var title = "";
			var message = "";

			switch (trackingState)
			{
				case ARTrackingState.NotAvailable:
					title = "Tracking Not Available";
					break;
				case ARTrackingState.Normal:
					title = "Tracking Normal";
					break;
				case ARTrackingState.Limited:
					title = "Tracking Limited";
					switch (reason)
					{
						case ARTrackingStateReason.ExcessiveMotion:
							message = "because of excessive motion";
							break;
						case ARTrackingStateReason.Initializing:
							message = "because tracking is initializing";
							break;
						case ARTrackingStateReason.InsufficientFeatures:
							message = "because of insufficient features in the environment";
							break;
						case ARTrackingStateReason.None:
							message = "because of an unknown reason";
							break;
					}
					break;
			}

			ShowMessage($"{title} {message}", autoHide);
		}

		public void EscalateFeedback(ARTrackingState trackingState, ARTrackingStateReason reason, double displayDuration)
		{
			InvokeOnMainThread(() =>
			{
				var title = "Tracking status: Limited.";
				var message = "Tracking status has been limited for an extended time. ";

				// Stop any running timer
				TrackingStateFeedbackMessageTimer?.Invalidate();

				// Create new timer
				TrackingStateFeedbackMessageTimer = NSTimer.CreateScheduledTimer(displayDuration, false, (timer) =>
				{
					TrackingStateFeedbackMessageTimer?.Invalidate();
					SchedulingMessageBlocked = true;

					// Take action based on th tracking state
					switch (trackingState)
					{
						case ARTrackingState.NotAvailable:
							title = "Tracking Not Available";
							break;
						case ARTrackingState.Normal:
							title = "Tracking Normal";
							break;
						case ARTrackingState.Limited:
							title = "Tracking Limited";
							switch (reason)
							{
								case ARTrackingStateReason.ExcessiveMotion:
									message = "because of excessive motion";
									break;
								case ARTrackingStateReason.Initializing:
									message = "because tracking is initializing";
									break;
								case ARTrackingStateReason.InsufficientFeatures:
									message = "because of insufficient features in the environment";
									break;
								case ARTrackingStateReason.None:
									message = "because of an unknown reason";
									break;
							}
							break;
					}

					// Create and display an Alert Message
					var restartAction = UIAlertAction.Create("Reset", UIAlertActionStyle.Destructive, (obj) =>
					{
						Controller.RestartExperience(this);
						SchedulingMessageBlocked = false;
					});

					var okAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, (obj) =>
					{
						SchedulingMessageBlocked = false;
					});

					ShowAlert(title, message, new UIAlertAction[] { restartAction, okAction });
				});
			});
		}

		public void CancelScheduledMessage(MessageType messageType)
		{
			InvokeOnMainThread(() =>
			{
				// Get timer based on the message type
				switch (messageType)
				{
					case MessageType.ContentPlacement:
						ScheduleMessageTimer = ContentPlacementMessageTimer;
						break;
					case MessageType.FocusSquare:
						ScheduleMessageTimer = FocusSquareMessageTimer;
						break;
					case MessageType.PlaneEstimation:
						ScheduleMessageTimer = PlaneEstimationMessageTimer;
						break;
					case MessageType.TrackingStateEscalation:
						ScheduleMessageTimer = TrackingStateFeedbackMessageTimer;
						break;
				}

				// Stop any running timers
				ScheduleMessageTimer?.Invalidate();
				ScheduleMessageTimer = null;
			});
		}

		public void CancelAllScheduledMessages()
		{
			InvokeOnMainThread(() =>
			{
				CancelScheduledMessage(MessageType.ContentPlacement);
				CancelScheduledMessage(MessageType.FocusSquare);
				CancelScheduledMessage(MessageType.PlaneEstimation);
				CancelScheduledMessage(MessageType.TrackingStateEscalation);
			});
		}

		public void ShowAlert(string title, string message, UIAlertAction[] actions = null)
		{
			InvokeOnMainThread(() =>
			{
				// Create an alert controller
				AlertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

				// Any actions to add?
				if (actions != null && actions.Length > 0)
				{
					// Add actions to controller
					foreach (UIAlertAction action in actions)
					{
						AlertController.AddAction(action);
					}
				}
				else
				{
					// Add default button
					AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (obj) => { }));
				}

				// Display the alert
				Controller.PresentViewController(AlertController, true, null);
			});
		}

		public void DismissPresentedAlert()
		{
			InvokeOnMainThread(() =>
			{
				AlertController?.DismissViewController(true, null);
			});
		}

		public void BlurBackground()
		{
			InvokeOnMainThread(() =>
			{
				var blurEffect = UIBlurEffect.FromStyle(UIBlurEffectStyle.Light);
				var blurEffectView = new UIVisualEffectView(blurEffect)
				{
					Frame = Controller.View.Bounds,
					AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
					Tag = BlurEffectViewTag
				};
				Controller.View.AddSubview(blurEffectView);
			});
		}

		public void UnblurBackground()
		{
			InvokeOnMainThread(() =>
			{
				// Scan all views
				foreach (UIView view in Controller.View.Subviews)
				{
					// Is this the view a blur effect view?
					if (view is UIVisualEffectView && view.Tag == BlurEffectViewTag)
					{
						// Yes, remove it
						view.RemoveFromSuperview();
					}
				}
			});
		}

		private void UpdateMessagePanelVisibility()
		{
			InvokeOnMainThread(() =>
			{
				// Is any message visible?
				var anyMessageShown = !Controller.MessageLabel.Hidden;

				// Only show message panel if something is visible
				Controller.MessagePanel.Hidden = !anyMessageShown;
			});
		}

		private void ShowHideMessage(bool hide, bool animated)
		{
			InvokeOnMainThread(() =>
			{
				// Animated?
				if (animated)
				{
					// Fade out message
					UIView.Animate(0.2f, 0, UIViewAnimationOptions.AllowUserInteraction, () =>
					{
						Controller.MessageLabel.Hidden = hide;
						UpdateMessagePanelVisibility();
					}, null);
				}
				else
				{
					// Instantly hide message
					Controller.MessageLabel.Hidden = hide;
				}
			});
		}

	}
}
