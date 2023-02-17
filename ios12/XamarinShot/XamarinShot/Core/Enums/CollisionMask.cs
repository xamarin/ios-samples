
namespace XamarinShot.Models.Enums {
	[System.Flags]
	public enum CollisionMask {
		RigidBody = 1,
		GlitterObject = 2,
		Ball = 4,
		Phantom = 32,    // for detecting collisions with trigger volumes
		TriggerVolume = 64,  // trigger behavior without affecting physics
		CatapultTeamA = 128,
		CatapultTeamB = 256
	}
}
