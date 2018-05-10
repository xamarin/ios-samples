
namespace Fox2
{
    /// <summary>
    /// This class represents the player.
    /// </summary>
    public class PlayerComponent : BaseComponent
    {
        public Character Character { get; set; }

        public override void Update(double deltaTimeInSeconds)
        {
            this.PositionAgentFromNode();
            base.Update(deltaTimeInSeconds);
        }
    }
}