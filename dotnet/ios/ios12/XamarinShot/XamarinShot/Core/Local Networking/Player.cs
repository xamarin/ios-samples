namespace XamarinShot.Models;

public class Player
{
	readonly int hashValue;

	public Player (MCPeerID peerID)
	{
		PeerId = peerID;
		hashValue = peerID.GetHashCode ();
	}

	public Player (string username)
	{
		PeerId = new MCPeerID (username);
		hashValue = PeerId.GetHashCode ();
	}

	public string Username => PeerId.DisplayName;

	public MCPeerID PeerId { get; private set; }

	public override bool Equals (object? obj)
	{
		return PeerId == (obj as Player)?.PeerId;
	}

	public override int GetHashCode ()
	{
		return PeerId.GetHashCode ();
	}
}
