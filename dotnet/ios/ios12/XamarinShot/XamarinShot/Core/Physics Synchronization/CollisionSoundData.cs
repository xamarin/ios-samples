
namespace XamarinShot.Models;

public class CollisionSoundData
{
	public CollisionSoundData (int gameObjectIndex, CollisionEvent soundEvent)
	{
		GameObjectIndex = gameObjectIndex;
		SoundEvent = soundEvent;
	}

	public int GameObjectIndex { get; set; }

	public CollisionEvent SoundEvent { get; set; }
}
