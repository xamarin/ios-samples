namespace XamarinShot.Models;

public class GameObjectManager
{
        readonly NSLock @lock = new NSLock ();

        #region Block Management

        public List<GameObject> BlockObjects { get; private set; } = new List<GameObject> ();

        public void AddBlockObject (GameObject block)
        {
                if (!BlockObjects.Contains (block))
                {
                        BlockObjects.Add (block);
                }
        }

        #endregion

        #region Projectile Management

        readonly List<Projectile> projectiles = new List<Projectile> ();

        public void AddProjectile (Projectile projectile)
        {
                @lock.Lock ();
                projectiles.Add (projectile);
                @lock.Unlock ();
        }

        public void ReplaceProjectile (Projectile projectile)
        {
                @lock.Lock ();

                var oldProjectile = projectiles.FirstOrDefault (tile => tile.Index == projectile.Index);
                if (oldProjectile is not null)
                {
                        projectiles [projectiles.IndexOf (oldProjectile)] = projectile;
                }
                else
                {
                        throw new Exception ($"Cannot find the projectile to replace {projectile.Index}");
                }

                @lock.Unlock ();
        }

        public void DidBeginContactAll (SCNPhysicsContact contact)
        {
                @lock.Lock ();

                foreach (var projectile in projectiles)
                {
                        projectile.DidBeginContact (contact);
                }

                @lock.Unlock ();
        }

        #endregion

        #region Shared Management

        public void Update (double deltaTime)
        {
                @lock.Lock ();

                foreach (var projectile in projectiles)
                {
                        projectile.Update (deltaTime);
                }

                @lock.Unlock ();
        }

        public void OnDidApplyConstraints (ISCNSceneRenderer renderer)
        {
                @lock.Lock ();

                foreach (var projectile in projectiles)
                {
                        projectile.OnDidApplyConstraints (renderer);
                }

                @lock.Unlock ();
        }

        #endregion
}
