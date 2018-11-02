
namespace XamarinShot.Models
{
    using Foundation;
    using SceneKit;
    using XamarinShot.Models.Enums;
    using XamarinShot.Utils;
    using System;
    using System.Collections.Generic;

    public interface IProjectileDelegate
    {
        bool IsServer { get; }
        void AddParticles(SCNNode particlesNode, SCNVector3 worldPosition);
        void DespawnProjectile(Projectile projectile);
        void AddNodeToLevel(SCNNode node);
    }

    public class Projectile : GameObject
    {
        // Projectile life time should be set so that projectiles will not be depleted from the pool
        private const double FadeTimeToLifeTimeRatio = 0.1d;

        private Team team = Team.None;

        private double startTime;

        private bool isLaunched;

        private double lifeTime;

        public Projectile() { }

        public Projectile(SCNNode node, int? index, Dictionary<string, object> gamedefs, bool isAlive, bool isServer) : base(node, index, gamedefs, false, false) { }

        public Projectile(NSCoder coder) => throw new Exception("init(coder:) has not been implemented");

        public double Age => this.isLaunched ? (GameTime.Time - this.startTime) : 0d;

        protected double FadeStartTime => this.lifeTime * (1d - FadeTimeToLifeTimeRatio);

        public IProjectileDelegate Delegate { get; set; }

        public SCNPhysicsBody PhysicsBody { get; set; }

        public Team Team 
        {
            get
            {
                return this.team;
            }

            set 
            {
                this.team = value;

                // we assume the geometry and lod are unique to geometry and lod here
                if (this.GeometryNode?.Geometry?.FirstMaterial?.Diffuse != null)
                {
                    this.GeometryNode.Geometry.FirstMaterial.Diffuse.Contents = team.GetColor();
                }

                var levelsOfDetail = this.GeometryNode?.Geometry?.LevelsOfDetail;
                if (levelsOfDetail != null)
                {
                    foreach(var lod in levelsOfDetail)
                    {
                        if (lod.Geometry?.FirstMaterial?.Diffuse.Contents != null)
                        {
                            lod.Geometry.FirstMaterial.Diffuse.Contents = team.GetColor();
                        }
                    }
                }
            }
        }
        public static T Create<T>(SCNNode prototypeNode, int? index, Dictionary<string, object> gamedefs) where T : Projectile, new()
        {
            var node = prototypeNode.Clone();
            // geometry and materials are reference types, so here we
            // do a deep copy. that way, each projectile gets its own color.
            node.CopyGeometryAndMaterials();

            var physicsNode = node.FindNodeWithPhysicsBody();
            if(physicsNode?.PhysicsBody == null)
            {
                throw new Exception("Projectile node has no physics");
            }

            physicsNode.PhysicsBody.ContactTestBitMask = (int)(CollisionMask.RigidBody | CollisionMask.GlitterObject | CollisionMask.TriggerVolume);
            physicsNode.PhysicsBody.CategoryBitMask = (int)CollisionMask.Ball;

            var result = GameObject.Create<T>(node, index, gamedefs, false, false);
            result.PhysicsNode = physicsNode;
            result.PhysicsBody = physicsNode.PhysicsBody;

            return result;
        }

        public static T Create<T>(SCNNode prototypeNode) where T : Projectile, new()  { return Projectile.Create<T>(prototypeNode, null, new Dictionary<string, object>()); }
            
        public virtual void Launch(GameVelocity velocity, double lifeTime, IProjectileDelegate @delegate) 
        {
            this.startTime = GameTime.Time;
            this.isLaunched = true;
            this.lifeTime = lifeTime;
            this.Delegate = @delegate;
        
            if (this.PhysicsNode != null && this.PhysicsBody != null)
            {
                this.PhysicsBody.VelocityFactor = SCNVector3.One;
                this.PhysicsBody.AngularVelocityFactor = SCNVector3.One;
                this.PhysicsBody.Velocity = velocity.Vector;
                this.PhysicsNode.Name = "ball";
                this.PhysicsNode.WorldPosition = velocity.Origin;
                this.PhysicsBody.ResetTransform();
            }
            else
            {
                throw new System.Exception("Projectile not setup");
            }
        }

        public virtual void OnDidApplyConstraints(ISCNSceneRenderer renderer) { }

        public void DidBeginContact(SCNPhysicsContact contact) { }

        public virtual void OnSpawn() { }

        public override void Update(double deltaTimeInSeconds)
        {
            base.Update(deltaTimeInSeconds);
        
            // Projectile should fade and disappear after a while
            if (this.Age > this.lifeTime)
            {
                this.ObjectRootNode.Opacity = 1f;
                this.Despawn();
            }
            else if (this.Age > this.FadeStartTime)
            {
                this.ObjectRootNode.Opacity = (float)(1f - (this.Age - this.FadeStartTime) / (this.lifeTime - this.FadeStartTime));
            }
        }

        public virtual void Despawn()
        {
            if(this.Delegate == null)
            {
                throw new Exception("No Delegate");
            }

            this.Delegate.DespawnProjectile(this);
        }

        public override PhysicsNodeData GeneratePhysicsData()
        {
            var data = base.GeneratePhysicsData();
            if (data != null)
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
        public ChickenProjectile() { }

        public static ChickenProjectile Create(SCNNode prototypeNode) { return Projectile.Create<ChickenProjectile>(prototypeNode); }

        public static ChickenProjectile Create(SCNNode prototypeNode, int? index, Dictionary<string, object> gamedefs) { return Projectile.Create<ChickenProjectile>(prototypeNode, index, gamedefs); }
    }
}