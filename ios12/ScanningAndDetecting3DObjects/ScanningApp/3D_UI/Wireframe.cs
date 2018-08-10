using System;
using System.Collections.Generic;
using Foundation;
using OpenTK;
using SceneKit;
using UIKit;

namespace ScanningAndDetecting3DObjects
{
	// A visualization of the edges of a 3D box.
	internal class Wireframe : SCNNode
	{
		private double lineThickness = 0.002;
		private List<SCNNode> lineNodes = new List<SCNNode>();
		private UIColor color = Utilities.AppYellow;
		private NSTimer flashTimer;
		private const double flashDuration = 0.1;


		internal Wireframe(NVector3 extent, UIColor color, double scale = 1.0)
		{
			base.Init();

			this.color = color;
			lineThickness *= scale;

			// Repeat 12x
			for (int i = 1; i <= 12; i++)
			{
				var line = new SCNNode();
				lineNodes.Add(line);
				AddChildNode(line);
			}

			Setup(extent);

			NSNotificationCenter.DefaultCenter.AddObserver(ObjectOrigin.MovedOutsideBoxNotificiationName, Flash);
		}


		private void Setup(NVector3 extent)
		{
			// Translate and rotate line nodes to the correct transform
			var halfX = extent.X / 2;
			var halfY = extent.Y / 2;
			var halfZ = extent.Z / 2;

			// Two helper functions to isolate the allocations and casts
			Action<SCNNode,float, float, float> xlat = (node, extentX, extentY, extentZ) => node.LocalTranslate(new SCNVector3(extentX, extentY, extentZ));

			Action<SCNNode, float, float, float, Axis> xlatAndRot = (node, extentX, extentY, extentZ, axis) =>
			{
				xlat(node, extentX, extentY, extentZ);
				node.LocalRotate(SCNQuaternion.FromAxisAngle(axis.Normal().ToSCNVector3(), (float) -Math.PI / 2));
			};

			var halfWidth = extent.X / 2;
			var halfHeight = extent.Y / 2;
			var halfDepth = extent.Z / 2;
			xlatAndRot( lineNodes[0],          0, -halfHeight, -halfDepth, Axis.Z);
			xlatAndRot( lineNodes[1], -halfWidth, -halfHeight,          0, Axis.X);
			xlatAndRot( lineNodes[2],          0, -halfHeight,  halfDepth, Axis.Z);
			xlatAndRot( lineNodes[3],  halfWidth, -halfHeight,          0, Axis.X);
			xlat(       lineNodes[4],  -extent.X,           0, -halfDepth);
			xlat(       lineNodes[5], -halfWidth,           0,  halfDepth);
			xlat(       lineNodes[6],  halfWidth,           0, -halfDepth);
			xlat(       lineNodes[7],  halfWidth,           0,  halfDepth);
			xlatAndRot( lineNodes[8],          0,  halfHeight, -halfDepth, Axis.Z);
			xlatAndRot( lineNodes[9], -halfWidth,  halfHeight,          0, Axis.X);
			xlatAndRot(lineNodes[10],          0,  halfHeight,  halfDepth, Axis.Z);
			xlatAndRot(lineNodes[11],  halfWidth,  halfHeight,          0, Axis.X);

			// Assign geometries
			lineNodes[0].Geometry = Cylinder(extent.X);
			lineNodes[1].Geometry = Cylinder(extent.Z);
			lineNodes[2].Geometry = Cylinder(extent.X);
			lineNodes[3].Geometry = Cylinder(extent.Z);
			lineNodes[4].Geometry = Cylinder(extent.Y);
			lineNodes[5].Geometry = Cylinder(extent.Y);
			lineNodes[6].Geometry = Cylinder(extent.Y);
			lineNodes[7].Geometry = Cylinder(extent.Y);
			lineNodes[8].Geometry = Cylinder(extent.X);
			lineNodes[9].Geometry = Cylinder(extent.Z);
			lineNodes[10].Geometry = Cylinder(extent.X);
			lineNodes[11].Geometry = Cylinder(extent.Z);
		}

		private SCNGeometry Cylinder(float height, float? width = null, UIColor colorArg = null ){
			var radius = width == null ? (float)lineThickness / 2 : width.Value / 2;
			var clr = colorArg ?? this.color;
			var cylinderGeometry = SCNCylinder.Create(radius, height);
			cylinderGeometry.Materials = new[] { Utilities.Material(clr) };
			return cylinderGeometry;
		}

		bool highlighted;
		internal bool Highlighted
		{
			get => highlighted;
			set
			{
				highlighted = value;
				if (highlighted)
				{
					foreach (var child in ChildNodes)
					{
						if (child.Geometry != null)
						{
							child.Geometry.Materials = new[] { Utilities.Material(UIColor.Red) };
						}
					}
				}
				else
				{
					foreach (var child in ChildNodes)
					{
						if (child.Geometry != null)
						{
							child.Geometry.Materials = new[] { Utilities.Material(color) };
						}
					}
				}
			}
		}

		internal void Update(NVector3 extent)
		{
			if (lineNodes.Count != 12)
			{
				return;
			}

			var halfWidth = extent.X / 2;
			var halfHeight = extent.Y / 2;
			var halfDepth = extent.Z / 2;
			// Translate and rotate line nodes to the right transform
			lineNodes[0].Position = new SCNVector3(0, -halfHeight, -halfDepth);
			lineNodes[1].Position = new SCNVector3(-halfWidth, -halfHeight, 0);
			lineNodes[2].Position = new SCNVector3(0, -halfHeight, halfDepth);
			lineNodes[3].Position = new SCNVector3(halfWidth, -halfHeight, 0);
			lineNodes[4].Position = new SCNVector3(-halfWidth, 0, -halfDepth);
			lineNodes[5].Position = new SCNVector3(-halfWidth, 0, halfDepth);
			lineNodes[6].Position = new SCNVector3(halfWidth, 0, -halfDepth);
			lineNodes[7].Position = new SCNVector3(halfWidth, 0, halfDepth);
			lineNodes[8].Position = new SCNVector3(0, halfHeight, -halfDepth);
			lineNodes[9].Position = new SCNVector3(-halfWidth, halfHeight, 0);
			lineNodes[10].Position = new SCNVector3(0, halfHeight, halfDepth);
			lineNodes[11].Position = new SCNVector3(halfWidth, halfHeight, 0);

			// Update the line's heights
			Action<SCNNode, float> ht = (node, height) => (node.Geometry as SCNCylinder).Height = height;
			ht(lineNodes[0], extent.X);
			ht(lineNodes[1], extent.Z);
			ht(lineNodes[2], extent.X);
			ht(lineNodes[3], extent.Z);

			ht(lineNodes[4], extent.Y);
			ht(lineNodes[5], extent.Y);
			ht(lineNodes[6], extent.Y);
			ht(lineNodes[7], extent.Y);

			ht(lineNodes[8], extent.X);
			ht(lineNodes[9], extent.Z);
			ht(lineNodes[10], extent.X);
			ht(lineNodes[11], extent.Z);
		}


		private void Flash(NSNotification notification)
		{
			Highlighted = true;

			flashTimer?.Invalidate();
			flashTimer = NSTimer.CreateScheduledTimer(flashDuration, (_) => Highlighted = false);
		}
	}
}