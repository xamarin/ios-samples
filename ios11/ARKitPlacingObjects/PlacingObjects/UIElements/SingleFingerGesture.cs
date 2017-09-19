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
	public class SingleFingerGesture : Gesture
	{
		CGPoint initialTouchPosition = new CGPoint();
		CGPoint latestTouchPosition = new CGPoint();

		VirtualObject firstTouchedObject;

		readonly float translationThreshold = 30;
		bool translationThresholdPassed = false;
		bool hasMovedObject = false;

		CGPoint dragOffset = new CGPoint();

		public SingleFingerGesture(NSSet touches, ARSCNView scnView, VirtualObject lastUsedObject, VirtualObjectManager vom)
			: base(touches, scnView, lastUsedObject, vom)
		{
			var firstTouch = (UITouch)touches.First();
			initialTouchPosition = firstTouch.LocationInView(scnView);
			latestTouchPosition = initialTouchPosition;

			firstTouchedObject = this.VirtualObjectAt(initialTouchPosition);
		}

		override protected void UpdateGesture(object state)
		{
			var virtualObject = firstTouchedObject;
			if (virtualObject == null)
			{
				return;
			}
			// We know there's a first since the gesture begins with at least one
			InvokeOnMainThread(() =>
			{
				if (this.currentTouches.Count() == 0)
				{
					return;
				}
				latestTouchPosition = ((UITouch)this.currentTouches.First()).LocationInView(this.SceneView);

				if (!translationThresholdPassed)
				{
					var initialLocationToCurrentPosition = latestTouchPosition.Subtract(initialTouchPosition);
					var distanceFromStartLocation = initialLocationToCurrentPosition.Length();
					if (distanceFromStartLocation >= translationThreshold)
					{
						translationThresholdPassed = true;
						var currentObjectLocation = SceneView.ProjectPoint(virtualObject.Position).ToCGPoint();
						dragOffset = latestTouchPosition.Subtract(currentObjectLocation);
					}
				}

				// A single finger drag will occur if the drag started on the object and the threshold has been passed.
				if (translationThresholdPassed)
				{
					var offsetPos = latestTouchPosition.Subtract(dragOffset);
					this.manager.Translate(virtualObject, SceneView, offsetPos, false, true);
					hasMovedObject = true;
					LastUsedObject = virtualObject;
				}
			});
		}

		override protected void FinishGesture()
		{
			// Single finger touch allows teleporting the object or interacting with it.

			// Do not do anything if this gesture is being finished because
			// another finger has started touching the screen.
			if (currentTouches.Count > 1)
			{
				return;
			}

			// Do not do anything either if the touch has dragged the object around.
			if (hasMovedObject)
			{
				return;
			}

			if (LastUsedObject != null)
			{
				// If this gesture hasn't moved the object then perform a hit test against
				// the geometry to check if the user has tapped the object itself.
				// - Note: If the object covers a significant
				// percentage of the screen then we should interpret the tap as repositioning
				// the object.
				var isObjectHit = VirtualObjectAt(latestTouchPosition) != null;

				if (!isObjectHit)
				{
					// Teleport the object to where-ever the user touched the screen - as long as the
					// drag threshold has not been reached.
					if (!translationThresholdPassed)
					{
						manager.Translate(LastUsedObject, SceneView, latestTouchPosition, true, false);
					}
				}
			}
		}
	}
}
