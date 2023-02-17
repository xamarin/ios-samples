
/*
 Displayed at the top of the main interface of the app that allows users to see
 the status of the AR experience, as well as the ability to control restarting
 the experience altogether.
 */

namespace ARKitVision {
	using ARKit;
	using Foundation;
	using System;
	using System.Collections.Generic;
	using UIKit;

	/// <summary>
	/// Utility class for showing messages above the AR view.
	/// </summary>
	public partial class StatusViewController : UIViewController {
		/// Seconds before the timer message should fade out. Adjust if the app needs longer transient messages.
		private const double DisplayDuration = 6d;

		private Dictionary<MessageType, NSTimer> timers = new Dictionary<MessageType, NSTimer> ();

		// Timer for hiding messages.
		private NSTimer messageHideTimer;

		public StatusViewController (IntPtr handle) : base (handle) { }

		/// <summary>
		/// Trigerred when the "Restart Experience" button is tapped.
		/// </summary>
		public Action RestartExperienceHandler { get; set; }

		#region Message Handling

		public void ShowMessage (string text, bool autoHide = true)
		{
			// Cancel any previous hide timer.
			this.messageHideTimer?.Invalidate ();

			this.messageLabel.Text = text;

			// Make sure status is showing.
			this.SetMessageHidden (false, true);

			if (autoHide) {
				this.messageHideTimer = NSTimer.CreateScheduledTimer (DisplayDuration, false, (timer) => {
					this.SetMessageHidden (true, true);
				});
			}
		}

		public void ScheduleMessage (string text, double seconds, MessageType messageType)
		{
			this.CancelScheduledMessage (messageType);

			var timer = NSTimer.CreateScheduledTimer (seconds, false, (internalTimer) => {
				this.ShowMessage (text);
				internalTimer.Invalidate ();
			});

			this.timers [messageType] = timer;
		}

		public void CancelScheduledMessage (MessageType messageType)
		{
			if (this.timers.TryGetValue (messageType, out NSTimer timer)) {
				timer.Invalidate ();
				this.timers.Remove (messageType);
			}
		}

		public void CancelAllScheduledMessages ()
		{
			foreach (MessageType messageType in Enum.GetValues (typeof (MessageType))) {
				this.CancelScheduledMessage (messageType);
			}
		}

		#endregion

		#region ARKit

		public void ShowTrackingQualityInfo (ARCamera camera, bool autoHide)
		{
			this.ShowMessage (camera.GetPresentationString (), autoHide);
		}

		public void EscalateFeedback (ARCamera camera, double seconds)
		{
			this.CancelScheduledMessage (MessageType.TrackingStateEscalation);

			var timer = NSTimer.CreateScheduledTimer (seconds, false, (internalTimer) => {
				this.CancelScheduledMessage (MessageType.TrackingStateEscalation);

				var message = camera.GetPresentationString ();
				var recommendation = camera.GetRecommendation ();
				if (!string.IsNullOrEmpty (recommendation)) {
					message += $": {recommendation}";
				}

				this.ShowMessage (message, false);
			});

			this.timers [MessageType.TrackingStateEscalation] = timer;
		}

		#endregion

		#region IBActions

		partial void restartExperience (UIButton sender)
		{
			this.RestartExperienceHandler ();
		}

		#endregion

		#region Panel Visibility

		private void SetMessageHidden (bool hide, bool animated)
		{
			// The panel starts out hidden, so show it before animating opacity.
			messagePanel.Hidden = false;

			if (animated) {
				UIView.Animate (0.2d,
							   0d,
							   UIViewAnimationOptions.BeginFromCurrentState,
							   () => this.messagePanel.Alpha = hide ? 0f : 1f,
							   null);
			} else {
				this.messagePanel.Alpha = hide ? 0f : 1f;
			}
		}

		#endregion
	}
}
