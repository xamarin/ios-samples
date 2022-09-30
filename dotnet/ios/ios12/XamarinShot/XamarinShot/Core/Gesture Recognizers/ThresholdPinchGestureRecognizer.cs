namespace XamarinShot.Models.GestureRecognizers;


/// <summary>
/// A custom pinch gesture reconizer that fires only when a threshold is passed.
/// </summary>
[Register ("ThresholdPinchGestureRecognizer")]
public class ThresholdPinchGestureRecognizer : UIPinchGestureRecognizer
{
        /// The threshold in screen pixels after which this gesture is detected.
        static float Threshold = 40f;

        /// The initial touch location when this gesture started.
        float initialTouchDistance = 0f;

        public ThresholdPinchGestureRecognizer (IntPtr handle) : base (handle) { }

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
                                        break;
                        }
                }
        }

        public override void TouchesMoved (NSSet touches, UIEvent evt)
        {
                if (touches.Count == 2)
                {
                        base.TouchesMoved (touches, evt);

                        switch (State)
                        {
                                case UIGestureRecognizerState.Began:
                                        initialTouchDistance = TouchDistance (touches.Cast<UITouch> ().ToList ());
                                        break;

                                case UIGestureRecognizerState.Changed:
                                        var touchDistance = TouchDistance (touches.Cast<UITouch> ().ToList ());
                                        if (Math.Abs (touchDistance - initialTouchDistance) > ThresholdPinchGestureRecognizer.Threshold)
                                        {
                                                IsThresholdExceeded = true;
                                        }
                                        break;

                                default:
                                        break;
                        }

                        if (!IsThresholdExceeded)
                        {
                                Scale = 1f;
                        }
                }
        }

        float TouchDistance (IList<UITouch> touches)
        {
                if (touches.Count == 2)
                {
                        var points = new List<CGPoint> ();
                        foreach (var touch in touches)
                        {
                                points.Add (touch.LocationInView (View));
                        }

                        var distance = Math.Sqrt ((points [0].X - points [1].X) * (points [0].X - points [1].X) + (points [0].Y - points [1].Y) * (points [0].Y - points [1].Y));
                        return (float)distance;
                }
                else
                {
                        return 0f;
                }
        }
}
