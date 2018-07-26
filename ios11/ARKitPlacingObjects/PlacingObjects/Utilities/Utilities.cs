using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreGraphics;
using SceneKit;
using ARKit;
using System.Collections;
using System.Linq;

namespace PlacingObjects
{
	public static class Utilities
	{
		public static SCNVector3? RayIntersectionWithHorizontalPlane(SCNVector3 rayOrigin, SCNVector3 direction, float planeY) {
			// Normalize direction
			direction = direction.Normalized();

			// Special case handling: Check if the ray is horizontal as well.
			if (direction.Y == 0) {
				if (rayOrigin.Y == planeY) {
					// The ray is horizontal and on the plane, thus all points on the ray intersect with the plane.
					// Therefore we simply return the ray origin.
					return rayOrigin;
				} else {
					// The ray is parallel to the plane and never intersects.
					return null;
				}
			}

			// The distance from the ray's origin to the intersection point on the plane is:
			//   (pointOnPlane - rayOrigin) dot planeNormal
			//  --------------------------------------------
			//          direction dot planeNormal

			// Since we know that horizontal planes have normal (0, 1, 0), we can simplify this to:
			var dist = (planeY - rayOrigin.Y) / direction.Y;

			// Do not return intersections behind the ray's origin.
			if (dist <0) {
				return null;
			}

			// Return the intersection point.
			return rayOrigin.Add(direction * dist);
		}

		public static SCNNode CreateAxesNode(float quiverLength, float quiverThickness) {
			quiverThickness = (quiverLength / 50f) * quiverThickness;
			var chamferRadius = quiverThickness / 2f;

			var xQuiverBox = SCNBox.Create(quiverLength, quiverThickness, quiverThickness, chamferRadius);
			xQuiverBox.InsertMaterial(SCNMaterialExtensions.CreateMaterial(UIColor.Red, false), 0);
			var xQuiverNode = SCNNode.FromGeometry(xQuiverBox);
			xQuiverNode.Position = new SCNVector3((quiverLength / 2f), 0, 0);

			var yQuiverBox = SCNBox.Create(quiverThickness, quiverLength, quiverThickness, chamferRadius);
			yQuiverBox.InsertMaterial(SCNMaterialExtensions.CreateMaterial(UIColor.Green, false), 0);
			var yQuiverNode = SCNNode.FromGeometry(yQuiverBox);
			yQuiverNode.Position = new SCNVector3(0, (quiverLength / 2f), 0);

			var zQuiverBox = SCNBox.Create(quiverThickness, quiverThickness, quiverLength, chamferRadius);
			zQuiverBox.InsertMaterial(SCNMaterialExtensions.CreateMaterial(UIColor.Blue, false), 0);
			var zQuiverNode = SCNNode.FromGeometry(zQuiverBox);
			zQuiverNode.Position = new SCNVector3(0, 0, (quiverLength / 2f));

			// Assemble node
			var quiverNode = new SCNNode(){
				Name = "Axes"
			};
			quiverNode.AddChildNode(xQuiverNode);
			quiverNode.AddChildNode(yQuiverNode);
			quiverNode.AddChildNode(zQuiverNode);

			// Return results
			return quiverNode;
		}

		public static SCNNode CreateCrossNode(float size, string style, bool horizontal = true, float opacity = 1f){
			// Create a size x size m plane and put a grid texture onto it.
			var planeDimention = size;

			var image = UIImage.FromBundle(style);

			var planeNode = SCNNode.FromGeometry(CreateSquarePlane(planeDimention, image));
			var material = planeNode.Geometry.Materials[0];
			material.Ambient.Contents = UIColor.Black;
			material.LightingModelName = SCNLightingModel.Constant;

			if (horizontal) {
				planeNode.EulerAngles = new SCNVector3((float)Math.PI / 2f, 0, (float)Math.PI);
			} else {
				planeNode.Constraints = new SCNConstraint[] { new SCNBillboardConstraint()};
			}

			// Assemble cross
			var cross = new SCNNode() {
				Name = "Cross"
			};
			cross.AddChildNode(planeNode);
			cross.Opacity = opacity;

			// Return results
			return cross;
		}

		public static SCNPlane CreateSquarePlane(float size, NSObject contents) {
			var plane = SCNPlane.Create(size, size);
			plane.InsertMaterial(SCNMaterialExtensions.CreateMaterial(contents), 0);
			return plane;
		}

		public static SCNPlane CreatePlane(CGSize size, NSObject contents)
		{
			var plane = SCNPlane.Create(size.Width, size.Height);
			plane.InsertMaterial(SCNMaterialExtensions.CreateMaterial(contents), 0);
			return plane;
		}

		public static SCNVector3 AverageVector3List(SCNVector3[] vectors) {
			var average = new SCNVector3(0, 0, 0);

			// Process all vectors
			foreach(SCNVector3 vector in vectors) {
				average.X += vector.X;
				average.Y += vector.Y;
				average.Z += vector.Z;
			}

			// Compute average
			average.X /= vectors.Length;
			average.Y /= vectors.Length;
			average.Z /= vectors.Length;

			// return results
			return average;
		}
	}
}
