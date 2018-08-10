using System;
using CoreGraphics;
using SceneKit;
using OpenTK;
using Foundation;
using System.Collections.Generic;
using UIKit;
using ARKit;
using System.Linq;

namespace ScanningAndDetecting3DObjects
{
	internal class BoundingBox : SCNNode
	{
		internal static readonly NSString ScanPercentageUserKey = new NSString("ScanPercentage"); //typeof(value) == NSNumber wrapping a double in range [0. ... 100.]
		internal static readonly NSString BoxExtentUserInfoKey = new NSString("BoxExtent"); //typeof(value) = SimpleBox<NVector3> 
		internal static readonly NSString ScanPercentageChangedNotificationName = new NSString("ScanPercentageChanged");
		internal static readonly NSString ExtentChangedNotificationName = new NSString("BoundingBoxExtentChanged");
		internal static readonly NSString PositionChangedNotificationName = new NSString("BoundingBoxPositionChanged");

		private double minSize = 0.1;

		internal bool HasBeenAdjustedByUser { get; set; }
		private double maxDistanceToFocusPoint = 0.05;


		private NVector3 extent = new NVector3(0.1F, 0.1F, 0.1F);
		internal NVector3 Extent
		{
			get => extent;
			set
			{
				extent = Max(value, minSize);
				UpdateVisualization();
				var notification = NSNotification.FromName(ExtentChangedNotificationName, this, NSDictionary.FromObjectAndKey(new SimpleBox<NVector3>(extent), BoxExtentUserInfoKey));
				NSNotificationCenter.DefaultCenter.PostNotification(notification);
			}
		}

		public override SCNVector3 Position
		{
			get => base.Position;
			set
			{
				if (value.Distance(base.Position) > 0.001)
				{
					var notification = NSNotification.FromName(PositionChangedNotificationName, this);
					NSNotificationCenter.DefaultCenter.PostNotification(notification);
				}
				base.Position = value;
			}
		}

		private NVector3 Max(NVector3 value, double min)
		{
			return new NVector3((float)Math.Max(value.X, min), (float)Math.Max(value.Y, min), (float)Math.Max(value.Z, min));
		}

		struct SideDrag
		{
			internal NMatrix4 PlaneTransform;
			internal BoundingBoxSide Side;
			internal SCNVector3 BeginWorldPos;
			internal NVector3 BeginExtent;

			internal SideDrag(BoundingBoxSide side, NMatrix4 transform, SCNVector3 worldPosition, NVector3 extent) : this()
			{
				this.Side = side;
				this.PlaneTransform = transform;
				this.BeginWorldPos = worldPosition;
				this.BeginExtent = extent;
			}
		}

		private SideDrag? currentSideDrag;
		private PlaneDrag? currentSidePlaneDrag;
		private PlaneDrag? currentGroundPlaneDrag;

		private Wireframe wireframe;

		private SCNNode sidesNode = new SCNNode();
		private Dictionary<BoundingBoxSide.PositionName, BoundingBoxSide> sides = new Dictionary<BoundingBoxSide.PositionName, BoundingBoxSide>();

		private UIColor color = Utilities.AppYellow;

		private List<(Ray, NVector3)> cameraRaysAndHitLocations = new List<(Ray, NVector3)>();

		private int frameCounter = 0;

		internal int ProgressPercentage { get; private set; }

		private bool updatingCaptureProcess = false;

		private ARSCNView sceneView;

		private bool snappedToHorizontalPlane = false;

		private NSObject notificationObserverHandle;

		internal BoundingBox(ARSCNView sceneView)
		{
			this.sceneView = sceneView;
			base.Init();

			notificationObserverHandle = NSNotificationCenter.DefaultCenter.AddObserver(Scan.ScanningStateChangedNotificationName, ScanningStateChanged);
			UpdateVisualization();
		}

		void ScanningStateChanged(NSNotification notification)
		{
			var scanState = notification.UserInfo[Scan.StateUserInfoKey] as SimpleBox<Scan.ScanState>;
			if (scanState == null)
			{
				return;
			}
			switch (scanState.Value)
			{
				case Scan.ScanState.Ready:
				case Scan.ScanState.DefineBoundingBox:
					ResetCapturingProgress();
					foreach (var s in sides.Values)
					{
						s.Hidden = false;
					}
					break;
				case Scan.ScanState.Scanning:
					foreach (var s in sides.Values)
					{
						s.Hidden = false;
					}
					break;
				case Scan.ScanState.AdjustingOrigin:
					// Hide the sides while adjusting the origin
					foreach (var s in sides.Values)
					{
						s.Hidden = true;
					}
					break;
			}
		}

		//Performance Note: O(pointCould.Points.Length^2)
		internal void FitOverPointCloud(ARPointCloud pointCloud, NVector3 focusPoint)
		{
			var filteredPoints = new List<NVector3>();

			foreach (var point in pointCloud.Points)
			{
				// Skip this point if it is more than maxDistanceToFocusPoint meters away from the focus point.
				var distanceToFocusPoint = point.Minus(focusPoint).Length();
				if (distanceToFocusPoint > maxDistanceToFocusPoint)
				{
					continue;
				}

				// Skip this point if it is an outlier (not at least 3 other points closer than 3 cm)
				var nearbyPoints = 0;
				foreach (var otherPoint in pointCloud.Points)
				{
					if (otherPoint != point && point.Distance(otherPoint) < 0.03)
					{
						++nearbyPoints;
						if (nearbyPoints >= 3)
						{
							filteredPoints.Append(point);
							break;
						}
					}
				}
			}

			if (filteredPoints.Count == 0)
			{
				return;
			}

			var localMin = extent.Times(-0.5f);
			var localMax = extent.Times(0.5f);

			foreach (var point in filteredPoints)
			{
				// The bounding box is in local coordinates, so convert to local, too.
				var localPointScnVector3 = ConvertPositionFromNode(new SCNVector3(point.X, point.Y, point.Z), null);
				var localPoint = new NVector3(localPointScnVector3.X, localPointScnVector3.Y, localPointScnVector3.Z);
				localMin = localMin.Min(localPoint);
				localMax = localMax.Max(localPoint);
			}

			// Update the position & extent of the bounding box based on the new min and max values
			Position = Position.Plus(localMin.Plus(localMax).Times(0.5f));
			Extent = localMax.Minus(localMin);
		}

		private void UpdateVisualization()
		{
			UpdateSides();
			UpdateWireframe();
		}

		private void UpdateSides()
		{
			// When this method is called the first time, create the sides and them to the sideNodes
			if (sides.Count != 6)
			{
				CreateSides();
				AddChildNode(sidesNode);
				return;
			}
			else
			{
				// Otherwise just update the geometry's size and position
				foreach (var s in sides.Values)
				{
					s.UpdateBoundingBoxExtent(Extent);
				}
			}
		}

		private void CreateSides()
		{
			var allSides = new[]
			{
				BoundingBoxSide.PositionName.Back, BoundingBoxSide.PositionName.Front,
				BoundingBoxSide.PositionName.Left, BoundingBoxSide.PositionName.Right,
				BoundingBoxSide.PositionName.Bottom, BoundingBoxSide.PositionName.Top
			};
			foreach (var position in allSides)
			{
				sides[position] = new BoundingBoxSide(position, Extent, color);
				sidesNode.AddChildNode(sides[position]);
			}
		}

		private void UpdateWireframe()
		{
			// When this method is called the first time, create the wireframe and add as child node
			if (wireframe == null)
			{
				wireframe = new Wireframe(Extent, color);
				AddChildNode(wireframe);
			}
			else
			{
				wireframe.Update(Extent);
			}
		}

		internal void StartSideDrag(CGPoint screenPos)
		{
			var camera = sceneView.PointOfView;
			if (camera == null)
			{
				return;
			}

			// Check if the user is starting the drag on one of the sides. If so, pull/push that side
			var options = new SCNHitTestOptions();
			options.RootNode = sidesNode;
			options.IgnoreHiddenNodes = false;
			var hitResults = sceneView.HitTest(screenPos, options);

			foreach (var result in hitResults)
			{
				if(result.Node.ParentNode is BoundingBoxSide side)
				{
					side.ShowZAxisExtensions();

					var sideNormalInWorld = ConvertVectorToNode(side.Normal, null).Normalized() - ConvertVectorToNode(SCNVector3.Zero, null);
					var ray = new Ray(result.WorldCoordinates, sideNormalInWorld);
					var transform = ray.DragPlaneTransform(camera.WorldPosition);

					currentSideDrag = new SideDrag(side, transform, WorldPosition, Extent);
					HasBeenAdjustedByUser = true;
					return;
				}
			}
		}

		internal void UpdateSideDrag(CGPoint screenPos)
		{
			if (currentSideDrag.HasValue)
			{
				var drag = currentSideDrag.Value;


				// Compute a new position for this side of the bounding box based on the given screen position.
				SCNVector3? hitPos = sceneView.UnprojectPointLocal(screenPos, drag.PlaneTransform);
				if (hitPos != null)
				{
					var movementAlongRay = hitPos.Value.X;

					// First column of the planeTransform is the ray along which the box 
					// is manipulated, in world coordinates. The center of the bounding box
					// has to be moved by half of the finger's movement on that ray. 

					var originOffset = (drag.PlaneTransform.Column0 * (movementAlongRay / 2)).Xyz;

					var extentOffset = drag.Side.DragAxis.Normal().Times(movementAlongRay);
					var newExtent = drag.BeginExtent.Plus(extentOffset);
					if (newExtent.X >= minSize && newExtent.Y >= minSize && newExtent.Z >= minSize)
					{
						// Push/pull a single side of the bounding box by a combination 
						// of moving & changing the extent of the box
						WorldPosition = drag.BeginWorldPos + originOffset;
						Extent = newExtent;
					}
				}
			}
		}

		internal void EndSideDrag()
		{
			if (currentSideDrag.HasValue)
			{
				currentSideDrag.Value.Side.HideZAxisExtensions();
				currentSideDrag = null;
			}
		}

		BoundingBoxSide AncestralBoundingBox(SCNNode node)
		{
			if (node.ParentNode == null)
			{
				return null;
			}
			if (node.ParentNode is BoundingBoxSide)
			{
				return node.ParentNode as BoundingBoxSide;
			}
			return AncestralBoundingBox(node);
		}

		internal void StartSidePlaneDrag(CGPoint screenPos)
		{
			var camera = sceneView.PointOfView;
			if (camera == null)
			{
				return;
			}

			var options = new SCNHitTestOptions
			{
				RootNode = sidesNode,
				IgnoreChildNodes = false,
				IgnoreHiddenNodes = false
			};

			var hitResults = sceneView.HitTest(screenPos, options);

			foreach (var result in hitResults)
			{
				var side = AncestralBoundingBox(result.Node);
				if (side != null)
				{
					side.ShowYAxisExtensions();
					side.ShowXAxisExtensions();

					var sideNormalInWorld = (ConvertVectorToNode(side.DragAxis.Normal().ToSCNVector3(), null) - ConvertVectorToNode(SCNVector3.Zero, null)).Normalized();
					var planeNormalRay = new Ray(result.WorldCoordinates, sideNormalInWorld);
					var transform = planeNormalRay.DragPlaneTransform(camera);

					var offset = new NVector3();
					NVector3 hitPos = sceneView.Unproject(screenPos, transform);
					offset = WorldPosition.Minus(hitPos);
					currentSidePlaneDrag = new PlaneDrag(transform, offset);
					HasBeenAdjustedByUser = true;
					return;
				}
			}
		}


		internal void UpdateSidePlaneDrag(CGPoint screenPos)
		{
			if (!currentSidePlaneDrag.HasValue)
			{
				return;
			}
			var drag = currentSidePlaneDrag.Value;
			var hitPos = sceneView.Unproject(screenPos, drag.PlaneTransform);
			WorldPosition = hitPos.Plus(drag.Offset).ToSCNVector3();
			SnapToHorizontalPlane();
		}


		internal void EndSidePlaneDrag()
		{
			currentSidePlaneDrag = null;
			HideExtensionsOnAllAxes();

			snappedToHorizontalPlane = false;
		}

		private void HideExtensionsOnAllAxes()
		{
			foreach (var side in sides.Values)
			{
				side.HideXAxisExtensions();
				side.HideYAxisExtensions();
				side.HideZAxisExtensions();
			}
		}

		internal void StartGroundPlaneDrag(CGPoint screenPos)
		{
			var dragPlane = WorldTransform.ToNMatrix4();
			var offset = new NVector3();
			var hitPos = sceneView.Unproject(screenPos, dragPlane);

			offset = WorldPosition.Minus(hitPos);

			currentGroundPlaneDrag = new PlaneDrag(dragPlane, offset);
			HasBeenAdjustedByUser = true;
		}

		internal void UpdateGroundPlaneDrag(CGPoint screenPos)
		{
			sides[BoundingBoxSide.PositionName.Bottom].ShowXAxisExtensions();
			sides[BoundingBoxSide.PositionName.Bottom].ShowYAxisExtensions();

			if (currentGroundPlaneDrag.HasValue)
			{
				var drag = currentGroundPlaneDrag.Value;
				var hitPos = sceneView.Unproject(screenPos, drag.PlaneTransform);

				WorldPosition = hitPos.Plus(drag.Offset).ToSCNVector3();
			}
		}

		internal void EndGroundPlaneDrag()
		{
			currentGroundPlaneDrag = null;
			sides[BoundingBoxSide.PositionName.Bottom].HideXAxisExtensions();
			sides[BoundingBoxSide.PositionName.Bottom].HideYAxisExtensions();
		}

		internal bool IsHit(CGPoint screenPos)
		{
			var options = new SCNHitTestOptions();
			options.RootNode = sidesNode;
			options.IgnoreHiddenNodes = false;

			var hitResults = sceneView.HitTest(screenPos, options);
			return hitResults.Any(r => r.Node.ParentNode is BoundingBoxSide);
		}

		private void ResetCapturingProgress()
		{
			cameraRaysAndHitLocations.Clear();
			foreach (var side in sides.Values)
			{
				foreach (var tile in side.Tiles)
				{
					tile.Captured = false;
					tile.Highlighted = false;
					tile.UpdateVisualization();
				}
			}
		}


		internal void HighlightCurrentTitle()
		{
			var camera = sceneView.PointOfView;
			if (camera != null || Contains(camera.WorldPosition))
			{
				return;
			}

			// Create a new hit test ray. A line segment defined by its start and end point
			// is used to hit test against bounding box tiles. The ray's length allows for 
			// intersectins if the user is no more than five meters away from the bounding box
			var ray = new Ray(camera, 5.0f);

			foreach (var side in sides.Values)
			{
				foreach (var tile in side.Tiles)
				{
					if (tile.Highlighted)
					{
						tile.Highlighted = false;
					}
				}
			}

			var tileAndCoords = TileHitBy(ray);
			if (tileAndCoords.HasValue)
			{
				var hitTile = tileAndCoords.Value.Item1;
				hitTile.Highlighted = true;
			}

			// Update the opacity of all tiles
			foreach (var side in sides.Values)
			{
				foreach (var tile in side.Tiles)
				{
					tile.UpdateVisualization();
				}
			}
		}

		private (Tile, NVector3)? TileHitBy(Ray ray)
		{
			// Perform hit test with given ray
			var options = new SCNHitTestOptions();
			options.IgnoreHiddenNodes = false;
			options.BoundingBoxOnly = true;
			// Immutable `Dictionary` is known defect. Until fix: 
			options.Dictionary[SCNHitTest.OptionSearchModeKey] = new NSNumber((int)SCNHitTestSearchMode.All);

			var hitResults = sceneView.Scene.RootNode.HitTest(ray.Origin, ray.Direction, options);

			// We cannot just look at the first result because we might have hits with other than the tile geometries
			foreach (var result in hitResults)
			{
				if (result.Node is Tile)
				{
					var tile = result.Node as Tile;
					var side = tile.ParentNode as BoundingBoxSide;
					if (side == null || side.IsBusyUpdatingTiles)
					{
						continue;
					}

					// Each ray should only hit one tile, so we can stop iterating through results if a hit was successful.
					return (tile, result.WorldCoordinates.ToNVector3());
				}
			}
			return null;
		}

		internal void UpdateCapturingProcess()
		{
			var camera = sceneView.PointOfView;
			if (camera == null || Contains(camera.WorldPosition))
			{
				return;
			}

			++frameCounter;

			// Add new hit test rays at a lower frame rate to keep the list of previous rays
			// at a reasonable size.
			if (frameCounter % 20 == 0)
			{
				frameCounter = 0;

				// Create a new hit test ray. A line segment defined by its start and end point 
				// is used to hit test against bounding box tiles. The ray's length allows for 
				// intersections if the user is no more than five meters away from the bounding box.
				var currentRay = new Ray(camera, 5.0F);

				// Only remember the ray if it hit the bounding box,
				// and the hit location is significantly different from all previous hit locations.
				var tuple = TileHitBy(currentRay);
				if (tuple.HasValue)
				{
					var hitLocation = tuple.Value.Item2;
					if (HitLocationDifferentFromPreviousRayHitTests(hitLocation))
					{
						cameraRaysAndHitLocations.Add((currentRay, hitLocation));
					}
				}
			}

			// Update tiles at a frame rate that provides a trade-off between responsiveness and performance.
			if (frameCounter % 10 != 0 || updatingCaptureProcess)
			{
				return;
			}

			updatingCaptureProcess = true;

			var capturedTiles = new List<Tile>();

			// Perform hit test with all previous rays. 
			foreach(var hitTest in cameraRaysAndHitLocations)
			{
				var tuple = TileHitBy(hitTest.Item1);
				if (tuple.HasValue)
				{
					var tile = tuple.Value.Item1;
					capturedTiles.Add(tile);
					tile.Captured = true;
				}
			}

			foreach(var side in sides.Values)
			{
				foreach(var tile in side.Tiles)
				{
					if (! capturedTiles.Contains(tile))
					{
						tile.Captured = false;
					}
				}
			}

			// Update the opacity of all tiles
			foreach (var side in sides.Values)
			{
				foreach (var tile in side.Tiles)
				{
					tile.UpdateVisualization();
				}
			}

			// Update scan percentage for all sides, except the bottom
			var sum = 0.0;
			foreach(var (pos, side) in sides)
			{
				if (pos != BoundingBoxSide.PositionName.Bottom)
				{
					sum += side.Completion / 5.0;
				}
			}
			var progressPercentage = (int)Math.Min((int)Math.Floor(sum * 100), 100);
			if (progressPercentage != this.ProgressPercentage)
			{
				this.ProgressPercentage = progressPercentage;
				var notification = NSNotification.FromName(BoundingBox.ScanPercentageChangedNotificationName, this, NSDictionary.FromObjectAndKey(new NSNumber(progressPercentage), BoundingBox.ScanPercentageUserKey));
				NSNotificationCenter.DefaultCenter.PostNotification(notification);
			}

			updatingCaptureProcess = false;
		}

		// Returns true if the given location differs from all hit locations in the cameraRaysAndHitLocation collection
		// by at least the threshold differenec
		private bool HitLocationDifferentFromPreviousRayHitTests(NVector3 location)
		{
			var distThreshold = 0.03;
			// Clone cameraRaysAndHitLocations since Reverse() is in-place
			var reversed = new List<(Ray, NVector3)>(cameraRaysAndHitLocations);
			reversed.Reverse();

			foreach(var hitTest in reversed)
			{
				if (hitTest.Item2.Distance(location) < distThreshold)
				{
					return false;
				}
			}
			return true;
		}

		BoundingBoxSide[] SidesForAxis(Axis axis)
		{
			switch (axis)
			{
				case Axis.X: return new[] { sides[BoundingBoxSide.PositionName.Left], sides[BoundingBoxSide.PositionName.Right] };
				case Axis.Y: return new[] { sides[BoundingBoxSide.PositionName.Top], sides[BoundingBoxSide.PositionName.Bottom] };
				case Axis.Z: return new[] { sides[BoundingBoxSide.PositionName.Front], sides[BoundingBoxSide.PositionName.Back] };
			}
			throw new ArgumentOutOfRangeException("axis");
		}

		internal void UpdateOnEveryFrame()
		{
			var frame = sceneView.Session.CurrentFrame;
			if (frame != null)
			{
				TryToAlignWithPlanes(frame.Anchors);
			}

			foreach(var side in sides.Values)
			{
				side.UpdateVisualizationIfNeeded();
			}
		}

		internal void TryToAlignWithPlanes(ARAnchor[] anchors)
		{
			if (HasBeenAdjustedByUser || ViewController.Instance?.CurrentScan?.State == Scan.ScanState.DefineBoundingBox)
			{
				return;
			}

			var bottomCenter = new SCNVector3(Position.X, Position.Y - Extent.Y / 2, Position.Z);

			var distanceToNearestPlane = float.MaxValue;
			var offsetToNearestPlaneOnY = 0.0F;
			var planeFound = false;

			// Check which plane is nearest to the bounding box
			foreach(var anchor in anchors)
			{
				var plane = anchor as ARPlaneAnchor;
				if (plane == null)
				{
					continue;
				}
				var planeNode = sceneView.GetNode(plane);
				if (planeNode == null)
				{
					continue;
				}

				// Get the position of the bottom center of this bounding box in the plane's coordinate system
				var bottomCenterInPlaneCoords = planeNode.ConvertPositionFromNode(bottomCenter, ParentNode);

				// Add 10% tolerance to the corner of the plane.
				var tolerance = 0.1;
				var minX = plane.Center.X - plane.Extent.X / 2 - plane.Extent.X * tolerance;
				var maxX = plane.Center.X + plane.Extent.X / 2 + plane.Extent.X * tolerance;
				var minZ = plane.Center.Z - plane.Extent.Z / 2 - plane.Extent.Z * tolerance;
				var maxZ = plane.Center.Z + plane.Extent.Z / 2 + plane.Extent.Z * tolerance;

				if (bottomCenterInPlaneCoords.X < minX || bottomCenterInPlaneCoords.X > maxX 
				   || bottomCenterInPlaneCoords.Z < minZ || bottomCenterInPlaneCoords.Z > minZ
				   )
				{
					continue;
				}

				var offsetToPlaneOnY = bottomCenterInPlaneCoords.Y;
				var distanceToPlane = Math.Abs(offsetToPlaneOnY);

				if (distanceToPlane < distanceToNearestPlane)
				{
					distanceToNearestPlane = distanceToPlane;
					offsetToNearestPlaneOnY = offsetToPlaneOnY;
					planeFound = true;
				}
			}

			if ( ! planeFound)
			{
				return;
			}

			// Check that the object is not already on the nearest plane (closer than 1mm). 
			var epsilon = 0.001;
			if (distanceToNearestPlane <= epsilon)
			{
				return;
			}

			// Check if the nearest plane is close enough to the bounding box to "snap" to that 
			// plane. The threshold is half of the bounding box extent on the y axis. 
			var maxDistance = Extent.Y / 2;
			if (distanceToNearestPlane < maxDistance && offsetToNearestPlaneOnY > 0)
			{
				// Adjust the bounding box position & extent such that the bottom of the box 
				// aligns with the plane
				Position = new SCNVector3(Position.X, Position.Y - offsetToNearestPlaneOnY / 2, Position.Z);
				// Note that assigning to Extent triggers additional behavior (see `set`)
				Extent = new NVector3(Extent.X, Extent.Y + offsetToNearestPlaneOnY, Extent.Z);
			}
		}

		private bool Contains(SCNVector3 pointInWorld)
		{
			var localMin = Extent.Times(-0.5F);
			var localMax = Extent.Times(0.5F);

			// The bounding box in local coordinates, so convert point to local, too.
			var localPoint = ConvertPositionFromNode(pointInWorld, null);

			return localMin.X <= localPoint.X
				   && localMax.X >= localPoint.X
				   && localMin.Y <= localPoint.Y
				   && localMax.Y >= localPoint.Y
				   && localMin.Z <= localPoint.Z
				   && localMax.Z >= localPoint.Z;
		}



		private void SnapToHorizontalPlane()
		{
			// Snap to align with horizontal plane if y-position is close enough
			var snapThreshold = 0.01;
			var withinSnapThreshold = false;
			var bottomY = WorldPosition.Y - Extent.Y / 2;

			var currentFrame = ViewController.Instance?.SceneView?.Session?.CurrentFrame;
			if (currentFrame == null)
			{
				return;
			}

			foreach(var anyAnchor in currentFrame.Anchors)
			{
				if (anyAnchor is ARPlaneAnchor)
				{
					var anchor = anyAnchor as ARPlaneAnchor;
					var distanceFromHorizontalPlane = Math.Abs(bottomY - anchor.Transform.Position().Y);

					if (distanceFromHorizontalPlane < snapThreshold)
					{
						withinSnapThreshold = true;
						WorldPosition = new SCNVector3(WorldPosition.X, anchor.Transform.Position().Y + Extent.Y / 2, WorldPosition.Z);

						// Provide haptic feedback when reaching the snapThreshold for the first time
						if (!snappedToHorizontalPlane)
						{
							snappedToHorizontalPlane = true;
							Snapping.PlayHapticFeedback();
						}
					}
				}
			}

			if (! withinSnapThreshold)
			{
				snappedToHorizontalPlane = false;
			}
		}

		// Dispose pattern for debugging purposes
		private bool disposed = false;

		internal bool IsDisposed
		{
			get
			{
				return disposed;
			}
		}

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
}
		}

		internal void Dispose()
		{
			Dispose(true);
		}
	}
}