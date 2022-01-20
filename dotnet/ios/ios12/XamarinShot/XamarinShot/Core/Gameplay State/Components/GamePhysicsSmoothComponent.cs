namespace XamarinShot.Models.GameplayState;


public class GamePhysicsSmoothComponent : GKComponent
{
        const float MaxCorrection = 0.5f;
        const float MaxRotation = 0.3f;

        readonly SCNNode physicsNode;
        SCNNode geometryNode;
        SCNVector3 parentOffPos;
        SCNQuaternion parentOffRot;
        SCNVector3 sourceOffPos = SCNVector3.Zero;
        SCNQuaternion sourceOffRot = new SCNQuaternion ();

        public GamePhysicsSmoothComponent (SCNNode physicsNode, SCNNode geometryNode) : base ()
        {
                this.physicsNode = physicsNode;
                this.geometryNode = geometryNode;

                // get initial offset
                parentOffPos = geometryNode.Position;
                parentOffRot = geometryNode.Orientation;
        }

        public GamePhysicsSmoothComponent (NSCoder coder) => throw new NotImplementedException ("init(coder:) has not been implemented");

        // we can make this call when the physics changes to smooth it
        // make sure its called BEFORE the physics value is changed in the rigid body
        // it works by separating the actual geometry slightly from the physics to correct for the visual pop when position is changed
        void CorrectPhysics (SCNNode node, SCNVector3 pos, SCNQuaternion rot)
        {
                // find old value
                var oldTransform = geometryNode.WorldTransform;

                // change position of object
                node.Position = pos;
                node.Orientation = rot;

                physicsNode.PhysicsBody?.ResetTransform ();

                // restore offset
                if (node != geometryNode)
                {
                        geometryNode.WorldTransform = oldTransform;
                        sourceOffPos = geometryNode.Position;
                        sourceOffRot = geometryNode.Orientation;
                }
                else
                {
                        sourceOffPos = parentOffPos;
                        sourceOffRot = parentOffRot;
                }

                // cap the maximum deltas we allow in rotation and position space
                UpdateSmooth (1d / 60d);
        }

        /// <summary>
        /// Inch geometry back to original offset from rigid body
        /// </summary>
        void UpdateSmooth (double deltaTime)
        {
                //  allow some motion up to a maximum offset
                var posDelta = parentOffPos - sourceOffPos;
                if (posDelta.Length > MaxCorrection)
                {
                        posDelta = MaxCorrection * SCNVector3.Normalize (posDelta);
                }

                // lerp pos
                var newPos = sourceOffPos + posDelta;
                geometryNode.Position = newPos;

                // cap the max rotation that can show through
                var quatDelta = parentOffRot.Divide (sourceOffRot);
                quatDelta.ToAxisAngle (out SCNVector3 _, out float angle);

                if (angle > MaxRotation)
                {
                        geometryNode.Orientation = SCNQuaternion.Slerp (sourceOffRot, parentOffRot, MaxRotation / angle);
                }
                else
                {
                        geometryNode.Orientation = parentOffRot;
                }
        }

        public override void Update (double deltaTimeInSeconds)
        {
                UpdateSmooth (deltaTimeInSeconds);
        }
}
