namespace XamarinShot.Models;

public class TrailBallProjectile : Projectile
{
        public const float DefaultTrailWidth = 0.5f; // Default trail with with ball size as the unit (1.0 represents same width as the ball)

        public const int DefaultTrailLength = 108;

        private const float Epsilon = 1.19209290E-07f; // upper limit on float rounding error

        // default values the result of a vast user study.
        const float BallSize = 0.275f;

        readonly List<SCNVector3> worldPositions = new List<SCNVector3> ();

        readonly List<SCNVector3> tempWorldPositions = new List<SCNVector3> ();

        readonly List<SCNVector4> colors = new List<SCNVector4> ();

        readonly SCNNode trailNode = new SCNNode ();


        public TrailBallProjectile () { }

        public TrailBallProjectile (SCNNode node, int? index, Dictionary<string, object> gamedefs, bool isAlive, bool isServer) : base (node, index, gamedefs, isAlive, isServer) { }

        public float TrailHalfWidth => (UserDefaults.TrailWidth ?? TrailBallProjectile.DefaultTrailWidth) * BallSize * 0.5f;

        public int MaxTrailPositions => UserDefaults.TrailLength ?? TrailBallProjectile.DefaultTrailLength;

        public bool TrailShouldNarrow => UserDefaults.TrailShouldNarrow;

        public static TrailBallProjectile Create (SCNNode prototypeNode, int? index, Dictionary<string, object> gamedefs) { return Projectile.Create<TrailBallProjectile> (prototypeNode, index, gamedefs); }

        public static TrailBallProjectile Create (SCNNode prototypeNode) { return Projectile.Create<TrailBallProjectile> (prototypeNode); }

        public override void Launch (GameVelocity velocity, double lifeTime, IProjectileDelegate @delegate)
        {
                base.Launch (velocity, lifeTime, @delegate);
                AddTrail ();
        }

        public override void OnSpawn ()
        {
                base.OnSpawn ();
                AddTrail ();
        }

        public override void Despawn ()
        {
                base.Despawn ();
                RemoveTrail ();
        }

        void AddTrail ()
        {
                if (PhysicsNode is not null)
                {
                        trailNode.CastsShadow = false;
                        if (PhysicsNode.PhysicsBody is not null)
                        {
                                PhysicsNode.PhysicsBody.AngularDamping = 1f;
                        }

                        if (Delegate is not null)
                        {
                                Delegate.AddNodeToLevel (trailNode);
                        }
                }
        }

        void RemoveTrail ()
        {
                trailNode.RemoveFromParentNode ();
        }

        public override void OnDidApplyConstraints (ISCNSceneRenderer renderer)
        {
                var frameSkips = 3;
                if ((GameTime.FrameCount + Index) % frameSkips == 0)
                {
                        if (PhysicsNode is not null)
                        {
                                if (worldPositions.Count > (MaxTrailPositions / frameSkips))
                                {
                                        RemoveVerticesPair ();
                                }

                                var position = PhysicsNode.PresentationNode.WorldPosition;

                                SCNVector3 trailDir;
                                if (worldPositions.Any ())
                                {
                                        var previousPosition = worldPositions.LastOrDefault ();
                                        trailDir = position - previousPosition;

                                        var lengthSquared = trailDir.LengthSquared;
                                        if (lengthSquared < Epsilon)
                                        {
                                                RemoveVerticesPair ();
                                                UpdateColors ();
                                                var tempPositions = tempWorldPositions.Select (tempPosition => trailNode.PresentationNode.ConvertPositionFromNode (tempPosition, null)).ToList ();
                                                trailNode.PresentationNode.Geometry = CreateTrailMesh (tempPositions, colors);
                                                return;
                                        }

                                        trailDir = SCNVector3.Normalize (trailDir);
                                } else {
                                        trailDir = ObjectRootNode!.WorldFront;
                                }

                                var right = SCNVector3.Cross (SCNVector3.UnitY, trailDir);
                                right = SCNVector3.Normalize (right);
                                var scale = 1f; //Float(i - 1) / worldPositions.count
                                var halfWidth = TrailHalfWidth;
                                if (TrailShouldNarrow)
                                {
                                        halfWidth *= scale;
                                }

                                var u = position + right * halfWidth;
                                var v = position - right * halfWidth;

                                worldPositions.Add (position);
                                tempWorldPositions.Add (u);
                                tempWorldPositions.Add (v);

                                colors.Add (SCNVector4.Zero);
                                colors.Add (SCNVector4.Zero);

                                UpdateColors ();
                                var localPositions = tempWorldPositions.Select (tempPosition => trailNode.PresentationNode.ConvertPositionFromNode (tempPosition, null)).ToList ();
                                trailNode.PresentationNode.Geometry = CreateTrailMesh (localPositions, colors);
                        }
                }
        }

        void RemoveVerticesPair ()
        {
                worldPositions.RemoveAt (0);
                tempWorldPositions.RemoveRange (0, 2);
                colors.RemoveRange (0, 2);
        }

        void UpdateColors ()
        {
                var baseColor = SimdExtensions.CreateVector4 (Team.GetColor ());
                for (var i = 0; i < colors.Count; i++)
                {
                        var scale = (float)i / (float)colors.Count;
                        colors [i] = baseColor * scale;
                }
        }

        SCNGeometry? CreateTrailMesh (List<SCNVector3> positions, List<SCNVector4> colors)
        {
                SCNGeometry? result = null;
                if (positions.Count >= 4)
                {
                        var array = new byte [positions.Count];
                        for (byte i = 0; i < positions.Count; i++)
                        {
                                array [i] = i;
                        }

                        var positionSource = SCNGeometrySource.FromVertices (positions.ToArray ());
                        var colorSource = SCNGeometrySourceExtensions.Create (colors);
                        var element = SCNGeometryElement.FromData (NSData.FromArray (array), SCNGeometryPrimitiveType.TriangleStrip, positions.Count - 2, 2);
                        result = SCNGeometry.Create (new SCNGeometrySource [] { positionSource, colorSource }, new SCNGeometryElement [] { element });

                        var material = result.FirstMaterial;
                        if (material is null)
                        {
                                throw new Exception ("created geometry without material");
                        }

                        material.DoubleSided = true;
                        material.LightingModelName = SCNLightingModel.Constant;
                        material.BlendMode = SCNBlendMode.Add;
                        material.WritesToDepthBuffer = false;
                }

                return result;
        }
}
