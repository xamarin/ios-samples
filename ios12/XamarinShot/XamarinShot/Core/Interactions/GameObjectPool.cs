
namespace XamarinShot.Models
{
    using SceneKit;
    using XamarinShot.Models.Enums;
    using XamarinShot.Models.GameplayState;
    using XamarinShot.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
        private readonly SCNNode cannonball;

        private readonly SCNNode chicken;

        public GameObjectPool()
        {
            this.cannonball = SCNNodeExtensions.LoadSCNAsset("projectiles_ball");
            this.chicken = SCNNodeExtensions.LoadSCNAsset("projectiles_chicken");

            this.InitialPoolCount = 30;
        }

        public IGameObjectPoolDelegate Delegate { get; set; }

        public IProjectileDelegate ProjectileDelegate { get; set; }

        public List<Projectile> ProjectilePool { get; } = new List<Projectile>();

        public int InitialPoolCount { get; private set; } = 0;

        public Projectile SpawnProjectile() 
        {
            var count = 0;
            foreach(var projectile in this.ProjectilePool.Where(projectile => !projectile.IsAlive))
            {
                count += 1;
            }

            foreach (var projectile in this.ProjectilePool.Where(projectile => !projectile.IsAlive))
            {
                return this.SpawnProjectile(projectile.Index);
            }

            throw new Exception("No more free projectile in the pool");
        }

        /// <summary>
        /// Spawn projectile with specific object index
        /// </summary>
        public Projectile SpawnProjectile(int objectIndex)
        {
            if(this.Delegate == null)
            {
                throw new Exception("No Delegate");
            }

            this.Delegate.OnSpawnedProjectile();

            var projectile = this.ProjectilePool.FirstOrDefault(tile => tile.Index == objectIndex);
            if(projectile != null)
            {
                var newProjectile = this.CreateProjectile(ProjectileType.Cannonball, projectile.Index);
                newProjectile.IsAlive = true;
                this.ProjectilePool[this.ProjectilePool.IndexOf(projectile)] = newProjectile;
                newProjectile.Delegate = this.ProjectileDelegate;
                newProjectile.OnSpawn();
                return newProjectile;
            }

            throw new Exception($"Could not find projectile with index: {objectIndex}");
        }

        public void DespawnProjectile(Projectile projectile)
        {
            projectile.Disable();
        }

        public void CreatePoolObjects(IGameObjectPoolDelegate @delegate) 
        {
            this.Delegate = @delegate;
            for (var i = 0; i < this.InitialPoolCount; i++)
            {
                var newProjectile = this.CreateProjectile(ProjectileType.Cannonball, null);
                this.ProjectilePool.Add(newProjectile);
            }
        }

        public Projectile CreateProjectile(ProjectileType projectileType, int? index)
        {
            if (this.Delegate == null)
            {
                throw new Exception("No Delegate");
            }

            Projectile projectile = null;
            switch (projectileType)
            {
                case ProjectileType.Cannonball:
                    projectile = TrailBallProjectile.Create(this.cannonball, index, this.Delegate.GameDefinitions);
                    break;
                    // Add other projectile types here as needed
                case ProjectileType.Chicken:
                    projectile = ChickenProjectile.Create(this.chicken, index, this.Delegate.GameDefinitions);
                    break;
                default:
                    throw new Exception("Trying to get .none projectile");
            }

            projectile.AddComponent(new RemoveWhenFallenComponent());
            return projectile;
        }

        #region IDisposable 

        private bool isDisposed; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.chicken.Dispose();
                    this.cannonball.Dispose();
                }

                this.isDisposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(true);
        }

        #endregion
    }
}