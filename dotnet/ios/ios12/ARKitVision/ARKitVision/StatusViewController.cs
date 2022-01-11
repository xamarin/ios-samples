
/*
 Displayed at the top of the main interface of the app that allows users to see
 the status of the AR experience, as well as the ability to control restarting
 the experience altogether.
 */

using ARKit;

namespace ARKitVision;

/// <summary>
/// Utility class for showing messages above the AR view.
/// </summary>
public partial class StatusViewController : UIViewController
{
        /// Seconds before the timer message should fade out. Adjust if the app needs longer transient messages.
        private const double DisplayDuration = 6d;

        private Dictionary<MessageType, NSTimer> timers = new Dictionary<MessageType, NSTimer> ();

        // Timer for hiding messages.
        private NSTimer? messageHideTimer;

        public StatusViewController (IntPtr handle) : base (handle) { }

        /// <summary>
        /// Trigerred when the "Restart Experience" button is tapped.
        /// </summary>
        public Action RestartExperienceHandler { get; set; } = () => { };

        #region Message Handling

        public void ShowMessage (string text, bool autoHide = true)
        {
                // Cancel any previous hide timer.
                messageHideTimer?.Invalidate ();

                messageLabel.Text = text;

                // Make sure status is showing.
                SetMessageHidden (false, true);

                if (autoHide)
                {
                        messageHideTimer = NSTimer.CreateScheduledTimer (DisplayDuration, false, (timer) =>
                         {
                                 SetMessageHidden (true, true);
                         });
                }
        }

        public void ScheduleMessage (string text, double seconds, MessageType messageType)
        {
                CancelScheduledMessage (messageType);

                var timer = NSTimer.CreateScheduledTimer (seconds, false, (internalTimer) =>
                 {
                         ShowMessage (text);
                         internalTimer.Invalidate ();
                 });

                timers [messageType] = timer;
        }

        public void CancelScheduledMessage (MessageType messageType)
        {
                if (timers.TryGetValue (messageType, out NSTimer? timer))
                {
                        timer.Invalidate ();
                        timers.Remove (messageType);
                }
        }

        public void CancelAllScheduledMessages ()
        {
                foreach (var messageType in Enum.GetValues <MessageType> ())
                {
                        CancelScheduledMessage (messageType);
                }
        }

        #endregion

        #region ARKit

        public void ShowTrackingQualityInfo (ARCamera camera, bool autoHide)
        {
                ShowMessage (camera.GetPresentationString (), autoHide);
        }

        public void EscalateFeedback (ARCamera camera, double seconds)
        {
                CancelScheduledMessage (MessageType.TrackingStateEscalation);

                var timer = NSTimer.CreateScheduledTimer (seconds, false, (internalTimer) =>
                 {
                         CancelScheduledMessage (MessageType.TrackingStateEscalation);

                         var message = camera.GetPresentationString ();
                         var recommendation = camera.GetRecommendation ();
                         if (!string.IsNullOrEmpty (recommendation))
                         {
                                 message += $": {recommendation}";
                         }

                         ShowMessage (message, false);
                 });

                timers [MessageType.TrackingStateEscalation] = timer;
        }

        #endregion

        #region IBActions

        partial void restartExperience (UIButton sender)
        {
                RestartExperienceHandler ();
        }

        #endregion

        #region Panel Visibility

        private void SetMessageHidden (bool hide, bool animated)
        {
                // The panel starts out hidden, so show it before animating opacity.
                messagePanel.Hidden = false;

                if (animated)
                {
                        UIView.Animate (0.2d,
                                       0d,
                                       UIViewAnimationOptions.BeginFromCurrentState,
                                       () => messagePanel.Alpha = hide ? 0f : 1f,
                                       null);
                }
                else
                {
                        messagePanel.Alpha = hide ? 0f : 1f;
                }
        }

        #endregion
}
