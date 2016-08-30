using System;
using System.Collections.Generic;

using UIKit;
using CoreGraphics;

using static SpeedSketch.RingControlState;
using static SpeedSketch.CGMathExtensions;

namespace SpeedSketch
{
	public enum RingControlState
	{
		Selected,
		Normal,
		LocationFan,
		LocationOrigin
	}

	public class RingView : UIView
	{
		// Closures that configure the view for the corresponding state.
		public Dictionary<RingControlState, Action> StateClosures { get; } = new Dictionary<RingControlState, Action> ();

		public bool Selected { get; set; }
		public bool FannedOut { get; set; }

		// The actionClosure will be executed on selection.
		public Action ActionClosure { get; set; }

		public Action SelectionState {
			get {
				return StateClosures [Selected ? RingControlState.Selected : Normal];
			}
		}

		public Action LocationState {
			get {
				if (Selected)
					return StateClosures [FannedOut ? LocationFan : LocationOrigin];

				var fanState = StateClosures [LocationFan];
				var transform = FannedOut ? CGAffineTransform.MakeIdentity () : CGAffineTransform.MakeScale (0.01f, 0.01f);
				var alpha = FannedOut ? 1f : 0f;

				return () => {
					fanState?.Invoke ();
					Transform = transform;
					Alpha = alpha;
				};
			}
		}

		public RingView (CGRect frame)
				: base (frame)
		{
			var layer = Layer;
			layer.CornerRadius = frame.Width / 2;
			layer.BorderColor = UIColor.Black.CGColor;
			layer.BorderWidth = 2;
		}

		public override bool PointInside (CGPoint point, UIEvent uievent)
		{
			// Quadrance as the square of the length requires less computation and cases
			var quadrance = Vector(Bounds.GetCenter (), point).Quadrance ();
			var maxQuadrance = NMath.Pow (Bounds.Width / 2, 2);

			return quadrance < maxQuadrance;
		}
	}
}
