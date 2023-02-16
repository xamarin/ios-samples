
namespace XamarinShot.Models {
	using SceneKit;
	using XamarinShot.Models.Enums;
	using XamarinShot.Utils;
	using System.Collections.Generic;

	public class CameraInfo {
		public CameraInfo (SCNMatrix4 transform)
		{
			this.Transform = transform;
		}

		public SCNMatrix4 Transform { get; private set; }

		[Newtonsoft.Json.JsonIgnore]
		public Ray Ray {
			get {
				var position = this.Transform.GetTranslation ();
				var direction = SCNVector3.Normalize (this.Transform.Multiply (new SCNVector4 (0f, 0f, -1f, 0f)).Xyz);
				return new Ray (position, direction);
			}
		}
	}

	public class GameCommand {
		public GameCommand (Player player, GAction action)
		{
			this.Player = player;
			this.Action = action;
		}

		public Player Player { get; private set; }

		public GAction Action { get; private set; }
	}

	/// <summary>
	/// When a catapult is knocked down
	/// </summary>
	public class HitCatapult {
		public HitCatapult (int catapultId, bool justKnockedout, bool vortex)
		{
			this.CatapultId = catapultId;
			this.JustKnockedout = justKnockedout;
			this.Vortex = vortex;
		}

		public int CatapultId { get; private set; }

		public bool JustKnockedout { get; private set; }

		public bool Vortex { get; private set; }
	}

	// GameVelocity stores the origin and vector of velocity.
	// It is similar to ray, but whereas ray will have normalized direction, the .vector is the velocity vector
	public class GameVelocity {
		public GameVelocity (SCNVector3 origin, SCNVector3 vector)
		{
			this.Origin = origin;
			this.Vector = vector;
		}

		public SCNVector3 Origin { get; private set; }

		public SCNVector3 Vector { get; private set; }

		[Newtonsoft.Json.JsonIgnore]
		public static GameVelocity Zero { get; } = new GameVelocity (SCNVector3.Zero, SCNVector3.Zero);
	}

	public class SlingData {
		public SlingData (int catapultId, ProjectileType projectileType, GameVelocity velocity)
		{
			CatapultId = catapultId;
			ProjectileType = projectileType;
			Velocity = velocity;
		}

		public int CatapultId { get; private set; }

		public ProjectileType ProjectileType { get; private set; }

		public GameVelocity Velocity { get; private set; }
	}

	public class GrabInfo {
		public GrabInfo (int grabbableId, CameraInfo cameraInfo)
		{
			this.GrabbableId = grabbableId;
			this.CameraInfo = cameraInfo;
		}

		public int GrabbableId { get; private set; }

		public CameraInfo CameraInfo { get; private set; }
	}

	public class LeverMove {
		public LeverMove (int leverId, float eulerAngleX)
		{
			this.LeverId = leverId;
			this.EulerAngleX = eulerAngleX;
		}

		public int LeverId { get; private set; }

		public float EulerAngleX { get; private set; }
	}

	public class StartGameMusicTime {
		public StartGameMusicTime (bool startNow, List<double> timestamps)
		{
			this.StartNow = startNow;
			this.Timestamps = timestamps;
		}

		public bool StartNow { get; private set; }

		public List<double> Timestamps { get; private set; }
	}
}
