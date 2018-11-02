
namespace XamarinShot.Models
{
    using Newtonsoft.Json;
    using SceneKit;
    using XamarinShot.Models.Formattings;
    using XamarinShot.Utils;

    [JsonConverter(typeof(PhysicsNodeDataFormatting))]
    public class PhysicsNodeData
    {
        // Below these delta values, node's linear/angular velocity will not sync across
        private const float PositionDeltaToConsiderNotMoving = 0.0002f;

        private const float OrientationDeltaToConsiderNotMoving = 0.002f;

        public PhysicsNodeData() { }

        public PhysicsNodeData(SCNNode node, bool alive, Team team = Team.None)
        {
            this.IsAlive = alive;
            this.Team = team;

            var presentationNode = node.PresentationNode;
            var newPosition = presentationNode.WorldPosition;
            var newOrientation = presentationNode.Orientation;

            this.Position = newPosition;
            this.Orientation = newOrientation;

            var physicsBody = node.PhysicsBody;
            if (physicsBody != null)
            {
                // Do not sync across physicsBodies
                this.IsMoving = !newPosition.AlmostEqual(this.Position, PositionDeltaToConsiderNotMoving) ||
                                !newOrientation.GetVector().AlmostEqual(this.Orientation.GetVector(), OrientationDeltaToConsiderNotMoving);

                if (this.IsMoving)
                {
                    this.Velocity = physicsBody.Velocity;
                    this.AngularVelocity = physicsBody.AngularVelocity;
                }
                else
                {
                    this.Velocity = SCNVector3.Zero;
                    this.AngularVelocity = SCNVector4.UnitW;
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

        public override string ToString()
        {
            var position = this.Position;
            var rot = this.Orientation.GetVector();
            return $"isMoving:{this.IsMoving}, pos:{position.X},{position.Y},{position.Z}, rot:{rot.X},{rot.Y},{rot.Z},{rot.W}";
        }
    }
}