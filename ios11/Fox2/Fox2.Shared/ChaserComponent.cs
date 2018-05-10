
namespace Fox2
{
    using Fox2.Enums;
    using GameplayKit;
    using OpenTK;
    using SceneKit;
    using System;

    /// <summary>
    ///  This class implements the chasing behavior.
    /// </summary>
    public class ChaserComponent : BaseComponent
    {
        private float maxAcceleration = 8f;
        private float hitDistance = 0.5f;
        private float chaseDistance = 3f;
        private float wanderSpeed = 1f;
        private float chaseSpeed = 9f;
        private float mass = 0.3f;

        private ChaserState state = ChaserState.Chase;
        private float speed = 9f;

        private GKGoal chaseGoal;
        private GKGoal wanderGoal;
        private GKGoal centerGoal;

        private GKBehavior behavior;

        private PlayerComponent player;

        public PlayerComponent Player
        {
            get
            {
                return this.player;
            }

            set
            {
                this.player = value;

                this.Agent.Mass = this.mass;
                this.Agent.MaxAcceleration = this.maxAcceleration;

                this.chaseGoal = GKGoal.GetGoalToSeekAgent(this.player?.Agent);
                this.wanderGoal = GKGoal.GetGoalToWander(this.wanderSpeed);

                var center = new Vector2[]
                {
                    new Vector2(-1f, 9f),
                    new Vector2(1f, 9f),
                    new Vector2(1f, 11f),
                    new Vector2(-1f, 11f)
                };

                var path = new GKPath(center, 0.5f, true);
                this.centerGoal = GKGoal.GetGoalToStayOnPath(path, 1d);
                this.behavior = GKBehavior.FromGoals(new GKGoal[] { this.chaseGoal, this.wanderGoal, this.centerGoal });
                this.Agent.Behavior = behavior;
                this.StartWandering();
            }
        }

        public override bool IsDead()
        {
            return this.state == ChaserState.Dead;
        }

        private void StartWandering()
        {
            if (this.behavior != null)
            {
                this.Agent.MaxSpeed = this.wanderSpeed;
                this.behavior.SetWeight(1f, this.wanderGoal);
                this.behavior.SetWeight(0f, this.chaseGoal);
                this.behavior.SetWeight(0.6f, this.centerGoal);
                this.state = ChaserState.Wander;
            }
        }

        private void StartChasing()
        {
            if (this.behavior != null)
            {
                this.Agent.MaxSpeed = this.speed;
                this.behavior.SetWeight(0f, this.wanderGoal);
                this.behavior.SetWeight(1f, this.chaseGoal);
                this.behavior.SetWeight(0.1f, this.centerGoal);
                this.state = ChaserState.Chase;
            }
        }

        public override void Update(double deltaTimeInSeconds)
        {
            if (this.state != ChaserState.Dead)
            {
                var character = this.player?.Character;
                var playerComponent = this.player?.Entity?.GetComponent(typeof(GKSCNNodeComponent)) as GKSCNNodeComponent;
                var nodeComponent = base.Entity?.GetComponent(typeof(GKSCNNodeComponent)) as GKSCNNodeComponent;
                if (character != null && playerComponent != null && nodeComponent != null)
                {
                    var enemyNode = nodeComponent.Node;
                    var playerNode = playerComponent.Node;

                    var distance = (enemyNode.WorldPosition - playerNode.WorldPosition).Length;
                    // Chase if below chaseDistance from enemy, wander otherwise.
                    switch (this.state)
                    {
                        case ChaserState.Wander:
                            if (distance < this.chaseDistance)
                            {
                                this.StartChasing();
                            }
                            break;
                        case ChaserState.Chase:
                            if (distance > this.chaseDistance)
                            {
                                this.StartWandering();
                            }
                            break;
                        case ChaserState.Dead:
                            break;
                    }

                    this.speed = Math.Min(this.chaseSpeed, (float)distance);

                    this.HandleEnemyResponse(character, enemyNode);

                    base.Update(deltaTimeInSeconds);
                }
            }
        }

        private void HandleEnemyResponse(Character character, SCNNode enemy)
        {
            var direction = enemy.WorldPosition - character.Node.WorldPosition;
            if (direction.Length < this.hitDistance)
            {
                if (character.IsAttacking)
                {
                    this.state = ChaserState.Dead;

                    character.DidHitEnemy();

                    this.PerformEnemyDieWithExplosion(enemy, direction);
                }
                else
                {
                    character.WasTouchedByEnemy();
                }
            }
        }
    }
}