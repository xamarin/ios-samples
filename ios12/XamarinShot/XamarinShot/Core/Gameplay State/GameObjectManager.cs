
namespace XamarinShot.Models {
	using Foundation;
	using SceneKit;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class GameObjectManager {
		private readonly NSLock @lock = new NSLock ();

		#region Block Management

		public List<GameObject> BlockObjects { get; private set; } = new List<GameObject> ();

		public void AddBlockObject (GameObject block)
		{
			if (!this.BlockObjects.Contains (block)) {
				this.BlockObjects.Add (block);
			}
		}

		#endregion

		#region Projectile Management

		private readonly List<Projectile> projectiles = new List<Projectile> ();

		public void AddProjectile (Projectile projectile)
		{
			this.@lock.Lock ();
			this.projectiles.Add (projectile);
			this.@lock.Unlock ();
		}

		public void ReplaceProjectile (Projectile projectile)
		{
			this.@lock.Lock ();

			var oldProjectile = this.projectiles.FirstOrDefault (tile => tile.Index == projectile.Index);
			if (oldProjectile != null) {
				this.projectiles [this.projectiles.IndexOf (oldProjectile)] = projectile;
			} else {
				throw new Exception ($"Cannot find the projectile to replace {projectile.Index}");
			}

			this.@lock.Unlock ();
		}

		public void DidBeginContactAll (SCNPhysicsContact contact)
		{
			this.@lock.Lock ();

			foreach (var projectile in this.projectiles) {
				projectile.DidBeginContact (contact);
			}

			this.@lock.Unlock ();
		}

		#endregion

		#region Shared Management

		public void Update (double deltaTime)
		{
			this.@lock.Lock ();

			foreach (var projectile in this.projectiles) {
				projectile.Update (deltaTime);
			}

			this.@lock.Unlock ();
		}

		public void OnDidApplyConstraints (ISCNSceneRenderer renderer)
		{
			this.@lock.Lock ();

			foreach (var projectile in this.projectiles) {
				projectile.OnDidApplyConstraints (renderer);
			}

			this.@lock.Unlock ();
		}

		#endregion
	}
}
