
namespace XamarinShot.Models.GestureRecognizers
{
    using Foundation;
    using System;
    using UIKit;

    /// <summary>
    /// A custom rotation gesture reconizer that fires only when a threshold is passed.
    /// </summary>
    [Register("ThresholdRotationGestureRecognizer")]
    public class ThresholdRotationGestureRecognizer : UIRotationGestureRecognizer
    {
        /// The threshold in screen pixels after which this gesture is detected.
        private static float Threshold = (float)Math.PI / 18f; // (10°)

        private nfloat previousRotation = 0f;

        private nfloat rotationDelta = 0f;

        public ThresholdRotationGestureRecognizer(IntPtr handle) : base(handle) { }

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
                        this.IsThresholdExceeded = false;
                        this.previousRotation = 0f;
                        this.rotationDelta = 0f;
                        break;
                }
            }
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);

            if (this.IsThresholdExceeded)
            {
                this.rotationDelta = this.Rotation - this.previousRotation;
                this.previousRotation = this.Rotation;
            }

            if (!this.IsThresholdExceeded && Math.Abs(this.Rotation) > ThresholdRotationGestureRecognizer.Threshold)
            {
                this.IsThresholdExceeded = true;
                this.previousRotation = this.Rotation;
            }
        }
    }
}