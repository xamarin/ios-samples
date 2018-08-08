using ARKit;
using CoreGraphics;
using Foundation;
using SceneKit;
using System;
using OpenTK;

namespace ScanningAndDetecting3DObjects
{
	internal class ScannedObject : SCNNode
	{
		private ARSCNView sceneView;

		internal static readonly NSString PositionChangedNotificationName = new NSString("ScannedObjectPositionChanged");
		internal static readonly NSString BoundingBoxCreatedNotificationName = new NSString("BoundingBoxWasCreated");
		internal static readonly NSString GhostBoundingBoxCreatedNotificationName = new NSString("GhostBoundingBoxWasCreated");
		internal static readonly NSString GhostBoundingBoxRemovedNotificationName = new NSString("GhostBoundingBoxWasRemoved");

		private NSObject notificationObserverHandle;

		internal ScannedObject(ARSCNView sceneView)
		{
			this.sceneView = sceneView;

			ScanName = $"Scan_{DateTime.Now.ToString("s").Replace('-', '_').Replace(':', '_')}";
			base.Init();

			Console.WriteLine("Creating a new ScannedObject");
			notificationObserverHandle = NSNotificationCenter.DefaultCenter.AddObserver(Scan.ScanningStateChangedNotificationName, ScanningStateChanged);
		}


		private BoundingBox boundingBox;
		private BoundingBox ghostBoundingBox;

		internal ObjectOrigin Origin { get; private set; }
		internal BoundingBox EitherBoundingBox { get => boundingBox ?? GhostBoundingBox; }
		internal BoundingBox BoundingBox { get => boundingBox; private set => boundingBox = value; }
		internal BoundingBox GhostBoundingBox { get => ghostBoundingBox; private set => ghostBoundingBox = value; }
		internal string ScanName { get; private set; }

		public override SCNVector3 Position
		{
			get => base.Position;
			set
			{
				base.Position = value;
				NSNotificationCenter.DefaultCenter.PostNotificationName(PositionChangedNotificationName, this);
			}
		}

		internal void RotateOnYAxis(float angle)
		{
			LocalRotate(SCNQuaternion.FromAxisAngle(Axis.Y.Normal().ToSCNVector3(), angle));
			BoundingBox.HasBeenAdjustedByUser = true;
		}

		internal void Set3DModel(NSUrl url)
		{
			Origin?.Set3DModel(url, boundingBox.Extent);
		}

		internal void CreateOrMoveBoundingBox(CGPoint screenPos)
		{
			if (boundingBox != null)
			{
				if (!boundingBox.IsHit(screenPos))
				{
					// Perform a hit test against the feature point cloud.
					var result = sceneView.SmartHitTest(screenPos);
					if (result == null)
					{
						Console.WriteLine("Warning: Failed to find a position for the bounding box.");
						return;
					}
					WorldPosition = result.WorldTransform.Position();
				}
			}
			else
			{
				CreateBoundingBox(screenPos);
			}
		}


		private void CreateBoundingBoxFromGhost()
		{
			if (ghostBoundingBox == null)
			{
				return;
			}
			boundingBox = ghostBoundingBox;
			boundingBox.Opacity = 1.0F;
			if (Constraints != null)
			{
				foreach (var constraint in Constraints)
				{
					constraint.Dispose();
				}
			}
			Constraints = null;
			// No call to Dispose() because `ghostBoundingBox` object reference is now held by `boundingBox`. 
			ghostBoundingBox = null;

			Origin = new ObjectOrigin(boundingBox.Extent, sceneView);
			boundingBox.AddChildNode(Origin);

			NSNotificationCenter.DefaultCenter.PostNotificationName(ScannedObject.BoundingBoxCreatedNotificationName, null);
		}

		internal void FitOverPointCloud(ARPointCloud pointCloud)
		{
			// Do the automatic adjustment of the bounding box only if the user
			// hasn't adjusted it yet.
			if (boundingBox == null || boundingBox.HasBeenAdjustedByUser)
			{
				return;
			}

			var hitTestResults = sceneView.HitTest(ViewController.Instance.ScreenCenter, ARHitTestResultType.FeaturePoint);
			if (hitTestResults == null || hitTestResults.Length == 0)
			{
				return;
			}

			var userFocusPoint = hitTestResults[0].WorldTransform.Position();
			var nUserFocusPoint = new NVector3(userFocusPoint.X, userFocusPoint.Y, userFocusPoint.Z);
			boundingBox.FitOverPointCloud(pointCloud, nUserFocusPoint);
		}

		internal void TryToAlignWithPlanes(ARPlaneAnchor[] anchors)
		{
			if (boundingBox != null)
			{
				boundingBox.TryToAlignWithPlanes(anchors);
			}
		}

		private void CreateBoundingBox(CGPoint screenPos)
		{
			// Perform a hit test against the feature point cloud. 
			var result = sceneView.SmartHitTest(screenPos);
			if (result == null)
			{
				Console.WriteLine("Warning: failed to find a position for the bounding box.");
				return;
			}

			if (boundingBox != null)
			{
				boundingBox.Dispose();
			}
			boundingBox = new BoundingBox(sceneView);
			Console.WriteLine($"boundingBox newed {boundingBox}");
			AddChildNode(boundingBox);

			// Set the initial extent of the bounding box based on the direction of the camera. 
			var newExtent = (float)result.Distance / 3;
			boundingBox.Extent = new NVector3(newExtent, newExtent, newExtent);

			// Set the position of scanned object to a point on the ray which is offset
			// from the hit test result by half of the bounding box's extent. 
			var cameraToHit = result.WorldTransform.Position() - sceneView.PointOfView.WorldPosition;
			var normalizedDirection = cameraToHit.Normalized();
			var boundingBoxOffset = normalizedDirection * (float)newExtent / 2;
			WorldPosition = result.WorldTransform.Position() + boundingBoxOffset;

			Origin = new ObjectOrigin(boundingBox.Extent, sceneView);
			boundingBox.AddChildNode(Origin);

			NSNotificationCenter.DefaultCenter.PostNotificationName(ScannedObject.BoundingBoxCreatedNotificationName, null);
		}

		private void UpdateOrCreateGhostBoundingBox()
		{
			// Perform a hit test against the feature point cloud. 
			var result = sceneView.SmartHitTest(ViewController.Instance.ScreenCenter);
			if (result == null)
			{
				// No feature points in range (20cm to 3m), so remove ghostBoundingBox from display, if it exists at all
				// unless the `Scan` object has passed a reference to the ghostBoundingBox in to the asynch function `CreateReferenceObject`
				if (ghostBoundingBox != null && !busyCreatingReferenceObject)
				{
					ghostBoundingBox.RemoveFromParentNode();
					Console.WriteLine($"Disposing ghostBoundingBox because there are no feature points");
					ghostBoundingBox.Dispose();
					ghostBoundingBox = null;
					NSNotificationCenter.DefaultCenter.PostNotificationName(ScannedObject.GhostBoundingBoxRemovedNotificationName, null);
				}
				return;
			}

			//There are some feature points in range

			var newExtent = (float)result.Distance / 3;

			// Set the position of scanned object to a point on the ray which is offset
			// from the hit test result by half the bounding box's extent.
			var cameraToHit = result.WorldTransform.Position() - sceneView.PointOfView.WorldPosition;
			var normalizedDirection = cameraToHit.Normalized();
			var boundingBoxOffset = normalizedDirection * newExtent / 2;
			WorldPosition = result.WorldTransform.Position() + boundingBoxOffset;

			if (ghostBoundingBox != null)
			{
				// Update the ghostBoundingBox's Extent and rotate it so that it faces the user
				Console.WriteLine("Updating ghostBoundingBox Extent");
				ghostBoundingBox.Extent = new NVector3(newExtent, newExtent, newExtent);
				// Change the orientation of the bounding box to always face the user. 
				var currentFrame = sceneView.Session.CurrentFrame;
				if (currentFrame != null)
				{
					var cameraY = currentFrame.Camera.EulerAngles.Y;
					Rotation = new SCNVector4(0, 1, 0, cameraY);
				}
			}
			else
			{
				// Create a new ghostBoundingBox
				ghostBoundingBox = new BoundingBox(sceneView);
				Console.WriteLine($"ghostBoundingBox alloced {ghostBoundingBox}");
				ghostBoundingBox.Opacity = 0.25F;
				AddChildNode(ghostBoundingBox);
				ghostBoundingBox.Extent = new NVector3(newExtent, newExtent, newExtent);
				NSNotificationCenter.DefaultCenter.PostNotificationName(ScannedObject.GhostBoundingBoxCreatedNotificationName, null);
			}
		}

		void MoveOriginToBottomOfBoundingBox()
		{
			// Only move the origin to the bottom of the bounding box if it hasn't been
			// repositioned by the user yet.
			if (boundingBox == null || Origin == null || Origin.PositionHasBeenAdjustedByUser)
			{
				return;
			}

			Origin.Position = new SCNVector3(Position.X, -boundingBox.Extent.Y / 2, Position.Z);
		}

		void UpdatePosition(SCNVector3 worldPos)
		{
			var offset = worldPos - WorldPosition;
			WorldPosition = worldPos;

			if (boundingBox != null)
			{
				// Adjust the position of the bounding box to compensate for the 
				// move, so that the bounding box stays where it was.
				boundingBox.WorldPosition -= offset;
			}
		}

		internal void UpdateOnEveryFrame()
		{
			if (boundingBox != null)
			{
				boundingBox.UpdateOnEveryFrame();

				// Note: SCNVector3 supports value equality & floating-point comparison, so the following is okay.
				if (boundingBox.Position != new SCNVector3(0, 0, 0))
				{
					// Make sure the position of the ScannedObject and its nested 
					// BoundingBox is always identical.
					UpdatePosition(boundingBox.WorldPosition);
				}
			}
			else
			{
				UpdateOrCreateGhostBoundingBox();
			}
		}

		internal void ScaleBoundingBox(float scale)
		{
			if (boundingBox == null)
			{
				return;
			}

			var oldYExtent = boundingBox.Extent.Y;

			boundingBox.Extent = boundingBox.Extent.Times(scale);
			boundingBox.HasBeenAdjustedByUser = true;

			// Correct y position so that the floor of the box remains at the same position. 
			var diffOnY = oldYExtent - boundingBox.Extent.Y;
			var bbwp = boundingBox.WorldPosition;
			boundingBox.WorldPosition = new SCNVector3(bbwp.X, bbwp.Y - diffOnY / 2, bbwp.Z);
		}




		private void ScanningStateChanged(NSNotification notification)
		{
			var state = notification.UserInfo[Scan.StateUserInfoKey] as SimpleBox<Scan.ScanState>;
			if (state == null)
			{
				return;
			}
			switch (state.Value)
			{
				case Scan.ScanState.Ready:
					boundingBox?.RemoveFromParentNode();
					boundingBox?.Dispose();
					boundingBox = null;
					break;
				case Scan.ScanState.DefineBoundingBox:
					if (boundingBox == null)
					{
						CreateBoundingBoxFromGhost();
					}
					ghostBoundingBox?.RemoveFromParentNode();
					Console.WriteLine("Disposing ghostBoundingBox because I am defining the real bounding box");
					ghostBoundingBox?.Dispose();
					ghostBoundingBox = null;
					break;
				case Scan.ScanState.Scanning:
					break;
				case Scan.ScanState.AdjustingOrigin:
					MoveOriginToBottomOfBoundingBox();
					break;
			}
		}

#region Microsoft additions

		bool busyCreatingReferenceObject = false;

		// While asynchronous `SceneView.CreateSynchronousObject` is running, do not change refs or `Dispose()` bounding boxes
		// (See README.MD "Avoiding ghostBoundingBox Dispose() asynch defect"
		internal void LockBoundingBoxForReferenceObjectCreation(bool isBusyCreatingReferenceObject)
		{
			this.busyCreatingReferenceObject = isBusyCreatingReferenceObject;
		}

		internal new void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private bool disposed = false;
		override protected void Dispose(bool disposing)
		{
			if (!disposed)
			{
				disposed = true;
				if (disposing)
				{
					base.Dispose();
				}

				NSNotificationCenter.DefaultCenter.RemoveObserver(notificationObserverHandle);
				Origin?.Dispose();
				BoundingBox?.Dispose();
				GhostBoundingBox?.Dispose();

			}
		}

#endregion
	}
}