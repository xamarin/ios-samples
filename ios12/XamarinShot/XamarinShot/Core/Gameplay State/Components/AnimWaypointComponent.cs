
namespace XamarinShot.Models.GameplayState
{
    using Foundation;
    using GameplayKit;
    using SceneKit;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using XamarinShot.Utils;

    public class AnimWaypointComponent : GKComponent
    {
        private const string WaypointPrefix = "_waypoint";

        private readonly List<Waypoint> wayPoints = new List<Waypoint>();

        private readonly double speed = 1d;

        private double currentTime = 0d;

        private int currentFrame = 0;

        private SCNNode node;

        public AnimWaypointComponent(SCNNode node, Dictionary<string, object> properties) : base()
        {
            this.node = node;

            if (properties.TryGetValue("speed", out object speed))
            {
                this.speed = (double)speed;
            }

            // find all waypoints
            this.InitWaypoints(this.node);
            this.CalculateTangents();

            // does this animation support random start times?
            if (properties.TryGetValue("random", out object random))
            {
                if (random is bool isRandom && isRandom && this.wayPoints.Any())
                {
                    var last = this.wayPoints.Last();
                    this.currentTime = new Random().NextDouble() * last.Time;
                }
            }

            // do we want to start at a particular percentage along curve?
            if (properties.TryGetValue("phase", out object objectPhase))
            {
                if (objectPhase is double phase && this.wayPoints.Any())
                {
                    var desiredPhase = DigitExtensions.Clamp(phase, 0d, 1d);
                    var last = this.wayPoints.Last();
                    this.currentTime = desiredPhase * last.Time;
                }

            }

            // do we want to start at a specific point in time?
            if (properties.TryGetValue("offset", out object objectOffset))
            {
                if (objectOffset is double offset && this.wayPoints.Any())
                {
                    var last = this.wayPoints.Last();
                    this.currentTime = offset * last.Time;
                }

            }
        }

        public AnimWaypointComponent(NSCoder coder) => throw new NotImplementedException("init(coder:) has not been implemented");

        public bool HasWaypoints => this.wayPoints.Count > 1;

        private void CalcCurrentFrameIndex()
        {
            var last = this.wayPoints.LastOrDefault();
            if (last != null)
            {
                // loop if we are past endpoint
                while (this.currentTime > last.Time && last.Time > 0)
                {
                    this.currentTime -= last.Time;
                }

                // update frame when past time value
                for (int i = 0; i < this.wayPoints.Count; i++)
                {
                    if (this.currentTime > this.wayPoints[i].Time)
                    {
                        this.currentFrame = i;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void InitWaypoints(SCNNode node)
        {
            // find start of animation group system
            var systemRoot = node.ParentWithPrefix("_system");
            if (systemRoot != null)
            {
                // find all of the waypoints
                this.IterateForWaypoints(systemRoot);

                // close the loop
                var first = this.wayPoints.FirstOrDefault();
                if (first != null)
                {
                    var waypoint = new Waypoint(first.Pos,
                                                SCNVector3.Zero,
                                                first.Rot,
                                                (double)this.wayPoints.Count / this.speed);
                    this.wayPoints.Add(waypoint);
                }
            }
        }

        /// <summary>
        /// Find all way points
        /// </summary>
        private void IterateForWaypoints(SCNNode node)
        {
            if (!string.IsNullOrEmpty(node.Name) && node.Name.StartsWith(WaypointPrefix, StringComparison.Ordinal))
            {
                var waypoint = new Waypoint(node.WorldPosition,
                                            SCNVector3.Zero,
                                            node.WorldOrientation,
                                            (double)this.wayPoints.Count / this.speed);
                this.wayPoints.Add(waypoint);
            }

            foreach (var child in node.ChildNodes)
            {
                if (!string.IsNullOrEmpty(node.Name) && !node.Name.StartsWith("_system", StringComparison.Ordinal))
                {
                    // ignore child nodes part of another system
                    this.IterateForWaypoints(child);
                }
            }
        }

        /// <summary>
        /// generate a spline if given 2 positions and 2 tangents
        /// </summary>
        /// <returns>The curve.</returns>
        private float HermiteCurve(float pos1, float pos2, float tangent1, float tangent2, float time)
        {
            var tSqr = time * time;
            var tCube = tSqr * time;
            var h1 = 2f * tCube - 3f * tSqr + 1f;
            var h2 = -2f * tCube + 3f * tSqr;
            var h3 = tCube - 2f * tSqr + time;
            var h4 = tCube - tSqr;
            var spline = h1 * pos1 + h2 * pos2 + h3 * tangent1 + h4 * tangent2;

            return spline;
        }

        /// <summary>
        /// Generate approximate spline tangents for every point
        /// </summary>
        private void CalculateTangents()
        {
            for (int i = 0; i < this.wayPoints.Count; i++)
            {
                // TODO: check
                var next = (i + 1) % this.wayPoints.Count;
                var prev = (i + this.wayPoints.Count - 1) % this.wayPoints.Count;
                this.wayPoints[i].Tangent = (this.wayPoints[next].Pos - this.wayPoints[prev].Pos) / 3;
            }
        }

        #region IUpdatableComponent

        public override void Update(double deltaTimeInSeconds)
        {
            this.currentTime += deltaTimeInSeconds;
            this.CalcCurrentFrameIndex();

            var alpha = (float)((this.currentTime - this.wayPoints[currentFrame].Time) /
                                (this.wayPoints[currentFrame + 1].Time - this.wayPoints[currentFrame].Time));
            alpha = DigitExtensions.Clamp(alpha, 0f, 1f);

            var curPos = this.wayPoints[currentFrame].Pos;
            var curTan = this.wayPoints[currentFrame].Tangent;
            var curRot = this.wayPoints[currentFrame].Rot;
            var nextPos = this.wayPoints[currentFrame + 1].Pos;
            var nextTan = this.wayPoints[currentFrame + 1].Tangent;
            var nextRot = this.wayPoints[currentFrame + 1].Rot;


            var newPosX = this.HermiteCurve(curPos.X, nextPos.X, curTan.X, nextTan.X, alpha);
            var newPosY = this.HermiteCurve(curPos.Y, nextPos.Y, curTan.Y, nextTan.Y, alpha);
            var newPosZ = this.HermiteCurve(curPos.Z, nextPos.Z, curTan.Z, nextTan.Z, alpha);
            var newQuat = SCNQuaternion.Slerp(curRot, nextRot, alpha);
            node.WorldPosition = new SCNVector3(newPosX, newPosY, newPosZ);
            node.WorldOrientation = newQuat;

            // update child rigid bodies to percolate into physics
            if (this.Entity is GameObject entity)
            {
                if (entity.PhysicsNode?.PhysicsBody != null)
                {
                    entity.PhysicsNode.PhysicsBody.ResetTransform();
                }
            }
        }

        #endregion

        class Waypoint
        {
            public Waypoint(SCNVector3 pos, SCNVector3 tangent, SCNQuaternion rot, double time)
            {
                this.Pos = pos;
                this.Tangent = tangent;
                this.Rot = rot;
                this.Time = time;
            }

            public SCNVector3 Pos { get; set; }

            public SCNVector3 Tangent { get; set; }

            public SCNQuaternion Rot { get; set; }

            public double Time { get; set; }
        }
    }
}