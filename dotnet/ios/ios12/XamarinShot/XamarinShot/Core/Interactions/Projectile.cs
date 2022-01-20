
namespace XamarinShot.Models;

public interface IProjectileDelegate
{
        bool IsServer { get; }
        void AddParticles (SCNNode particlesNode, SCNVector3 worldPosition);
        void DespawnProjectile (Projectile projectile);
        void AddNodeToLevel (SCNNode node);
}

public class Projectile : GameObject
{
        // Projectile life time should be set so that projectiles will not be depleted from the pool
        const double FadeTimeToLifeTimeRatio = 0.1d;

        Team team = Team.None;

        double startTime;

        bool isLaunched;

        double lifeTime;

        public Projectile () { }

        public Projectile (SCNNode node, int? index, Dictionary<string, object> gamedefs, bool isAlive, bool isServer) : base (node, index, gamedefs, false, false) { }

        public Projectile (NSCoder coder) => throw new Exception ("init(coder:) has not been implemented");

        public double Age => isLaunched ? (GameTime.Time - startTime) : 0d;

        protected double FadeStartTime => lifeTime * (1d - FadeTimeToLifeTimeRatio);

        public IProjectileDelegate? Delegate { get; set; }

        public SCNPhysicsBody? PhysicsBody { get; set; }

        public Team Team
        {
                get
                {
                        return team;
                }

                set
                {
                        team = value;

                        // we assume the geometry and lod are unique to geometry and lod here
                        if (GeometryNode?.Geometry?.FirstMaterial?.Diffuse is not null)
                        {
                                GeometryNode.Geometry.FirstMaterial.Diffuse.Contents = team.GetColor ();
                        }

                        var levelsOfDetail = GeometryNode?.Geometry?.LevelsOfDetail;
                        if (levelsOfDetail is not null)
                        {
                                foreach (var lod in levelsOfDetail)
                                {
                                        if (lod.Geometry?.FirstMaterial?.Diffuse.Contents is not null)
                                        {
                                                lod.Geometry.FirstMaterial.Diffuse.Contents = team.GetColor ();
                                        }
                                }
                        }
                }
        }
        public static T Create<T> (SCNNode prototypeNode, int? index, Dictionary<string, object> gamedefs) where T : Projectile, new()
        {
                var node = prototypeNode.Clone ();
                // geometry and materials are reference types, so here we
                // do a deep copy. that way, each projectile gets its own color.
                node.CopyGeometryAndMaterials ();

                var physicsNode = node.FindNodeWithPhysicsBody ();
                if (physicsNode?.PhysicsBody is null)
                {
                        throw new Exception ("Projectile node has no physics");
                }

                physicsNode.PhysicsBody.ContactTestBitMask = (int)(CollisionMask.RigidBody | CollisionMask.GlitterObject | CollisionMask.TriggerVolume);
                physicsNode.PhysicsBody.CategoryBitMask = (int)CollisionMask.Ball;

                var result = GameObject.Create<T> (node, index, gamedefs, false, false);
                result.PhysicsNode = physicsNode;
                result.PhysicsBody = physicsNode.PhysicsBody;

                return result;
        }

        public static new T Create<T> (SCNNode prototypeNode) where T : Projectile, new() { return Projectile.Create<T> (prototypeNode, null, new Dictionary<string, object> ()); }

        public virtual void Launch (GameVelocity velocity, double lifeTime, IProjectileDelegate @delegate)
        {
                startTime = GameTime.Time;
                isLaunched = true;
                this.lifeTime = lifeTime;
                Delegate = @delegate;

                if (PhysicsNode is not null && PhysicsBody is not null)
                {
                        PhysicsBody.VelocityFactor = SCNVector3.One;
                        PhysicsBody.AngularVelocityFactor = SCNVector3.One;
                        PhysicsBody.Velocity = velocity.Vector;
                        PhysicsNode.Name = "ball";
                        PhysicsNode.WorldPosition = velocity.Origin;
                        PhysicsBody.ResetTransform ();
                } else {
                        throw new System.Exception ("Projectile not setup");
                }
        }

        public virtual void OnDidApplyConstraints (ISCNSceneRenderer renderer) { }

        public void DidBeginContact (SCNPhysicsContact contact) { }

        public virtual void OnSpawn () { }

        public override void Update (double deltaTimeInSeconds)
        {
                base.Update (deltaTimeInSeconds);

                // Projectile should fade and disappear after a while
                if (Age > lifeTime)
                {
                        ObjectRootNode!.Opacity = 1f;
                        Despawn ();
                } else if (Age > FadeStartTime)
                {
                        ObjectRootNode!.Opacity = (float)(1f - (Age - FadeStartTime) / (lifeTime - FadeStartTime));
                }
        }

        public virtual void Despawn ()
        {
                if (Delegate is null)
                {
                        throw new Exception ("No Delegate");
                }

                Delegate.DespawnProjectile (this);
        }

        public override PhysicsNodeData? GeneratePhysicsData ()
        {
                var data = base.GeneratePhysicsData ();
                if (data is not null)
                {
                        data.Team = team;
                }

                return data;
        }
}

/// <summary>
/// Chicken example of how we make a new projectile type
/// </summary>
public class ChickenProjectile : Projectile
{
        public ChickenProjectile () { }

        public static ChickenProjectile Create (SCNNode prototypeNode) { return Projectile.Create<ChickenProjectile> (prototypeNode); }

        public static ChickenProjectile Create (SCNNode prototypeNode, int? index, Dictionary<string, object> gamedefs) { return Projectile.Create<ChickenProjectile> (prototypeNode, index, gamedefs); }
}
