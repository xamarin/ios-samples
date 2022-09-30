

namespace XamarinShot.Models.GameplayState;


public class AnimWaypointComponent : GKComponent
{
        const string WaypointPrefix = "_waypoint";

        readonly List<Waypoint> wayPoints = new List<Waypoint> ();

        readonly double speed = 1d;

        double currentTime = 0d;

        int currentFrame = 0;

        SCNNode node;

        public AnimWaypointComponent (SCNNode node, Dictionary<string, object> properties) : base ()
        {
                this.node = node;

                if (properties.TryGetValue ("speed", out object? speed))
                {
                        this.speed = (double)speed;
                }

                // find all waypoints
                InitWaypoints (this.node);
                CalculateTangents ();

                // does this animation support random start times?
                if (properties.TryGetValue ("random", out object? random))
                {
                        if (random is bool isRandom && isRandom && wayPoints.Any ())
                        {
                                var last = wayPoints.Last ();
                                currentTime = new Random ().NextDouble () * last.Time;
                        }
                }

                // do we want to start at a particular percentage along curve?
                if (properties.TryGetValue ("phase", out object? objectPhase))
                {
                        if (objectPhase is double phase && wayPoints.Any ())
                        {
                                var desiredPhase = DigitExtensions.Clamp (phase, 0d, 1d);
                                var last = wayPoints.Last ();
                                currentTime = desiredPhase * last.Time;
                        }

                }

                // do we want to start at a specific point in time?
                if (properties.TryGetValue ("offset", out object? objectOffset))
                {
                        if (objectOffset is double offset && wayPoints.Any ())
                        {
                                var last = wayPoints.Last ();
                                currentTime = offset * last.Time;
                        }

                }
        }

        public AnimWaypointComponent (NSCoder coder) => throw new NotImplementedException ("init(coder:) has not been implemented");

        public bool HasWaypoints => wayPoints.Count > 1;

        void CalcCurrentFrameIndex ()
        {
                var last = wayPoints.LastOrDefault ();
                if (last is not null)
                {
                        // loop if we are past endpoint
                        while (currentTime > last.Time && last.Time > 0)
                        {
                                currentTime -= last.Time;
                        }

                        // update frame when past time value
                        for (int i = 0; i < wayPoints.Count; i++)
                        {
                                if (currentTime > wayPoints [i].Time)
                                {
                                        currentFrame = i;
                                }
                                else
                                {
                                        break;
                                }
                        }
                }
        }

        void InitWaypoints (SCNNode node)
        {
                // find start of animation group system
                var systemRoot = node.ParentWithPrefix ("_system");
                if (systemRoot is not null)
                {
                        // find all of the waypoints
                        IterateForWaypoints (systemRoot);

                        // close the loop
                        var first = wayPoints.FirstOrDefault ();
                        if (first is not null)
                        {
                                var waypoint = new Waypoint (first.Pos,
                                                            SCNVector3.Zero,
                                                            first.Rot,
                                                            (double)wayPoints.Count / speed);
                                wayPoints.Add (waypoint);
                        }
                }
        }

        /// <summary>
        /// Find all way points
        /// </summary>
        void IterateForWaypoints (SCNNode node)
        {
                if (!string.IsNullOrEmpty (node.Name) && node.Name.StartsWith (WaypointPrefix, StringComparison.Ordinal))
                {
                        var waypoint = new Waypoint (node.WorldPosition,
                                                    SCNVector3.Zero,
                                                    node.WorldOrientation,
                                                    (double)wayPoints.Count / speed);
                        wayPoints.Add (waypoint);
                }

                foreach (var child in node.ChildNodes)
                {
                        if (!string.IsNullOrEmpty (node.Name) && !node.Name.StartsWith ("_system", StringComparison.Ordinal))
                        {
                                // ignore child nodes part of another system
                                IterateForWaypoints (child);
                        }
                }
        }

        /// <summary>
        /// generate a spline if given 2 positions and 2 tangents
        /// </summary>
        /// <returns>The curve.</returns>
        float HermiteCurve (float pos1, float pos2, float tangent1, float tangent2, float time)
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
        void CalculateTangents ()
        {
                for (int i = 0; i < wayPoints.Count; i++)
                {
                        // TODO: check
                        var next = (i + 1) % wayPoints.Count;
                        var prev = (i + wayPoints.Count - 1) % wayPoints.Count;
                        wayPoints [i].Tangent = (wayPoints [next].Pos - wayPoints [prev].Pos) / 3;
                }
        }

        #region IUpdatableComponent

        public override void Update (double deltaTimeInSeconds)
        {
                currentTime += deltaTimeInSeconds;
                CalcCurrentFrameIndex ();

                var alpha = (float)((currentTime - wayPoints [currentFrame].Time) /
                                    (wayPoints [currentFrame + 1].Time - wayPoints [currentFrame].Time));
                alpha = DigitExtensions.Clamp (alpha, 0f, 1f);

                var curPos = wayPoints [currentFrame].Pos;
                var curTan = wayPoints [currentFrame].Tangent;
                var curRot = wayPoints [currentFrame].Rot;
                var nextPos = wayPoints [currentFrame + 1].Pos;
                var nextTan = wayPoints [currentFrame + 1].Tangent;
                var nextRot = wayPoints [currentFrame + 1].Rot;


                var newPosX = HermiteCurve (curPos.X, nextPos.X, curTan.X, nextTan.X, alpha);
                var newPosY = HermiteCurve (curPos.Y, nextPos.Y, curTan.Y, nextTan.Y, alpha);
                var newPosZ = HermiteCurve (curPos.Z, nextPos.Z, curTan.Z, nextTan.Z, alpha);
                var newQuat = SCNQuaternion.Slerp (curRot, nextRot, alpha);
                node.WorldPosition = new SCNVector3 (newPosX, newPosY, newPosZ);
                node.WorldOrientation = newQuat;

                // update child rigid bodies to percolate into physics
                if (Entity is GameObject entity)
                {
                        if (entity.PhysicsNode?.PhysicsBody is not null)
                        {
                                entity.PhysicsNode.PhysicsBody.ResetTransform ();
                        }
                }
        }

        #endregion

        class Waypoint
        {
                public Waypoint (SCNVector3 pos, SCNVector3 tangent, SCNQuaternion rot, double time)
                {
                        Pos = pos;
                        Tangent = tangent;
                        Rot = rot;
                        Time = time;
                }

                public SCNVector3 Pos { get; set; }

                public SCNVector3 Tangent { get; set; }

                public SCNQuaternion Rot { get; set; }

                public double Time { get; set; }
        }
}
