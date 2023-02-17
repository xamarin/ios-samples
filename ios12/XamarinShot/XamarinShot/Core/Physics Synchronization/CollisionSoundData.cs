
namespace XamarinShot.Models {
	public class CollisionSoundData {
		public CollisionSoundData (int gameObjectIndex, CollisionEvent soundEvent)
		{
			this.GameObjectIndex = gameObjectIndex;
			this.SoundEvent = soundEvent;
		}

		public int GameObjectIndex { get; set; }

		public CollisionEvent SoundEvent { get; set; }
	}
}
