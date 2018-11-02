
namespace XamarinShot.Models
{
    using SceneKit;
    using XamarinShot.Models.Enums;
    using XamarinShot.Utils;
    using System;

    public interface IVortexActivationDelegate
    {
        void VortexDidActivate(VortexInteraction vortex);
    }

    public class VortexInteraction : IInteraction, ILeverInteractionDelegate
    {
        private const double VortexAnimationDuration = 11.5d;
        private const double LiftStageStartTime = 1d;
        private const double LiftStageEndTime = 3d;

        private double startInitialFloatTime;
        private double startVortexTime;

        private State state = State.None;

        // Chasm
        private SCNVector3 chasmFinalScale = new SCNVector3(0.96f, 1f, 0.96f);
        private readonly SCNNode chasmPhysics;
        private SCNNode chasmExpandObject;

        // Vortex Cylinder's shape represents vortex shape, and is used to animate vortex
        private SCNNode vortexCylinder;

        public VortexInteraction(IInteractionDelegate @delegate) 
        {
            this.Delegate = @delegate;

            // Chasm Occluder Box: that will take over the table occluder once table occluder is removed for Vortex Interaction
            var vortex = SCNNodeExtensions.LoadSCNAsset("chasm_animation");

            // Chasm
            this.chasmPhysics = vortex.FindChildNode("chasm", true);
            if(this.chasmPhysics == null)
            {
                throw new Exception("Vortex has no chasm");
            }

            foreach(var child in this.chasmPhysics.ChildNodes)
            {
                if (!String.IsNullOrEmpty(child.Name) && child.Name.StartsWith("occluder", StringComparison.Ordinal))
                {
                    child.SetNodeToOccluder();
                }
            }

            this.chasmPhysics.WorldPosition = new SCNVector3(0f, -0.1f, 0f); // avoid z-fight with ShadowPlane
            this.chasmPhysics.Scale = this.chasmFinalScale;
            this.chasmPhysics.StopAllAnimations();
        }

        /// <summary>
        /// Stage time
        /// </summary>
        protected double TimeSinceInitialFloatStart => GameTime.Time - this.startInitialFloatTime;

        protected double TimeSinceVortexStart => GameTime.Time - this.startVortexTime;

        public bool IsActivated => this.state != State.None;

        public IInteractionDelegate Delegate { get; private set; }

        public IVortexActivationDelegate VortexActivationDelegate { get; set; }

        public SFXCoordinator SfxCoordinator { get; set; }

        public MusicCoordinator MusicCoordinator { get; set; }

        public void Activate()
        {
            if(this.Delegate == null)
            {
                throw new Exception("No delegate");
            }

            this.Delegate.DispatchActionToServer(new GameActionType { Type = GameActionType.GActionType.OneHitKOAnimate });
        }

        public void Handle(GameActionType gameAction, Player player)
        {
            if (this.Delegate == null)
            {
                throw new Exception("No delegate");
            }

            if (gameAction.Type == GameActionType.GActionType.OneHitKOAnimate && this.state == State.None)
            {
                this.state = State.InitialWait;
                this.startInitialFloatTime = GameTime.Time;

                this.Delegate.RemoveTableBoxNodeFromLevel();
                this.Delegate.RemoveAllPhysicsBehaviors();

                // Kill all catapults
                this.VortexActivationDelegate?.VortexDidActivate(this);
                this.SetBlocksToNoGravity();

                this.Delegate.ServerDispatchActionToAll(new GameActionType { Type = GameActionType.GActionType.OneHitKOAnimate });

                if (this.SfxCoordinator != null)
                {
                    this.SfxCoordinator.PlayAudioFile("vortex_04", 0.5f, false);
                }

                if (this.MusicCoordinator != null)
                {
                    this.MusicCoordinator.StopCurrentMusic(2d);
                }
            }
        }

        public void Update(CameraInfo cameraInfo)
        {
            this.UpdateVortexState();
        }

        private void UpdateVortexState()
        {
            switch (this.state)
            {
                case State.None:
                    break;

                case State.InitialWait:
                    if (this.TimeSinceInitialFloatStart > LiftStageStartTime)
                    {
                        this.PrepareForVortexAnimationStart();
                        this.state = State.AnimateLift;
                    }
                    break;

                case State.AnimateLift:
                    if (this.TimeSinceInitialFloatStart > LiftStageEndTime)
                    {
                        this.PrepareForVortexAnimationEnded();
                        this.state = State.AnimateVortex;
                    }
                    break;

                case State.AnimateVortex:
                    if (this.TimeSinceVortexStart < VortexAnimationDuration)
                    {
                        this.AnimateVortex();
                    }
                    else
                    {
                        this.OnVortexAnimationEnded();
                        this.state = State.None;
                    }
                    break;
            }
        }

        #region Animate Vortex

        // Stable vortex (values found through experimentation)
        private const float RadialSpringConstant = 100f;
        private const float TangentVelocitySpringContant = 40f;
        private const float MaxVelocity = 3f;

        private const float MaxRandomVortexTorque = 0f;
        private const double MaxRandomVortexForce = 0.2d;

        private SCNVector3 lastFront = new SCNVector3(0f, 0f, -1f);
        private float lastVortexCenterY = 0f;
        private float lastVortexHeight = 0f;
        private float lastOuterRadius = 0f;

        public void AnimateVortex()
        {
            if (this.Delegate == null)
            {
                throw new Exception("No delegate");
            }

            if (this.vortexCylinder == null)
            {
                throw new Exception("Vortex animation cylinder not set");
            }

            // Vortex shape from animation
            var vortexShape = this.vortexCylinder.PresentationNode.Scale;
            var vortexHeightDelta = vortexShape.Y - this.lastVortexHeight;
            this.lastVortexHeight = vortexShape.Y;

            var vortexCenterY = this.vortexCylinder.PresentationNode.WorldPosition.Y;
            var vortexCenterYDelta = vortexCenterY - this.lastVortexCenterY;
            this.lastVortexCenterY = vortexCenterY;

            // Deform shape over time
            var maxOuterRadius = vortexShape.X;
            var maxInnerRadius = maxOuterRadius * 0.2f; // 20 % from experiment
            var maxOuterRadiusDelta = maxOuterRadius - this.lastOuterRadius;
            this.lastOuterRadius = maxOuterRadius;

            // Orbital velocity
            var currentFront = this.vortexCylinder.PresentationNode.WorldFront;
            var orbitalMoveDelta = (currentFront - this.lastFront).Length * maxInnerRadius;
            this.lastFront = currentFront;

            var orbitalVelocityFactor = 5f;
            var orbitalVelocity = (orbitalMoveDelta / (float)GameTime.DeltaTime) * orbitalVelocityFactor;

            var topBound = vortexCenterY + vortexShape.Y * 0.5f;
            var bottomBound = vortexCenterY - vortexShape.Y * 0.5f;

            var blockObjects = this.Delegate.AllBlockObjects;
            var up = SCNVector3.UnitY;
            foreach (var block in blockObjects)
            {
                if (block.PhysicsNode?.PhysicsBody != null)
                {
                    var position = block.PhysicsNode.PresentationNode.WorldPosition;
                    var positionWithoutY = new SCNVector3(position.X, 0f, position.Z);
                    var distanceFromCenter = positionWithoutY.Length;
                    var directionToCenter = -SCNVector3.Normalize(positionWithoutY);

                    // Adjust radius into curve
                    // Equation representing a half radius chord of circle equation
                    var normalizedY = DigitExtensions.Clamp(position.Y / topBound, 0f, 1f);
                    var radiusFactor = (float)Math.Sqrt(4f - 3f * normalizedY * normalizedY) - 1f;
                    radiusFactor = radiusFactor * 0.8f + 0.2f;
                    var innerRadius = maxInnerRadius * radiusFactor;
                    var outerRadius = maxOuterRadius * radiusFactor;

                    // Cap velocity
                    var maxVelocity = 30f;
                    if (block.PhysicsNode.PhysicsBody.Velocity.Length > maxVelocity)
                    {
                        block.PhysicsNode.PhysicsBody.Velocity = SCNVector3.Normalize(block.PhysicsNode.PhysicsBody.Velocity) * maxVelocity;
                    }

                    var force = SCNVector3.Zero;

                    // Stage specific manipulation
                    var vortexDirection = SCNVector3.Cross(directionToCenter, up);
                    var speedInVortexDirection = SCNVector3.Dot(block.PhysicsNode.PhysicsBody.Velocity, vortexDirection);

                    // Stable vortex pull
                    var pullForceMagnitude = (speedInVortexDirection * speedInVortexDirection) * (float)(block.PhysicsNode.PhysicsBody.Mass) / distanceFromCenter;
                    force += pullForceMagnitude * directionToCenter;

                    // Pull into outer radius
                    var radialInwardForceMagnitude = RadialSpringConstant * (float)Math.Max(0d, distanceFromCenter - outerRadius);
                    force += radialInwardForceMagnitude * directionToCenter;

                    // Pull away from inner radius
                    var radialOutwardForceMagnitude = RadialSpringConstant * (float)Math.Max(0d, innerRadius - distanceFromCenter);
                    force += -radialOutwardForceMagnitude * directionToCenter;

                    // Vortex velocity adjustment
                    if (distanceFromCenter > innerRadius)
                    {
                        var tangentForceMagnitude = TangentVelocitySpringContant * (speedInVortexDirection - orbitalVelocity);
                        force += -tangentForceMagnitude * vortexDirection * (0.5f + (float)(random.NextDouble() * 1d));
                    }

                    // Random forces/torque
                    force += force.Length * (float)((random.NextDouble() * 2d - 1d) * MaxRandomVortexForce) * up;
                    this.ApplyRandomTorque(block.PhysicsNode.PhysicsBody, MaxRandomVortexTorque);

                    // Top bound pull down
                    var topBoundForceMagnitude = RadialSpringConstant * (float)Math.Max(0d, position.Y - topBound);
                    force += topBoundForceMagnitude * -up;

                    // Bottom bound pull up
                    var bottomBoundForceMagnitude = RadialSpringConstant * (float)Math.Max(0d, bottomBound - position.Y);
                    force += bottomBoundForceMagnitude * up;

                    block.PhysicsNode.PhysicsBody.ApplyForce(force, false);

                    // Scale the vortex
                    // The higher position in the bound, more it should move upward to scale the vortex
                    var normalizedPositionInBoundY = DigitExtensions.Clamp((position.Y - bottomBound) / vortexShape.Y, 0f, 1f);
                    var heightMoveFactor = Math.Abs(normalizedPositionInBoundY - 0.5f);
                    var newPositionY = position.Y + vortexCenterYDelta + vortexHeightDelta * heightMoveFactor;

                    var positionXZ = new SCNVector3(position.X, 0f, position.Z);
                    var radialMoveFactor = DigitExtensions.Clamp(distanceFromCenter / outerRadius, 0f, 1f);
                    var newPositionXZ = positionXZ + maxOuterRadiusDelta * radialMoveFactor * -directionToCenter;

                    block.PhysicsNode.WorldPosition = new SCNVector3(newPositionXZ.X, newPositionY, newPositionXZ.Z);
                    block.PhysicsNode.WorldOrientation = block.PhysicsNode.PresentationNode.WorldOrientation;
                    block.PhysicsNode.PhysicsBody.ResetTransform();
                }
            }
        }

        public void HandleTouch(TouchType type, Ray camera) { }

        #endregion

        #region Prepare for Vortex

        private const float MaxInitialImpulse = 3f;
        private const float MaxInitialTorque = 1f; // Found through experimentation

        private void SetBlocksToNoGravity()
        {
            this.EnumerateThroughBlocks((physicsBody) =>
            {
                physicsBody.AffectedByGravity = false;
            });
        }

        private Random random = new Random();

        private void PrepareForVortexAnimationStart()
        {
            this.EnumerateThroughBlocks((physicsBody) =>
            {
                physicsBody.AffectedByGravity = false;
                var initialImpulse = MaxInitialImpulse * (float)(random.NextDouble() * 0.7d + 0.3d);
                physicsBody.ApplyForce(new SCNVector3(0f, initialImpulse, 0f), true);

                this.ApplyRandomTorque(physicsBody, MaxInitialTorque);

                physicsBody.Damping = 0.4f;
            });
        }

        private void PrepareForVortexAnimationEnded()
        {
            if(this.Delegate == null)
            {
                throw new Exception("No Delegate");
            }

            this.EnumerateThroughBlocks((physicsBody) =>
            {
                physicsBody.Damping = 0.1f;
            });

            // Chasm Expand Object (used for animation)
            var vortex = SCNNodeExtensions.LoadSCNAsset("chasm_animation");
            this.vortexCylinder = vortex.FindChildNode("Cylinder", true);
            if(this.vortexCylinder == null)
            {
                throw new Exception("Vortex has no cone");
            }

            this.Delegate.AddNodeToLevel(this.vortexCylinder);
            this.vortexCylinder.StopAllAnimations();
            this.vortexCylinder.PlayAllAnimations();
            this.vortexCylinder.Hidden = true; // Vortex Cylinder is only used for deriving the vortex shape animation

            // Chasm Expand Object (used for animation)
            this.chasmExpandObject = vortex.FindChildNode("chasm", true);
            if(this.chasmExpandObject == null)
            {
                throw new Exception("Vortex has no chasm");
            }

            foreach(var child in this.chasmExpandObject.ChildNodes)
            {
                if (!string.IsNullOrEmpty(child.Name) && child.Name.StartsWith("occluder", StringComparison.Ordinal))
                {
                    child.SetNodeToOccluder();
                }
            }

            this.Delegate.AddNodeToLevel(this.chasmExpandObject);
            this.chasmExpandObject.StopAllAnimations();
            this.chasmExpandObject.PlayAllAnimations();

            var vortexShape = this.vortexCylinder.PresentationNode.Scale;
            this.lastOuterRadius = vortexShape.X;
            this.lastVortexCenterY = this.vortexCylinder.PresentationNode.WorldPosition.Y;
            this.lastVortexHeight = vortexShape.Y;
            this.lastFront = this.vortexCylinder.PresentationNode.WorldFront;

            this.startVortexTime = GameTime.Time;
        }

        private void OnVortexAnimationEnded()
        {
            // Remove or hide everything
            this.vortexCylinder?.StopAllAnimations();
            this.vortexCylinder?.RemoveFromParentNode();
            this.chasmExpandObject?.RemoveFromParentNode();

            if(this.Delegate == null)
            {
                throw new Exception("No delegate");
            }

            var blockObjects = this.Delegate.AllBlockObjects;
            foreach(var block in blockObjects)
            {
                block.ObjectRootNode.Hidden = true;
            }
        }

        private void EnumerateThroughBlocks(Action<SCNPhysicsBody> physicsFunction)
        {
            if (this.Delegate == null)
            {
                throw new Exception("No delegate");
            }

            var blockObjects = this.Delegate.AllBlockObjects;
            foreach(var block in blockObjects)
            {
                if (block.PhysicsNode?.PhysicsBody != null)
                {
                    physicsFunction(block.PhysicsNode.PhysicsBody);
                }
            }
        }

        #endregion

        #region Helper Function

        private void ApplyRandomTorque(SCNPhysicsBody physicsBody, float maxTorque)
        {
            var randomAxis = new SCNVector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            randomAxis = SCNVector3.Normalize(randomAxis);

            var randomTorque = new SCNVector4(randomAxis, (float)(random.NextDouble() * 2d - 1d) * maxTorque);
            physicsBody.ApplyTorque(randomTorque, true);
        }

        private float StageProgress(double startTime, double endTime)
        {
            var progress = (float)((this.TimeSinceInitialFloatStart - startTime) / (endTime - startTime));
            return DigitExtensions.Clamp(progress, 0f, 1f);
        }

        #endregion

        public void DidCollision(SCNNode node, SCNNode otherNode, SCNVector3 position, float impulse) { }

        enum State
        {
            None,
            InitialWait,
            AnimateLift,
            AnimateVortex
        }
    }
}