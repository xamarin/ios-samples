
namespace XamarinShot.Models.GameplayState
{
    using Foundation;
    using GameplayKit;
    using SceneKit;
    using XamarinShot.Utils;
    using System;

    public class GamePhysicsSmoothComponent : GKComponent
    {
        private const float MaxCorrection = 0.5f;
        private const float MaxRotation = 0.3f;

        private readonly SCNNode physicsNode;
        private SCNNode geometryNode;
        private SCNVector3 parentOffPos;
        private SCNQuaternion parentOffRot;
        private SCNVector3 sourceOffPos = SCNVector3.Zero;
        private SCNQuaternion sourceOffRot = new SCNQuaternion();

        public GamePhysicsSmoothComponent(SCNNode physicsNode, SCNNode geometryNode) : base()
        {
            this.physicsNode = physicsNode;
            this.geometryNode = geometryNode;

            // get initial offset
            this.parentOffPos = this.geometryNode.Position;
            this.parentOffRot = this.geometryNode.Orientation;
        }

        public GamePhysicsSmoothComponent(NSCoder coder) => throw new NotImplementedException("init(coder:) has not been implemented");

        // we can make this call when the physics changes to smooth it
        // make sure its called BEFORE the physics value is changed in the rigid body
        // it works by separating the actual geometry slightly from the physics to correct for the visual pop when position is changed
        private void CorrectPhysics(SCNNode node, SCNVector3 pos, SCNQuaternion rot)
        {
            // find old value
            var oldTransform = this.geometryNode.WorldTransform;

            // change position of object
            node.Position = pos;
            node.Orientation = rot;

            this.physicsNode.PhysicsBody.ResetTransform();

            // restore offset
            if (node != this.geometryNode)
            {
                this.geometryNode.WorldTransform = oldTransform;
                this.sourceOffPos = this.geometryNode.Position;
                this.sourceOffRot = geometryNode.Orientation;
            }
            else
            {
                this.sourceOffPos = parentOffPos;
                this.sourceOffRot = parentOffRot;
            }

            // cap the maximum deltas we allow in rotation and position space
            this.UpdateSmooth(1d / 60d);
        }

        /// <summary>
        /// Inch geometry back to original offset from rigid body
        /// </summary>
        private void UpdateSmooth(double deltaTime)
        {
            //  allow some motion up to a maximum offset
            var posDelta = this.parentOffPos - this.sourceOffPos;
            if (posDelta.Length > MaxCorrection)
            {
                posDelta = MaxCorrection * SCNVector3.Normalize(posDelta);
            }

            // lerp pos
            var newPos = this.sourceOffPos + posDelta;
            this.geometryNode.Position = newPos;

            // cap the max rotation that can show through
            var quatDelta = this.parentOffRot.Divide(this.sourceOffRot);
            quatDelta.ToAxisAngle(out SCNVector3 _, out float angle);

            if (angle > MaxRotation)
            {
                this.geometryNode.Orientation = SCNQuaternion.Slerp(this.sourceOffRot, this.parentOffRot, MaxRotation / angle);
            }
            else
            {
                this.geometryNode.Orientation = this.parentOffRot;
            }
        }

        public override void Update(double deltaTimeInSeconds)
        {
            this.UpdateSmooth(deltaTimeInSeconds);
        }
    }
}