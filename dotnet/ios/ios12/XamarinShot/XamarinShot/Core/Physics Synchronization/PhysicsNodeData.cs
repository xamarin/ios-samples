
using XamarinShot.Models.Formattings;

namespace XamarinShot.Models;

[JsonConverter (typeof (PhysicsNodeDataFormatting))]
public class PhysicsNodeData
{
        // Below these delta values, node's linear/angular velocity will not sync across
        private const float PositionDeltaToConsiderNotMoving = 0.0002f;

        private const float OrientationDeltaToConsiderNotMoving = 0.002f;

        public PhysicsNodeData () { }

        public PhysicsNodeData (SCNNode node, bool alive, Team team = Team.None)
        {
                IsAlive = alive;
                Team = team;

                var presentationNode = node.PresentationNode;
                var newPosition = presentationNode.WorldPosition;
                var newOrientation = presentationNode.Orientation;

                Position = newPosition;
                Orientation = newOrientation;

                var physicsBody = node.PhysicsBody;
                if (physicsBody is not null)
                {
                        // Do not sync across physicsBodies
                        IsMoving = !newPosition.AlmostEqual (Position, PositionDeltaToConsiderNotMoving) ||
                                !newOrientation.GetVector ().AlmostEqual (Orientation.GetVector (), OrientationDeltaToConsiderNotMoving);

                        if (IsMoving)
                        {
                                Velocity = physicsBody.Velocity;
                                AngularVelocity = physicsBody.AngularVelocity;
                        }
                        else
                        {
                                Velocity = SCNVector3.Zero;
                                AngularVelocity = SCNVector4.UnitW;
                        }
                }
        }

        public bool IsAlive { get; set; } = true;

        public bool IsMoving { get; set; }

        public Team Team { get; set; } = Team.None;

        public SCNVector3 Position { get; set; } = SCNVector3.Zero;

        public SCNQuaternion Orientation { get; set; } = SCNQuaternion.Identity;

        public SCNVector3 Velocity { get; set; } = SCNVector3.Zero;

        public SCNVector4 AngularVelocity { get; set; } = SCNVector4.Zero;

        public override string ToString ()
        {
                var position = Position;
                var rot = Orientation.GetVector ();
                return $"isMoving:{IsMoving}, pos:{position.X},{position.Y},{position.Z}, rot:{rot.X},{rot.Y},{rot.Z},{rot.W}";
        }
}
