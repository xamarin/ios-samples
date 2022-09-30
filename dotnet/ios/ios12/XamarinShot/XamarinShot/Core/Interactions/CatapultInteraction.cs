using XamarinShot.Models.Interactions;

namespace XamarinShot.Models;

/// <summary>
/// Abstract:
/// User interaction for the slingshot.
/// </summary>
public class CatapultInteraction : IInteraction, IGrabInteractionDelegate, IVortexActivationDelegate
{
        readonly Dictionary<int, Catapult> catapults = new Dictionary<int, Catapult> ();

        // this is a ball that doesn't have physics
        readonly SCNNode dummyBall;

        GrabInteraction? grabInteraction;

        public CatapultInteraction (IInteractionDelegate @delegate)
        {
                Delegate = @delegate;
                dummyBall = SCNNodeExtensions.LoadSCNAsset ("projectiles_ball_8k");

                // stri the geometry out of a low-lod model, assume it doesn't have an lod
                if (dummyBall.Geometry is not null)
                {
                        var lod = SCNNodeExtensions.LoadSCNAsset ("projectiles_ball");
                        if (lod.Geometry is not null)
                        {
                                lod.Geometry.Materials = dummyBall.Geometry.Materials;

                                // this radius will be replaced by fixLevelsOfDetail
                                // when the level is placed
                                dummyBall.Geometry.LevelsOfDetail = new SCNLevelOfDetail [] { SCNLevelOfDetail.CreateWithScreenSpaceRadius (lod.Geometry, 100f) };
                        }
                }
        }

        public IInteractionDelegate Delegate { get; private set; }

        public GrabInteraction? GrabInteraction
        {
                get
                {
                        return grabInteraction;
                }

                set
                {
                        grabInteraction = value;
                        // Add hook up to grabInteraction delegate automatically
                        if (grabInteraction is not null)
                        {
                                grabInteraction.GrabDelegate = this;
                        }
                }
        }

        public void AddCatapult (Catapult catapult)
        {
                if (grabInteraction is null)
                {
                        throw new Exception ("GrabInteraction not set");
                }

                grabInteraction.AddGrabbable (catapult);
                catapults [catapult.CatapultId] = catapult;
                SetProjectileOnCatapult (catapult, ProjectileType.Cannonball);
        }

        void SetProjectileOnCatapult (Catapult catapult, ProjectileType projectileType)
        {
                if (Delegate is null)
                {
                        throw new Exception ("No delegate");
                }

                var projectile = TrailBallProjectile.Create (dummyBall.Clone ());
                projectile.IsAlive = true;
                projectile.Team = catapult.Team;

                if (projectile.PhysicsNode is null)
                {
                        throw new Exception ("Projectile has no physicsNode");
                }

                projectile.PhysicsNode.PhysicsBody = null;
                Delegate.AddNodeToLevel (projectile.PhysicsNode);

                catapult.SetProjectileType (projectileType, projectile.ObjectRootNode!);
        }

        public bool CanGrabAnyCatapult (Ray cameraRay)
        {
                return catapults.Any (keyValuePair => keyValuePair.Value.CanGrab (cameraRay));
        }

        #region Interactions

        public void Update (CameraInfo cameraInfo)
        {
                foreach (var catapult in catapults.Values)
                {
                        catapult.Update ();
                }
        }

        #endregion

        #region Game Action Handling

        public void Handle (GameActionType gameAction, Player player)
        {
                if (gameAction.Type == GameActionType.GActionType.CatapultRelease)
                {
                        if (Delegate is null)
                        {
                                throw new Exception ("No Delegate");
                        }

                        if (gameAction.CatapultRelease is not null)
                                HandleCatapultReleaseAction (gameAction.CatapultRelease, player, Delegate);
                }
        }

        void ReleaseCatapultGrab (int catapultId)
        {
                if (!catapults.TryGetValue (catapultId, out Catapult? catapult))
                {
                        throw new Exception ($"No catapult {catapultId}");
                }

                if (grabInteraction is null)
                {
                        throw new Exception ("GrabInteraction not set");
                }

                catapult.IsGrabbed = false;

                // Empty grabbedCatapult if this catapult was grabbed by player
                if (grabInteraction.GrabbedGrabbable is Catapult grabbedCatapult && grabbedCatapult.CatapultId == catapultId)
                {
                        grabInteraction.GrabbedGrabbable = null;
                }
        }

        void HandleCatapultReleaseAction (SlingData data, Player player, IInteractionDelegate @delegate)
        {
                if (catapults.TryGetValue (data.CatapultId, out Catapult? catapult))
                {
                        catapult.OnLaunch (GameVelocity.Zero);
                        ReleaseCatapultGrab (data.CatapultId);
                }
        }

        #endregion

        #region Grab Interaction Delegate

        public bool ShouldForceRelease (IGrabbable grabbable)
        {
                if (grabbable is Catapult catapult)
                {
                        return catapult.IsPulledTooFar || IsCatapultFloating (catapult);
                } else {
                        throw new Exception ("Grabbable is not catapult");
                }
        }

        bool IsCatapultFloating (Catapult catapult)
        {
                if (Delegate is null)
                {
                        throw new Exception ("No Delegate");
                }

                if (catapult.Base.PhysicsBody is not null)
                {
                        var contacts = Delegate.PhysicsWorld.ContactTest (catapult.Base.PhysicsBody, (SCNPhysicsTest)null);
                        return !contacts.Any ();
                } else {
                        return false;
                }
        }

        public void OnGrabStart (IGrabbable grabbable, CameraInfo cameraInfo, Player player)
        {
                if (grabbable is Catapult catapult)
                {
                        catapult.OnGrabStart ();
                }
        }

        public void OnServerRelease (IGrabbable grabbable, CameraInfo cameraInfo, Player player)
        {
                if (Delegate is null)
                {
                        throw new Exception ("No Delegate");
                }

                if (grabbable is Catapult catapult)
                {
                        // Launch the ball
                        var velocity = catapult.TryGetLaunchVelocity (cameraInfo);
                        if (velocity is not null)
                        {
                                catapult.OnLaunch (velocity);
                                SlingBall (catapult, velocity);
                                catapult.ReleaseSlingGrab ();

                                ReleaseCatapultGrab (catapult.CatapultId);

                                var slingData = new SlingData (catapult.CatapultId, catapult.ProjectileType, velocity);

                                // succeed in launching catapult, notify all clients of the update
                                Delegate.ServerDispatchActionToAll (new GameActionType { CatapultRelease = slingData, Type = GameActionType.GActionType.CatapultRelease });
                        }
                }
        }

        public void OnServerGrab (IGrabbable grabbable, CameraInfo cameraInfo, Player player)
        {
                if (grabbable is Catapult catapult)
                {
                        catapult.ServerGrab (cameraInfo.Ray);
                }
        }

        public void OnUpdateGrabStatus (IGrabbable grabbable, CameraInfo cameraInfo)
        {
                if (grabbable is Catapult catapult)
                {
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
                if (gameObject is not null && gameObject is Catapult catapult)
                {
                        var otherGameObject = otherNode.NearestParentGameObject ();
                        if (otherGameObject is not null)
                        {
                                // Projectile case
                                if (otherGameObject is Projectile projectile)
                                {
                                        var catapultNode = catapult.Base;
                                        if (catapultNode.PhysicsBody is null)
                                        {
                                                throw new Exception ("Catapult has no physicsBody");
                                        }

                                        // Do not let projectile from the same team kill the catapult
                                        if (catapult.Team == projectile.Team)
                                        {
                                                catapultNode.PhysicsBody.Velocity = SCNVector3.Zero;
                                                catapultNode.PhysicsBody.AngularVelocity = SCNVector4.UnitY;
                                        }
                                }

                                // Server tries to release the catapult if it got impulse from block or projectile
                                if (impulse > MinImpuseToReleaseCatapult)
                                {
                                        if (Delegate is null)
                                        {
                                                throw new Exception ("No delegate");
                                        }

                                        if (Delegate.IsServer)
                                        {
                                                // Any game objects (blocks or projectiles) case
                                                var data = new GrabInfo (catapult.CatapultId, catapult.LastCameraInfo);
                                                Delegate.DispatchActionToServer (new GameActionType { TryRelease = data, Type = GameActionType.GActionType.TryRelease });
                                        }
                                }
                        }
                }
        }

        #endregion

        #region Sling Ball

        public void SlingBall (Catapult catapult, GameVelocity velocity)
        {
                if (Delegate is null)
                {
                        throw new Exception ("No delegate");
                }

                var newProjectile = Delegate.SpawnProjectile ();
                newProjectile.Team = catapult.Team;

                Delegate.AddNodeToLevel (newProjectile.ObjectRootNode!);

                // The lifeTime of projectile needed to sustain the pool is defined by:
                // (Catapult Count) * (1 + (lifeTime) / (cooldownTime)) = (Pool Count)
                var poolCount = Delegate.GameObjectPoolCount ();
                var lifeTime = (double)(poolCount / catapults.Count - 1) * catapult.CoolDownTime;

                newProjectile.Launch (velocity, lifeTime, Delegate.ProjectileDelegate);

                // assign the catapult source to this ball
                if (newProjectile.PhysicsNode?.PhysicsBody is not null)
                {
                        newProjectile.PhysicsNode.SetValueForKey (NSObject.FromObject (catapult.CatapultId), new NSString ("Source"));
                        newProjectile.PhysicsNode.PhysicsBody.CollisionBitMask = (int)(CollisionMask.RigidBody | CollisionMask.GlitterObject);
                        if (catapult.Team == Team.TeamA)
                        {
                                var collisionMask = (CollisionMask)(int)(newProjectile.PhysicsNode.PhysicsBody.CollisionBitMask);
                                newProjectile.PhysicsNode.PhysicsBody.CollisionBitMask = (nuint)(int)(collisionMask | CollisionMask.CatapultTeamB);

                                var categoryBitMask = (CollisionMask)(int)(newProjectile.PhysicsNode.PhysicsBody.CategoryBitMask);
                                newProjectile.PhysicsNode.PhysicsBody.CategoryBitMask = (nuint)(int)(categoryBitMask | CollisionMask.CatapultTeamA);
                        }
                        else
                        {
                                var collisionMask = (CollisionMask)(int)(newProjectile.PhysicsNode.PhysicsBody.CollisionBitMask);
                                newProjectile.PhysicsNode.PhysicsBody.CollisionBitMask = (nuint)(int)(collisionMask | CollisionMask.CatapultTeamA);

                                var categoryBitMask = (CollisionMask)(int)(newProjectile.PhysicsNode.PhysicsBody.CategoryBitMask);
                                newProjectile.PhysicsNode.PhysicsBody.CategoryBitMask = (nuint)(int)(categoryBitMask | CollisionMask.CatapultTeamB);
                        }
                }
        }

        public void HandleTouch (TouchType type, Ray camera) { }

        #endregion

        #region IVortexActivationDelegate

        public void VortexDidActivate (VortexInteraction vortex)
        {
                // Kill all catapults when vortex activates
                if (Delegate is null)
                {
                        throw new Exception ("No delegate");
                }

                foreach (var catapult in catapults.Values)
                {
                        var data = new HitCatapult (catapult.CatapultId, false, true);
                        Delegate.DispatchActionToAll (new GameActionType { CatapultKnockOut = data, Type = GameActionType.GActionType.HitCatapult });
                }
        }

        #endregion
}
