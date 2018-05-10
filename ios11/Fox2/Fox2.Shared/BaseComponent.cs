
namespace Fox2
{
    using CoreAnimation;
    using Fox2.Extensions;
    using GameplayKit;
    using SceneKit;
    using System;

    /// <summary>
    /// This class is used as a base class for all game components.
    /// </summary>
    public class BaseComponent : GKComponent
    {
        public static float EnemyAltitude => -0.46f;

        public GKAgent2D Agent { get; } = new GKAgent2D();

        public bool IsAutoMoveNode { get; set; } = true;

        public virtual bool IsDead()
        {
            return false;
        }

        public void PositionAgentFromNode()
        {
            var nodeComponent = base.Entity.GetComponent(typeof(GKSCNNodeComponent)) as GKSCNNodeComponent;
            this.Agent.SetTransform(nodeComponent.Node.Transform);
        }

        public void PositionNodeFromAgent()
        {
            var nodeComponent = base.Entity.GetComponent(typeof(GKSCNNodeComponent)) as GKSCNNodeComponent;
            nodeComponent.Node.Transform = this.Agent.GetTransform();
        }

        public void ConstrainPosition()
        {
            var position = this.Agent.Position;
            if (position.X > 2f)
            {
                position.X = 2f;
            }
            else if (position.X < -2f)
            {
                position.X = -2f;
            }

            if (position.Y > 12.5f)
            {
                position.Y = 12.5f;
            }
            else if (position.Y < 8.5f)
            {
                position.Y = 8.5f;
            }

            this.Agent.Position = position;
        }

        public override void Update(double deltaTimeInSeconds)
        {
            if (!this.IsDead())
            {
                this.Agent.Update(deltaTimeInSeconds);
                this.ConstrainPosition();

                if (this.IsAutoMoveNode)
                {
                    this.PositionNodeFromAgent();
                }

                base.Update(deltaTimeInSeconds);
            }
        }

        protected void PerformEnemyDieWithExplosion(SCNNode enemy, SCNVector3 direction)
        {
            var explositionScene = SCNScene.FromFile("art.scnassets/enemy/enemy_explosion.scn");
            if (explositionScene != null)
            {
                SCNTransaction.Begin();
                SCNTransaction.AnimationDuration = 0.4f;
                SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);

                SCNTransaction.SetCompletionBlock(() =>
                {
                    explositionScene.RootNode.EnumerateHierarchy((SCNNode node, out bool stop) =>
                    {
                        stop = false;
                        if (node.ParticleSystems != null)
                        {
                            foreach (var particle in node.ParticleSystems)
                            {
                                enemy.AddParticleSystem(particle);
                            }
                        }
                    });

                    // Hide
                    if (enemy.ChildNodes.Length > 0)
                    {
                        enemy.ChildNodes[0].Opacity = 0f;
                    }
                });

                direction.Y = 0;
                enemy.RemoveAllAnimations();
                enemy.EulerAngles = new SCNVector3(enemy.EulerAngles.X, enemy.EulerAngles.X + (float)Math.PI * 4.0f, enemy.EulerAngles.Z);

                enemy.WorldPosition += SCNVector3.Normalize(direction) * 1.5f;
                this.PositionAgentFromNode();

                SCNTransaction.Commit();
            }
            else
            {
                Console.WriteLine("Missing enemy_explosion.scn");
            }
        }
    }
}