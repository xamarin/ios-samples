
namespace XamarinShot.Models {
	using Foundation;
	using SceneKit;
	using XamarinShot.Models.Enums;
	using XamarinShot.Models.Interactions;
	using XamarinShot.Utils;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Abstract:
	/// User interaction for the slingshot.
	/// </summary>
	public class CatapultInteraction : IInteraction, IGrabInteractionDelegate, IVortexActivationDelegate {
		private readonly Dictionary<int, Catapult> catapults = new Dictionary<int, Catapult> ();

		// this is a ball that doesn't have physics
		private readonly SCNNode dummyBall;

		private GrabInteraction grabInteraction;

		public CatapultInteraction (IInteractionDelegate @delegate)
		{
			this.Delegate = @delegate;
			this.dummyBall = SCNNodeExtensions.LoadSCNAsset ("projectiles_ball_8k");

			// stri the geometry out of a low-lod model, assume it doesn't have an lod
			if (this.dummyBall.Geometry != null) {
				var lod = SCNNodeExtensions.LoadSCNAsset ("projectiles_ball");
				if (lod.Geometry != null) {
					lod.Geometry.Materials = this.dummyBall.Geometry.Materials;

					// this radius will be replaced by fixLevelsOfDetail
					// when the level is placed
					this.dummyBall.Geometry.LevelsOfDetail = new SCNLevelOfDetail [] { SCNLevelOfDetail.CreateWithScreenSpaceRadius (lod.Geometry, 100f) };
				}
			}
		}

		public IInteractionDelegate Delegate { get; private set; }

		public GrabInteraction GrabInteraction {
			get {
				return this.grabInteraction;
			}

			set {
				this.grabInteraction = value;
				// Add hook up to grabInteraction delegate automatically
				if (this.grabInteraction != null) {
					this.grabInteraction.GrabDelegate = this;
				}
			}
		}

		public void AddCatapult (Catapult catapult)
		{
			if (this.grabInteraction == null) {
				throw new Exception ("GrabInteraction not set");
			}

			this.grabInteraction.AddGrabbable (catapult);
			this.catapults [catapult.CatapultId] = catapult;
			this.SetProjectileOnCatapult (catapult, ProjectileType.Cannonball);
		}

		private void SetProjectileOnCatapult (Catapult catapult, ProjectileType projectileType)
		{
			if (this.Delegate == null) {
				throw new Exception ("No delegate");
			}

			var projectile = TrailBallProjectile.Create (this.dummyBall.Clone ());
			projectile.IsAlive = true;
			projectile.Team = catapult.Team;

			if (projectile.PhysicsNode == null) {
				throw new Exception ("Projectile has no physicsNode");
			}

			projectile.PhysicsNode.PhysicsBody = null;
			this.Delegate.AddNodeToLevel (projectile.PhysicsNode);

			catapult.SetProjectileType (projectileType, projectile.ObjectRootNode);
		}

		public bool CanGrabAnyCatapult (Ray cameraRay)
		{
			return this.catapults.Any (keyValuePair => keyValuePair.Value.CanGrab (cameraRay));
		}

		#region Interactions

		public void Update (CameraInfo cameraInfo)
		{
			foreach (var catapult in this.catapults.Values) {
				catapult.Update ();
			}
		}

		#endregion

		#region Game Action Handling

		public void Handle (GameActionType gameAction, Player player)
		{
			if (gameAction.Type == GameActionType.GActionType.CatapultRelease) {
				if (this.Delegate == null) {
					throw new Exception ("No Delegate");
				}

				this.HandleCatapultReleaseAction (gameAction.CatapultRelease, player, this.Delegate);
			}
		}

		private void ReleaseCatapultGrab (int catapultId)
		{
			if (!this.catapults.TryGetValue (catapultId, out Catapult catapult)) {
				throw new Exception ($"No catapult {catapultId}");
			}

			if (this.grabInteraction == null) {
				throw new Exception ("GrabInteraction not set");
			}

			catapult.IsGrabbed = false;

			// Empty grabbedCatapult if this catapult was grabbed by player
			if (this.grabInteraction.GrabbedGrabbable is Catapult grabbedCatapult && grabbedCatapult.CatapultId == catapultId) {
				this.grabInteraction.GrabbedGrabbable = null;
			}
		}

		private void HandleCatapultReleaseAction (SlingData data, Player player, IInteractionDelegate @delegate)
		{
			if (this.catapults.TryGetValue (data.CatapultId, out Catapult catapult)) {
				catapult.OnLaunch (GameVelocity.Zero);
				this.ReleaseCatapultGrab (data.CatapultId);
			}
		}

		#endregion

		#region Grab Interaction Delegate

		public bool ShouldForceRelease (IGrabbable grabbable)
		{
			if (grabbable is Catapult catapult) {
				return catapult.IsPulledTooFar || this.IsCatapultFloating (catapult);
			} else {
				throw new Exception ("Grabbable is not catapult");
			}
		}

		private bool IsCatapultFloating (Catapult catapult)
		{
			if (this.Delegate == null) {
				throw new Exception ("No Delegate");
			}

			if (catapult.Base.PhysicsBody != null) {
				var contacts = this.Delegate.PhysicsWorld.ContactTest (catapult.Base.PhysicsBody, (SCNPhysicsTest) null);
				return !contacts.Any ();
			} else {
				return false;
			}
		}

		public void OnGrabStart (IGrabbable grabbable, CameraInfo cameraInfo, Player player)
		{
			if (grabbable is Catapult catapult) {
				catapult.OnGrabStart ();
			}
		}

		public void OnServerRelease (IGrabbable grabbable, CameraInfo cameraInfo, Player player)
		{
			if (this.Delegate == null) {
				throw new Exception ("No Delegate");
			}

			if (grabbable is Catapult catapult) {
				// Launch the ball
				var velocity = catapult.TryGetLaunchVelocity (cameraInfo);
				if (velocity != null) {
					catapult.OnLaunch (velocity);
					this.SlingBall (catapult, velocity);
					catapult.ReleaseSlingGrab ();

					this.ReleaseCatapultGrab (catapult.CatapultId);

					var slingData = new SlingData (catapult.CatapultId, catapult.ProjectileType, velocity);

					// succeed in launching catapult, notify all clients of the update
					this.Delegate.ServerDispatchActionToAll (new GameActionType { CatapultRelease = slingData, Type = GameActionType.GActionType.CatapultRelease });
				}
			}
		}

		public void OnServerGrab (IGrabbable grabbable, CameraInfo cameraInfo, Player player)
		{
			if (grabbable is Catapult catapult) {
				catapult.ServerGrab (cameraInfo.Ray);
			}
		}

		public void OnUpdateGrabStatus (IGrabbable grabbable, CameraInfo cameraInfo)
		{
			if (grabbable is Catapult catapult) {
				catapult.BallVisible = BallVisible.Visible;
				catapult.OnGrab (cameraInfo);
			}
		}

		#endregion

		#region Collision

		private const float MinImpuseToReleaseCatapult = 1.5f;

		public void DidCollision (SCNNode node, SCNNode otherNode, SCNVector3 pos, float impulse)
		{
			var gameObject = node.NearestParentGameObject ();
			if (gameObject != null && gameObject is Catapult catapult) {
				var otherGameObject = otherNode.NearestParentGameObject ();
				if (otherGameObject != null) {
					// Projectile case
					if (otherGameObject is Projectile projectile) {
						var catapultNode = catapult.Base;
						if (catapultNode.PhysicsBody == null) {
							throw new Exception ("Catapult has no physicsBody");
						}

						// Do not let projectile from the same team kill the catapult
						if (catapult.Team == projectile.Team) {
							catapultNode.PhysicsBody.Velocity = SCNVector3.Zero;
							catapultNode.PhysicsBody.AngularVelocity = SCNVector4.UnitY;
						}
					}

					// Server tries to release the catapult if it got impulse from block or projectile
					if (impulse > MinImpuseToReleaseCatapult) {
						if (this.Delegate == null) {
							throw new Exception ("No delegate");
						}

						if (this.Delegate.IsServer) {
							// Any game objects (blocks or projectiles) case
							var data = new GrabInfo (catapult.CatapultId, catapult.LastCameraInfo);
							this.Delegate.DispatchActionToServer (new GameActionType { TryRelease = data, Type = GameActionType.GActionType.TryRelease });
						}
					}
				}
			}
		}

		#endregion

		#region Sling Ball

		public void SlingBall (Catapult catapult, GameVelocity velocity)
		{
			if (this.Delegate == null) {
				throw new Exception ("No delegate");
			}

			var newProjectile = this.Delegate.SpawnProjectile ();
			newProjectile.Team = catapult.Team;

			this.Delegate.AddNodeToLevel (newProjectile.ObjectRootNode);

			// The lifeTime of projectile needed to sustain the pool is defined by:
			// (Catapult Count) * (1 + (lifeTime) / (cooldownTime)) = (Pool Count)
			var poolCount = this.Delegate.GameObjectPoolCount ();
			var lifeTime = (double) (poolCount / this.catapults.Count - 1) * catapult.CoolDownTime;

			newProjectile.Launch (velocity, lifeTime, this.Delegate.ProjectileDelegate);

			// assign the catapult source to this ball
			if (newProjectile.PhysicsNode?.PhysicsBody != null) {
				newProjectile.PhysicsNode.SetValueForKey (NSObject.FromObject (catapult.CatapultId), new NSString ("Source"));
				newProjectile.PhysicsNode.PhysicsBody.CollisionBitMask = (int) (CollisionMask.RigidBody | CollisionMask.GlitterObject);
				if (catapult.Team == Team.TeamA) {
					var collisionMask = (CollisionMask) (int) (newProjectile.PhysicsNode.PhysicsBody.CollisionBitMask);
					newProjectile.PhysicsNode.PhysicsBody.CollisionBitMask = (nuint) (int) (collisionMask | CollisionMask.CatapultTeamB);

					var categoryBitMask = (CollisionMask) (int) (newProjectile.PhysicsNode.PhysicsBody.CategoryBitMask);
					newProjectile.PhysicsNode.PhysicsBody.CategoryBitMask = (nuint) (int) (categoryBitMask | CollisionMask.CatapultTeamA);
				} else {
					var collisionMask = (CollisionMask) (int) (newProjectile.PhysicsNode.PhysicsBody.CollisionBitMask);
					newProjectile.PhysicsNode.PhysicsBody.CollisionBitMask = (nuint) (int) (collisionMask | CollisionMask.CatapultTeamA);

					var categoryBitMask = (CollisionMask) (int) (newProjectile.PhysicsNode.PhysicsBody.CategoryBitMask);
					newProjectile.PhysicsNode.PhysicsBody.CategoryBitMask = (nuint) (int) (categoryBitMask | CollisionMask.CatapultTeamB);
				}
			}
		}

		public void HandleTouch (TouchType type, Ray camera) { }

		#endregion

		#region IVortexActivationDelegate

		public void VortexDidActivate (VortexInteraction vortex)
		{
			// Kill all catapults when vortex activates
			if (this.Delegate == null) {
				throw new Exception ("No delegate");
			}

			foreach (var catapult in this.catapults.Values) {
				var data = new HitCatapult (catapult.CatapultId, false, true);
				this.Delegate.DispatchActionToAll (new GameActionType { CatapultKnockOut = data, Type = GameActionType.GActionType.HitCatapult });
			}
		}

		#endregion
	}
}
