using System;
using Foundation;
using OpenTK;
using SceneKit;
using UIKit;

namespace ScanningAndDetecting3DObjects {
	// An interactive visualization a single x/y/z coordinate axis for use in placing the origin/anchor point of a scanned object.
	internal class ObjectOriginAxis : SCNNode {
		private Axis axis;

		private NSTimer flashTimer;
		private const double flashDuration = 0.1;

		internal ObjectOriginAxis (Axis axis, float length, float thickness, float radius, float handleSize)
		{
			this.axis = axis;

			base.Init ();

			UIColor color = default (UIColor);
			UIImage texture = default (UIImage);
			SCNVector3 dimensions = default (SCNVector3);
			var position = new SCNVector3 ((float) (axis.Normal ().X * (length / 2.0)), (float) (axis.Normal ().Y * (length / 2.0)), (float) (axis.Normal ().Z * (length / 2.0)));
			var axisHandlePosition = new SCNVector3 ((float) (axis.Normal ().X * length), (float) (axis.Normal ().Y * length), (float) (axis.Normal ().Z * length));

			switch (axis) {
			case Axis.X:
				color = UIColor.Red;
				texture = UIImage.FromFile ("handle_red");
				dimensions = new SCNVector3 (length, thickness, thickness);
				break;
			case Axis.Y:
				color = UIColor.Green;
				texture = UIImage.FromFile ("handle_green");
				dimensions = new SCNVector3 (thickness, length, thickness);
				break;
			case Axis.Z:
				color = UIColor.Blue;
				texture = UIImage.FromFile ("handle_blue");
				dimensions = new SCNVector3 (thickness, thickness, length);
				break;
			}

			var axisGeo = SCNBox.Create (dimensions.X, dimensions.Y, dimensions.Z, radius);
			axisGeo.Materials = new [] { Utilities.Material (color) };
			var axisNode = SCNNode.FromGeometry (axisGeo);

			var axisHandleGeo = SCNPlane.Create (handleSize, handleSize);
			axisHandleGeo.Materials = new [] { Utilities.Material (texture, false) };
			var axisHandle = SCNNode.FromGeometry (axisHandleGeo);
			axisHandle.Constraints = new [] { new SCNBillboardConstraint () };

			axisNode.Position = position;
			axisHandle.Position = axisHandlePosition;

			// Increase the axis handle geometry's bounding box that is used for hit testing to make it easier to hit
			SCNVector3 min = default (SCNVector3);
			SCNVector3 max = default (SCNVector3);
			axisHandle.GetBoundingBox (ref min, ref max);
			var padding = handleSize * 0.8F;
			var newMin = new SCNVector3 (min.X - padding, min.Y - padding, min.Z - padding);
			var newMax = new SCNVector3 (max.X + padding, max.Y + padding, max.Z + padding);
			axisHandle.SetBoundingBox (ref newMin, ref newMax);

			AddChildNode (axisNode);
			AddChildNode (axisHandle);
		}

		private bool highlighted = false;
		internal bool Highlighted {
			get => highlighted;
			set {
				highlighted = value;
				var emissionColor = highlighted ? UIColor.White : UIColor.Black;
				foreach (var child in ChildNodes) {
					var emission = child.Geometry?.FirstMaterial?.Emission;
					if (emission != null) {
						emission.ContentColor = emissionColor;
					}
				}
			}
		}

		internal Axis Axis { get => axis; }

		private void UpdateRenderOrder (SCNNode node, bool isOnTop)
		{
			node.RenderingOrder = isOnTop ? 2 : 0;

			foreach (var material in node.Geometry?.Materials ?? new SCNMaterial [] { }) {
				material.ReadsFromDepthBuffer = !isOnTop;
			}

			foreach (var child in node.ChildNodes) {
				// recurse
				UpdateRenderOrder (child, isOnTop);
			}
		}

		internal void DisplayNodeHierarchyOnTop (bool isOnTop)
		{
			// Recursively traverse the node's children to update the rendering order
			// depening on the `isOnTop` parameter
			UpdateRenderOrder (this, isOnTop);

		}

		internal void Flash ()
		{
			Highlighted = true;

			flashTimer?.Invalidate ();
			flashTimer = NSTimer.CreateScheduledTimer (flashDuration, (_) => Highlighted = false);
		}
	}
}
