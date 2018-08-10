using System;
using OpenTK;
using SceneKit;

namespace ScanningAndDetecting3DObjects
{
	internal struct Ray
	{
		internal SCNVector3 Origin { get; private set; }
		internal SCNVector3 Direction { get; private set; }

		internal Ray(SCNVector3 origin, SCNVector3 sideNormalInWorld) 
		{
			this.Origin = origin;
			this.Direction = sideNormalInWorld;
		}

		Ray(SCNVector3 origin, NVector3 direction)
		{
			this.Origin = origin;
			this.Direction = direction.ToSCNVector3();
		}

		internal Ray(SCNNode pointOfView, float length)
		{
			pointOfView.WorldFront.NormalizeFast();
			var cameraNormal = pointOfView.WorldFront.Times(length);
			Origin = pointOfView.WorldPosition;
			Direction = cameraNormal.ToSCNVector3();
		}

		internal NMatrix4 DragPlaneTransform(SCNVector3 cameraPos)
		{
			var camToRayOrigin = this.Origin - cameraPos;
			camToRayOrigin.NormalizeFast();

			// Create a transform for a XZ-plane. This transform can be passed to unproject() to
			// map the user's touch position in screen space onto that plane in 3D space.
			// The plane's transform is constructed such that:
			// 1. The ray along which we want to drag the object is the plane's X axis.
			// 2. The plane's Z axis is ortogonal to the X axis and orthogonal to the vector
			//    from the camera to the object.
			//
			// Defining the plane this way has two main benefits:
			// 1. Since we want to drag the object along an axis (not on a plane), we need to
			//    do one more projection from the plane's 2D space to a 1D axis. Since the axis to
			//    drag on is the plane's X-axis, we can later simply convert the un-projected point
			//    into the plane's local coordinate system and use the value on the X axis.
			// 2. The plane's Z-axis is chosen to maximize the plane's coverage of screen space.
			//    The unprojectPoint() method will stop returning positions if the user drags their
			//    finger on the screen across the plane's horizon, leading to a bad user experience.
			//    So the ideal plane is parallel or almost parallel to the screen, but this is not
			//    possible when dragging along an axis which is pointing at the camera. For that case
			//    we try to find a plane which covers as much screen space as possible.
			var xVector = Direction;
			var zVector = SCNVector3.Cross(xVector, camToRayOrigin).Normalized();
			var yVector = SCNVector3.Cross(xVector, zVector).Normalized();

			var asIfSimd4x4 = Utilities.NMatrix4Create(new[] {
				Utilities.SCNVector4Create(xVector, 0),
				Utilities.SCNVector4Create(yVector, 0),
				Utilities.SCNVector4Create(zVector, 0),
				Utilities.SCNVector4Create(Origin, 1)
				});
			// But we're not, so transpose
			asIfSimd4x4.Transpose();
			return asIfSimd4x4;
		}

		internal NMatrix4 DragPlaneTransform(SCNNode camera)
		{
			// Create a transform for a XZ-plane. This transform can be passed to unproject() to
			// map the user's touch position in screen space onto that plane in 3D space.
			// The plane's transform is constructed from a given normal.
			var yVector = Direction.Normalized();
			var xVector = SCNVector3.Cross(yVector, camera.WorldRight);
			var zVector = SCNVector3.Cross(xVector, yVector).Normalized();
			var asIfsimd4x4 = Utilities.NMatrix4Create(new[] {
				Utilities.SCNVector4Create(xVector, 0),
				Utilities.SCNVector4Create(yVector, 0),
				Utilities.SCNVector4Create(zVector, 0),
				Utilities.SCNVector4Create(Origin, 1)
			});
			// But we're not, so transpose 
			asIfsimd4x4.Transpose();
			return asIfsimd4x4;
		}
	}
}
