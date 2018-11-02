
namespace XamarinShot.Models
{
    public class NetworkGame
    {
        private readonly int locationId;

        public NetworkGame(Player host, string name = null, int locationId = 0)
        {
            this.Host = host;
            this.Name = name ?? $"{this.Host.Username}'s Game";
            this.locationId = locationId;
        }

        public GameTableLocation Location => GameTableLocation.GetLocation(this.locationId);

        public Player Host { get; private set; }

        public string Name { get; private set; }
    }
}