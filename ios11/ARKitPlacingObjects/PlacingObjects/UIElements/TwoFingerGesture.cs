using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreGraphics;
using SceneKit;
using ARKit;
using System.Linq;

namespace PlacingObjects
{
	public class TwoFingerGesture : Gesture
	{

		protected UITouch FirstTouch { get; set; }
		protected UITouch SecondTouch { get; set; }

		protected float TranslationThreshold { get; set; } = 40f;
		protected float TranslationThresholdHarder { get; set; } = 70f;
		protected bool TranslationThresholdPassed { get; set; } = false;
		protected bool AllowTranslation { get; set; } = false;
		protected CGPoint DragOffset { get; set; } = new CGPoint();
		protected CGPoint InitialMidpoint { get; set; } = new CGPoint(0, 0);

		protected float RotationThreshold { get; set; } = (float)(Math.PI / 15); // (12°)
		protected float RotationThresholdHarder { get; set; } = (float)(Math.PI / 10); // (18°)
		protected bool RotationThresholdPassed { get; set; } = false;
		protected bool AllowRotation { get; set; } = false;
		protected double InitialFingerAngle { get; set; } = 0;
		protected float InitialObjectAngle { get; set; } = 0;
		protected VirtualObject FirstTouchedObject { get; set; } = null;

		protected float ScaleThreshold { get; set; } = 50f;
		protected float ScaleThresholdHarder { get; set; } = 90f;
		protected bool ScaleThresholdPassed { get; set; } = false;

		protected float InitialDistanceBetweenFingers { get; set; } = 0;
		protected float BaseDistanceBetweenFingers { get; set; } = 0;
		protected double ObjectBaseScale { get; set; } = 1.0;

		public TwoFingerGesture(NSSet touches, ARSCNView view, VirtualObject parentObject, VirtualObjectManager manager) : base(touches, view, parentObject, manager)
		{
			var tArray = touches.ToArray<UITouch>();
			FirstTouch = tArray[0];
			SecondTouch = tArray[1];

			var firstTouchPoint = FirstTouch.LocationInView(SceneView);
			var secondTouchPoint = SecondTouch.LocationInView(SceneView);

			InitialMidpoint = firstTouchPoint.Add(secondTouchPoint).Divide(2f);

			// Compute the two other corners of the rectangle defined by the two fingers
			// and compute the points in between.
			var thirdCorner = new CGPoint(firstTouchPoint.X, secondTouchPoint.Y);
			var fourthCorner = new CGPoint(secondTouchPoint.X, firstTouchPoint.Y);

			//  Compute points in between.
			var midpoints = new[]
			{
				thirdCorner.Add(firstTouchPoint).Divide(2f),
				thirdCorner.Add(secondTouchPoint).Divide(2f),
				fourthCorner.Add(firstTouchPoint).Divide(2f),
				fourthCorner.Add(secondTouchPoint).Divide(2f),
				InitialMidpoint.Add(firstTouchPoint).Divide(2f),
				InitialMidpoint.Add(secondTouchPoint).Divide(2f),
				InitialMidpoint.Add(thirdCorner).Divide(2f),
				InitialMidpoint.Add(fourthCorner).Divide(2f)
			};

			// Check if any of the two fingers or their midpoint is touching the object.
			// Based on that, translation, rotation and scale will be enabled or disabled.
			var allPoints = new List<CGPoint>(new[] { firstTouchPoint, secondTouchPoint, thirdCorner, fourthCorner, InitialMidpoint });
			allPoints.AddRange(midpoints);
			FirstTouchedObject = allPoints.Select(pt => this.VirtualObjectAt(pt)).Where(vo => vo != null).FirstOrDefault();

			if (FirstTouchedObject != null)
			{
				ObjectBaseScale = FirstTouchedObject.Scale.X;
				AllowTranslation = true;
				AllowRotation = true;
				InitialDistanceBetweenFingers = (firstTouchPoint.Subtract(secondTouchPoint)).Length();
				InitialFingerAngle = Math.Atan2(InitialMidpoint.X, InitialMidpoint.Y);
				InitialObjectAngle = FirstTouchedObject.EulerAngles.Y;
			}
			else
			{
				AllowTranslation = false;
				AllowRotation = false;
			}

		}

		override protected void UpdateGesture(object state)
		{
			// Two finger touch enables combined translation, rotation and scale.
			if (FirstTouchedObject == null)
			{
				return;
			}
			var virtualObject = FirstTouchedObject;

			// First: Update the touches.
			if (currentTouches.Count < 2)
			{
				return;
			}
			var tAry = currentTouches.ToArray<UITouch>();
			var newTouch1 = tAry[0];
			var newTouch2 = tAry[1];

			if (newTouch1.GetHashCode() == FirstTouch.GetHashCode())
			{
				FirstTouch = newTouch1;
				SecondTouch = newTouch2;
			}
			else
			{
				FirstTouch = newTouch2;
				SecondTouch = newTouch1;
			}

			InvokeOnMainThread(() =>
			{
				var loc1 = FirstTouch.LocationInView(SceneView);
				var loc2 = SecondTouch.LocationInView(SceneView);

				if (AllowTranslation)
				{
					// 1. Translation using the midpoint between the two fingers.
					UpdateTranslation(virtualObject, loc1.MidPoint(loc2));
				}

				var spanBetweenTouches = loc1.Subtract(loc2);

				if (AllowRotation)
				{
					// 2. Rotation based on the relative rotation of the fingers on a unit circle.
					UpdateRotation(virtualObject, spanBetweenTouches);
				}
			});
		}

		override public Gesture UpdateGestureFromTouches(NSSet touches, TouchEventType touchType)
		{
			base.UpdateGestureFromTouches(touches, touchType);

			// Take action based on the number of touches
			switch (currentTouches.Count)
			{
				case 0:
					// Nothing to process
					return this;
				case 2:
					// Update this gesture
					UpdateGesture(null);
					return this;
				default:
					// Finish this two finger gesture and switch to no gesture -> The user
					// will have to release all other fingers and touch the screen again
					// to start a new gesture.
					FinishGesture();
					this.refreshTimer?.Dispose();
					// Erase reference to timer in case callback fires one last time
					this.refreshTimer = null;
					return null;
			}
		}

		protected override void FinishGesture()
		{
			// Nothing to do when finishing two-finger gesture
		}

		public void UpdateTranslation(VirtualObject virtualObject, CGPoint midpoint)
		{

			if (!TranslationThresholdPassed)
			{
				var initialLocationToCurrentLocation = midpoint.Subtract(InitialMidpoint);
				var distanceFromStartLocation = initialLocationToCurrentLocation.Length();

				// Check if the translate gesture has crossed the threshold.
				// If the user is already rotating and or scaling we use a bigger threshold.

				var threshold = TranslationThreshold;
				if (RotationThresholdPassed || TranslationThresholdPassed)
				{
					threshold = TranslationThresholdHarder;
				}

				if (distanceFromStartLocation >= threshold)
				{
					TranslationThresholdPassed = true;

					var currentObjectLocation = CGPointExtensions.FromVector(SceneView.ProjectPoint(virtualObject.Position));
					DragOffset = midpoint.Subtract(currentObjectLocation);
				}
			}

			if (TranslationThresholdPassed)
			{
				var offsetPos = midpoint.Subtract(DragOffset);
				manager.Translate(virtualObject, SceneView, offsetPos, false, true);
				LastUsedObject = virtualObject;
			}
		}

		public void UpdateRotation(VirtualObject virtualObject, CGPoint span)
		{

			var midPointToFirstTouch = span.Divide(2f);
			var currentAngle = Math.Atan2(midPointToFirstTouch.X, midPointToFirstTouch.Y);

			var currentAngleToInitialFingerAngle = InitialFingerAngle - currentAngle;

			if (!RotationThresholdPassed)
			{
				var threshold = RotationThreshold;

				if (TranslationThresholdPassed || ScaleThresholdPassed)
				{
					threshold = RotationThresholdHarder;
				}

				if ((float)Math.Abs(currentAngleToInitialFingerAngle) > threshold)
				{
					RotationThresholdPassed = true;

					// Change the initial object angle to prevent a sudden jump after crossing the threshold.
					if (currentAngleToInitialFingerAngle > 0f)
					{
						InitialObjectAngle += threshold;
					}
					else
					{
						InitialObjectAngle -= threshold;
					}
				}
			}

			if (RotationThresholdPassed)
			{
				// Note:
				// For looking down on the object (99% of all use cases), we need to subtract the angle.
				// To make rotation also work correctly when looking from below the object one would have to
				// flip the sign of the angle depending on whether the object is above or below the camera...
				var x = virtualObject.EulerAngles.X;
				var y = InitialObjectAngle - currentAngleToInitialFingerAngle;
				var z = virtualObject.EulerAngles.Z;
				virtualObject.EulerAngles = new SCNVector3(x, (float) y, z);
				LastUsedObject = virtualObject;
			}
		}
	}
}
