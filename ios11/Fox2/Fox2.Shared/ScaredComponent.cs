
namespace Fox2
{
    using Fox2.Enums;
    using GameplayKit;
    using OpenTK;
    using SceneKit;

    /// <summary>
    /// This class implements the scared behavior.
    /// </summary>
    public class ScaredComponent : BaseComponent
    {
        private float maxAcceleration = 2.534f;
        private float fleeDistance = 2f;
        private float wanderSpeed = 1f;
        private float mass = 0.326f;

        private ScaredState state = ScaredState.Wander;
        private GKGoal fleeGoal;
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

                this.fleeGoal = GKGoal.GetGoalToFleeAgent(this.player?.Agent);
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
                this.behavior = GKBehavior.FromGoals(new GKGoal[] { this.fleeGoal, this.wanderGoal, this.centerGoal });
                this.Agent.Behavior = behavior;
                this.StartWandering();
            }
        }

        public override bool IsDead()
        {
            return this.state == ScaredState.Dead;
        }

        private void StartWandering()
        {
            if (this.behavior != null)
            {
                this.behavior.SetWeight(1f, this.wanderGoal);
                this.behavior.SetWeight(0f, this.fleeGoal);
                this.behavior.SetWeight(0.3f, this.centerGoal);
                this.state = ScaredState.Wander;
            }
        }

        private void StartFleeing()
        {
            if (this.behavior != null)
            {
                this.behavior.SetWeight(0f, this.wanderGoal);
                this.behavior.SetWeight(1f, this.fleeGoal);
                this.behavior.SetWeight(0.4f, this.centerGoal);
                this.state = ScaredState.Flee;
            }
        }

        public override void Update(double deltaTimeInSeconds)
        {
            if (this.state != ScaredState.Dead)
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
                        case ScaredState.Wander:
                            if (distance < this.fleeDistance)
                            {
                                this.StartFleeing();
                            }
                            break;
                        case ScaredState.Flee:
                            if (distance > this.fleeDistance)
                            {
                                this.StartWandering();
                            }
                            break;
                        case ScaredState.Dead:
                            break;
                    }

                    this.HandleEnemyResponse(character, enemyNode);
                    base.Update(deltaTimeInSeconds);
                }
            }
        }

        private void HandleEnemyResponse(Character character, SCNNode enemy)
        {
            var direction = enemy.WorldPosition - character.Node.WorldPosition;
            if (direction.Length < 0.5f)
            {
                if (character.IsAttacking)
                {
                    this.state = ScaredState.Dead;

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