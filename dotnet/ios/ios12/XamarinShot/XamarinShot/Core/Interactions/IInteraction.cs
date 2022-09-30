namespace XamarinShot.Models;

public class GameRayCastHitInfo
{
	public GameRayCastHitInfo (SCNVector3 position, SCNVector3 direction, IList<SCNHitTestResult> hits)
	{
		Position = position;
		Direction = direction;
		Hits = hits;
	}

	public SCNVector3 Position { get; set; }

	public SCNVector3 Direction { get; set; }

	public IList<SCNHitTestResult> Hits { get; set; }

	public Ray Ray ()
	{
		return new Ray (Position, Direction);
	}
}

public interface IInteractionDelegate
{
	Player CurrentPlayer { get; }
	SCNPhysicsWorld PhysicsWorld { get; }
	IProjectileDelegate ProjectileDelegate { get; }
	bool IsServer { get; }
	List<GameObject> AllBlockObjects { get; }
	List<Catapult> Catapults { get; }

	void RemoveTableBoxNodeFromLevel ();

	void AddNodeToLevel (SCNNode node);
	Projectile SpawnProjectile ();
	Projectile CreateProjectile ();  // Create projectile without putting it into a pool, useful for using it to show when catapult gets pulled
	int GameObjectPoolCount ();
	void RemoveAllPhysicsBehaviors ();

	void AddInteraction (IInteraction interaction);

	void DispatchActionToServer (GameActionType gameAction);
	void DispatchActionToAll (GameActionType gameAction); // including self
	void ServerDispatchActionToAll (GameActionType gameAction);
	void DispatchToPlayer (GameActionType gameAction, Player player);

	void PlayWinSound ();
	void StartGameMusic (IInteraction interaction);
}

public interface IInteraction
{
	void Update (CameraInfo cameraInfo);

	/// <summary>
	/// Handle Inputs
	/// </summary>
	void HandleTouch (TouchType type, Ray camera);

	/// <summary>
	/// Handle Action
	/// </summary>
	void Handle (GameActionType gameAction, Player player);

	/// <summary>
	/// Handle Collision
	/// </summary>
	void DidCollision (SCNNode node, SCNNode otherNode, SCNVector3 pos, float impulse);
}
