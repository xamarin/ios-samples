using System;
using System.Collections.Generic;
using System.Linq;
using ARKit;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using OpenTK;
using SceneKit;
using UIKit;

namespace PlacingObjects
{
	public class VirtualObjectManager : NSObject
	{
		static VirtualObjectManager()
		{
			var jsonPath = NSBundle.MainBundle.PathForResource("VirtualObjects", "json");
			var jsonData = System.IO.File.ReadAllText(jsonPath);
			AvailableObjects = JsonConvert.DeserializeObject<List<VirtualObjectDefinition>>(jsonData);
		}

		public static List<VirtualObjectDefinition> AvailableObjects { get; protected set; }

		DispatchQueue queue;
		public IVirtualObjectManagerDelegate Delegate { get; set; }
		public List<VirtualObject> VirtualObjects { get; protected set; } = new List<VirtualObject>();

		VirtualObject lastUsedObject = null;
		Gesture currentGesture = null;

		public VirtualObjectManager(DispatchQueue queue)
		{
			this.queue = queue;
		}

		public void Translate(VirtualObject vObject, ARSCNView sceneView, CGPoint screenPos, bool instantly, bool infinitePlane)
		{
			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
				var result = WorldPositionFromScreenPosition(screenPos, sceneView, vObject.Position, infinitePlane);
				var newPosition = result.Item1;
				if (newPosition == null)
				{
					if (this.Delegate != null)
					{
						this.Delegate.CouldNotPlace(this, vObject);
						return;
					}
				}
				var currentFrame = ViewController.CurrentFrame;
				if (currentFrame == null || currentFrame.Camera == null)
				{
					return;
				}
				var cameraTransform = currentFrame.Camera.Transform;
				queue.DispatchAsync(() => SetPosition(vObject, newPosition.Value, instantly, result.Item3, cameraTransform));
			});
		}

		internal void RemoveAllVirtualObjects()
		{
			foreach (var vo in VirtualObjects)
			{
				vo.Unload();
			}
			VirtualObjects.Clear();
		}

		private void SetPosition(VirtualObject virtualObject, SCNVector3 position, bool instantly, bool filterPosition, NMatrix4 cameraTransform)
		{
			if (instantly)
			{
				SetNewVirtualObjectPosition(virtualObject, position, cameraTransform);
			}
			else
			{
				UpdateVirtualObjectPosition(virtualObject, position, filterPosition, cameraTransform);
			}
		}

		public void UpdateVirtualObjectPosition(VirtualObject virtualObject, SCNVector3 position, bool filterPosition, NMatrix4 cameraTransform)
		{
			var cameraWorldPos = cameraTransform.Translation();
			var cameraToPosition = position.Subtract(cameraWorldPos);

			// Limit the distance of the object from the camera to a maximum of 10m
			if (cameraToPosition.LengthFast > 10)
			{
				cameraToPosition = cameraToPosition.Normalized() * 10;
			}

			// Compute the average distance of the object from the camera over the last ten
			// updates. If filterPosition is true, compute a new position for the object
			// with this average. Notice that the distance is applied to the vector from
			// the camera to the content, so it only affects the percieved distance of the
			// object - the averaging does _not_ make the content "lag".
			var hitTestResultDistance = cameraToPosition.LengthFast;

			virtualObject.RecentVirtualObjectDistances.Add(hitTestResultDistance);
			virtualObject.RecentVirtualObjectDistances.KeepLast(10);

			if (filterPosition)
			{
				var averageDistance = virtualObject.RecentVirtualObjectDistances.Average();
				var averagedDistancePos = cameraWorldPos + cameraToPosition.Normalized() * averageDistance;
				virtualObject.Position = averagedDistancePos;
			}
			else
			{
				virtualObject.Position = cameraWorldPos + cameraToPosition;
			}
		}

		private void SetNewVirtualObjectPosition(VirtualObject virtualObject, SCNVector3 position, NMatrix4 cameraTransform)
		{
			var cameraWorldPos = cameraTransform.Translation();
			var cameraToPosition = position.Subtract(cameraWorldPos);

			// Limit the distance of the object from the camera to a maximum of 10m
			if (cameraToPosition.LengthFast > 10)
			{
				cameraToPosition = cameraToPosition.Normalized() * 10;
			}

			virtualObject.Position = cameraWorldPos + cameraToPosition;
			virtualObject.RecentVirtualObjectDistances.Clear();
		}


		public void CheckIfObjectShouldMoveOntoPlane(ARPlaneAnchor anchor, SCNNode planeAnchorNode)
		{
			foreach (var vo in VirtualObjects)
			{
				// Get the object's position in the plane's coordinate system.
				var objectPos = planeAnchorNode.ConvertPositionToNode(vo.Position, vo.ParentNode);

				if (Math.Abs(objectPos.Y) < float.Epsilon)
				{
					return; // The object is already on the plane - nothing to do here.
				}

				// Add 10% tolerance to the corners of the plane.
				var tolerance = 0.1f;

				var minX = anchor.Center.X - anchor.Extent.X / 2f - anchor.Extent.X * tolerance;
				var maxX = anchor.Center.X + anchor.Extent.X / 2f + anchor.Extent.X * tolerance;
				var minZ = anchor.Center.Z - anchor.Extent.Z / 2f - anchor.Extent.Z * tolerance;
				var maxZ = anchor.Center.Z + anchor.Extent.Z / 2f + anchor.Extent.Z * tolerance;

				if (objectPos.X < minX || objectPos.X > maxX || objectPos.Z < minZ || objectPos.Z > maxZ)
				{
					return;
				}

				// Drop the object onto the plane if it is near it.
				var verticalAllowance = 0.05f;
				var epsilon = 0.001; // Do not bother updating if the difference is less than a mm.
				var distanceToPlane = Math.Abs(objectPos.Y);
				if (distanceToPlane > epsilon && distanceToPlane < verticalAllowance)
				{
					Delegate.DidMoveObjectOntoNearbyPlane(this, vo);

					SCNTransaction.Begin();
					SCNTransaction.AnimationDuration = distanceToPlane * 500; // Move 2mm per second
					SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
					vo.Position = new SCNVector3(vo.Position.X, anchor.Transform.M32, vo.Position.Z);
					SCNTransaction.Commit();
				}
			}
		}

		internal void ReactToTouchesCancelled(NSSet touches, UIEvent evt)
		{
			if (VirtualObjects.Count() == 0)
			{
				return;
			}
			currentGesture = currentGesture?.UpdateGestureFromTouches(touches, TouchEventType.TouchCanceled);
		}

		private void MoveIfNecessary(NSSet touches, UIEvent evt, TouchEventType evtType)
		{
			if (VirtualObjects.Count() == 0)
			{
				return;
			}
			currentGesture = currentGesture?.UpdateGestureFromTouches(touches, evtType);
			var newObj = currentGesture?.LastUsedObject;
			if (newObj != null)
			{
				lastUsedObject = newObj;
			}

			var gesture = currentGesture;
			var touchedObj = gesture?.LastUsedObject;
			if (gesture != null && touchedObj != null)
			{
				Delegate?.TransformDidChangeFor(this, touchedObj);
			}

		}

		internal void ReactToTouchesEnded(NSSet touches, UIEvent evt)
		{
			MoveIfNecessary(touches, evt, TouchEventType.TouchEnded);
		}

		internal void ReactToTouchesMoved(NSSet touches, UIEvent evt)
		{
			MoveIfNecessary(touches, evt, TouchEventType.TouchMoved);
		}

		internal (SCNVector3?, ARPlaneAnchor, Boolean) WorldPositionFromScreenPosition(CGPoint position, ARSCNView sceneView, SCNVector3? objectPos, bool infinitePlane = false)
		{
			var dragOnInfinitePlanesEnabled = AppSettings.DragOnInfinitePlanes;

			// -------------------------------------------------------------------------------
			// 1. Always do a hit test against exisiting plane anchors first.
			//    (If any such anchors exist & only within their extents.)
			var planeHitTestResults = sceneView.HitTest(position, ARHitTestResultType.ExistingPlaneUsingExtent);
			var result = planeHitTestResults.FirstOrDefault();
			if (result != null)
			{
				var planeHitTestPosition = result.WorldTransform.Translation();
				var planeAnchor = result.Anchor;
				return (planeHitTestPosition, (ARPlaneAnchor)planeAnchor, true);
			}

			// -------------------------------------------------------------------------------
			// 2. Collect more information about the environment by hit testing against
			//    the feature point cloud, but do not return the result yet.
			SCNVector3? featureHitTestPosition = null;
			var highQualityFeatureHitTestResult = false;

			var highQualityfeatureHitTestResults = sceneView.HitTestWithFeatures(position, 18, 0.2, 2.0);
			if (highQualityfeatureHitTestResults.Count() > 0)
			{
				var highQualityFeatureHit = highQualityfeatureHitTestResults.First();
				featureHitTestPosition = highQualityFeatureHit.Position;
				highQualityFeatureHitTestResult = true;
			}


			// -------------------------------------------------------------------------------
			// 3. If desired or necessary (no good feature hit test result): Hit test
			//    against an infinite, horizontal plane (ignoring the real world).
			if ((infinitePlane && dragOnInfinitePlanesEnabled) || !highQualityFeatureHitTestResult)
			{
				if (objectPos.HasValue)
				{
					var pointOnInfinitePlane = sceneView.HitTestWithInfiniteHorizontalPlane(position, objectPos.Value);
					if (pointOnInfinitePlane != null)
					{
						return (pointOnInfinitePlane, null, true);
					}
				}
			}

			// -------------------------------------------------------------------------------
			// 4. If available, return the result of the hit test against high quality
			//    features if the hit tests against infinite planes were skipped or no
			//    infinite plane was hit.
			if (highQualityFeatureHitTestResult)
			{
				return (featureHitTestPosition, null, false);
			}

			// -------------------------------------------------------------------------------
			// 5. As a last resort, perform a second, unfiltered hit test against features.
			//    If there are no features in the scene, the result returned here will be nil.
			var unfilteredFeatureHitTestResults = sceneView.HitTestWithFeatures(position);
			if (unfilteredFeatureHitTestResults.Count() > 0)
			{
				var unfilteredFeaturesResult = unfilteredFeatureHitTestResults.First();
				return (unfilteredFeaturesResult.Position, null, false);
			}

			return (null, null, false);
		}

		public void ReactToTouchesBegan(NSSet touches, UIEvent evt, ARSCNView scnView)
		{
			if (!VirtualObjects.Any())
			{
				return;
			}
			if (currentGesture == null)
			{
				currentGesture = Gesture.StartGestureFromTouches(touches, scnView, lastUsedObject, this);
			}
			else
			{
				currentGesture = currentGesture.UpdateGestureFromTouches(touches, TouchEventType.TouchBegan);

			}
			if (currentGesture != null && currentGesture.LastUsedObject != null)
			{
				lastUsedObject = currentGesture.LastUsedObject;
			}
		}


		public void LoadVirtualObject(VirtualObject vo, SCNVector3 position, NMatrix4 cameraTransform)
		{
			VirtualObjects.Add(vo);
			if (Delegate != null)
			{
				Delegate.WillLoad(this, vo);
			}
			vo.Load();

			// Immediately place the object in 3D space
			SetNewVirtualObjectPosition(vo, position, cameraTransform);
			lastUsedObject = vo;
			if (Delegate != null)
			{
				Delegate.DidLoad(this, vo);
			}
		}

		public void RemoveVirtualObject(int index)
		{
			if (index < 0 || index >= AvailableObjects.Count())
			{
				return;
			}
			var def = AvailableObjects[index];
			var vo = VirtualObjects.Where(vo2 => vo2.Definition.Equals(def)).FirstOrDefault();
			if (vo == null)
			{
				return;
			}
			UnloadVirtualObject(vo);
			if (index >= VirtualObjects.Count())
			{
				return;
			}
			var pos = VirtualObjects[index];
			VirtualObjects.RemoveAt(index);
		}

		private void UnloadVirtualObject(VirtualObject vo)
		{
			vo.Unload();
			vo.RemoveFromParentNode();
			if (lastUsedObject == vo)
			{
				lastUsedObject = null;
				if (VirtualObjects.Count() > 1)
				{
					lastUsedObject = VirtualObjects[0];
				}
			}
		}
	}

}
