
namespace XamarinShot.Models.GestureRecognizers;

/// <summary>
/// A custom rotation gesture reconizer that fires only when a threshold is passed.
/// </summary>
[Register ("ThresholdRotationGestureRecognizer")]
public class ThresholdRotationGestureRecognizer : UIRotationGestureRecognizer
{
        /// The threshold in screen pixels after which this gesture is detected.
        static float Threshold = (float)Math.PI / 18f; // (10°)

        nfloat previousRotation = 0f;

        nfloat rotationDelta = 0f;

        public ThresholdRotationGestureRecognizer (IntPtr handle) : base (handle) { }

        /// <summary>
        /// Indicates whether the currently active gesture has exceeeded the threshold.
        /// </summary>
        public bool IsThresholdExceeded { get; private set; }

        /// <summary>
        /// Observe when the gesture's `state` changes to reset the threshold.
        /// </summary>
        public override UIGestureRecognizerState State
        {
                get
                {
                        return base.State;
                }

                set
                {
                        base.State = value;
                        switch (base.State)
                        {
                                case UIGestureRecognizerState.Began:
                                case UIGestureRecognizerState.Changed:
                                        break;
                                default:
                                        // Reset threshold check.
                                        IsThresholdExceeded = false;
                                        previousRotation = 0f;
                                        rotationDelta = 0f;
                                        break;
                        }
                }
        }

        public override void TouchesMoved (NSSet touches, UIEvent evt)
        {
                base.TouchesMoved (touches, evt);

                if (IsThresholdExceeded)
                {
                        rotationDelta = Rotation - previousRotation;
                        previousRotation = Rotation;
                }

                if (!IsThresholdExceeded && Math.Abs (Rotation) > ThresholdRotationGestureRecognizer.Threshold)
                {
                        IsThresholdExceeded = true;
                        previousRotation = Rotation;
                }
        }
}
