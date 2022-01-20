namespace XamarinShot.Models;

public class NetworkGame
{
	readonly int locationId;

	public NetworkGame (Player host, string? name = null, int locationId = 0)
	{
		Host = host;
		Name = name ?? $"{Host.Username}'s Game";
		this.locationId = locationId;
	}

	public GameTableLocation Location => GameTableLocation.GetLocation (locationId);

	public Player Host { get; private set; }

	public string Name { get; private set; }
}
