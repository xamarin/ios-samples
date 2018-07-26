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
	public abstract class Gesture : NSObject
	{
		public VirtualObject LastUsedObject { get; protected set; }

		protected System.Threading.Timer refreshTimer;
		protected VirtualObjectManager manager;
		protected ARSCNView SceneView { get; set; }
		protected NSSet currentTouches;

		public Gesture(NSSet currentTouches, ARSCNView sceneView, VirtualObject lastUsedObject, VirtualObjectManager manager)
		{
			this.currentTouches = currentTouches;
			this.SceneView = sceneView;
			this.LastUsedObject = lastUsedObject;
			this.manager = manager;

			// Refresh the current gesture at 60 Hz - This ensures smooth updates even when no
			// new touch events are incoming (but the camera might have moved).
			this.refreshTimer = new System.Threading.Timer(CheckedUpdateGesture, null, 0, 17);
		}

		// Hit tests against the `sceneView` to find an object at the provided point.
		protected VirtualObject VirtualObjectAt(CGPoint loc)
		{
			var hitTestOptions = new SCNHitTestOptions { BoundingBoxOnly = true };
			var hitTestResults = SceneView.HitTest(loc, hitTestOptions);
			if (hitTestResults.Count() == 0)
			{
				return null;
			}
			var firstNodeHit = hitTestResults.First((result) => VirtualObject.IsNodePartOfVirtualObject(result.Node));
			var voNode = VirtualObject.ForChildNode(firstNodeHit.Node);
			return voNode;
		}

		virtual public Gesture UpdateGestureFromTouches(NSSet touches, TouchEventType touchType)
		{
			if (touches.Count() == 0)
			{
				// No touches, do nothing
				return null;
			}

			// Update the set of current touches
			switch (touchType)
			{
				case TouchEventType.TouchBegan:
				case TouchEventType.TouchMoved:
					var touchesUnion = currentTouches.Union(touches);
					var touchesArray = new UITouch[touchesUnion.Count()];
					int i = 0;
					foreach(UITouch touch in touchesUnion)
					{
						touchesArray[i] = touch;
						i++;
					}
					currentTouches = new NSSet(touchesArray);
					break;
				case TouchEventType.TouchCanceled:
				case TouchEventType.TouchEnded:
					var notInTouches = NSPredicate.FromExpression(new NSPredicateEvaluator((evaluatedObject, bindings) => !touches.Contains(evaluatedObject)));
					currentTouches = currentTouches.FilterUsingPredicate(notInTouches);
					break;
			}


			var expectedTouchCount = (this is SingleFingerGesture) ? 1 : 2;
			if (currentTouches.Count() == expectedTouchCount)
			{
				this.UpdateGesture(null);
				return this;
			}
			else
			{
				this.FinishGesture();
				this.refreshTimer?.Dispose();
				// Erase reference to timer in case callback fires one last time
				this.refreshTimer = null;

				// Switch to two-finger gesture if was single-finger. If was two-finger, return null
				return (this is SingleFingerGesture)
					? Gesture.StartGestureFromTouches(currentTouches, this.SceneView, LastUsedObject, manager)
					 : null;
			}

		}

		protected void CheckedUpdateGesture(object state)
		{
			// Confirm refreshTimer has not been disposed and null'ed out
			if (this.refreshTimer != null)
			{
				UpdateGesture(state);
			}
		}

		abstract protected void UpdateGesture(object state);
		abstract protected void FinishGesture();

		// Static factory method
		public static Gesture StartGestureFromTouches(NSSet currentTouches, ARSCNView sceneView, VirtualObject lastUsedObject, VirtualObjectManager manager)
		{
			switch (currentTouches.Count)
			{
				case 1: return new SingleFingerGesture(currentTouches, sceneView, lastUsedObject, manager);
				case 2: return new TwoFingerGesture(currentTouches, sceneView, lastUsedObject, manager);
				default:
					return null;
			}
		}
	}
}
