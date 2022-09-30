namespace XamarinShot.Models.GestureRecognizers;


/// <summary>
/// A custom pan gesture reconizer that fires only when a threshold is passed.
/// </summary>
[Register ("ThresholdPanGestureRecognizer")]
public class ThresholdPanGestureRecognizer : UIPanGestureRecognizer
{
        /// The threshold in screen pixels after which this gesture is detected.
        static float Threshold = 30f;

        /// The initial touch location when this gesture started.
        CGPoint initialLocation = CGPoint.Empty;

        public ThresholdPanGestureRecognizer (IntPtr handle) : base (handle) { }

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
                                case UIGestureRecognizerState.Possible:
                                case UIGestureRecognizerState.Began:
                                case UIGestureRecognizerState.Changed:
                                        break;
                                default:
                                        // Reset variables.
                                        IsThresholdExceeded = false;
                                        initialLocation = CGPoint.Empty;
                                        break;
                        }
                }
        }

        public override void TouchesBegan (NSSet touches, UIEvent evt)
        {
                base.TouchesBegan (touches, evt);
                initialLocation = LocationInView (View);
        }

        public override void TouchesMoved (NSSet touches, UIEvent evt)
        {
                base.TouchesMoved (touches, evt);

                var translationMagnitude = TranslationInView (View).Length ();

                if (!IsThresholdExceeded && translationMagnitude > ThresholdPanGestureRecognizer.Threshold)
                {
                        IsThresholdExceeded = true;

                        // Set the overall translation to zero as the gesture should now begin.
                        SetTranslation (CGPoint.Empty, View);
                }
        }

        public override CGPoint LocationInView (UIView? view)
        {
                switch (State)
                {
                        case UIGestureRecognizerState.Began:
                        case UIGestureRecognizerState.Changed:
                                var correctedLocation = new CGPoint (initialLocation.X + TranslationInView (view).X,
                                        initialLocation.Y + TranslationInView (view).Y);
                                return correctedLocation;

                        default:
                                return base.LocationInView (view);
                }
        }
}
