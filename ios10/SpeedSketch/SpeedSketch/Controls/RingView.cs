using System;
using System.Collections.Generic;

using UIKit;
using CoreGraphics;

using static SpeedSketch.RingControlState;

namespace SpeedSketch
{
	enum RingControlState
	{
		Selected,
		Normal,
		LocationFan,
		LocationOrigin
	}

	public class RingView : UIView
	{
		// Closures that configure the view for the corresponding state.
		readonly Dictionary<RingControlState, Action> stateClosures = new Dictionary<RingControlState, Action> ();

		bool selected;
		bool fannedOut;

		// The actionClosure will be executed on selection.
		Action actionClosure;

		Action SelectionState {
			get {
				return stateClosures [selected ? Selected : Normal];
			}
		}

		Action LocationState {
			get {
				if (selected)
					return stateClosures [fannedOut ? LocationFan : LocationOrigin];

				var fanState = stateClosures [LocationFan];
				var transform = fannedOut ? CGAffineTransform.MakeIdentity () : CGAffineTransform.MakeScale (0.01f, 0.01f);
				var alpha = fannedOut ? 1f : 0f;

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
			var quadrance = Bounds.GetCenter ().Sub (point).Quadrance ();
			var maxQuadrance = NMath.Pow (Bounds.Width / 2, 2);

			return quadrance < maxQuadrance;
		}
	}
}
