using XamarinShot.Models.GameplayState;
using XamarinShot.Models.Interactions;

namespace XamarinShot.Models;


public enum Team
{
        None = 0, // default
        TeamA,
        TeamB,
}

public static class TeamExtensions
{
        public static string GetDescription (this Team team)
        {
                switch (team)
                {
                        case Team.None: return NSBundle.MainBundle.GetLocalizedString ("none");
                        case Team.TeamA: return NSBundle.MainBundle.GetLocalizedString ("Blue");
                        case Team.TeamB: return NSBundle.MainBundle.GetLocalizedString ("Yellow");
                }
                throw new NotImplementedException ();
        }

        public static UIColor GetColor (this Team team)
        {
                switch (team)
                {
                        case Team.None: return UIColor.White;
                        case Team.TeamA: return UIColorExtensions.Create (45, 128, 208);
                        case Team.TeamB: return UIColorExtensions.Create (239, 153, 55);
                }
                throw new NotImplementedException ();
        }
}

public static class UIColorExtensions
{
        public static UIColor Create (byte hexRed, byte green, byte blue)
        {
                var fred = (float)hexRed / 255f;
                var fgreen = (float)green / 255f;
                var fblue = (float)blue / 255f;

                return new UIColor (fred, fgreen, fblue, 1f);
        }
}

public interface ICatapultDelegate
{
        void DidBeginGrab (Catapult catapult);
        void DidMove (Catapult catapult, float stretchDistance, float stretchRate);
        void DidLaunch (Catapult catapult, GameVelocity velocity);
        void DidBreak (Catapult catapult, bool justKnockedout, bool vortex);
}

public class CatapultProperties
{
        // angle of rotation around base from 0
        public double MinYaw { get; set; } = -45.0;
        public double MaxYaw { get; set; } = 45.0;

        // angle of rotation up/down to angle shots
        public double MinPitch { get; set; } = -30.0;
        public double MaxPitch { get; set; } = 30.0;

        // when launched, the pull distance is normalized and scaled to this (linear not exponential power)
        public double MinVelocity { get; set; } = 1.0; // so the ball doesn't just drop and take out your own stuff
        public double MaxVelocity { get; set; } = 50.0;

        // when pulled, these are the min/max stretch of pull
        public double MinStretch { get; set; } = 0.05; // always have some energy, so ball doesn't just drop
        public double MaxStretch { get; set; } = 5.0; // ball will poke through sling if bigger

        // these animations play with these times
        public double GrowAnimationTime { get; set; } = 0.2;
        public double DropAnimationTime { get; set; } = 0.3;
        public double GrabAnimationTime { get; set; } = 0.15; // don't set to 0.2, ball/sling separate

        // before another ball appears, there is a cooldown and the grow/drop animation
        public double CooldownTime { get; set; } = 3.0;

        // how close payer has to be from the pull to grab it
        public double PickRadius { get; set; } = 5.0;
}

public enum StrapVisible
{
        Hidden,
        Visible,
}

public enum BallVisible
{
        Hidden,
        Partial,
        Visible,
}

// Catapult provides an interface which users can use to manipulate the sling
// CatapultInteraction which represents player's interaction with the catapult use these interfaces to manipulate catapult locally
// Network sync is handled by CatapultInteraction
public class Catapult : GameObject, IGrabbable
{
        // convenience for idenfifying catapult during collisions
        const string CollisionKey = "id";

        readonly CatapultProperties properties = new CatapultProperties ();

        // highlight assistance
        readonly UIColor highlightColor = UIColor.White;

        // for audio:
        readonly AVAudioEnvironmentNode audioEnvironment;

        readonly SCNNode? catapultStrap;

        // Can drop the ball from the active to below with a transaction.
        // Below rests on the strap, and above is above that at the top of the fork.
        // This must be finish before the catapult can be grabbed, or the transition
        // animations will compete.
        readonly SCNNode? ballOriginInactiveAbove;
        readonly SCNNode? ballOriginInactiveBelow;

        // This is a rope simulator only for the new catapult.
        readonly CatapultRope rope;

        // original world position of sling to restore the position to
        SCNVector3 baseWorldPosition = SCNVector3.Zero;

        // Projectiles are fired out of the local -Z axis direction. (worldFront)
        // This is the upper center of the base where projectiles are fired through.
        SCNNode? pullOrigin;

        bool ballCanBeGrabbed;

        // The starting position of the player when they grab the pull
        SCNVector3 playerWorldPosition = SCNVector3.Zero;

        // Can only pull back the slingshot for now.  Eventually will be able to direct shots within a cone.
        double stretch = 0d;
        double lastStretch = 0d;
        double lastStretchTime = 0d;

        // Track the start of the grab.  Can use for time exceeded auto-launch.
        double firstGrabTime = 0d;

        // Stores the last launch of a projectile.  Cooldown while sling animates and bounces back.
        double lastLaunchTime = 0d;

        SCNVector3 projectileScale = SCNVector3.One;

        public Catapult (SCNNode node, SFXCoordinator sfxCoordinator, int identifier, Dictionary<string, object> gamedefs) : base (node, null, gamedefs, true, false)
        {
                Base = node;
                audioEnvironment = sfxCoordinator.AudioEnvironment;

                // Base teamID and name off looking up teamA or teamB folder in the level parents
                // This won't work on the old levels.
                Team = Base.GetTeam ();
                TeamName = Team.GetDescription ();

                // have team id established
                Base.SetPaintColors ();

                // correct for the pivot point to place catapult flat on ground
                Base.Position = new SCNVector3 (Base.Position.X, Base.Position.Y - 0.13f, Base.Position.Z);

                // highlight setup
                HighlightObject = node.FindChildNode ("Highlight", true);
                if (HighlightObject is not null)
                {
                        HighlightObject = HighlightObject.FindNodeWithGeometry ();
                }

                // hide the highlights on load
                if (HighlightObject is not null)
                {
                        HighlightObject.Hidden = true;
                }

                if (HighlightObject?.Geometry?.FirstMaterial?.Diffuse?.Contents is UIColor color)
                {
                        highlightColor = color;
                }

                // they should only have y orientation, nothing in x or z
                // current scene files have the catapults with correct orientation, but the
                // eulerAngles are different - x and z are both π, y is within epsilon of 0
                // That's from bad decomposition of the matrix.  Need to restore the eulerAngles from the source.
                // Especially if we have animations tied to the euler angles.
                if (Math.Abs (node.EulerAngles.X) > 0.001f || Math.Abs (node.EulerAngles.Z) > 0.001f)
                {
                        //Console.WriteLine("Catapult can only have y rotation applied");
                }

                // where to place the ball so it sits on the strap
                catapultStrap = Base.FindChildNode ("catapultStrap", true);
                if (catapultStrap is null)
                {
                        throw new Exception ("No node with name catapultStrap");
                }

                // this only rotates, and represents the center of the catapult through which to fire
                pullOrigin = Base.FindChildNode ("pullOrigin", true);
                if (pullOrigin is null)
                {
                        throw new Exception ("No node with name pullOrigin");
                }

                // This is a rope simulation meant for a fixed catapult, the catapult rotates.
                rope = new CatapultRope (node);

                // attach ball to the inactive strap, search for ballOriginInactiveBelow
                ballOriginInactiveBelow = Base.FindChildNode ("ballOriginInactiveBelow", true);
                if (ballOriginInactiveBelow is null)
                {
                        throw new Exception ("No node with name ballOriginInactiveBelow");
                }

                ballOriginInactiveAbove = Base.FindChildNode ("ballOriginInactiveAbove", true);
                if (ballOriginInactiveAbove is null)
                {
                        throw new Exception ("No node with name ballOriginInactiveAbove");
                }

                // ball will be made visible and drop once projectile is set and cooldown exceeded
                StrapVisible = StrapVisible.Visible;

                CatapultId = identifier;

                Base.SetValueForKey (NSObject.FromObject (CatapultId), new NSString (Catapult.CollisionKey));

                AudioPlayer = new CatapultAudioSampler (Base, sfxCoordinator);

                // use the teamID to set the collision category mask
                if (PhysicsNode?.PhysicsBody is not null)
                {
                        if (Team == Team.TeamA)
                        {
                                PhysicsNode.PhysicsBody.CategoryBitMask = (int)CollisionMask.CatapultTeamA;
                                var collisionBitMask = (CollisionMask)(int)PhysicsNode.PhysicsBody.CollisionBitMask;
                                PhysicsNode.PhysicsBody.CollisionBitMask = (nuint)(int)(collisionBitMask | CollisionMask.CatapultTeamB);
                        } else if (Team == Team.TeamB) {
                                PhysicsNode.PhysicsBody.CategoryBitMask = (int)CollisionMask.CatapultTeamB;
                                var collisionBitMask = (CollisionMask)(int)PhysicsNode.PhysicsBody.CollisionBitMask;
                                PhysicsNode.PhysicsBody.CollisionBitMask = (nuint)(int)(collisionBitMask | CollisionMask.CatapultTeamA);
                        }
                }
        }

        public Catapult (NSCoder coder) : base (coder) => throw new NotImplementedException ("init(coder:) has not been implemented");

        public double CoolDownTime => properties.CooldownTime;

        public SCNNode Base { get; }

        public ICatapultDelegate? Delegate { get; set; }

        public bool IsPulledTooFar { get; private set; }

        public CameraInfo LastCameraInfo { get; set; } = new CameraInfo (SCNMatrix4.Identity);

        public SCNNode? Projectile { get; private set; }

        public ProjectileType ProjectileType { get; private set; } = ProjectileType.None;

        public bool Disabled { get; private set; }

        public CatapultAudioSampler AudioPlayer { get; private set; }

        // Each catapult has a unique index.
        // 1-3 are on one side, 4-6 are on the other side
        public int CatapultId { get; private set; } = 0;

        public Team Team { get; private set; } = Team.None;

        public string TeamName { get; private set; }

        // highlight assistance
        public SCNVector3 ProjectedPosition { get; set; } = SCNVector3.Zero;

        public SCNNode? HighlightObject { get; private set; }

        /// <summary>
        /// Grabbable Id to be set by GrabInteraction
        /// </summary>
        public int GrabbableId { get; set; } = 0;

        // Player who grabbed a catapult. A player can only operate one catapult at a time.
        // Players/teams may be restricted to a set of catapults.
        // Note: Player setting is managed by CatapultInteraction which will resolves sync.
        //       Player do not get set in clients, since clients do not need to know who grabbed the catapult,
        //       whereas server needs to know the player owning the catapult to avoid conflicts.
        public Player? Player { get; set; }

        private bool isGrabbed;

        public bool IsGrabbed
        {
                get
                {
                        return isGrabbed;
                }

                // In the case of clients, actual player owner does not need to be known
                set
                {
                        isGrabbed = value;
                        if (!isGrabbed)
                        {
                                Player = null;
                        }
                }
        }

        /// <summary>
        /// Highlight assistance
        /// </summary>
        /// <value><c>true</c> if is visible; otherwise, <c>false</c>.</value>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Highlight assistance
        /// </summary>
        public bool IsHighlighted { get; private set; }

        StrapVisible strapVisible = StrapVisible.Hidden;
        public StrapVisible StrapVisible
        {
                get
                {
                        return strapVisible;
                }

                set
                {
                        strapVisible = value;
                        UpdateStrapVisibility ();
                }
        }

        void UpdateStrapVisibility ()
        {
                if (catapultStrap is null)
                        return;
                switch (strapVisible)
                {
                        case StrapVisible.Hidden:
                                catapultStrap.Hidden = true;
                                break;

                        case StrapVisible.Visible:
                                catapultStrap.Hidden = false;
                                break;
                }
        }

        BallVisible ballVisible = BallVisible.Hidden;
        /// <summary>
        /// Whether the ball in the sling is visible or partially visible.
        /// </summary>
        public BallVisible BallVisible
        {
                get
                {
                        return ballVisible;
                }

                set
                {
                        ballVisible = value;
                        UpdateFakeProjectileVisibility ();
                }
        }

        void UpdateFakeProjectileVisibility ()
        {
                switch (ballVisible)
                {
                        case BallVisible.Hidden:
                                if (Projectile is not null)
                                {
                                        Projectile.Opacity = 1f;
                                        Projectile.Hidden = true;
                                        Projectile.WorldPosition = ballOriginInactiveAbove?.WorldPosition ?? SCNVector3.One;
                                        Projectile.Scale = new SCNVector3 (0.01f, 0.01f, 0.01f);
                                }
                                break;

                        case BallVisible.Partial:
                                if (Projectile is not null)
                                {
                                        Projectile.Opacity = 1f;
                                        Projectile.Hidden = false;
                                        AnimateBallGrowAndDrop ();
                                }
                                break;

                        case BallVisible.Visible:
                                if (Projectile is not null)
                                {
                                        Projectile.Opacity = 1f;
                                        Projectile.Hidden = false;
                                        // it's in the strap fromn .partial animation
                                }
                                break;
                }
        }

        public void AnimationRopeToRestPose (double duration)
        {
                rope.InterpolateToRestPoseAnimation (duration);
        }

        public void AnimateBallGrowAndDrop ()
        {
                // the block is the total time of the transcation, so sub-blocks are limited by that too
                SCNTransaction.Begin ();
                SCNTransaction.AnimationDuration = properties.GrowAnimationTime;

                // correct the rope sim by animating back to reset pose no matter what
                var fixupLaunchAnimationTime = 0.1f;
                rope.InterpolateToRestPoseAnimation (fixupLaunchAnimationTime);

                // first scale the ball from small to original size
                if (Projectile is not null)
                {
                        Projectile.Scale = projectileScale;
                }

                SCNTransaction.SetCompletionBlock (() =>
                {
                        // after back to scale, then run the ball drop
                        SCNTransaction.Begin ();
                        SCNTransaction.AnimationDuration = properties.DropAnimationTime;

                        // next drop from ballOriginInactiveAbove to ballOriginInactive
                        if (Projectile is not null)
                        {
                                Projectile.WorldPosition = ballOriginInactiveBelow?.WorldPosition ?? SCNVector3.One;
                        }

                        SCNTransaction.SetCompletionBlock (() =>
                        {
                                // only allow the ball to be grabbed after animation completes
                                ballCanBeGrabbed = true;
                        });

                        SCNTransaction.Commit ();
                });

                SCNTransaction.Commit ();
        }

        /// <summary>
        /// Distance away from catapult base
        /// </summary>
        public float DistanceFrom (SCNVector3 worldPosition)
        {
                var distance = worldPosition - Base.WorldPosition;
                return distance.Length;
        }

        public static SCNNode ReplaceCatapultPlaceholder (SCNNode placeholder)
        {
                var node = SCNNodeExtensions.LoadSCNAsset ("catapult");

                // somehow setting the world transform doesn't update the Euler angles (180, 0, 180) is decoded
                //  but need it to be 0, 180, 0
                node.Transform = placeholder.Transform;
                node.EulerAngles = placeholder.EulerAngles;

                // Add physics body to it
                node.WorldPosition += new SCNVector3 (0f, 0.2f, 0f);
                node.PhysicsBody?.ResetTransform ();

                var baseGeomNode = node.FindChildNode ("catapultBase", true);
                if (baseGeomNode is null)
                {
                        throw new Exception ("No catapultBase");
                }

                var prongGeomNode = node.FindChildNode ("catapultProngs", true);
                if (prongGeomNode is null)
                {
                        throw new Exception ("No catapultProngs");
                }

                // shift center of mass of the prong from the bottom
                // the 0.55 value is from experimentation
                var prongPivotShiftUp = new SCNVector3 (0f, 0.55f, 0f);
                prongGeomNode.Pivot = SCNMatrix4.CreateTranslation (prongPivotShiftUp);
                prongGeomNode.Position += prongPivotShiftUp;

                var baseShape = SCNPhysicsShape.Create (baseGeomNode, new SCNPhysicsShapeOptions () { ShapeType = SCNPhysicsShapeType.ConvexHull });
                var prongShape = SCNPhysicsShape.Create (prongGeomNode, new SCNPhysicsShapeOptions () { ShapeType = SCNPhysicsShapeType.ConvexHull });

                var compoundShape = SCNPhysicsShape.Create (new SCNPhysicsShape [] { baseShape, prongShape },
                                                           new SCNMatrix4 [] { SCNMatrix4.Identity, SCNMatrix4.Identity });
                if (node.PhysicsBody is not null)
                {
                        node.PhysicsBody.PhysicsShape = compoundShape;
                }

                // rename back to placeholder name must happen after gameObject is assigned
                // currently placeholders are all Catapult1 to Catapult6, they may be under a teamA, teamB parent
                // so stash the placeholder name for later
                if (!string.IsNullOrEmpty (placeholder.Name))
                {
                        node.SetValueForKey (new NSString (placeholder.Name), new NSString ("nameRestore"));
                }

                placeholder.ParentNode?.ReplaceChildNode (placeholder, node);

                node.Name = "catapult";

                return node;
        }

        public void SetProjectileType (ProjectileType projectileType, SCNNode projectile)
        {
                Projectile?.RemoveFromParentNode ();
                Projectile = projectile;
                ProjectileType = projectileType;
                projectileScale = projectile.Scale;

                // the rope adjusts to the radius of the ball
                var projectilePaddingScale = 1f;

                nfloat radius = 0f;
                var temp = SCNVector3.Zero;
                projectile.GetBoundingSphere (ref temp, ref radius);
                rope.SetBallRadius ((float)radius * projectilePaddingScale);

                // need ball to set a teamID, and then can color with same mechanism
                //projectile.setPaintColor()

                // will be made visible and drop when cooldown is exceeded,
                // this way ball doesn't change suddenly while visible
                BallVisible = BallVisible.Hidden;
                UpdateFakeProjectileVisibility ();
        }

        public void UpdateProperties ()
        {
                properties.MinStretch = PropDouble ("minStretch")!.Value;
                properties.MaxStretch = PropDouble ("maxStretch")!.Value;

                properties.MinYaw = PropDouble ("minYaw")!.Value;
                properties.MaxYaw = PropDouble ("maxYaw")!.Value;

                properties.MinPitch = PropDouble ("minPitch")!.Value;
                properties.MaxPitch = PropDouble ("maxPitch")!.Value;

                properties.MinVelocity = PropDouble ("minVelocity")!.Value;
                properties.MaxVelocity = PropDouble ("maxVelocity")!.Value;

                properties.CooldownTime = PropDouble ("cooldownTime")!.Value;
                properties.PickRadius = PropDouble ("pickRadius")!.Value;
        }

        #region Catapult Grab

        public bool CanGrab (Ray cameraRay)
        {
                if (isGrabbed)
                {
                        return false;
                }

                if (Disabled)
                {
                        return false;
                }

                // there is a cooldown timer before you can launch again
                // when animation completes this will be set to true
                if (!ballCanBeGrabbed)
                {
                        return false;
                }

                if (!isCatapultStable)
                {
                        return false;
                }

                if (!HitTestPull (cameraRay))
                {
                        return false;
                }

                return true;
        }

        bool HitTestPull (Ray cameraRay)
        {
                // Careful not to make this too large or you'll pick a neighboring slingshot or one across the table.
                // We're not sorting hits across all of the slignshots and picking the smallest, but we visit them all.
                // Within radius behind the ball/pull allow the slingshot to be picked.
                var playerDistanceFromPull = cameraRay.Position - pullOrigin?.WorldPosition ?? SCNVector3.One;

                // This is a linear distance along the current firing direction of the catapult
                // Just using it here to make sure you're behind the pull (the pull is visible).
                var stretchDistance = SCNVector3.Dot (playerDistanceFromPull, -FiringDirection ());

                // make sure player is on positive side of pull + some fudge factor to make sure we can see it
                // to avoid flickering highlight on and off, we add a buffer when highlighted
                if (stretchDistance <= 0.01f && HighlightObject is not null && HighlightObject.Hidden)
                {
                        return false;
                } else if (stretchDistance < -0.03f) {
                        // slack during highlight mode
                        return false;
                }

                // player can be inside a highlight radius or the pick radius
                // one approach is to auto grab when within radius (but facing the catapult)
                if (playerDistanceFromPull.Length > properties.PickRadius)
                {
                        return false;
                }

                return true;
        }

        bool isCatapultStable = true; // Stable means not knocked and not moving
        bool isCatapultKnocked = false; // Knocked means it is either tilted or fell off the table
        SCNVector3? lastPosition;

        double catapultKnockedStartTime = 0d;
        public double CatapultKnockedTime => !isCatapultKnocked ? 0d : GameTime.Time - catapultKnockedStartTime;

        const double CatapultPhysicsSettleTime = 5d;
        const float MinStableTiltBaseUpY = 0.7f;
        const float MaxSpeedToCountAsStable = 0.05f;

        void UpdateCatapultStable ()
        {
                if (!Disabled)
                {
                        // Catapult will be unstable when the physics settles, therefore we do not update catapult's stability status
                        if (GameTime.TimeSinceLevelStart > CatapultPhysicsSettleTime)
                        {
                                // Cannot use simdVelocity on client since simdVelocity could be high from physicsSync interacting with local physics engine
                                if (!lastPosition.HasValue)
                                {
                                        lastPosition = Base.PresentationNode.WorldPosition;
                                        return;
                                }

                                var position = Base.PresentationNode.WorldPosition;
                                var speed = ((position - lastPosition.Value) / (float)GameTime.DeltaTime).Length;
                                lastPosition = position;

                                // Base below table?
                                // Base tilted? base's up vector must maintain some amount of y to be determined as stable
                                var transform = Base.PresentationNode.Transform;
                                transform.Transpose ();
                                var baseUp = SCNVector4.Normalize (transform.Column1);
                                if (position.Y < -1f || Math.Abs (baseUp.Y) < MinStableTiltBaseUpY)
                                {
                                        // Switch to knocked mode
                                        if (!isCatapultKnocked)
                                        {
                                                catapultKnockedStartTime = GameTime.Time;
                                        }

                                        isCatapultKnocked = true;
                                        isCatapultStable = false;
                                        return;
                                }

                                isCatapultKnocked = false;

                                // Base could be moving although the catapult is not knocked
                                isCatapultStable = speed < MaxSpeedToCountAsStable;
                        }
                }
        }

        /// <summary>
        /// When a user has control of a slingshot, no other player can grab it.
        /// </summary>
        public void ServerGrab (Ray cameraRay)
        {
                // Trying to grab catapult with player
                if (!isGrabbed)
                {
                        // do slingshot grab
                        if (Base.GetGameObject ()?.GetComponent (typeof (SlingshotComponent)) is SlingshotComponent slingComponent)
                        {
                                slingComponent.SetGrabMode (true);
                        }
                }
        }

        public void OnGrabStart ()
        {
                // do local effects/haptics if this event was generated by the current player
                Delegate?.DidBeginGrab (this);
        }

        public void OnGrab (CameraInfo cameraInfo)
        {
                // this is now always at the center, so it shouldn't be affected by yaw
                baseWorldPosition = Base.PresentationNode.WorldPosition;

                firstGrabTime = GameTime.Time;

                playerWorldPosition = cameraInfo.Ray.Position;

                var ballPosition = ComputeBallPosition (cameraInfo);
                rope.GrabBall (ballPosition);

                strapVisible = StrapVisible.Visible;
                BallVisible = BallVisible.Visible;

                AlignCatapult (cameraInfo); // rotate the slingshot before we move it
                AnimateGrab (ballPosition);

                IsPulledTooFar = false;
        }

        void AnimateGrab (SCNVector3 ballPosition)
        {
                // here we want to animate the rotation of the current yaw to the new yaw
                // and also animate the strap moving to the center of the view

                // drop from ballOriginInactiveAbove to ballOriginInactive in a transaction
                SCNTransaction.Begin ();
                SCNTransaction.AnimationDuration = properties.GrabAnimationTime;

                // animate the sling and ball to the camera
                rope.UpdateRopeModel ();

                // animate the ball to the player
                if (Projectile is not null)
                {
                        Projectile.WorldPosition = ballPosition;
                }

                SCNTransaction.Commit ();
        }

        readonly DateTime Time1970 = new DateTime (1970, 1, 1);

        public void DoHighlight (bool show, SFXCoordinator? sfxCoordinator)
        {
                if (HighlightObject is not null)
                {
                        IsHighlighted = show;
                        HighlightObject.Hidden = !show;

                        if (show)
                        {
                                var seconds = (DateTime.UtcNow - Time1970).TotalSeconds;
                                var intensity = (float)(Math.Sin (seconds.TruncatingRemainder (1) * 3.1415 * 2.0) * 0.2);
                                if (HighlightObject.Geometry?.FirstMaterial is not null)
                                {
                                        var color = new CIColor (highlightColor);
                                        HighlightObject.Geometry.FirstMaterial.Diffuse.Contents = UIColor.FromRGBA ((Single)DigitExtensions.Clamp (color.Red + intensity, 0, 1),
                                                                                                                        (Single)DigitExtensions.Clamp (color.Green + intensity, 0, 1),
                                                                                                                        (Single)DigitExtensions.Clamp (color.Blue + intensity, 0, 1),
                                                                                                                        (Single)1);
                                }
                        }

                        sfxCoordinator?.CatapultDidChangeHighlight (this, show);
                }
        }

        SCNVector3 FiringDirection ()
        {
                // this can change as the catapult rotates
                return Base.WorldFront;
        }

        #endregion

        #region Sling Move

        SCNVector3 ComputeBallPosition (CameraInfo cameraInfo)
        {
                var cameraRay = cameraInfo.Ray;

                // These should be based on the projectile radius.
                // This affects centering of ball, and can hit near plane of camera
                // This is always centering to one edge of screen independent of screen orient
                // We always want the ball at the bottom of the screen.
                var distancePullToCamera = 0.21f;
                var ballShiftDown = 0.2f;

                var targetBallPosition = cameraRay.Position + cameraRay.Direction * distancePullToCamera;

                var cameraDown = -SCNVector4.Normalize (cameraInfo.Transform.Column1).Xyz;
                targetBallPosition += cameraDown * ballShiftDown;

                // Clamp to only the valid side
                var pullWorldPosition = pullOrigin?.WorldPosition ?? SCNVector3.One;
                if (pullWorldPosition.Z < 0f)
                {
                        targetBallPosition.Z = Math.Min (targetBallPosition.Z, pullWorldPosition.Z);
                } else {
                        targetBallPosition.Z = Math.Max (targetBallPosition.Z, pullWorldPosition.Z);
                }

                // Clamp to cone/circular core
                var yDistanceFromPull = Math.Max (0f, pullWorldPosition.Y - targetBallPosition.Y);
                var minBallDistanceFromPull = 0.5f;
                var pullBlockConeSlope = 1.0f;
                var pullBlockConeRadius = yDistanceFromPull / pullBlockConeSlope;
                var pullBlockCoreRadius = Math.Max (minBallDistanceFromPull, pullBlockConeRadius);

                // if pull is in the core, move it out.
                var pullWorldPositionGrounded = new SCNVector3 (pullWorldPosition.X, 0f, pullWorldPosition.Z);
                var targetPullPositionGrounded = new SCNVector3 (targetBallPosition.X, 0f, targetBallPosition.Z);
                var targetInitialToTargetPull = targetPullPositionGrounded - pullWorldPositionGrounded;

                if (pullBlockCoreRadius > targetInitialToTargetPull.Length)
                {
                        var moveOutDirection = SCNVector3.Normalize (targetInitialToTargetPull);
                        var newTargetPullPositionGrounded = pullWorldPositionGrounded + moveOutDirection * pullBlockCoreRadius;
                        targetBallPosition = new SCNVector3 (newTargetPullPositionGrounded.X, targetBallPosition.Y, newTargetPullPositionGrounded.Z);
                }

                // only use the 2d distance, so that user can gauage stretch indepdent of mtch
                var distance2D = targetBallPosition - pullWorldPosition;
                var stretchY = Math.Abs (distance2D.Y);
                distance2D.Y = 0f;

                var stretchDistance = distance2D.Length;
                stretch = DigitExtensions.Clamp ((double)stretchDistance, properties.MinStretch, properties.MaxStretch);

                // clamp a little bit farther than maxStretch
                // can't let the strap move back too far right now
                var clampedStretchDistance = (float)(1.1d * properties.MaxStretch);
                if (stretchDistance > clampedStretchDistance)
                {
                        targetBallPosition = (clampedStretchDistance / stretchDistance) * (targetBallPosition - pullWorldPosition) + pullWorldPosition;
                        stretchDistance = clampedStretchDistance;
                }

                // Make this optional, not required.  You're often at max stretch.
                // Also have a timer for auto-launch.  This makes it very difficuilt to test
                // storing state in member data
                IsPulledTooFar = stretchDistance > (float)(properties.MaxStretch) || stretchY > (float)(properties.MaxStretch);

                return targetBallPosition;
        }

        void AlignCatapult (CameraInfo cameraInfo)
        {
                if (pullOrigin is null)
                        return;
                var targetBallPosition = ComputeBallPosition (cameraInfo);

                // Set catapult position
                var catapultFront = pullOrigin.WorldPosition - targetBallPosition;
                catapultFront.Y = 0f;
                Base.WorldPosition = baseWorldPosition;
                Base.Look (baseWorldPosition + catapultFront);

                if (Base.PhysicsBody is not null)
                {
                        Base.PhysicsBody.AffectedByGravity = false;
                        Base.PhysicsBody.ResetTransform ();
                }
        }

        /// <summary>
        /// As players move, track the stretch of the sling.
        /// </summary>
        public void Move (CameraInfo cameraInfo)
        {
                // move actions can be processed only after the catapult has been released
                if (isGrabbed)
                {
                        // trying to move before grabbing catapult

                        LastCameraInfo = cameraInfo;
                        var targetBallPosition = ComputeBallPosition (cameraInfo);

                        // Set catapult position
                        var catapultFront = pullOrigin!.WorldPosition - targetBallPosition;
                        catapultFront.Y = 0f;
                        Base.WorldPosition = baseWorldPosition;
                        Base.Look (baseWorldPosition + catapultFront);

                        if (Base.PhysicsBody is not null)
                        {
                                Base.PhysicsBody.AffectedByGravity = false;
                                Base.PhysicsBody.ResetTransform ();
                        }

                        if (Projectile is null)
                        {
                                throw new Exception ("Grabbed but no projectile");
                        }

                        Projectile.WorldPosition = targetBallPosition;

                        rope.MoveBall (targetBallPosition);

                        // calculate the change in stretch position and rate for audio:
                        var stretchRate = 0f;
                        if (GameTime.Time - lastStretchTime > 0)
                        {
                                stretchRate = (float)((stretch - lastStretch) / (GameTime.Time - lastStretchTime));
                        }

                        Delegate?.DidMove (this, (float)stretch, stretchRate);

                        lastStretch = stretch;
                        lastStretchTime = GameTime.Time;
                }
        }

        #endregion

        #region Catapult Launch

        public void OnLaunch (GameVelocity velocity)
        {
                if (isGrabbed)
                {
                        // can't grab again until the cooldown animations play
                        ballCanBeGrabbed = false;

                        // update local information for current player if that is what is pulling the catapult

                        // start the launch animation
                        rope.LaunchBall ();

                        // must reset the move to distance 0 before the launch, otherwise it will start a new
                        // stretch sound.
                        Delegate?.DidMove (this, 0f, 0f);
                        Delegate?.DidLaunch (this, velocity);

                        // set the ball to invisible
                        BallVisible = BallVisible.Hidden;

                        // record the last launch time, and enforce a cooldown before ball reappears (need an update call then?)
                        lastLaunchTime = GameTime.Time;
                }
        }

        public GameVelocity? TryGetLaunchVelocity (CameraInfo cameraInfo)
        {
                GameVelocity? result = null;

                if (Projectile is null)
                {
                        throw new Exception ("Trying to launch without a ball");
                }

                // Move the catapult to make sure that it is moved at least once before launch (prevent NaN in launch direction)
                Move (cameraInfo);

                var stretchNormalized = DigitExtensions.Clamp ((stretch - properties.MinStretch) / (properties.MaxStretch - properties.MinStretch), 0.0, 1.0);

                // this is a lerp
                var velocity = (float)(properties.MinVelocity * (1d - stretchNormalized) + properties.MaxVelocity * stretchNormalized);

                var launchDir = SCNVector3.Normalize (pullOrigin!.WorldPosition - Projectile.WorldPosition);
                if (!launchDir.HasNaN ())
                {
                        var liftFactor = 0.05f * Math.Abs (1f - SCNVector3.Dot (launchDir, SCNVector3.UnitY)); // used to keep ball in air longer
                        var lift = SCNVector3.UnitY * velocity * liftFactor;

                        result = new GameVelocity (Projectile.WorldPosition, launchDir * velocity + lift);
                }

                return result;
        }

        public void ReleaseSlingGrab ()
        {
                // restore the pull back to resting state
                // do this by calling slingshot release
                if (Base.GetGameObject ()?.GetComponent (typeof (SlingshotComponent)) is SlingshotComponent slingComponent)
                {
                        slingComponent.SetGrabMode (false);
                }

                if (Base.PhysicsBody is not null)
                        Base.PhysicsBody.AffectedByGravity = true;
        }

        #endregion

        #region Hit By Object

        public void ProcessKnockOut (HitCatapult knockoutInfo)
        {
                // hide the ball and strap
                strapVisible = StrapVisible.Hidden;
                BallVisible = BallVisible.Hidden;
                Disabled = true;

                // Remove everything except catpault base/prong
                Delegate?.DidBreak (this, knockoutInfo.JustKnockedout, knockoutInfo.Vortex);
        }

        #endregion

        #region Auxiliary

        public void Update ()
        {
                if (Disabled)
                {
                        BallVisible = BallVisible.Hidden;
                } else {
                        rope.UpdateRopeModel ();

                        // ball on the sling will remain invisible until cooldown time exceeded
                        // base this on animation of sling coming back to rest
                        if (ballVisible == BallVisible.Hidden)
                        {
                                // make sure cooldown doesn't occur starting the ball animation
                                // until a few seconds after loading the level
                                if (lastLaunchTime == 0d)
                                {
                                        lastLaunchTime = GameTime.Time;
                                }

                                // only allow grabbing the ball after the cooldown animations play (grow + drop)
                                var timeElapsed = GameTime.Time - lastLaunchTime;
                                var timeForCooldown = properties.CooldownTime - properties.GrowAnimationTime - properties.DropAnimationTime;
                                if (timeForCooldown < 0.01)
                                {
                                        timeForCooldown = 0d;
                                }

                                var startCooldownAnimation = timeElapsed > timeForCooldown;
                                if (startCooldownAnimation)
                                {
                                        // show the ball at the ballOrigin, that's in the sling
                                        BallVisible = BallVisible.Partial;
                                }
                        }

                        UpdateCatapultStable ();

                        // Make sure that the ball stays in its place even if the catapult move
                        if (ballCanBeGrabbed)
                        {
                                if (isGrabbed)
                                {
                                        if (Projectile is null)
                                        {
                                                throw new Exception ("isGrabbed but has no projectile");
                                        }

                                        rope.MoveBall (Projectile.WorldPosition);
                                } else {
                                        if (Projectile is not null)
                                        {
                                                Projectile.WorldPosition = ballOriginInactiveBelow?.PresentationNode.WorldPosition ?? SCNVector3.One;
                                        }
                                }
                        }
                }
        }

        #endregion
}
