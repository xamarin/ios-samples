using SceneKit;
using ARKit;
using System;
using OpenTK;
using System.Collections.Generic;
using Foundation;
using System.Linq;
using UIKit;

namespace ScanningAndDetecting3DObjects
{
	//A visualization of a 3D point cloud during object scanning
	internal class ScannedPointCloud : SCNNode
	{
		private readonly SCNNode pointNode = new SCNNode();

		//The latest known set of points inside the reference object.
		private List<SCNVector3> referenceObjectPoints = new List<SCNVector3>();

		// The set of currently rendered points, in world coordinates,
		// Note: We render them in world coordinates instead of local coordinates to
		// prevent rendering issues with points jittering e.g. when the bounding box
		// is rotated.
		private List<SCNVector3> renderedPoints = new List<SCNVector3>();

		private BoundingBox boundingBox;

		private NSObject[] notificationObservationHandles; 

		internal int Count { get => renderedPoints.Count; }

		internal ScannedPointCloud()
		{
			base.Init();

			AddChildNode(pointNode);

			notificationObservationHandles = new NSObject[4];

			notificationObservationHandles[0] = NSNotificationCenter.DefaultCenter.AddObserver(Scan.ScanningStateChangedNotificationName, ScanningStateChanged);
			notificationObservationHandles[1] = NSNotificationCenter.DefaultCenter.AddObserver(BoundingBox.ExtentChangedNotificationName, BoundingBoxPositionOrExtentChanged);
			notificationObservationHandles[2] = NSNotificationCenter.DefaultCenter.AddObserver(BoundingBox.PositionChangedNotificationName, BoundingBoxPositionOrExtentChanged);
			notificationObservationHandles[3] = NSNotificationCenter.DefaultCenter.AddObserver(ScannedObject.PositionChangedNotificationName, ScannedObjectPositionChanged);
		}


		private void BoundingBoxPositionOrExtentChanged(NSNotification notification)
		{
			var boundingBox = notification.Object as BoundingBox;
			if (boundingBox == null || boundingBox.IsDisposed)
			{
				return;
			}
			UpdateBoundingBox(boundingBox);
		}

		private void ScannedObjectPositionChanged(NSNotification notification)
		{
			var scannedObject = notification.Object as ScannedObject;
			if (scannedObject == null)
			{
				return;
			}
			var boundingBox = scannedObject.BoundingBox != null ? scannedObject.BoundingBox : scannedObject.GhostBoundingBox;
			if (!boundingBox.IsDisposed)
			{
				UpdateBoundingBox(boundingBox);
			}
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
				case Scan.ScanState.Scanning:
				case Scan.ScanState.DefineBoundingBox:
					Hidden = false;
					break;
				case Scan.ScanState.AdjustingOrigin:
					Hidden = true;
					break;
			}
		}

		private void UpdateBoundingBox(BoundingBox boundingBox) => this.boundingBox = boundingBox;

		internal void Update(ARPointCloud pointCloud, BoundingBox boundingBox)
		{
			// Convert the points to world coordinates because we display them
			// in world coordinates.
			var pointsInWorld = pointCloud.Points.Select(pt => boundingBox.ConvertPositionToNode(pt.ToSCNVector3(), null));
			referenceObjectPoints = pointsInWorld.ToList();
		}

		internal void UpdateOnEveryFrame()
		{
			if (Hidden)
			{
				return;
			}

			if (referenceObjectPoints.Count == 0 || boundingBox == null || boundingBox.IsDisposed)
			{
				pointNode.Geometry?.Dispose();
				pointNode.Geometry = null;
				return;
			}

			renderedPoints = new List<SCNVector3>();

			var min = -boundingBox.Extent.ToSCNVector3() / 2;
			var max = boundingBox.Extent.ToSCNVector3() / 2;

			// Abort if the bounding box has no extent yet
			if (max.X <= float.Epsilon)
			{
				return;
			}

			// Check which of the reference object's points are still within the bounding box. 
			// Note: The creation of the latest ARReferenceObject happens at a lower frequency
			// than rendering and updates of the bounding box, so some of the points may no longer
			// be in the inside of the box. 
			foreach (var point in referenceObjectPoints)
			{
				//See where point lies, relative to the bounding box
				var localPoint = boundingBox.ConvertPositionFromNode(point, null);

				if (localPoint.X >= min.X && localPoint.X <= max.X
					&& localPoint.Y >= min.Y && localPoint.Y <= max.Y
					&& localPoint.Z >= min.Z && localPoint.Z <= max.Z)
				{
					// Add the point (not the localPoint!) to the visualization
					renderedPoints.Add(point);
				}

			}
			pointNode.Geometry?.Dispose();
			pointNode.Geometry = PointCloud.CreateVisualization(renderedPoints.Select(p => p.ToNVector3()).ToArray(), Utilities.AppYellow, 12);
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

				foreach (var notificationObserverHandle in notificationObservationHandles)
				{
					NSNotificationCenter.DefaultCenter.RemoveObserver(notificationObserverHandle);
				}
			}
		}
	}
}