using XamarinShot.Models.GameplayState;

namespace XamarinShot.Models;

public interface IGameObjectPoolDelegate
{
Dictionary<string, object> GameDefinitions { get; }

void OnSpawnedProjectile();
}

// Pool that makes it possible for clients to join the game after a ball has been shot
// In this case, the pool helps manage fixed projectile slots used for physics sync.
// The pool do not actually reuse the object (that functionality could be added if necessary).
public class GameObjectPool : IDisposable
{
        readonly SCNNode cannonball;

        readonly SCNNode chicken;

        public GameObjectPool ()
        {
                cannonball = SCNNodeExtensions.LoadSCNAsset ("projectiles_ball");
                chicken = SCNNodeExtensions.LoadSCNAsset ("projectiles_chicken");

                InitialPoolCount = 30;
        }

        public IGameObjectPoolDelegate? Delegate { get; set; }

        public IProjectileDelegate? ProjectileDelegate { get; set; }

        public List<Projectile> ProjectilePool { get; } = new List<Projectile> ();

        public int InitialPoolCount { get; private set; } = 0;

        public Projectile SpawnProjectile ()
        {
                var count = 0;
                foreach (var projectile in ProjectilePool.Where (projectile => !projectile.IsAlive))
                {
                        count += 1;
                }

                foreach (var projectile in ProjectilePool.Where (projectile => !projectile.IsAlive))
                {
                        return SpawnProjectile (projectile.Index);
                }

                throw new Exception ("No more free projectile in the pool");
        }

        /// <summary>
        /// Spawn projectile with specific object index
        /// </summary>
        public Projectile SpawnProjectile (int objectIndex)
        {
                if (Delegate is null)
                {
                        throw new Exception ("No Delegate");
                }

                Delegate.OnSpawnedProjectile ();

                var projectile = ProjectilePool.FirstOrDefault (tile => tile.Index == objectIndex);
                if (projectile is not null)
                {
                        var newProjectile = CreateProjectile (ProjectileType.Cannonball, projectile.Index);
                        newProjectile.IsAlive = true;
                        ProjectilePool [ProjectilePool.IndexOf (projectile)] = newProjectile;
                        newProjectile.Delegate = ProjectileDelegate;
                        newProjectile.OnSpawn ();
                        return newProjectile;
                }

                throw new Exception ($"Could not find projectile with index: {objectIndex}");
        }

        public void DespawnProjectile (Projectile projectile)
        {
                projectile.Disable ();
        }

        public void CreatePoolObjects (IGameObjectPoolDelegate @delegate)
        {
                Delegate = @delegate;
                for (var i = 0; i < InitialPoolCount; i++)
                {
                        var newProjectile = CreateProjectile (ProjectileType.Cannonball, null);
                        ProjectilePool.Add (newProjectile);
                }
        }

        public Projectile CreateProjectile (ProjectileType projectileType, int? index)
        {
                if (Delegate is null)
                {
                        throw new Exception ("No Delegate");
                }

                Projectile? projectile = null;
                switch (projectileType)
                {
                        case ProjectileType.Cannonball:
                                projectile = TrailBallProjectile.Create (cannonball, index, Delegate.GameDefinitions);
                                break;
                        // Add other projectile types here as needed
                        case ProjectileType.Chicken:
                                projectile = ChickenProjectile.Create (chicken, index, Delegate.GameDefinitions);
                                break;
                        default:
                                throw new Exception ("Trying to get .none projectile");
                }

                projectile.AddComponent (new RemoveWhenFallenComponent ());
                return projectile;
        }

        #region IDisposable 

        bool isDisposed; // To detect redundant calls

        protected virtual void Dispose (bool disposing)
        {
                if (!isDisposed)
                {
                        if (disposing)
                        {
                                chicken.Dispose ();
                                cannonball.Dispose ();
                        }

                        isDisposed = true;
                }
        }

        public void Dispose ()
        {
                Dispose (true);
                GC.SuppressFinalize (this);
        }

        #endregion
}
