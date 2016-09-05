using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using CoreGraphics;

using static SpeedSketch.RingControlState;
using static CoreGraphics.CGAffineTransform;

namespace SpeedSketch
{
	public class RingControl : UIView
	{
		RingView selectedView;

		nfloat RingRadius {
			get {
				return Bounds.Width / 2;
			}
		}

		public List<RingView> RingViews { get; } = new List<RingView> ();

		public RingControl (CGRect frame, int itemCount)
			: base (frame)
		{
			SetupRings (itemCount);
		}

		void SetupRings (int itemCount)
		{
			// Define some nice colors.
			var borderColorSelected = UIColor.FromHSBA (0.07f, 0.81f, 0.98f, 1).CGColor;
			var borderColorNormal = UIColor.DarkGray.CGColor;
			var fillColorSelected = UIColor.FromHSBA (0.07f, 0.21f, 0.98f, 1);
			var fillColorNormal = UIColor.White;

			// We define generators to return closures which we use to define
			// the different states of our item ring views.
			Func<RingView, Action> selectedGenerator = (view) => () => {
				view.Layer.BorderColor = borderColorSelected;
				view.BackgroundColor = fillColorSelected;
			};

			Func<RingView, Action> normalGenerator = (view) => () => {
				view.Layer.BorderColor = borderColorNormal;
				view.BackgroundColor = fillColorNormal;
			};

			CGPoint startPosition = Bounds.GetCenter ();
			Func<RingView, Action> locationNormalGenerator = (view) => () => {
				view.Center = startPosition;
				if (!view.Selected)
					view.Alpha = 0;
			};

			Func<RingView, CGVector, Action> locationFanGenerator = (view, offset) => () => {
				view.Center = startPosition.Add (offset);
				view.Alpha = 1;
			};

			// tau is a full circle in radians
			var tau = NMath.PI * 2;
			var absoluteRingSegment = tau / 4;
			var requiredLengthPerRing = RingRadius * 2 + 5;
			var totalRequiredCirlceSegment = requiredLengthPerRing * (itemCount - 1);
			var fannedControlRadius = NMath.Max (requiredLengthPerRing, totalRequiredCirlceSegment / absoluteRingSegment);
			var normalDistance = new CGVector (0, -fannedControlRadius);

			var scale = UIScreen.MainScreen.Scale;

			// Setup our item views.
			for (int index = 0; index < itemCount; index++) {
				var view = new RingView (Bounds);
				view.StateClosures [Selected] = selectedGenerator (view);
				view.StateClosures [Normal] = normalGenerator (view);

				nfloat angle = index / (nfloat)(itemCount - 1) * absoluteRingSegment;
				var fan = normalDistance.Apply (MakeRotation (angle)).RoundTo (scale);
				view.StateClosures [LocationFan] = locationFanGenerator (view, fan);

				view.StateClosures [LocationOrigin] = locationNormalGenerator (view);
				AddSubview (view);
				RingViews.Add (view);

				var gr = new UITapGestureRecognizer (Tap);
				view.AddGestureRecognizer (gr);
			}

			// Setup the initial selection state.
			var rv = RingViews [0];
			AddSubview (rv);
			rv.Selected = true;
			selectedView = rv;

			UpdateViews (animated: false);
		}

		void Tap (UITapGestureRecognizer recognizer)
		{
			var view = recognizer.View as RingView;
			if (view == null)
				return;

			var fanState = view.FannedOut;
			if (fanState)
				Select (view);
			else
				RingViews.ForEach (v => v.FannedOut = true);

			UpdateViews (true);
		}

		public void CancelInteraction ()
		{
			if (!selectedView.FannedOut)
				return;

			RingViews.ForEach (v => v.FannedOut = false);
			UpdateViews (true);
		}

		void Select (RingView view)
		{
			foreach (var v in RingViews) {
				if (v.Selected) {
					v.Selected = false;
					v.SelectionState?.Invoke ();
				}
				v.FannedOut = false;
			}

			view.Selected = true;
			selectedView = view;
			view.ActionClosure?.Invoke ();
		}

		void UpdateViews (bool animated)
		{
			// Order the selected view in front.
			AddSubview (selectedView);

			var stateTransitions = new List<Action> ();

			foreach (var view in RingViews) {
				var ss = view.SelectionState;
				if (ss != null)
					stateTransitions.Add (ss);

				var ls = view.LocationState;
				if (ls != null)
					stateTransitions.Add (ls);
			}

			Action transition = () => {
				foreach (var st in stateTransitions)
					st ();
			};

			if (animated)
				Animate (0.25, transition);
			else
				transition ();
		}

		// Hit test on our ring views regardless of our own bounds.
		public override UIView HitTest (CGPoint point, UIEvent uievent)
		{
			foreach (var view in Subviews.Reverse ()) {
				var localPoint = view.ConvertPointFromView (point, this);
				if (view.PointInside (localPoint, uievent))
					return view;
			}
			// Don't hit-test ourself.
			return null;

		}

		public override bool PointInside (CGPoint point, UIEvent uievent)
		{
			foreach (var view in Subviews.Reverse ()) {
				if (view.PointInside (view.ConvertPointFromView (point, this), uievent))
					return true;
			}
			return base.PointInside (point, uievent);
		}
	}
}
