using System;
using System.Linq;
using ARKit;
using CoreGraphics;
using Foundation;
using OpenTK;
using SceneKit;
using static System.Math;

namespace ScanningAndDetecting3DObjects
{
	// An interactive visualization of x/y/z coordinate axes for use in placing the origin/anchor point of a scanned object.
	internal class ObjectOrigin : SCNNode
	{
		internal static readonly NSString PositionChangedNotificationName = new NSString("ObjectOriginPositionChanged");
		internal static readonly NSString MovedOutsideBoxNotificiationName = new NSString("ObjectOriginMovedOutsideBoundingBox");

		private const float axisLength = 1.0F;
		private const float axisThickness = 6.0F; // Axis thickness in percentage of length. 

		private const float axisSizeToObjectSizeRatio = 0.25F;
		private const float minAxisSize = 0.05F;
		private const float maxAxisSize = 0.2F;

		private ObjectOriginAxis xAxis;
		private ObjectOriginAxis yAxis;
		private ObjectOriginAxis zAxis;

		private SCNNode customModel;

		private PlaneDrag? currentAxisDrag;
		private PlaneDrag? currentPlaneDrag;

		private ARSCNView sceneView;

		internal bool PositionHasBeenAdjustedByUser { get; private set; }

		// Variables related to current snapping state
		private bool snappedToSide = false;
		private bool snappedToBottomCenter = false;
		private bool snappedTo90DegreeRotation = false;
		private float totalRotationSinceLastSnap = 0;

		private NSObject[] notificationObserverHandles;

		private bool DisplayingCustom3DModel { get => customModel != null; }

		internal ObjectOrigin(NVector3 extent, ARSCNView sceneView) : base()
		{
			this.sceneView = sceneView;

			var length = axisLength;
			var thickness = (axisLength / 100.0F) * axisThickness;
			var radius = axisThickness / 2;
			var handleSize = axisLength / 4;

			xAxis = new ObjectOriginAxis(Axis.X, length, thickness, radius, handleSize);
			yAxis = new ObjectOriginAxis(Axis.Y, length, thickness, radius, handleSize);
			zAxis = new ObjectOriginAxis(Axis.Z, length, thickness, radius, handleSize);

			AddChildNode(xAxis);
			AddChildNode(yAxis);
			AddChildNode(zAxis);

			Set3DModel(ViewController.Instance?.ModelUrl, extent);

			notificationObserverHandles = new NSObject[2];
			notificationObserverHandles[0] = NSNotificationCenter.DefaultCenter.AddObserver(Scan.ScanningStateChangedNotificationName, ScanningStateChanged);
			notificationObserverHandles[1] = NSNotificationCenter.DefaultCenter.AddObserver(BoundingBox.ExtentChangedNotificationName, BoundingBoxExtentChanged);

			Hidden = true;
		}

		internal void Set3DModel(NSUrl url, NVector3 extent)
		{
			customModel?.RemoveFromParentNode();
			customModel?.Dispose();
			customModel = null;

			SCNNode model = null;
			if (url != null)
			{
				model = Utilities.Load3DModel(url);
			}
			if (model != null)
			{
				ViewController.Instance?.SceneView.Prepare(model, () =>
				{
					AddChildNode(model);
					return true;
				});
				customModel = model;

				xAxis.DisplayNodeHierarchyOnTop(true);
				yAxis.DisplayNodeHierarchyOnTop(true);
				zAxis.DisplayNodeHierarchyOnTop(true);
			}
			else
			{
				xAxis.DisplayNodeHierarchyOnTop(false);
				yAxis.DisplayNodeHierarchyOnTop(false);
				zAxis.DisplayNodeHierarchyOnTop(false);
			}

			AdjustToExtent(extent);
		}

		private void AdjustToExtent(NVector3 extent)
		{
			if (extent == null)
			{
				Scale = new SCNVector3(1, 1, 1);
				xAxis.Scale = new SCNVector3(1, 1, 1);
				yAxis.Scale = new SCNVector3(1, 1, 1);
				zAxis.Scale = new SCNVector3(1, 1, 1);
			}

			// By default the origin's scale is 1x.
			Scale = new SCNVector3(1, 1, 1);

			// Compute a good scale for the axes based on the extent of the bounding box, 
			// but stay within a reasonable range.
			var axesScale = new[] { extent.X, extent.Y, extent.Z }.Min() * axisSizeToObjectSizeRatio;
			axesScale = Max(Min(axesScale, maxAxisSize), minAxisSize);

			// Adjust the scale of the axes (not the origin itself!)
			xAxis.Scale = new SCNVector3(axesScale, axesScale, axesScale);
			yAxis.Scale = new SCNVector3(axesScale, axesScale, axesScale);
			zAxis.Scale = new SCNVector3(axesScale, axesScale, axesScale);

			if (customModel != null)
			{
				// Scale the origin such that the custom 3D model fits into the given extent.
				SCNVector3 center = default(SCNVector3);
				nfloat radius = default(nfloat);
				customModel.GetBoundingSphere(ref center, ref radius);
				var modelExtent = radius * 2;
				var originScale = (float)(new[] { extent.X, extent.Y, extent.Z }.Min() / modelExtent);

				// Scale the origin itself, so that the scale will be preserved in the *.arobject file
				Scale = new SCNVector3(originScale, originScale, originScale);

				// Correct the scale of the axes to be the same size as before 
				var div = (float)(1.0 / originScale);
				// This is probably too fussy, as the scales are (certainly?) even along axes and I think all are equivalent
				xAxis.Scale = new SCNVector3(xAxis.Scale.X * div, xAxis.Scale.Y * div, xAxis.Scale.Z * div);
				yAxis.Scale = new SCNVector3(yAxis.Scale.X * div, yAxis.Scale.Y * div, yAxis.Scale.Z * div);
				zAxis.Scale = new SCNVector3(zAxis.Scale.X * div, zAxis.Scale.Y * div, zAxis.Scale.Z * div);
			}
		}

		internal void UpdateScale(float scale)
		{
			/* 
			 If a 3D model is being displayed, users should be able to change the scale
			 of the origin. This ensures that the scale at which the 3D model is displayed
			 will be preserved in the *.arobject file
			*/
			if (DisplayingCustom3DModel)
			{
				Scale = new SCNVector3(Scale.X * scale, Scale.Y * scale, Scale.Z * scale);

				// Correct the scale of the axes to be displayed at the same size as before.
				var div = (1.0F / scale);
				xAxis.Scale = new SCNVector3(xAxis.Scale.X * div, xAxis.Scale.Y * div, xAxis.Scale.Z * div);
				yAxis.Scale = new SCNVector3(yAxis.Scale.X * div, yAxis.Scale.Y * div, yAxis.Scale.Z * div);
				xAxis.Scale = new SCNVector3(zAxis.Scale.X * div, zAxis.Scale.Y * div, zAxis.Scale.Z * div);
			}

		}

		internal void StartAxisDrag(CGPoint screenPos)
		{
			var camera = sceneView.PointOfView;
			if (camera == null)
			{
				return;
			}

			// Check if the user is starting the drag on one of the axes. If so, drag along that axis
			var options = new SCNHitTestOptions();
			options.RootNode = this;
			options.BoundingBoxOnly = true;
			var hitResults = sceneView.HitTest(screenPos, options);

			foreach (var result in hitResults)
			{
				var hitAxis = result.Node.ParentNode as ObjectOriginAxis;
				if (hitAxis != null)
				{
					hitAxis.Highlighted = true;

					var worldAxis = hitAxis.ConvertVectorFromNode(hitAxis.Axis.Normal().ToSCNVector3(), null);
					var worldPosition = hitAxis.ConvertVectorFromNode(new SCNVector3(), null);
					var hitAxisNormalInWorld = (worldAxis - worldPosition).Normalized();

					var dragRay = new Ray(WorldPosition, hitAxisNormalInWorld);
					var transform = dragRay.DragPlaneTransform(camera.WorldPosition);

					var offset = new SCNVector3();
					var hitPos = sceneView.UnprojectPointLocal(screenPos, transform);
					if (hitPos != null)
					{
						// Project the result onto the plane's X axis & transform into world coordinates
						var posOnPlaneXAxis = new SCNVector4(hitPos.Value.X, 0, 0, 1);
						var worldPosOnPlaneXAxis = transform.ToSCNMatrix4().Times(posOnPlaneXAxis);

						offset = WorldPosition - worldPosOnPlaneXAxis.Xyz;
					}

					currentAxisDrag = new PlaneDrag(transform, offset.ToNVector3());
					PositionHasBeenAdjustedByUser = true;
					return;
				}
			}
		}

		internal void UpdateAxisDrag(CGPoint screenPos)
		{
			if (!currentAxisDrag.HasValue)
			{
				return;
			}
			var drag = currentAxisDrag.Value;

			var hitPos = sceneView.UnprojectPointLocal(screenPos, drag.PlaneTransform);
			if (!hitPos.HasValue)
			{
				return;
			}
			// Project the result onto the plane's X axis & transform into world coordinates
			var posOnPlaneXAxis = new SCNVector4(hitPos.Value.X, 0, 0, 1);
			var worldPosOnPlaneXAxis = drag.PlaneTransform.ToSCNMatrix4().Times(posOnPlaneXAxis);

			WorldPosition = worldPosOnPlaneXAxis.Xyz.Plus(drag.Offset);

			if (customModel == null)
			{
				// Snap origin to any side of the bounding box and to the bottom center.
				SnapToBoundingBoxSide();
			}

			NSNotificationCenter.DefaultCenter.PostNotification(NSNotification.FromName(ObjectOrigin.PositionChangedNotificationName, this));

			if (OutsideBoundingBox())
			{
				NSNotificationCenter.DefaultCenter.PostNotification(NSNotification.FromName(ObjectOrigin.MovedOutsideBoxNotificiationName, this));
			}
		}

		internal void EndAxisDrag()
		{
			currentAxisDrag = null;
			xAxis.Highlighted = false;
			yAxis.Highlighted = false;
			zAxis.Highlighted = false;
		}

		internal void StartPlaneDrag(CGPoint screenPos)
		{
			// Reposition the origin in the XZ-plane.
			var dragPlane = WorldTransform.ToNMatrix4();
			var offset = new SCNVector3();
			var hitPos = sceneView.Unproject(screenPos, dragPlane);

			offset = WorldPosition - hitPos.ToSCNVector3();
			currentPlaneDrag = new PlaneDrag(dragPlane, offset.ToNVector3());
			PositionHasBeenAdjustedByUser = true;
		}

		internal void UpdatePlaneDrag(CGPoint screenPos)
		{
			if (!currentPlaneDrag.HasValue)
			{
				return;
			}
			var drag = currentPlaneDrag.Value;

			var hitPos = sceneView.Unproject(screenPos, drag.PlaneTransform);

			WorldPosition = hitPos.Plus(drag.Offset).ToSCNVector3();

			if (customModel == null)
			{
				SnapToBoundingBoxCenter();
			}

			NSNotificationCenter.DefaultCenter.PostNotificationName(ObjectOrigin.PositionChangedNotificationName, this);

			if (OutsideBoundingBox())
			{
				NSNotificationCenter.DefaultCenter.PostNotificationName(ObjectOrigin.MovedOutsideBoxNotificiationName, this);
			}
		}


		internal void EndPlaneDrag()
		{
			currentPlaneDrag = null;
			snappedToSide = false;
			snappedToBottomCenter = false;
		}

		internal void FlashOrReposition(CGPoint screenPos)
		{
			// Check if the user tapped on one of the axes. If so, highlight it.
			var options = new SCNHitTestOptions();
			options.RootNode = this;
			options.BoundingBoxOnly = true;
			var hitResults = sceneView.HitTest(screenPos, options);

			foreach (var result in hitResults)
			{
				var hitAxis = result.Node.ParentNode as ObjectOriginAxis;
				if (hitAxis != null)
				{
					hitAxis.Flash();
					return;
				}
			}

			// If no axis was hit, reposition the origin in the XZ-plane.
			var hitPos = sceneView.Unproject(screenPos, WorldTransform.ToNMatrix4());
			WorldPosition = hitPos.ToSCNVector3();

			if (OutsideBoundingBox())
			{
				NSNotificationCenter.DefaultCenter.PostNotificationName(ObjectOrigin.MovedOutsideBoxNotificiationName, this);
			}

		}


		internal void RotateWithSnappingOnYAxis(float angle)
		{
			var snapInterval = PI / 2;
			var snapThreshold = PI / 30; // ~6°

			// Compute the snap angle, being the closest multiple of the snap interval.
			var snapAngle = Round(EulerAngles.Y / snapInterval) * snapInterval;

			if (!snappedTo90DegreeRotation)
			{
				// Compute the delta between current angle and computed snap angle.
				var deltaToSnapAngle = Abs(snapAngle - EulerAngles.Y);

				// Snap if the delta is below the snap threshold, otherwise rotate by the angle
				// received from teh gesture
				if (deltaToSnapAngle < snapThreshold)
				{
					LocalRotate(SCNQuaternion.FromAxisAngle(Axis.Y.Normal().ToSCNVector3(), (float)(Sign(angle) * deltaToSnapAngle)));
					snappedTo90DegreeRotation = true;
					totalRotationSinceLastSnap = 0;
					Snapping.PlayHapticFeedback();
				}
				else
				{
					LocalRotate(SCNQuaternion.FromAxisAngle(Axis.Y.Normal().ToSCNVector3(), totalRotationSinceLastSnap));
				}
			}
			else
			{
				totalRotationSinceLastSnap += angle;

				// Unsnap if the total rotation since the snap exceeds the snap threshold.
				if (Abs(totalRotationSinceLastSnap) > snapThreshold)
				{
					LocalRotate(SCNQuaternion.FromAxisAngle(Axis.Y.Normal().ToSCNVector3(), totalRotationSinceLastSnap));
					snappedTo90DegreeRotation = false;
				}
			}
		}

		private bool OutsideBoundingBox()
		{
			var boundingBox = ParentNode as BoundingBox;
			if (boundingBox == null)
			{
				return true;
			}

			var threshold = 0.002F;
			var extent = new SCNVector3(boundingBox.Extent.X + threshold, boundingBox.Extent.Y + threshold, boundingBox.Extent.Z + threshold);

			var pos = Position;
			return pos.X < -extent.X / 2
				|| pos.Y < -extent.Y / 2
				|| pos.Z < -extent.Z / 2
				|| pos.X > extent.X / 2
				|| pos.Y > extent.Y / 2
				|| pos.Z > extent.Z / 2;
		}

		private void SnapToBoundingBoxSide()
		{
			var boundingBox = ParentNode as BoundingBox;
			if (boundingBox == null)
			{
				return;
			}
			var extent = boundingBox.Extent;

			var snapThreshold = 0.01;
			var withinSnapThreshold = false;

			if (Abs(extent.X / 2 - Position.X) < snapThreshold)
			{
				Position = new SCNVector3(extent.X / 2, Position.Y, Position.Z);
				withinSnapThreshold = true;
			}
			else if (Abs(-extent.X / 2 - Position.X) < snapThreshold)
			{
				Position = new SCNVector3(-extent.X / 2, Position.Y, Position.Z);
				withinSnapThreshold = true;
			}

			if (Abs(extent.Y / 2 - Position.Y) < snapThreshold)
			{
				Position = new SCNVector3(Position.X, extent.Y / 2, Position.Z);
				withinSnapThreshold = true;
			}
			else if (Abs(-extent.Y / 2 - Position.Y) < snapThreshold)
			{
				Position = new SCNVector3(Position.X, -extent.Y / 2, Position.Z);
				withinSnapThreshold = true;
			}

			if (Abs(extent.Z / 2 - Position.Z) < snapThreshold)
			{
				Position = new SCNVector3(Position.X, Position.Y, extent.Z / 2);
				withinSnapThreshold = true;
			}
			else if (Abs(-extent.Z / 2 - Position.Z) < snapThreshold)
			{
				Position = new SCNVector3(Position.X, Position.Y, -extent.Z / 2);
				withinSnapThreshold = true;
			}

			// Provide haptic feedback when reaching the snapThreshold for the first time
			if (withinSnapThreshold && !snappedToSide)
			{
				snappedToSide = true;
				Snapping.PlayHapticFeedback();
			}
			else if (!withinSnapThreshold)
			{
				snappedToSide = false;
			}
		}


		private void SnapToBoundingBoxCenter()
		{
			var boundingBox = ParentNode as BoundingBox;
			if (boundingBox == null)
			{
				return;
			}

			var snapThreshold = 0.01F;
			var boundingBoxPos = boundingBox.Position;

			var withinSnapThreshold = false;

			if (Abs(boundingBox.Position.X - Position.X) < snapThreshold
			   && Abs(boundingBox.Position.Z - Position.Z) < snapThreshold)
			{
				Position = new SCNVector3(boundingBoxPos.X, Position.Y, boundingBoxPos.Z);
				withinSnapThreshold = true;
			}

			// Provide haptic feedback when reaching the snapThreshold for the first time
			if (withinSnapThreshold && !snappedToBottomCenter)
			{
				snappedToBottomCenter = true;
				Snapping.PlayHapticFeedback();
			}
			else
			{
				snappedToBottomCenter = false;
			}
		}


		private void BoundingBoxExtentChanged(NSNotification notification)
		{
			var boundingBox = notification.Object as BoundingBox;
			if (boundingBox == null)
			{
				return;
			}
			AdjustToExtent(boundingBox.Extent);
		}

		private void ScanningStateChanged(NSNotification notification)
		{
			var state = notification.UserInfo?[Scan.StateUserInfoKey] as SimpleBox<Scan.ScanState>;
			if (state == null)
			{
				return;
			}

			switch (state.Value)
			{
				case Scan.ScanState.Ready:
				case Scan.ScanState.DefineBoundingBox:
				case Scan.ScanState.Scanning:
					Hidden = true;
					break;
				case Scan.ScanState.AdjustingOrigin:
					Hidden = false;
					break;
			}
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

				NSNotificationCenter.DefaultCenter.RemoveObserver(notificationObserverHandles[0]);
				NSNotificationCenter.DefaultCenter.RemoveObserver(notificationObserverHandles[1]);
			}
		}
	}
}