
namespace XamarinShot.Models {
	using SceneKit;
	using XamarinShot.Utils;
	using System;
	using System.Collections.Generic;

	public class CollisionAudioSampler : AudioSampler {
		private readonly Config configuration;

		// Each time a note is played, choose a variant of +/- a bit of the midi note.
		// The sampler cannot play two of the same note at the same time (which makes
		// sense for music instruments like pianos, but we may want to hear two ball
		// bounce sounds, so they need to be different midi notes.)
		private short [] variant = new short [] { -1, 0, 1, 2 };

		public CollisionAudioSampler (SCNNode node, Config config, SFXCoordinator sfxCoordinator) : base (config.PresetName, node, sfxCoordinator)
		{
			this.configuration = config;
		}

		public CollisionEvent CreateCollisionEvent (float impulse, bool withBall, bool withTable)
		{
			CollisionEvent result = null;
			if (float.IsNaN (impulse) || impulse < this.configuration.MinimumImpulse) {

			} else {
				// Set mod wheel according to the impulse value. This will vary the attack of the sound
				// and make them less repetitive and more dynamic. The sampler patch is set up to play the full
				// sound with modWheel off, and shortened attack with increasing modwheel value. So we invert the
				// normalized range.
				//
				// Also, we want to alter the velocity so that higher impulse means a louder note.

				byte note;
				if (withTable) {
					note = Note.CollisionWithTable;
				} else if (withBall) {
					note = Note.CollisionWithBall;
				} else {
					note = Note.CollisionWithBlock;
				}

				note = (byte) (note + this.variant [0]);

				// move this variant randomly to another position
				var otherIndex = new Random ().Next (variant.Length - 1);// Int(arc4random_uniform(UInt32(variant.count - 1)))

				//variant.swapAt(0, 1 + otherIndex)
				var temp = variant [0];
				variant [0] = variant [1 + otherIndex];
				variant [1 + otherIndex] = temp;

				var normalizedImpulse = DigitExtensions.Clamp ((impulse - this.configuration.MinimumImpulse) / (this.configuration.MaximumImpulse - this.configuration.MinimumImpulse),
															  0f,
															  1f);

				// Once the impulse is normalized to the range 0...1, doing a sqrt
				// on it causes lower values to be higher. This curve was chosen because
				// it sounded better aesthetically.
				normalizedImpulse = (float) Math.Sqrt (normalizedImpulse);

				var rangedImpulse = this.configuration.VelocityMinimum + (this.configuration.VelocityMaximum - this.configuration.VelocityMinimum) * normalizedImpulse;
				var velocity = (byte) (DigitExtensions.Clamp (rangedImpulse, 0, 127));

				result = new CollisionEvent {
					Note = note,
					Velocity = velocity,
					ModWheel = 1f - normalizedImpulse,
				};
			}

			return result;
		}

		public void Play (CollisionEvent collisionEvent)
		{
			this.ModWheel = collisionEvent.ModWheel;
			this.Play (collisionEvent.Note, collisionEvent.Velocity);
		}

		/* helpers */

		static class Note {
			public static byte CollisionWithBall { get; } = 60; // Midi note for C4

			public static byte CollisionWithBlock { get; } = 52; // Midi note for E3

			public static byte CollisionWithTable { get; } = 55; // Midi note for G3
		}

		public class Config {
			public float MinimumImpulse { get; set; }

			public float MaximumImpulse { get; set; }

			public float VelocityMinimum { get; set; }

			public float VelocityMaximum { get; set; }

			public string PresetName { get; set; }

			public static Config Create (Dictionary<string, object> properties)
			{
				var minimumImpulse = properties ["minimumImpulse"];
				var maximumImpulse = properties ["maximumImpulse"];
				var velocityMinimum = properties ["velocityMinimum"];
				var velocityMaximum = properties ["velocityMaximum"];
				var presetName = properties ["presetName"];

				return new Config {
					MinimumImpulse = float.Parse (minimumImpulse.ToString ()),
					MaximumImpulse = float.Parse (maximumImpulse.ToString ()),
					VelocityMinimum = float.Parse (velocityMinimum.ToString ()),
					VelocityMaximum = float.Parse (velocityMaximum.ToString ()),
					PresetName = presetName.ToString ()
				};
			}
		}
	}
}
