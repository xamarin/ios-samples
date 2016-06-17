using System;

using Foundation;
using SceneKit;
using CoreAnimation;

namespace Fox {
	public enum FloorMaterial {
		Grass,
		Rock,
		Water,
		Air,
		Count
	};

	[Flags]
	public enum Bitmask : ulong {
		Collision = 1ul << 2,
		Collectable = 1ul << 3,
		Enemy = 1ul << 4,
		SuperCollectable = 1ul << 5,
		Water = 1ul << 6
	};

	public class Character : NSObject {
		const int StepsSoundCount = 10;
		const int StepsInWaterSoundCount = 4;
		const float CharacterSpeedFactor = 2f / 1.3f;

		CAAnimation walkAnimation;

		SCNNode fireEmitter;
		SCNNode smokeEmitter;
		SCNNode whiteSmokeEmitter;

		nfloat fireBirthRate;
		nfloat smokeBirthRate;
		nfloat whiteSmokeBirthRate;

		SCNAudioSource [,] steps = new SCNAudioSource [StepsSoundCount, (int)FloorMaterial.Count];

		public SCNNode Node { get; private set; }

		public FloorMaterial CurrentFloorMaterial { get; set; }

		public bool Burning { get; private set; }

		bool walking;
		public bool Walking {
			get {
				return walking;
			}
			set {
				if (walking == value)
					return;

				walking = value;
				if (walking)
					Node.AddAnimation (walkAnimation, "walk");
				else
					Node.RemoveAnimation ((NSString)"walk", 0.2f); // TODO RemoveAnimation should accept "string" and NSString 
			}
		}

		float direction;
		public float Direction {
			get {
				return direction;
			}
			set {
				if (Math.Abs (direction - value) < float.Epsilon)
					return;

				direction = value;
				Node.RunAction (SCNAction.RotateTo (0f, direction, 0f, 0.1, true));
			}
		}

		public Character ()
		{
			for (int i = 0; i < StepsSoundCount; i++) {
				steps [i, (int)FloorMaterial.Grass] = SCNAudioSource.FromFile (string.Format ("game.scnassets/sounds/Step_grass_0{0}.mp3", i));
				steps [i, (int)FloorMaterial.Grass].Volume = 0.5f;
				steps [i, (int)FloorMaterial.Rock] = SCNAudioSource.FromFile (string.Format ("game.scnassets/sounds/Step_rock_0{0}.mp3", i));
				if (i < StepsInWaterSoundCount) {
					steps [i, (int)FloorMaterial.Water] = SCNAudioSource.FromFile (string.Format ("game.scnassets/sounds/Step_splash_0{0}.mp3", i));
					steps [i, (int)FloorMaterial.Water].Load ();
				} else {
					steps [i, (int)FloorMaterial.Water] = steps [i % StepsInWaterSoundCount, (int)FloorMaterial.Water];
				}

				steps [i, (int)FloorMaterial.Rock].Load ();
				steps [i, (int)FloorMaterial.Grass].Load ();

				// Load the character.
				SCNScene characterScene = SCNScene.FromFile ("game.scnassets/panda.scn");
				SCNNode characterTopLevelNode = characterScene.RootNode.ChildNodes [0];

				Node = SCNNode.Create ();
				Node.AddChildNode (characterTopLevelNode);

				// Configure the "idle" animation to repeat forever
				foreach (var childNode in characterTopLevelNode.ChildNodes) {
					foreach (var key in childNode.GetAnimationKeys ()) {
						CAAnimation animation = childNode.GetAnimation (key);
						animation.UsesSceneTimeBase = false;
						animation.RepeatCount = float.PositiveInfinity;
						childNode.AddAnimation (animation, key);
					}
				}

				// retrieve some particle systems and save their birth rate
				fireEmitter = characterTopLevelNode.FindChildNode ("fire", true);
				fireBirthRate = fireEmitter.ParticleSystems [0].BirthRate;
				fireEmitter.ParticleSystems [0].BirthRate = 0;
				fireEmitter.Hidden = false;

				smokeEmitter = characterTopLevelNode.FindChildNode ("smoke", true);
				smokeBirthRate = smokeEmitter.ParticleSystems [0].BirthRate;
				smokeEmitter.ParticleSystems [0].BirthRate = 0;
				smokeEmitter.Hidden = false;

				whiteSmokeEmitter = characterTopLevelNode.FindChildNode ("whiteSmoke", true);
				whiteSmokeBirthRate = whiteSmokeEmitter.ParticleSystems [0].BirthRate;
				whiteSmokeEmitter.ParticleSystems [0].BirthRate = 0;
				whiteSmokeEmitter.Hidden = false;

				SCNVector3 min = SCNVector3.Zero;
				SCNVector3 max = SCNVector3.Zero;

				Node.GetBoundingBox (ref min, ref max);

				float radius = (max.X - min.X) * .4f;
				float height = (max.Y - min.Y);

				// Create a kinematic with capsule.
				SCNNode colliderNode = SCNNode.Create ();
				colliderNode.Name = "collider";
				colliderNode.Position = new SCNVector3 (0f, height * .51f, 0f);
				colliderNode.PhysicsBody = SCNPhysicsBody.CreateBody (
					SCNPhysicsBodyType.Kinematic,
					SCNPhysicsShape.Create (SCNCapsule.Create (radius, height))
				);

				// We want contact notifications with the collectables, enemies and walls.
				colliderNode.PhysicsBody.ContactTestBitMask = (nuint)(int)(Bitmask.SuperCollectable | Bitmask.Collectable | Bitmask.Collision | Bitmask.Enemy);
				Node.AddChildNode (colliderNode);

				walkAnimation = LoadAnimationFromSceneNamed ("game.scnassets/walk.scn");
				walkAnimation.UsesSceneTimeBase = false;
				walkAnimation.FadeInDuration = .3f;
				walkAnimation.FadeOutDuration = .3f;
				walkAnimation.RepeatCount = float.PositiveInfinity;
				walkAnimation.Speed = CharacterSpeedFactor;

				// Play foot steps at specific times in the animation
				walkAnimation.AnimationEvents = new [] {
					SCNAnimationEvent.Create (.1f, (animation, animatedObject, playingBackward) => PlayFootStep ()),
					SCNAnimationEvent.Create (.6f, (animation, animatedObject, playingBackward) => PlayFootStep ())
				};
			}
		}

		public void PlayFootStep ()
		{
			if (CurrentFloorMaterial == FloorMaterial.Air)
				return; // We are in the air, no sound to play

			int stepSoundIndex = Math.Min (StepsSoundCount - 1, new Random ().Next (0, 32767) * StepsSoundCount);
			Node.RunAction (SCNAction.PlayAudioSource (steps[stepSoundIndex, (int)CurrentFloorMaterial], false));
		}

		void UpdateWalkSpeed (nfloat speedFactor)
		{
			bool wasWalking = Walking;
			if (wasWalking)
				Walking = false;

			walkAnimation.Speed = (float)(CharacterSpeedFactor * speedFactor);
			Walking |= wasWalking;
		}

		public void Hit ()
		{
			Burning = true;
			fireEmitter.ParticleSystems [0].BirthRate = fireBirthRate;
			smokeEmitter.ParticleSystems [0].BirthRate = smokeBirthRate;

			UpdateWalkSpeed (2.3f);
		}

		public void Pshhhh ()
		{
			if (!Burning)
				return;

			Burning = false;
			fireEmitter.ParticleSystems [0].BirthRate = 0;
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 1.0;
			smokeEmitter.ParticleSystems [0].BirthRate = 0;
			SCNTransaction.Commit ();

			whiteSmokeEmitter.ParticleSystems [0].BirthRate = whiteSmokeBirthRate;
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 5.0;
			whiteSmokeEmitter.ParticleSystems [0].BirthRate = 0;
			SCNTransaction.Commit ();

			UpdateWalkSpeed (1f);
		}

		static CAAnimation LoadAnimationFromSceneNamed (string path)
		{
			var scene = SCNScene.FromFile (path);
			CAAnimation animation = null;
			foreach (var childNode in scene.RootNode.ChildNodes) {
				var animationKeys = childNode.ChildNodes [0].GetAnimationKeys ();
				if (animationKeys?.Length > 0) {
					animation = childNode.ChildNodes[0].GetAnimation (animationKeys[0]);
					break;
				}
			}

			return animation;
		}
	}
}