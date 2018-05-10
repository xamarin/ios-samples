
namespace Fox2
{
    using CoreAnimation;
    using CoreFoundation;
    using CoreGraphics;
    using Foundation;
    using Fox2.Enums;
    using Fox2.Extensions;
    using Fox2.Interfaces;
    using GameplayKit;
    using global::GameController;
    using OpenTK;
    using SceneKit;
    using System;
    using System.Collections.Generic;
    using System.Linq;

#if !__OSX__
    using UIKit;
    using SKColor = UIKit.UIColor;
#else
    using SKColor = AppKit.NSColor;
#endif

    /// <summary>
    /// This class serves as the app's source of control flow.
    /// </summary>
#if !__IOS__
    public class GameController : NSObject, ISCNSceneRendererDelegate, ISCNPhysicsContactDelegate, IMenuDelegate
#else
    public class GameController : NSObject, ISCNSceneRendererDelegate, ISCNPhysicsContactDelegate, IMenuDelegate, IPadOverlayDelegate, IButtonOverlayDelegate
#endif
    {
        private const int NSEC_PER_SEC = 1000000000;

        // Global settings
        private static float DefaultCameraTransitionDuration = 1.0f;
        private static float CameraOrientationSensitivity = 0.05f;
        private static int FriendsNumber = 100;

        private ISCNSceneRenderer sceneRenderer;
        private SCNScene scene;

        // Overlays
        private Overlay overlay;

        // Camera and targets
        private SCNVector3 lastActiveCameraFrontDirection = SCNVector3.Zero;
        private SCNNode lookAtTarget = new SCNNode();
        private SCNNode cameraNode = new SCNNode();
        private SCNNode lastActiveCamera;

        private bool playingCinematic;

        //triggers
        private bool firstTriggerDone;
        private SCNNode lastTrigger;

        //enemies
        private SCNNode enemy1;
        private SCNNode enemy2;

        //friends
        private readonly float[] friendsSpeed = new float[FriendsNumber];
        private readonly SCNNode[] friends = new SCNNode[FriendsNumber];
        private int friendCount = 0;
        private bool friendsAreFree;

        //collected objects
        private int collectedKeys = 0;
        private int collectedGems = 0;
        private bool keyIsVisible;

        // particles
        private readonly Dictionary<ParticleKind, List<SCNParticleSystem>> particleSystems = new Dictionary<ParticleKind, List<SCNParticleSystem>>();

        // audio
        private readonly SCNAudioSource[] audioSources = new SCNAudioSource[(int)AudioSourceKind.TotalCount];

        // GameplayKit
        private GKScene gkScene;

        // Game controller
        private GCControllerDirectionPad gamePadRight;
        private GCControllerDirectionPad gamePadLeft;
        private GCController gamePadCurrent;

        // update delta time
        private double lastUpdateTime;

        public GameController(SCNView scnView) : base()
        {
            this.sceneRenderer = scnView;
            this.sceneRenderer.WeakSceneRendererDelegate = this;

            // Uncomment to show statistics such as fps and timing information
            //scnView.ShowsStatistics = true;

            // setup overlay
            this.overlay = new Overlay(scnView.Bounds.Size, this);
            scnView.OverlayScene = this.overlay;

            //load the main scene
            this.scene = SCNScene.FromFile("art.scnassets/scene.scn");

            //setup physics
            this.SetupPhysics();

            //setup collisions
            this.SetupCollisions();

            //load the character
            this.SetupCharacter();

            //setup enemies
            this.SetupEnemies();

            //setup friends
            this.AddFriends(3);

            //setup platforms
            this.SetupPlatforms();

            //setup particles
            this.SetupParticleSystem();

            //setup lighting
            var light = this.scene.RootNode.FindChildNode("DirectLight", true).Light;
            light.ShadowCascadeCount = 3;  // turn on cascade shadows
            light.ShadowMapSize = new CGSize(512f, 512f);
            light.MaximumShadowDistance = 20f;
            light.ShadowCascadeSplittingFactor = 0.5f;

            //setup camera
            this.SetupCamera();

            //setup game controller
            this.SetupGameController();

            //configure quality
            this.ConfigureRenderingQuality();

            //assign the scene to the view
            this.sceneRenderer.Scene = this.scene;

            //setup audio
            this.SetupAudio();

            //select the point of view to use
            this.sceneRenderer.PointOfView = this.cameraNode;

            //register ourself as the physics contact delegate to receive contact notifications
            this.sceneRenderer.Scene.PhysicsWorld.ContactDelegate = this;
        }

        /// <summary>
        /// Character
        /// </summary>
        public Character Character { get; private set; }

        public SCNNode ActiveCamera { get; private set; }

        public void ResetPlayerPosition()
        {
            this.Character.QueueResetCharacterPosition();
        }

        #region Setup

        private void SetupGameController()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(GCController.DidConnectNotification, this.HandleControllerDidConnect, null);
            NSNotificationCenter.DefaultCenter.AddObserver(GCController.DidDisconnectNotification, this.HandleControllerDidDisconnect, null);
            var controller = GCController.Controllers.FirstOrDefault();
            if (controller != null)
            {
                this.RegisterGameController(controller);
            }
        }

        private void SetupCharacter()
        {
            this.Character = new Character(this.scene);

            // keep a pointer to the physicsWorld from the character because we will need it when updating the character's position
            this.Character.PhysicsWorld = this.scene.PhysicsWorld;
            this.scene.RootNode.AddChildNode(this.Character.Node);
        }

        private void SetupPhysics()
        {
            if (this.scene?.RootNode != null)
            {
                //make sure all objects only collide with the character
                this.scene.RootNode.EnumerateChildNodes((SCNNode node, out bool stop) =>
                {
                    stop = false;
                    if (node?.PhysicsBody != null)
                    {
                        node.PhysicsBody.CollisionBitMask = (uint)Bitmask.Character;
                    }
                });
            }
        }

        private void SetupCollisions()
        {
            // load the collision mesh from another scene and merge into main scene
            var collisionsScene = SCNScene.FromFile("art.scnassets/collision.scn");
            collisionsScene.RootNode.EnumerateChildNodes((SCNNode child, out bool stop) =>
            {
                stop = false;
                child.Opacity = 0f;
                this.scene.RootNode.AddChildNode(child);
            });
        }

        /// <summary>
        /// the follow camera behavior make the camera to follow the character, with a constant distance, altitude and smoothed motion
        /// </summary>
        private void SetupFollowCamera(SCNNode cameraNode)
        {
            // look at "lookAtTarget"
            var lookAtConstraint = SCNLookAtConstraint.Create(this.lookAtTarget);
            lookAtConstraint.InfluenceFactor = 0.07f;
            lookAtConstraint.GimbalLockEnabled = true;

            // distance constraints
            var follow = SCNDistanceConstraint.FromTarget(this.lookAtTarget);
            var distance = cameraNode.Position.Length;
            follow.MinimumDistance = distance;
            follow.MaximumDistance = distance;

            // configure a constraint to maintain a constant altitude relative to the character
            var desiredAltitude = Math.Abs((float)cameraNode.WorldPosition.Y);

            var weakSelf = new WeakReference<GameController>(this);
            var keepAltitude = SCNTransformConstraint.CreatePositionConstraint(true, (node, position) =>
            {
                var result = position;
                GameController self;
                if (weakSelf.TryGetTarget(out self))
                {
                    var position2 = new SCNVector3(position);
                    position2.Y = self.Character.BaseAltitude + desiredAltitude;
                    result = new SCNVector3(position2);
                }

                return result;
            });

            var accelerationConstraint = new SCNAccelerationConstraint
            {
                MaximumLinearVelocity = 1500f,
                MaximumLinearAcceleration = 50f,
                Damping = 0.05f,
            };

            // use a custom constraint to let the user orbit the camera around the character
            var transformNode = new SCNNode();

            var orientationUpdateConstraint = SCNTransformConstraint.Create(true, (node, transform) =>
            {
                var result = transform;
                GameController self;
                if (weakSelf.TryGetTarget(out self))
                {
                    if (self.ActiveCamera == node)
                    {
                        // Slowly update the acceleration constraint influence factor to smoothly reenable the acceleration.
                        accelerationConstraint.InfluenceFactor = NMath.Min(1f, accelerationConstraint.InfluenceFactor + 0.01f);

                        var targetPosition = self.lookAtTarget.PresentationNode.WorldPosition;
                        var cameraDirection = self.CameraDirection;
                        if (!cameraDirection.AllZero())
                        {
                            // Disable the acceleration constraint.
                            accelerationConstraint.InfluenceFactor = 0f;

                            var characterWorldUp = self.Character.Node.PresentationNode.WorldUp;
                            transformNode.Transform = transform;

                            var quaternion = SCNQuaternion.Multiply(
                                                SCNQuaternion.FromAxisAngle(characterWorldUp, GameController.CameraOrientationSensitivity * cameraDirection.X),
                                                SCNQuaternion.FromAxisAngle(transformNode.WorldRight, GameController.CameraOrientationSensitivity * cameraDirection.Y));

                            transformNode.Rotate(quaternion, targetPosition);
                            result = transformNode.Transform;
                        }
                    }
                }

                return result;
            });

            cameraNode.Constraints = new SCNConstraint[] { follow, keepAltitude, accelerationConstraint, orientationUpdateConstraint, lookAtConstraint };
        }

        /// <summary>
        /// the axis aligned behavior look at the character but remains aligned using a specified axis
        /// </summary>
        private void SetupAxisAlignedCamera(SCNNode cameraNode)
        {
            var distance = cameraNode.Position.Length;
            var originalAxisDirection = cameraNode.WorldFront;

            this.lastActiveCameraFrontDirection = originalAxisDirection;

            var symetricAxisDirection = new SCNVector3(-originalAxisDirection.X, originalAxisDirection.Y, -originalAxisDirection.Z);

            var weakSelf = new WeakReference<GameController>(this);

            // define a custom constraint for the axis alignment
            var axisAlignConstraint = SCNTransformConstraint.CreatePositionConstraint(true, (node, position) =>
            {
                var result = position;

                GameController self;
                if (weakSelf.TryGetTarget(out self))
                {
                    if (self.ActiveCamera != null)
                    {
                        var axisOrigin = self.lookAtTarget.PresentationNode.WorldPosition;
                        var referenceFrontDirection = self.ActiveCamera == node ? self.lastActiveCameraFrontDirection : self.ActiveCamera.PresentationNode.WorldFront;

                        var axis = SCNVector3.Dot(originalAxisDirection, referenceFrontDirection) > 0 ? originalAxisDirection : symetricAxisDirection;

                        var constrainedPosition = axisOrigin - distance * axis;
                        result = new SCNVector3(constrainedPosition);
                    }
                }

                return result;
            });

            var accelerationConstraint = new SCNAccelerationConstraint
            {
                MaximumLinearAcceleration = 20f,
                DecelerationDistance = 0.5f,
                Damping = 0.05f,
            };

            // look at constraint
            var lookAtConstraint = SCNLookAtConstraint.Create(this.lookAtTarget);
            lookAtConstraint.GimbalLockEnabled = true; // keep horizon horizontal

            cameraNode.Constraints = new SCNConstraint[] { axisAlignConstraint, lookAtConstraint, accelerationConstraint };
        }

        private void SetupCameraNode(SCNNode node)
        {
            var cameraName = node?.Name;
            if (!string.IsNullOrEmpty(cameraName))
            {
                if (cameraName.StartsWith("camTrav", StringComparison.OrdinalIgnoreCase))
                {
                    this.SetupAxisAlignedCamera(node);
                }
                else if (cameraName.StartsWith("camLookAt", StringComparison.OrdinalIgnoreCase))
                {
                    this.SetupFollowCamera(node);
                }
            }
        }

        private void SetupCamera()
        {
            //The lookAtTarget node will be placed slighlty above the character using a constraint
            var weakSelf = new WeakReference<GameController>(this);

            this.lookAtTarget.Constraints = new SCNConstraint[]{ SCNTransformConstraint.CreatePositionConstraint(true, (node, position)=>
            {
                var result = position;

                GameController self;
                if (weakSelf.TryGetTarget(out self))
                {
                    if(self.Character?.Node?.WorldPosition != null)
                    {
                        var worldPosition = self.Character.Node.WorldPosition;

                        worldPosition.Y = self.Character.BaseAltitude + 0.5f;
                        result = new SCNVector3(worldPosition);
                    }
                }

                return result;
            })};

            this.scene.RootNode.AddChildNode(this.lookAtTarget);

            this.scene.RootNode.EnumerateHierarchy((SCNNode node, out bool stop) =>
            {
                stop = false;
                if (node.Camera != null)
                {
                    this.SetupCameraNode(node);
                }
            });

            this.cameraNode.Camera = new SCNCamera();
            this.cameraNode.Name = "mainCamera";
            this.cameraNode.Camera.ZNear = 0.1f;
            this.scene.RootNode.AddChildNode(this.cameraNode);

            this.SetActiveCamera("camLookAt_cameraGame", 0f);
        }

        private void SetupEnemies()
        {
            this.enemy1 = this.scene?.RootNode?.FindChildNode("enemy1", true);
            this.enemy2 = this.scene?.RootNode?.FindChildNode("enemy2", true);

            var gkScene = new GKScene();

            // Player
            var playerEntity = new GKEntity();
            gkScene.AddEntity(playerEntity);
            playerEntity.AddComponent(GKSCNNodeComponent.FromNode(this.Character.Node));

            var playerComponent = new PlayerComponent { IsAutoMoveNode = false, Character = this.Character };
            playerEntity.AddComponent(playerComponent);
            playerComponent.PositionAgentFromNode();

            // Chaser
            var chaserEntity = new GKEntity();
            gkScene.AddEntity(chaserEntity);
            chaserEntity.AddComponent(GKSCNNodeComponent.FromNode(this.enemy1));
            var chaser = new ChaserComponent();
            chaserEntity.AddComponent(chaser);
            chaser.Player = playerComponent;
            chaser.PositionAgentFromNode();

            // Scared
            var scaredEntity = new GKEntity();
            gkScene.AddEntity(scaredEntity);
            scaredEntity.AddComponent(GKSCNNodeComponent.FromNode(this.enemy2));
            var scared = new ScaredComponent();
            scaredEntity.AddComponent(scared);
            scared.Player = playerComponent;
            scared.PositionAgentFromNode();

            // animate enemies (move up and down)
            var anim = CABasicAnimation.FromKeyPath("position");
            anim.From = NSValue.FromVector(new SCNVector3(0f, 0.1f, 0f));
            anim.To = NSValue.FromVector(new SCNVector3(0f, -0.1f, 0f));
            anim.Additive = true;
            anim.RepeatCount = float.PositiveInfinity;
            anim.AutoReverses = true;
            anim.Duration = 1.2f;
            anim.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);

            this.enemy1.AddAnimation(anim, "");
            this.enemy2.AddAnimation(anim, "");

            this.gkScene = gkScene;
        }

        private List<SCNParticleSystem> LoadParticleSystems(string path)
        {
            var result = new List<SCNParticleSystem>();

            var url = NSUrl.CreateFileUrl(new string[] { path });
            var directory = url.RemoveLastPathComponent();

            var fileName = url.LastPathComponent;
            var ext = url.PathExtension;

            if (ext == "scnp")
            {
                result.Add(SCNParticleSystem.Create(fileName, directory.RelativePath));
            }
            else
            {
                var particles = new List<SCNParticleSystem>();
                var scene = SCNScene.FromFile(fileName, directory.RelativePath, null as SCNSceneLoadingOptions);
                scene.RootNode.EnumerateHierarchy((SCNNode node, out bool stop) =>
                {
                    stop = false;
                    if (node.ParticleSystems != null)
                    {
                        result.AddRange(node.ParticleSystems);
                    }
                });
            }

            return result;
        }

        private void SetupParticleSystem()
        {
            this.particleSystems[ParticleKind.Collect] = this.LoadParticleSystems("art.scnassets/particles/collect.scnp");
            this.particleSystems[ParticleKind.CollectBig] = this.LoadParticleSystems("art.scnassets/particles/key_apparition.scn");
            this.particleSystems[ParticleKind.EnemyExplosion] = this.LoadParticleSystems("art.scnassets/particles/enemy_explosion.scn");
            this.particleSystems[ParticleKind.KeyApparition] = this.LoadParticleSystems("art.scnassets/particles/key_apparition.scn");
            this.particleSystems[ParticleKind.UnlockDoor] = this.LoadParticleSystems("art.scnassets/particles/unlock_door.scn");
        }

        private void SetupPlatforms()
        {
            var PLATFORM_MOVE_OFFSET = 1.5f;
            var PLATFORM_MOVE_SPEED = 0.5f;

            var alternate = 1f;
            // This could be done in the editor using the action editor.
            this.scene.RootNode.EnumerateHierarchy((SCNNode node, out bool stop) =>
            {
                stop = false;
                if (node.Name == "mobilePlatform" && node.ChildNodes.Any())
                {
                    node.Position = new SCNVector3(node.Position.X - (alternate * PLATFORM_MOVE_OFFSET / 2f), node.Position.Y, node.Position.Z);

                    var moveAction = SCNAction.MoveBy(new SCNVector3(alternate * PLATFORM_MOVE_OFFSET, 0, 0), 1d / PLATFORM_MOVE_SPEED);
                    moveAction.TimingMode = SCNActionTimingMode.EaseInEaseOut;
                    node.RunAction(SCNAction.RepeatActionForever(SCNAction.Sequence(new SCNAction[] { moveAction, moveAction.ReversedAction() })));

                    alternate = -alternate; // alternate movement of platforms to desynchronize them

                    node.EnumerateChildNodes((SCNNode child, out bool boolean) =>
                    {
                        boolean = false;
                        if (child.Name == "particles_platform")
                        {
                            if (child.ParticleSystems != null && child.ParticleSystems.Any())
                            {
                                child.ParticleSystems[0].OrientationDirection = new SCNVector3(0, 1, 0);
                            }
                        }
                    });
                }
            });
        }

        #endregion

        #region Camera transitions

        /// <summary>
        /// transition to the specified camera
        /// this method will reparent the main camera under the camera named "cameraNamed"
        /// and trigger the animation to smoothly move from the current position to the new position
        /// </summary>
        private void SetActiveCamera(string cameraName, double duration)
        {
            var camera = this.scene?.RootNode?.FindChildNode(cameraName, true);
            if (camera != null && this.ActiveCamera != camera)
            {
                this.lastActiveCamera = this.ActiveCamera;
                if (this.ActiveCamera != null)
                {
                    this.lastActiveCameraFrontDirection = this.ActiveCamera.PresentationNode.WorldFront;
                }

                this.ActiveCamera = camera;

                // save old transform in world space
                var oldTransform = this.cameraNode.PresentationNode.WorldTransform;

                // re-parent
                camera.AddChildNode(this.cameraNode);

                // compute the old transform relative to our new parent node (yeah this is the complex part)
                var parentTransform = camera.PresentationNode.WorldTransform;
                var parentInv = SCNMatrix4.Invert(parentTransform);

                // with this new transform our position is unchanged in workd space (i.e we did re-parent but didn't move).
                this.cameraNode.Transform = SCNMatrix4.Mult(oldTransform, parentInv);

                // now animate the transform to identity to smoothly move to the new desired position
                SCNTransaction.Begin();
                SCNTransaction.AnimationDuration = duration;
                SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
                this.cameraNode.Transform = SCNMatrix4.Identity;

                var cameraTemplate = camera.Camera;
                if (cameraTemplate != null)
                {
                    this.cameraNode.Camera.FieldOfView = cameraTemplate.FieldOfView;
                    this.cameraNode.Camera.WantsDepthOfField = cameraTemplate.WantsDepthOfField;
                    this.cameraNode.Camera.SensorHeight = cameraTemplate.SensorHeight;
                    this.cameraNode.Camera.FStop = cameraTemplate.FStop;
                    this.cameraNode.Camera.FocusDistance = cameraTemplate.FocusDistance;
                    this.cameraNode.Camera.BloomIntensity = cameraTemplate.BloomIntensity;
                    this.cameraNode.Camera.BloomThreshold = cameraTemplate.BloomThreshold;
                    this.cameraNode.Camera.BloomBlurRadius = cameraTemplate.BloomBlurRadius;
                    this.cameraNode.Camera.WantsHdr = cameraTemplate.WantsHdr;
                    this.cameraNode.Camera.WantsExposureAdaptation = cameraTemplate.WantsExposureAdaptation;
                    this.cameraNode.Camera.VignettingPower = cameraTemplate.VignettingPower;
                    this.cameraNode.Camera.VignettingIntensity = cameraTemplate.VignettingIntensity;
                }

                SCNTransaction.Commit();
            }
        }

        private void SetActiveCamera(string cameraName)
        {
            this.SetActiveCamera(cameraName, GameController.DefaultCameraTransitionDuration);
        }

        #endregion

        #region Audio

        private void PlaySound(AudioSourceKind audioName)
        {
            this.scene.RootNode.AddAudioPlayer(SCNAudioPlayer.FromSource(this.audioSources[(int)audioName]));
        }

        private void SetupAudio()
        {
            // Get an arbitrary node to attach the sounds to.
            var node = this.scene.RootNode;

            // ambience
            var audioSource = SCNAudioSource.FromFile("audio/ambience.mp3");
            if (audioSource != null)
            {
                audioSource.Loops = true;
                audioSource.Volume = 0.8f;
                audioSource.Positional = false;
                audioSource.ShouldStream = true;
                node.AddAudioPlayer(SCNAudioPlayer.FromSource(audioSource));
            }

            // volcano
            var volcanoNode = this.scene.RootNode.FindChildNode("particles_volcanoSmoke_v2", true);
            if (volcanoNode != null)
            {
                audioSource = SCNAudioSource.FromFile("audio/volcano.mp3");
                if (audioSource != null)
                {
                    audioSource.Loops = true;
                    audioSource.Volume = 5.0f;
                    volcanoNode.AddAudioPlayer(SCNAudioPlayer.FromSource(audioSource));
                }
            }

            // other sounds
            this.audioSources[(int)AudioSourceKind.Collect] = SCNAudioSource.FromFile("audio/collect.mp3");
            this.audioSources[(int)AudioSourceKind.CollectBig] = SCNAudioSource.FromFile("audio/collectBig.mp3");
            this.audioSources[(int)AudioSourceKind.UnlockDoor] = SCNAudioSource.FromFile("audio/unlockTheDoor.m4a");
            this.audioSources[(int)AudioSourceKind.HitEnemy] = SCNAudioSource.FromFile("audio/hitEnemy.wav");

            // adjust volumes
            this.audioSources[(int)AudioSourceKind.UnlockDoor].Positional = false;
            this.audioSources[(int)AudioSourceKind.Collect].Positional = false;
            this.audioSources[(int)AudioSourceKind.CollectBig].Positional = false;
            this.audioSources[(int)AudioSourceKind.HitEnemy].Positional = false;

            this.audioSources[(int)AudioSourceKind.UnlockDoor].Volume = 0.5f;
            this.audioSources[(int)AudioSourceKind.Collect].Volume = 4f;
            this.audioSources[(int)AudioSourceKind.CollectBig].Volume = 4f;
        }

        #endregion

        #region cinematic

        private void StartCinematic()
        {
            this.playingCinematic = true;
            this.Character.Node.Paused = true;
        }

        private void StopCinematic()
        {
            this.playingCinematic = false;
            this.Character.Node.Paused = false;
        }

        #endregion

        #region particles

        private List<SCNParticleSystem> ParticleSystems(ParticleKind kind)
        {
            return this.particleSystems[kind];
        }

        private void AddParticles(ParticleKind kind, SCNMatrix4 transform)
        {
            var particles = this.particleSystems[kind];
            foreach (var particle in particles)
            {
                this.scene.AddParticleSystem(particle, transform);
            }
        }

        #endregion

        #region Triggers

        /// <summary>
        /// "triggers" are triggered when a character enter a box with the collision mask BitmaskTrigger
        /// </summary>
        private void ExecTrigger(SCNNode triggerNode, double duration)
        {
            //exec trigger
            if (triggerNode.Name.StartsWith("trigCam_", StringComparison.OrdinalIgnoreCase))
            {
                var cameraName = triggerNode.Name.Substring(8);
                this.SetActiveCamera(cameraName, duration);
            }
            //action
            if (triggerNode.Name.StartsWith("trigAction_", StringComparison.OrdinalIgnoreCase))
            {
                if (this.collectedKeys > 0)
                {
                    var actionName = triggerNode.Name.Substring(11);
                    if (actionName == "unlockDoor")
                    {
                        this.UnlockDoor();
                    }
                }
            }
        }

        private void Trigger(SCNNode triggerNode)
        {
            if (!this.playingCinematic)
            {
                if (this.lastTrigger != triggerNode)
                {
                    this.lastTrigger = triggerNode;

                    // the very first trigger should not animate (initial camera position)
                    this.ExecTrigger(triggerNode, firstTriggerDone ? GameController.DefaultCameraTransitionDuration : 0);
                    this.firstTriggerDone = true;
                }
            }
        }

        #endregion

        #region Friends

        private void UpdateFriends(double deltaTime)
        {
            var pathCurve = 0.4f;

            // update pandas
            for (int i = 0; i < this.friendCount; i++)
            {
                var friend = this.friends[i];

                var position = friend.Position;
                var offsetx = position.X - (float)Math.Sin(pathCurve * position.Z);

                position.Z += this.friendsSpeed[i] * (float)deltaTime * 0.5f;
                position.X = (float)Math.Sin(pathCurve * position.Z) + offsetx;

                friend.Position = position;

                this.EnsureNoPenetrationOfIndex(i);
            }
        }

        private void AnimateFriends()
        {
            //animations
            var walkAnimation = Character.LoadAnimation("art.scnassets/character/max_walk.scn");

            SCNTransaction.Begin();
            for (int i = 0; i < this.friendCount; i++)
            {
                var walk = walkAnimation.Copy() as SCNAnimationPlayer;

                walk.Speed = friendsSpeed[i];
                friends[i].AddAnimation(walk, new NSString("walk"));
                walk.Play();
            }

            SCNTransaction.Commit();
        }

        private void AddFriends(int count)
        {
            if (count + this.friendCount > GameController.FriendsNumber)
            {
                count = GameController.FriendsNumber - this.friendCount;
            }

            var friendScene = SCNScene.FromFile("art.scnassets/character/max.scn");
            var friendModel = friendScene?.RootNode?.FindChildNode("Max_rootNode", true);
            if (friendModel != null)
            {
                friendModel.Name = "friend";

                var geometries = new SCNGeometry[3];
                var geometryNode = friendModel.FindChildNode("Max", true);
                if (geometryNode != null)
                {
                    geometryNode.Geometry.FirstMaterial.Diffuse.Intensity = 0.5f;

                    geometries[0] = geometryNode.Geometry.Copy() as SCNGeometry;
                    geometries[1] = geometryNode.Geometry.Copy() as SCNGeometry;
                    geometries[2] = geometryNode.Geometry.Copy() as SCNGeometry;

                    geometries[0].FirstMaterial = geometries[0].FirstMaterial.Copy() as SCNMaterial;
                    geometries[0].FirstMaterial.Diffuse.Contents = new NSString("art.scnassets/character/max_diffuseB.png");

                    geometries[1].FirstMaterial = geometries[1].FirstMaterial?.Copy() as SCNMaterial;
                    geometries[1].FirstMaterial.Diffuse.Contents = new NSString("art.scnassets/character/max_diffuseC.png");

                    geometries[2].FirstMaterial = geometries[2].FirstMaterial?.Copy() as SCNMaterial;
                    geometries[2].FirstMaterial.Diffuse.Contents = new NSString("art.scnassets/character/max_diffuseD.png");

                    //remove physics from our friends
                    friendModel.EnumerateHierarchy((SCNNode node, out bool stop) =>
                    {
                        stop = false;
                        node.PhysicsBody = null;
                    });

                    var friendPosition = new SCNVector3(-5.84f, -0.75f, 3.354f);
                    var FRIEND_AREA_LENGTH = 5f;

                    // group them
                    var friendsNode = this.scene.RootNode.FindChildNode("friends", false);
                    if (friendsNode == null)
                    {
                        friendsNode = new SCNNode();
                        friendsNode.Name = "friends";
                        this.scene.RootNode.AddChildNode(friendsNode);
                    }

                    //animations
                    var idleAnimation = Character.LoadAnimation("art.scnassets/character/max_idle.scn");
                    for (int i = 0; i < count; i++)
                    {
                        var friend = friendModel.Clone();

                        //replace texture
                        var geometryIndex = new Random().Next(0, 3);
                        geometryNode = friend.FindChildNode("Max", true);
                        if (geometryNode == null)
                        {
                            break;
                        }

                        geometryNode.Geometry = geometries[geometryIndex];

                        //place our friend
                        friend.Position = new SCNVector3(friendPosition.X + (1.4f * new Random().Next(0, 32767) / 32767f - 0.5f),
                                                         friendPosition.Y,
                                                         friendPosition.Z - (FRIEND_AREA_LENGTH * new Random().Next(0, 32767) / 32767f));

                        var idle = idleAnimation.Copy() as SCNAnimationPlayer;
                        idle.Speed = (float)(1.5f + 1.5f * new Random().Next(0, 32767) / 32767f);

                        friend.AddAnimation(idle, new NSString("idle"));
                        idle.Play();
                        friendsNode.AddChildNode(friend);

                        this.friendsSpeed[friendCount] = (float)idle.Speed;
                        this.friends[friendCount] = friend;
                        this.friendCount += 1;
                    }

                    for (int i = 0; i < friendCount; i++)
                    {
                        this.EnsureNoPenetrationOfIndex(i);
                    }
                }
            }
        }

        /// <summary>
        /// iterates on every friend and move them if they intersect friend at index i
        /// </summary>
        private void EnsureNoPenetrationOfIndex(int index)
        {
            var position = this.friends[index].Position;

            // ensure no penetration
            var pandaRadius = 0.15f;
            var pandaDiameter = pandaRadius * 2f;
            for (int j = 0; j < friendCount; j++)
            {
                if (j == index)
                {
                    continue;
                }

                var otherPos = new SCNVector3(friends[j].Position);
                var v = otherPos - position;
                var dist = v.Length;
                if (dist < pandaDiameter)
                {
                    // penetration
                    var pen = pandaDiameter - dist;
                    position -= SCNVector3.Normalize(v) * pen;
                }
            }

            //ensure within the box X[-6.662 -4.8] Z<3.354
            if (this.friends[index].Position.Z <= 3.354f)
            {
#if !__OSX__
                position.X = Math.Max(position.X, -6.662f);
                position.X = Math.Min(position.X, -4.8f);
#else
                position.X = NMath.Max(position.X, -6.662f);
                position.X = NMath.Min(position.X, -4.8f);
#endif
            }

            this.friends[index].Position = position;
        }

        #endregion

        #region Game actions

        private void UnlockDoor()
        {
            //already unlocked
            if (!this.friendsAreFree)
            {
                this.StartCinematic();  //pause the scene

                //play sound
                this.PlaySound(AudioSourceKind.UnlockDoor);

                //cinematic02
                SCNTransaction.Begin();
                SCNTransaction.AnimationDuration = 0f;
                SCNTransaction.SetCompletionBlock(() =>
                {
                    //trigger particles
                    var door = this.scene.RootNode.FindChildNode("door", true);
                    var particle_door = this.scene.RootNode.FindChildNode("particles_door", true);
                    this.AddParticles(ParticleKind.UnlockDoor, particle_door.WorldTransform);

                    //audio
                    this.PlaySound(AudioSourceKind.CollectBig);

                    //add friends
                    SCNTransaction.Begin();
                    SCNTransaction.AnimationDuration = 0f;
                    this.AddFriends(GameController.FriendsNumber);
                    SCNTransaction.Commit();

                    //open the door
                    SCNTransaction.Begin();
                    SCNTransaction.AnimationDuration = 1f;
                    SCNTransaction.SetCompletionBlock(() =>
                    {
                        //animate characters
                        this.AnimateFriends();

                        // update state
                        this.friendsAreFree = true;

                        // show end screen
                        // Double(Int64(1.0 * Double(NSEC_PER_SEC))) / Double(NSEC_PER_SEC)
                        DispatchQueue.MainQueue.DispatchAfter(new DispatchTime(DispatchTime.Now, 1L * NSEC_PER_SEC / NSEC_PER_SEC), () =>
                        {
                            this.ShowEndScreen();
                        });
                    });

                    door.Opacity = 0f;
                    SCNTransaction.Commit();
                });

                // change the point of view
                this.SetActiveCamera("CameraCinematic02", 1f);
                SCNTransaction.Commit();
            }
        }

        private void ShowKey()
        {
            this.keyIsVisible = true;

            // get the key node
            var key = this.scene.RootNode.FindChildNode("key", true);

            //sound fx
            this.PlaySound(AudioSourceKind.CollectBig);

            //particles
            this.AddParticles(ParticleKind.KeyApparition, key.WorldTransform);

            SCNTransaction.Begin();
            SCNTransaction.AnimationDuration = 1f;
            SCNTransaction.SetCompletionBlock(() =>
            {
                // Double(Int64(2.5 * Double(NSEC_PER_SEC))) / Double(NSEC_PER_SEC)
                DispatchQueue.MainQueue.DispatchAfter(new DispatchTime(DispatchTime.Now, (long)2.5f * NSEC_PER_SEC / NSEC_PER_SEC), () =>
                {
                    this.KeyDidAppear();
                });
            });

            key.Opacity = 1f; // show the key

            SCNTransaction.Commit();
        }

        private void KeyDidAppear()
        {
            this.ExecTrigger(this.lastTrigger, 0.75f); //revert to previous camera
            this.StopCinematic();
        }

        private void KeyShouldAppear()
        {
            this.StartCinematic();

            SCNTransaction.Begin();
            SCNTransaction.AnimationDuration = 0f;
            SCNTransaction.SetCompletionBlock(() =>
            {
                this.ShowKey();
            });

            this.SetActiveCamera("CameraCinematic01", 3f);
            SCNTransaction.Commit();
        }

        private void Collect(SCNNode collectable)
        {
            if (collectable.PhysicsBody != null)
            {
                //the Key
                if (collectable.Name == "key")
                {
                    //key visible already
                    if (this.keyIsVisible)
                    {
                        // play sound
                        this.PlaySound(AudioSourceKind.Collect);
                        this.overlay?.DidCollectKey();

                        this.collectedKeys += 1;
                    }
                }
                //the gems
                else if (collectable.Name == "CollectableBig")
                {
                    this.collectedGems += 1;

                    // play sound
                    this.PlaySound(AudioSourceKind.Collect);

                    // update the overlay
                    if (this.overlay != null)
                    {
                        this.overlay.CollectedGemsCount = this.collectedGems;
                    }

                    if (this.collectedGems == 1)
                    {
                        //we collect a gem, show the key after 1 second
                        DispatchQueue.MainQueue.DispatchAfter(new DispatchTime(DispatchTime.Now, (long)(0.5f * NSEC_PER_SEC) / NSEC_PER_SEC), () =>
                        {
                            this.KeyShouldAppear();
                        });
                    }
                }

                collectable.PhysicsBody = null; //not collectable anymore

                // particles
                this.AddParticles(ParticleKind.KeyApparition, collectable.WorldTransform);

                collectable.RemoveFromParentNode();
            }
        }

        #endregion

        #region Controlling the character

        public void ControllerJump(bool controllerJump)
        {
            this.Character.IsJump = controllerJump;
        }

        public void ControllerAttack()
        {
            if (!this.Character.IsAttacking)
            {
                this.Character.Attack();
            }
        }

        public Vector2 CharacterDirection
        {
            get
            {
                return this.Character.Direction;
            }
            set
            {
                var direction = value;
                var length = direction.Length;
                if (length > 1f)
                {
                    direction *= 1f / length;
                }

                this.Character.Direction = direction;
            }
        }

        private Vector2 cameraDirection = Vector2.Zero;

        public Vector2 CameraDirection
        {
            get
            {
                return this.cameraDirection;
            }
            set
            {
                this.cameraDirection = value;

                var length = this.cameraDirection.Length;
                if (length > 1f)
                {
                    this.cameraDirection *= 1f / length;
                }

                this.cameraDirection.Y = 0f;
            }
        }

        #endregion

        #region Update


        [Export("renderer:updateAtTime:")]
        public void Update(ISCNSceneRenderer renderer, double time)
        {
            // compute delta time
            if (this.lastUpdateTime == 0f)
            {
                this.lastUpdateTime = time;
            }

            var deltaTime = time - this.lastUpdateTime;
            this.lastUpdateTime = time;

            // Update Friends
            if (this.friendsAreFree)
            {
                this.UpdateFriends(deltaTime);
            }

            // stop here if cinematic
            if (!this.playingCinematic)
            {
                // update characters
                this.Character.Update(time, renderer);

                // update enemies
                foreach (var entity in this.gkScene.Entities)
                {
                    entity.Update(deltaTime);
                }
            }
        }

        #endregion

        #region contact delegate

        [Export("physicsWorld:didBeginContact:")]
        public void DidBeginContact(SCNPhysicsWorld world, SCNPhysicsContact contact)
        {
            // triggers
            if (contact.NodeA.PhysicsBody.CategoryBitMask == (uint)Bitmask.Trigger)
            {
                this.Trigger(contact.NodeA);
            }

            if (contact.NodeB.PhysicsBody.CategoryBitMask == (uint)Bitmask.Trigger)
            {
                this.Trigger(contact.NodeB);
            }

            // collectables
            if (contact.NodeA.PhysicsBody.CategoryBitMask == (uint)Bitmask.Collectable)
            {
                this.Collect(contact.NodeA);
            }

            if (contact.NodeB.PhysicsBody.CategoryBitMask == (uint)Bitmask.Collectable)
            {
                this.Collect(contact.NodeB);
            }
        }

        #endregion

        #region Congratulating the Player

        private void ShowEndScreen()
        {
            // Play the congrat sound.
            var victoryMusic = SCNAudioSource.FromFile("audio/Music_victory.mp3");
            if (victoryMusic != null)
            {
                victoryMusic.Volume = 0.5f;

                this.scene?.RootNode?.AddAudioPlayer(SCNAudioPlayer.FromSource(victoryMusic));
                this.overlay?.ShowEndScreen();
            }
        }

        #endregion

        #region Configure rendering quality

        private void TurnOffEXRForMaterialProperty(SCNMaterialProperty property)
        {
            var propertyPath = property.Contents as NSString;
            if (propertyPath != null)
            {
                if (propertyPath.PathExtension == "exr")
                {
                    propertyPath = propertyPath.DeletePathExtension().AppendPathExtension(new NSString("png"));
                    property.Contents = propertyPath;
                }
            }
        }

        private void TurnOffEXR()
        {
            this.TurnOffEXRForMaterialProperty(this.scene.Background);
            this.TurnOffEXRForMaterialProperty(this.scene.LightingEnvironment);

            this.scene.RootNode.EnumerateChildNodes((SCNNode child, out bool stop) =>
            {
                stop = false;

                var materials = child.Geometry?.Materials;
                if (materials != null)
                {
                    foreach (var material in materials)
                    {
                        this.TurnOffEXRForMaterialProperty(material.SelfIllumination);
                    }
                }
            });
        }

        private void TurnOffNormalMaps()
        {
            this.scene.RootNode.EnumerateChildNodes((SCNNode child, out bool stop) =>
            {
                stop = false;
                var materials = child.Geometry?.Materials;
                if (materials != null)
                {
                    foreach (var material in materials)
                    {
                        material.Normal.Contents = SKColor.Black;
                    }
                }
            });
        }

        private void TurnOffHDR()
        {
            this.scene.RootNode.EnumerateChildNodes((SCNNode child, out bool stop) =>
            {
                stop = false;
                if (child.Camera != null)
                {
                    child.Camera.WantsHdr = false;
                }
            });
        }

        private void TurnOffDepthOfField()
        {
            this.scene.RootNode.EnumerateChildNodes((SCNNode child, out bool stop) =>
            {
                stop = false;
                if (child.Camera != null)
                {
                    child.Camera.WantsDepthOfField = false;
                }
            });
        }

        private void TurnOffSoftShadows()
        {
            this.scene.RootNode.EnumerateChildNodes((SCNNode child, out bool stop) =>
            {
                stop = false;
                var lightSampleCount = child.Light?.ShadowSampleCount;
                if (lightSampleCount.HasValue)
                {
                    child.Light.ShadowSampleCount = NMath.Min(lightSampleCount.Value, 1);
                }
            });
        }

        private void TurnOffPostProcess()
        {
            this.scene.RootNode.EnumerateChildNodes((SCNNode child, out bool stop) =>
            {
                stop = false;
                var light = child.Light;
                if (light != null)
                {
                    light.ShadowCascadeCount = 0;
                    light.ShadowMapSize = new CGSize(1024f, 1024f);
                }
            });
        }

        private void TurnOffOverlay()
        {
            if (this.sceneRenderer != null)
            {
                this.sceneRenderer.OverlayScene.Dispose();
                this.sceneRenderer.OverlayScene = null;
            }

            if (this.overlay != null)
            {
                this.overlay.Dispose();
                this.overlay = null;
            }
        }

        private void TurnOffVertexShaderModifiers()
        {
            this.scene.RootNode.EnumerateChildNodes((SCNNode child, out bool stop) =>
            {
                stop = false;
                var shaderModifiers = child.Geometry?.ShaderModifiers;
                if (shaderModifiers != null)
                {
                    shaderModifiers.EntryPointGeometry = null;
                    child.Geometry.ShaderModifiers = shaderModifiers;
                }

                var materials = child.Geometry?.Materials;
                if (materials != null)
                {
                    foreach (var material in materials.Where(material => material.ShaderModifiers != null))
                    {
                        var materialShaderModifiers = material.ShaderModifiers;
                        materialShaderModifiers.EntryPointGeometry = null;
                        material.ShaderModifiers = materialShaderModifiers;
                    }
                }
            });
        }

        private void TurnOffVegetation()
        {
            this.scene.RootNode.EnumerateChildNodes((SCNNode child, out bool stop) =>
            {
                stop = false;
                var materialName = child.Geometry?.FirstMaterial?.Name;
                if (!string.IsNullOrEmpty(materialName))
                {
                    if (materialName.StartsWith("plante", StringComparison.OrdinalIgnoreCase))
                    {
                        child.Hidden = true;
                    }
                }
            });
        }

        private void ConfigureRenderingQuality()
        {
#if __TVOS__ 
            this.TurnOffEXR();  //tvOS doesn't support exr maps
            // the following things are done for low power device(s) only
            this.TurnOffNormalMaps();
            this.TurnOffHDR();
            this.TurnOffDepthOfField();
            this.TurnOffSoftShadows();
            this.TurnOffPostProcess();
            this.TurnOffOverlay();
            this.TurnOffVertexShaderModifiers();
            this.TurnOffVegetation();
#elif __OSX__
            // The original sample doesn't work without disabling shader modifiers
            this.TurnOffVertexShaderModifiers();
#endif
        }

        #endregion

        #region Debug menu

        public void FStopChanged(float value)
        {
            this.sceneRenderer.PointOfView.Camera.FStop = value;
        }

        public void FocusDistanceChanged(float value)
        {
            this.sceneRenderer.PointOfView.Camera.FocusDistance = value;
        }

        public void DebugMenuSelectCameraAtIndex(int index)
        {
            if (index == 0)
            {
                var key = this.scene?.RootNode?.FindChildNode("key", true);
                if (key != null)
                {
                    key.Opacity = 1f;
                }
            }

            this.SetActiveCamera($"CameraDof{index}");
        }

        #endregion

        #region GameController

        private void HandleControllerDidConnect(NSNotification notification)
        {
            if (this.gamePadCurrent == null)
            {
                var gameController = notification.Object as GCController;
                if (gameController != null)
                {
                    this.RegisterGameController(gameController);
                }
            }
        }

        private void HandleControllerDidDisconnect(NSNotification notification)
        {
            var gameController = notification.Object as GCController;
            if (gameController != null)
            {
                if (gameController == this.gamePadCurrent)
                {
                    this.UnregisterGameController();

                    foreach (var controller in GCController.Controllers.Where(controller => controller != gameController))
                    {
                        this.RegisterGameController(controller);
                    }
                }
            }
        }

        private void RegisterGameController(GCController gameController)
        {
            GCControllerButtonInput buttonA = null;
            GCControllerButtonInput buttonB = null;

            if (gameController.ExtendedGamepad != null)
            {
                this.gamePadLeft = gameController.ExtendedGamepad.LeftThumbstick;
                this.gamePadRight = gameController.ExtendedGamepad.RightThumbstick;
                buttonA = gameController.ExtendedGamepad.ButtonA;
                buttonB = gameController.ExtendedGamepad.ButtonB;
            }
            else if (gameController.Gamepad != null)
            {
                this.gamePadLeft = gameController.Gamepad.DPad;
                buttonA = gameController.Gamepad.ButtonA;
                buttonB = gameController.Gamepad.ButtonB;
            }
#if !__OSX__
            else if (gameController.MicroGamepad != null)
            {
                this.gamePadLeft = gameController.MicroGamepad.Dpad;
                buttonA = gameController.MicroGamepad.ButtonA;
                buttonB = gameController.MicroGamepad.ButtonX;
            }
#endif

            var weakController = new WeakReference<GameController>(this);

            this.gamePadLeft.ValueChangedHandler = (dpad, xValue, yValue) =>
            {
                GameController self;
                if (weakController.TryGetTarget(out self))
                {
                    self.CharacterDirection = new Vector2(xValue, -yValue);
                }
            };

            if (this.gamePadRight != null)
            {
                this.gamePadRight.ValueChangedHandler = (dpad, xValue, yValue) =>
                {
                    GameController self;
                    if (weakController.TryGetTarget(out self))
                    {
                        self.CameraDirection = new Vector2(xValue, yValue);
                    }
                };
            }

            if (buttonA != null)
            {
                buttonA.ValueChangedHandler = (button, value, pressed) =>
                {
                    GameController self;
                    if (weakController.TryGetTarget(out self))
                    {
                        self.ControllerJump(pressed);
                    }
                };
            }

            if (buttonB != null)
            {
                buttonB.ValueChangedHandler = (button, value, pressed) =>
                {
                    GameController self;
                    if (weakController.TryGetTarget(out self))
                    {
                        self.ControllerAttack();
                    }
                };
            }

#if __IOS__
            if (this.gamePadLeft != null)
            {
                this.overlay.HideVirtualPad();
            }
#endif
        }

        private void UnregisterGameController()
        {
            this.gamePadLeft = null;
            this.gamePadRight = null;
            this.gamePadCurrent = null;
#if __IOS__
            this.overlay.ShowVirtualPad();
#endif
        }

        #endregion

#if __IOS__
        #region PadOverlayDelegate

        public void PadOverlayVirtualStickInteractionDidStart(PadOverlay padNode)
        {
            if (padNode == this.overlay.ControlOverlay.LeftPad)
            {
                this.CharacterDirection = new Vector2((float)padNode.StickPosition.X, -(float)(padNode.StickPosition.Y));
            }
            else if (padNode == this.overlay.ControlOverlay.RightPad)
            {
                this.CameraDirection = new Vector2(-(float)(padNode.StickPosition.X), (float)padNode.StickPosition.Y);
            }
        }

        public void PadOverlayVirtualStickInteractionDidChange(PadOverlay padNode)
        {
            if (padNode == this.overlay.ControlOverlay.LeftPad)
            {
                this.CharacterDirection = new Vector2((float)padNode.StickPosition.X, -(float)(padNode.StickPosition.Y));
            }
            else if (padNode == this.overlay.ControlOverlay.RightPad)
            {
                this.CameraDirection = new Vector2(-(float)(padNode.StickPosition.X), (float)padNode.StickPosition.Y);
            }
        }

        public void PadOverlayVirtualStickInteractionDidEnd(PadOverlay padNode)
        {
            if (padNode == this.overlay.ControlOverlay.LeftPad)
            {
                this.CharacterDirection = new Vector2(0f, 0f);
            }
            else if (padNode == this.overlay.ControlOverlay.RightPad)
            {
                this.CameraDirection = new Vector2(0, 0);
            }
        }

        public void WillPress(ButtonOverlay button)
        {
            if (button == this.overlay.ControlOverlay.ButtonA)
            {
                this.ControllerJump(true);
            }
            else if (button == this.overlay.ControlOverlay.ButtonB)
            {
                this.ControllerAttack();
            }
        }

        public void DidPress(ButtonOverlay button)
        {
            if (button == this.overlay.ControlOverlay.ButtonA)
            {
                this.ControllerJump(false);
            }
        }

        #endregion
#endif
    }
}