
namespace Fox2
{
    using CoreFoundation;
    using Foundation;
    using Fox2.Enums;
    using Fox2.Extensions;
    using OpenTK;
    using SceneKit;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class manages the main character, including its animations, sounds and direction.
    /// </summary>
    public class Character : NSObject
    {
        private static SCNVector3 InitialPosition = new SCNVector3(0.1f, -0.2f, 0f);
        private static nfloat SpeedFactor = 2f;
        private static int StepsCount = 10;

        // some constants
        private static float Gravity = 0.004f;
        private static float JumpImpulse = 0.1f;
        private static float MinAltitude = -10f;
        private static bool IsEnableFootStepSound = true;
        private static float CollisionMargin = 0.04f;
        private static SCNVector3 ModelOffset = new SCNVector3(0f, -Character.CollisionMargin, 0f);
        private static float CollisionMeshBitMask = 8f;

        // Character handle
        private SCNNode characterOrientation; // the node to rotate to orient the character
        private SCNNode characterNode; // top level node
        private SCNNode model; // the model loaded from the character file

        // Physics
        private SCNVector3 collisionShapeOffsetFromModel = SCNVector3.Zero;
        private SCNPhysicsShape characterCollisionShape;
        private float downwardAcceleration = 0f;

        // Jump
        private SCNVector3 groundNodeLastPosition = SCNVector3.Zero;
        private SCNNode groundNode;
        private int jumpState = 0;

        private float targetAltitude = 0f;

        // void playing the step sound too often
        private int lastStepFrame = 0;
        private int frameCounter = 0;

        // Direction
        private Vector2 controllerDirection = Vector2.Zero;
        private double previousUpdateTime = 0d;

        // states
        private long lastHitTime = 0L;
        private int attackCount = 0;

        private bool shouldResetCharacterPosition;

        // Particle systems
        private SCNParticleSystem jumpDustParticle;
        private SCNParticleSystem fireEmitter;
        private SCNParticleSystem smokeEmitter;
        private SCNParticleSystem whiteSmokeEmitter;
        private SCNParticleSystem spinParticle;
        private SCNParticleSystem spinCircleParticle;

        private SCNNode spinParticleAttach;

        private nfloat whiteSmokeEmitterBirthRate = 0f;
        private nfloat smokeEmitterBirthRate = 0f;
        private nfloat fireEmitterBirthRate = 0f;

        // Sound effects
        private SCNAudioSource aahSound;
        private SCNAudioSource hitSound;
        private SCNAudioSource ouchSound;
        private SCNAudioSource jumpSound;
        private SCNAudioSource attackSound;
        private SCNAudioSource hitEnemySound;
        private SCNAudioSource catchFireSound;
        private SCNAudioSource explodeEnemySound;
        private readonly List<SCNAudioSource> steps = new List<SCNAudioSource>(Character.StepsCount);

        public Character(SCNScene scene) : base()
        {
            this.LoadCharacter();
            this.LoadParticles();
            this.LoadSounds();
            this.LoadAnimations();
        }

        public SCNNode Node => this.characterNode;

        public bool IsJump { get; set; }

        public float BaseAltitude { get; set; } = 0f;

        public SCNPhysicsWorld PhysicsWorld { get; set; }

        public Vector2 Direction { get; set; } = new Vector2();

        #region Initialization

        private void LoadCharacter()
        {
            // Load character from external file
            var scene = SCNScene.FromFile("art.scnassets/character/max.scn");
            this.model = scene.RootNode.FindChildNode("Max_rootNode", true);
            this.model.Position = Character.ModelOffset;

            /* setup character hierarchy
             character
             |_orientationNode
             |_model
             */

            this.characterNode = new SCNNode { Name = "character", Position = Character.InitialPosition };

            this.characterOrientation = new SCNNode();
            this.characterNode.AddChildNode(this.characterOrientation);
            this.characterOrientation.AddChildNode(this.model);

            var collider = this.model.FindChildNode("collider", true);
            if (collider?.PhysicsBody != null)
            {
                collider.PhysicsBody.CollisionBitMask = (int)(Bitmask.Enemy | Bitmask.Trigger | Bitmask.Collectable);
            }

            // Setup collision shape
            var min = new SCNVector3();
            var max = new SCNVector3();
            this.model.GetBoundingBox(ref min, ref max);
            nfloat collisionCapsuleRadius = (max.X - min.X) * 0.4f;
            nfloat collisionCapsuleHeight = max.Y - min.Y;

            var collisionGeometry = SCNCapsule.Create(collisionCapsuleRadius, collisionCapsuleHeight);
            this.characterCollisionShape = SCNPhysicsShape.Create(collisionGeometry,
                                                                  new NSMutableDictionary() { { SCNPhysicsShapeOptionsKeys.CollisionMargin, NSNumber.FromFloat(Character.CollisionMargin) } });
            this.collisionShapeOffsetFromModel = new SCNVector3(0f, (float)collisionCapsuleHeight * 0.51f, 0f);
        }

        private void LoadParticles()
        {
            var particleScene = SCNScene.FromFile("art.scnassets/character/jump_dust.scn");
            var particleNode = particleScene.RootNode.FindChildNode("particle", true);
            this.jumpDustParticle = particleNode.ParticleSystems.FirstOrDefault();

            particleScene = SCNScene.FromFile("art.scnassets/particles/burn.scn");
            var burnParticleNode = particleScene.RootNode.FindChildNode("particles", true);

            var particleEmitter = new SCNNode();
            this.characterOrientation.AddChildNode(particleEmitter);

            this.fireEmitter = burnParticleNode.FindChildNode("fire", true).ParticleSystems[0];
            this.fireEmitterBirthRate = fireEmitter.BirthRate;
            this.fireEmitter.BirthRate = 0f;

            this.smokeEmitter = burnParticleNode.FindChildNode("smoke", true).ParticleSystems[0];
            this.smokeEmitterBirthRate = smokeEmitter.BirthRate;
            this.smokeEmitter.BirthRate = 0f;

            this.whiteSmokeEmitter = burnParticleNode.FindChildNode("whiteSmoke", true).ParticleSystems[0];
            this.whiteSmokeEmitterBirthRate = whiteSmokeEmitter.BirthRate;
            this.whiteSmokeEmitter.BirthRate = 0f;

            particleScene = SCNScene.FromFile("art.scnassets/particles/particles_spin.scn");
            this.spinParticle = (particleScene.RootNode.FindChildNode("particles_spin", true)?.ParticleSystems?.FirstOrDefault());
            this.spinCircleParticle = (particleScene.RootNode.FindChildNode("particles_spin_circle", true)?.ParticleSystems?.FirstOrDefault());

            particleEmitter.Position = new SCNVector3(0f, 0.05f, 0f);
            particleEmitter.AddParticleSystem(this.fireEmitter);
            particleEmitter.AddParticleSystem(this.smokeEmitter);
            particleEmitter.AddParticleSystem(this.whiteSmokeEmitter);

            this.spinParticleAttach = this.model.FindChildNode("particles_spin_circle", true);
        }

        private void LoadSounds()
        {
            this.aahSound = SCNAudioSource.FromFile("audio/aah_extinction.mp3");
            this.aahSound.Volume = 1f;
            this.aahSound.Positional = false;
            this.aahSound.Load();

            this.catchFireSound = SCNAudioSource.FromFile("audio/panda_catch_fire.mp3");
            this.catchFireSound.Volume = 5f;
            this.catchFireSound.Positional = false;
            this.catchFireSound.Load();

            this.ouchSound = SCNAudioSource.FromFile("audio/ouch_firehit.mp3");
            this.ouchSound.Volume = 2f;
            this.ouchSound.Positional = false;
            this.ouchSound.Load();

            this.hitSound = SCNAudioSource.FromFile("audio/hit.mp3");
            this.hitSound.Volume = 2f;
            this.hitSound.Positional = false;
            this.hitSound.Load();

            this.hitEnemySound = SCNAudioSource.FromFile("audio/Explosion1.m4a");
            this.hitEnemySound.Volume = 2f;
            this.hitEnemySound.Positional = false;
            this.hitEnemySound.Load();

            this.explodeEnemySound = SCNAudioSource.FromFile("audio/Explosion2.m4a");
            this.explodeEnemySound.Volume = 2f;
            this.explodeEnemySound.Positional = false;
            this.explodeEnemySound.Load();

            this.jumpSound = SCNAudioSource.FromFile("audio/jump.m4a");
            this.jumpSound.Volume = 0.2f;
            this.jumpSound.Positional = false;
            this.jumpSound.Load();

            this.attackSound = SCNAudioSource.FromFile("audio/attack.mp3");
            this.attackSound.Volume = 1.0f;
            this.attackSound.Positional = false;
            this.attackSound.Load();

            for (var i = 0; i < Character.StepsCount; i++)
            {
                var audioSource = SCNAudioSource.FromFile($"audio/Step_rock_0{i}.mp3");
                audioSource.Volume = 0.5f;
                audioSource.Positional = false;
                audioSource.Load();

                this.steps.Add(audioSource);
            }
        }

        private void LoadAnimations()
        {
            var idleAnimation = Character.LoadAnimation("art.scnassets/character/max_idle.scn");
            this.model.AddAnimation(idleAnimation, new NSString("idle"));
            idleAnimation.Play();

            var walkAnimation = Character.LoadAnimation("art.scnassets/character/max_walk.scn");
            walkAnimation.Speed = Character.SpeedFactor;
            walkAnimation.Stop();

            if (Character.IsEnableFootStepSound)
            {
                walkAnimation.Animation.AnimationEvents = new SCNAnimationEvent[]
                {
                    SCNAnimationEvent.Create(0.1f, (animation, animatedObject, playingBackward) => { this.PlayFootStep(); }),
                    SCNAnimationEvent.Create(0.6f, (animation, animatedObject, playingBackward) => { this.PlayFootStep(); })
                };
            }

            this.model.AddAnimation(walkAnimation, new NSString("walk"));

            var jumpAnimation = Character.LoadAnimation("art.scnassets/character/max_jump.scn");
            jumpAnimation.Animation.RemovedOnCompletion = false;
            jumpAnimation.Stop();
            jumpAnimation.Animation.AnimationEvents = new SCNAnimationEvent[] { SCNAnimationEvent.Create(0f, (animation, animatedObject, playingBackward) => { this.PlayJumpSound(); }) };
            this.model.AddAnimation(jumpAnimation, new NSString("jump"));

            var spinAnimation = Character.LoadAnimation("art.scnassets/character/max_spin.scn");
            spinAnimation.Animation.RemovedOnCompletion = false;
            spinAnimation.Speed = 1.5f;
            spinAnimation.Stop();
            spinAnimation.Animation.AnimationEvents = new SCNAnimationEvent[] { SCNAnimationEvent.Create(0f, (animation, animatedObject, playingBackward) => { this.PlayAttackSound(); }) };
            this.model.AddAnimation(spinAnimation, new NSString("spin"));
        }

        public void QueueResetCharacterPosition()
        {
            this.shouldResetCharacterPosition = true;
        }

        #endregion

        #region Audio

        private bool isBurning;

        public bool IsBurning
        {
            get
            {
                return this.isBurning;
            }

            set
            {
                if (this.isBurning != value)
                {
                    this.isBurning = value;

                    //walk faster when burning
                    var oldSpeed = this.WalkSpeed;
                    this.WalkSpeed = oldSpeed;

                    if (this.isBurning)
                    {
                        this.model.RunAction(SCNAction.Sequence(new SCNAction[]
                        {
                            SCNAction.PlayAudioSource(catchFireSound, false),
                            SCNAction.PlayAudioSource(ouchSound, false),
                            SCNAction.RepeatActionForever(SCNAction.Sequence(new SCNAction[]
                            {
                                SCNAction.FadeOpacityTo(0.01f, 0.1),
                                SCNAction.FadeOpacityTo(1f, 0.1)
                            }))
                        }));

                        this.whiteSmokeEmitter.BirthRate = 0f;
                        this.fireEmitter.BirthRate = this.fireEmitterBirthRate;
                        this.smokeEmitter.BirthRate = this.smokeEmitterBirthRate;
                    }
                    else
                    {
                        this.model.RemoveAllAudioPlayers();
                        this.model.RemoveAllActions();
                        this.model.Opacity = 1f;
                        this.model.RunAction(SCNAction.PlayAudioSource(aahSound, false));

                        SCNTransaction.Begin();
                        SCNTransaction.AnimationDuration = 0f;
                        this.whiteSmokeEmitter.BirthRate = this.whiteSmokeEmitterBirthRate;
                        this.fireEmitter.BirthRate = 0f;
                        this.smokeEmitter.BirthRate = 0f;
                        SCNTransaction.Commit();

                        SCNTransaction.Begin();
                        SCNTransaction.AnimationDuration = 5f;
                        whiteSmokeEmitter.BirthRate = 0f;
                        SCNTransaction.Commit();
                    }
                }
            }
        }

        private void PlayFootStep()
        {
            if (this.groundNode != null && this.IsWalking)
            {
                // We are in the air, no sound to play.
                // Play a random step sound.
                int randSnd = (new Random().Next(0, 32767) * Character.StepsCount);
                var stepSoundIndex = Math.Min(Character.StepsCount - 1, randSnd);
                characterNode.RunAction(SCNAction.PlayAudioSource(this.steps[stepSoundIndex], false));
            }
        }

        private void PlayJumpSound()
        {
            this.characterNode.RunAction(SCNAction.PlayAudioSource(this.jumpSound, false));
        }

        private void PlayAttackSound()
        {
            this.characterNode.RunAction(SCNAction.PlayAudioSource(this.attackSound, false));
        }

        #endregion

        #region Controlling the character

        private nfloat directionAngle = 0f;

        public nfloat DirectionAngle
        {
            get
            {
                return this.directionAngle;
            }

            set
            {
                this.directionAngle = value;
                this.characterOrientation.RunAction(SCNAction.RotateTo(0f, this.directionAngle, 0f, 0.1f, true));
            }
        }

        public void Update(double time, ISCNSceneRenderer renderer)
        {
            this.frameCounter += 1;

            if (this.shouldResetCharacterPosition)
            {
                this.shouldResetCharacterPosition = false;
                this.ResetCharacterPosition();
            }
            else
            {
                var characterVelocity = SCNVector3.Zero;

                // setup
                var groundMove = SCNVector3.Zero;

                // did the ground moved?
                if (this.groundNode != null)
                {
                    var groundPosition = groundNode.WorldPosition;
                    groundMove = groundPosition - this.groundNodeLastPosition;
                }

                characterVelocity = new SCNVector3(groundMove.X, 0, groundMove.Z);

                var direction = this.CharacterDirection(renderer.PointOfView);

                if (this.previousUpdateTime == 0d)
                {
                    this.previousUpdateTime = time;
                }

                var deltaTime = time - previousUpdateTime;
                var characterSpeed = (nfloat)deltaTime * Character.SpeedFactor * this.WalkSpeed;
                var virtualFrameCount = (int)(deltaTime / (1d / 60d));
                this.previousUpdateTime = time;

                // move
                if (!direction.AllZero())
                {
                    characterVelocity = direction * (float)characterSpeed;
                    var runModifier = 1f;

#if __OSX__
                    // TODO: UI thread exception
                    //if (AppKit.NSEvent.CurrentModifierFlags.HasFlag(AppKit.NSEventModifierMask.ShiftKeyMask))
                    //{
                    //   runModifier = 2f;
                    //}
#endif
                    this.WalkSpeed = (nfloat)(runModifier * direction.Length);

                    // move character
                    this.DirectionAngle = (nfloat)Math.Atan2(direction.X, direction.Z);

                    this.IsWalking = true;
                }
                else
                {
                    this.IsWalking = false;
                }

                // put the character on the ground
                var up = new SCNVector3(0f, 1f, 0f);
                var wPosition = this.characterNode.WorldPosition;
                // gravity
                this.downwardAcceleration -= Character.Gravity;
                wPosition.Y += downwardAcceleration;
                var HIT_RANGE = 0.2f;
                var p0 = wPosition;
                var p1 = wPosition;
                p0.Y = wPosition.Y + up.Y * HIT_RANGE;
                p1.Y = wPosition.Y - up.Y * HIT_RANGE;

                var options = new NSMutableDictionary<NSString, NSObject>()
                {
                    { SCNHitTest.BackFaceCullingKey, NSObject.FromObject(false) },
                    { SCNHitTest.OptionCategoryBitMaskKey, NSNumber.FromFloat(Character.CollisionMeshBitMask) },
                    { SCNHitTest.IgnoreHiddenNodesKey, NSObject.FromObject(false) }
                };

                var hitFrom = new SCNVector3(p0);
                var hitTo = new SCNVector3(p1);
                var hitResult = renderer.Scene.RootNode.HitTest(hitFrom, hitTo, options).FirstOrDefault();

                var wasTouchingTheGroup = this.groundNode != null;
                this.groundNode = null;
                var touchesTheGround = false;
                var wasBurning = this.IsBurning;

                var hit = hitResult;
                if (hit != null)
                {
                    var ground = new SCNVector3(hit.WorldCoordinates);
                    if (wPosition.Y <= ground.Y + Character.CollisionMargin)
                    {
                        wPosition.Y = ground.Y + Character.CollisionMargin;
                        if (this.downwardAcceleration < 0f)
                        {
                            this.downwardAcceleration = 0f;
                        }
                        this.groundNode = hit.Node;
                        touchesTheGround = true;

                        //touching lava?
                        this.IsBurning = this.groundNode?.Name == "COLL_lava";
                    }
                }
                else
                {
                    if (wPosition.Y < Character.MinAltitude)
                    {
                        wPosition.Y = Character.MinAltitude;
                        //reset
                        this.QueueResetCharacterPosition();
                    }
                }

                this.groundNodeLastPosition = this.groundNode != null ? this.groundNode.WorldPosition : SCNVector3.Zero;

                //jump -------------------------------------------------------------
                if (this.jumpState == 0)
                {
                    if (this.IsJump && touchesTheGround)
                    {
                        this.downwardAcceleration += Character.JumpImpulse;
                        this.jumpState = 1;

                        this.model.GetAnimationPlayer(new NSString("jump"))?.Play();
                    }
                }
                else
                {
                    if (this.jumpState == 1 && !this.IsJump)
                    {
                        this.jumpState = 2;
                    }

                    if (this.downwardAcceleration > 0f)
                    {
                        for (int i = 0; i < virtualFrameCount; i++)
                        {
                            downwardAcceleration *= this.jumpState == 1 ? 0.99f : 0.2f;
                        }
                    }

                    if (touchesTheGround)
                    {
                        if (!wasTouchingTheGroup)
                        {
                            this.model.GetAnimationPlayer(new NSString("jump"))?.StopWithBlendOutDuration(0.1);

                            // trigger jump particles if not touching lava
                            if (this.IsBurning)
                            {
                                this.model.FindChildNode("dustEmitter", true)?.AddParticleSystem(this.jumpDustParticle);
                            }
                            else
                            {
                                // jump in lava again
                                if (wasBurning)
                                {
                                    this.characterNode.RunAction(SCNAction.Sequence(new SCNAction[]
                                    {
                                        SCNAction.PlayAudioSource(this.catchFireSound, false),
                                        SCNAction.PlayAudioSource(this.ouchSound,  false)
                                    }));
                                }
                            }
                        }

                        if (!this.IsJump)
                        {
                            this.jumpState = 0;
                        }
                    }
                }

                if (touchesTheGround && !wasTouchingTheGroup && !this.IsBurning && this.lastStepFrame < this.frameCounter - 10)
                {
                    // sound
                    this.lastStepFrame = frameCounter;
                    this.characterNode.RunAction(SCNAction.PlayAudioSource(this.steps[0], false));
                }

                if (wPosition.Y < this.characterNode.Position.Y)
                {
                    wPosition.Y = this.characterNode.Position.Y;
                }

                //------------------------------------------------------------------

                // progressively update the elevation node when we touch the ground
                if (touchesTheGround)
                {
                    this.targetAltitude = (float)wPosition.Y;
                }

                this.BaseAltitude *= 0.95f;
                this.BaseAltitude += this.targetAltitude * 0.05f;

                characterVelocity.Y += this.downwardAcceleration;
                if (characterVelocity.LengthSquared > 10E-4 * 10E-4)
                {
                    var startPosition = this.characterNode.PresentationNode.WorldPosition + this.collisionShapeOffsetFromModel;
                    this.SlideInWorld(startPosition, characterVelocity);
                }
            }
        }

        #endregion

        #region Animating the character

        private nfloat walkSpeed = 1f;

        private bool isWalking;

        public bool IsAttacking => this.attackCount > 0;

        public bool IsWalking
        {
            get
            {
                return this.isWalking;
            }

            set
            {
                if (this.isWalking != value)
                {
                    this.isWalking = value;
                    // Update node animation.
                    var player = this.model.GetAnimationPlayer(new NSString("walk"));
                    if (this.isWalking)
                    {
                        player?.Play();
                    }
                    else
                    {
                        player?.StopWithBlendOutDuration(0.2f);
                    }
                }
            }
        }

        public nfloat WalkSpeed
        {
            get
            {
                return this.walkSpeed;
            }

            set
            {
                this.walkSpeed = value;
                var burningFactor = this.IsBurning ? 2f : 1f;

                var player = this.model.GetAnimationPlayer(new NSString("walk"));
                if (player != null)
                {
                    player.Speed = Character.SpeedFactor * this.walkSpeed * burningFactor;
                }
            }
        }

        public void Attack()
        {
            this.attackCount += 1;
            this.model.GetAnimationPlayer(new NSString("spin"))?.Play();
            DispatchQueue.MainQueue.DispatchAfter(new DispatchTime(DispatchTime.Now, 5), () =>
            {
                this.attackCount -= 1;
            });

            this.spinParticleAttach.AddParticleSystem(this.spinCircleParticle);
        }

        private SCNVector3 CharacterDirection(SCNNode pointOfView)
        {
            SCNVector3 result;

            var controllerDir = this.Direction;
            if (controllerDir.AllZero())
            {
                result = SCNVector3.Zero;
            }
            else
            {
                var directionWorld = SCNVector3.Zero;
                if (pointOfView != null)
                {
                    var p1 = pointOfView.PresentationNode.ConvertPositionToNode(new SCNVector3(controllerDir.X, 0f, controllerDir.Y), null);
                    var p0 = pointOfView.PresentationNode.ConvertPositionToNode(SCNVector3.Zero, null);
                    directionWorld = p1 - p0;
                    directionWorld.Y = 0f;

                    if (directionWorld != SCNVector3.Zero)
                    {
                        var minControllerSpeedFactor = 0.2f;
                        var maxControllerSpeedFactor = 1f;
                        var speed = controllerDir.Length * (maxControllerSpeedFactor - minControllerSpeedFactor) + minControllerSpeedFactor;
                        directionWorld = speed * SCNVector3.Normalize(directionWorld);
                    }
                }

                result = directionWorld;
            }

            return result;
        }

        private void ResetCharacterPosition()
        {
            this.characterNode.Position = Character.InitialPosition;
            this.downwardAcceleration = 0f;
        }

        #endregion

        #region enemy

        public void DidHitEnemy()
        {
            this.model.RunAction(SCNAction.Group(new SCNAction[]
            {
                SCNAction.PlayAudioSource(this.hitEnemySound, false),
                SCNAction.Sequence(new SCNAction[]
                {
                    SCNAction.Wait(0.5),
                    SCNAction.PlayAudioSource(explodeEnemySound, false)
                })
            }));
        }

        public void WasTouchedByEnemy()
        {
            var time = DateTime.UtcNow.Ticks;
            if (time > this.lastHitTime + 1d)
            {
                this.lastHitTime = time;
                this.model.RunAction(SCNAction.Sequence(new SCNAction[]
                {
                    SCNAction.PlayAudioSource(this.hitSound, false),
                    SCNAction.RepeatAction(SCNAction.Sequence(new SCNAction[]
                    {
                        SCNAction.FadeOpacityTo(0.01f, 0.1f),
                        SCNAction.FadeOpacityTo(1f, 0.1f)
                    }), 4)
                }));
            }
        }

        #endregion

        #region utils

        public static SCNAnimationPlayer LoadAnimation(string sceneName)
        {
            var scene = SCNScene.FromFile(sceneName);

            SCNAnimationPlayer animationPlayer = null;

            // find top level animation
            scene.RootNode.EnumerateChildNodes((SCNNode child, out bool stop) =>
            {
                var keys = child.GetAnimationKeys();
                if (keys.Any())
                {
                    animationPlayer = child.GetAnimationPlayer(keys[0]);
                    stop = true;
                }
                else
                {
                    stop = false;
                }
            });

            return animationPlayer;
        }

        #endregion

        #region physics contact

        private void SlideInWorld(SCNVector3 start, SCNVector3 velocity)
        {
            var maxSlideIteration = 4;
            var iteration = 0;
            var stop = false;

            var replacementPoint = start;

            var options = new SCNPhysicsTest()
            {
                CollisionBitMask = (int)Bitmask.Collision,
                SearchMode = SCNPhysicsSearchMode.Closest,
            };

            while (!stop)
            {
                var from = SCNMatrix4.Identity;
                SimdExtensions.SetPosition(ref from, start);

                var to = SCNMatrix4.Identity;
                SimdExtensions.SetPosition(ref to, start + velocity);

                var contacts = this.PhysicsWorld.ConvexSweepTest(this.characterCollisionShape, from, to, options.Dictionary);
                if (contacts.Any())
                {
                    (velocity, start) = this.HandleSlidingAtContact(contacts.FirstOrDefault(), start, velocity);
                    iteration += 1;

                    if (velocity.LengthSquared <= (10E-3 * 10E-3) || iteration >= maxSlideIteration)
                    {
                        replacementPoint = start;
                        stop = true;
                    }
                }
                else
                {
                    replacementPoint = start + velocity;
                    stop = true;
                }
            }

            this.characterNode.WorldPosition = replacementPoint - this.collisionShapeOffsetFromModel;
        }

        private Tuple<SCNVector3, SCNVector3> HandleSlidingAtContact(SCNPhysicsContact closestContact, SCNVector3 start, SCNVector3 velocity)
        {
            var originalDistance = velocity.Length;

            var colliderPositionAtContact = start + (float)closestContact.SweepTestFraction * velocity;

            // Compute the sliding plane.
            var slidePlaneNormal = new SCNVector3(closestContact.ContactNormal);
            var slidePlaneOrigin = new SCNVector3(closestContact.ContactPoint);
            var centerOffset = slidePlaneOrigin - colliderPositionAtContact;

            // Compute destination relative to the point of contact.
            var destinationPoint = slidePlaneOrigin + velocity;

            // We now project the destination point onto the sliding plane.
            var distPlane = SCNVector3.Dot(slidePlaneOrigin, slidePlaneNormal);

            // Project on plane.
            var t = Utils.PlaneIntersect(slidePlaneNormal, distPlane, destinationPoint, slidePlaneNormal);

            var normalizedVelocity = velocity * (1f / originalDistance);
            var angle = SCNVector3.Dot(slidePlaneNormal, normalizedVelocity);

            var frictionCoeff = 0.3f;
            if (Math.Abs(angle) < 0.9f)
            {
                t += (float)10E-3;
                frictionCoeff = 1.0f;
            }

            var newDestinationPoint = (destinationPoint + t * slidePlaneNormal) - centerOffset;
            // Advance start position to nearest point without collision.
            var computedVelocity = frictionCoeff * (float)(1f - closestContact.SweepTestFraction) * originalDistance * SCNVector3.Normalize(newDestinationPoint - start);

            return new Tuple<SCNVector3, SCNVector3>(computedVelocity, colliderPositionAtContact);
        }

        #endregion
    }
}