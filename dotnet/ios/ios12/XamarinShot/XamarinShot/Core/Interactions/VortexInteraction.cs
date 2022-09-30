namespace XamarinShot.Models;

public interface IVortexActivationDelegate
{
        void VortexDidActivate (VortexInteraction vortex);
}

public class VortexInteraction : IInteraction, ILeverInteractionDelegate
{
        const double VortexAnimationDuration = 11.5d;
        const double LiftStageStartTime = 1d;
        const double LiftStageEndTime = 3d;

        double startInitialFloatTime;
        double startVortexTime;

        State state = State.None;

        // Chasm
        SCNVector3 chasmFinalScale = new SCNVector3 (0.96f, 1f, 0.96f);
        readonly SCNNode? chasmPhysics;
        SCNNode? chasmExpandObject;

        // Vortex Cylinder's shape represents vortex shape, and is used to animate vortex
        SCNNode? vortexCylinder;

        public VortexInteraction (IInteractionDelegate @delegate)
        {
                Delegate = @delegate;

                // Chasm Occluder Box: that will take over the table occluder once table occluder is removed for Vortex Interaction
                var vortex = SCNNodeExtensions.LoadSCNAsset ("chasm_animation");

                // Chasm
                chasmPhysics = vortex.FindChildNode ("chasm", true);
                if (chasmPhysics is null)
                {
                        throw new Exception ("Vortex has no chasm");
                }

                foreach (var child in chasmPhysics.ChildNodes)
                {
                        if (!string.IsNullOrEmpty (child.Name) && child.Name.StartsWith ("occluder", StringComparison.Ordinal))
                        {
                                child.SetNodeToOccluder ();
                        }
                }

                chasmPhysics.WorldPosition = new SCNVector3 (0f, -0.1f, 0f); // avoid z-fight with ShadowPlane
                chasmPhysics.Scale = chasmFinalScale;
                chasmPhysics.StopAllAnimations ();
        }

        /// <summary>
        /// Stage time
        /// </summary>
        protected double TimeSinceInitialFloatStart => GameTime.Time - startInitialFloatTime;

        protected double TimeSinceVortexStart => GameTime.Time - startVortexTime;

        public bool IsActivated => state != State.None;

        public IInteractionDelegate Delegate { get; private set; }

        public IVortexActivationDelegate? VortexActivationDelegate { get; set; }

        public SFXCoordinator? SfxCoordinator { get; set; }

        public MusicCoordinator? MusicCoordinator { get; set; }

        public void Activate ()
        {
                if (Delegate is null)
                {
                        throw new Exception ("No delegate");
                }

                Delegate.DispatchActionToServer (new GameActionType { Type = GameActionType.GActionType.OneHitKOAnimate });
        }

        public void Handle (GameActionType gameAction, Player player)
        {
                if (Delegate is null)
                {
                        throw new Exception ("No delegate");
                }

                if (gameAction.Type == GameActionType.GActionType.OneHitKOAnimate && state == State.None)
                {
                        state = State.InitialWait;
                        startInitialFloatTime = GameTime.Time;

                        Delegate.RemoveTableBoxNodeFromLevel ();
                        Delegate.RemoveAllPhysicsBehaviors ();

                        // Kill all catapults
                        VortexActivationDelegate?.VortexDidActivate (this);
                        SetBlocksToNoGravity ();

                        Delegate.ServerDispatchActionToAll (new GameActionType { Type = GameActionType.GActionType.OneHitKOAnimate });

                        if (SfxCoordinator is not null)
                        {
                                SfxCoordinator.PlayAudioFile ("vortex_04", 0.5f, false);
                        }

                        if (MusicCoordinator is not null)
                        {
                                MusicCoordinator.StopCurrentMusic (2d);
                        }
                }
        }

        public void Update (CameraInfo cameraInfo)
        {
                UpdateVortexState ();
        }

        void UpdateVortexState ()
        {
                switch (state)
                {
                        case State.None:
                                break;

                        case State.InitialWait:
                                if (TimeSinceInitialFloatStart > LiftStageStartTime)
                                {
                                        PrepareForVortexAnimationStart ();
                                        state = State.AnimateLift;
                                }
                                break;

                        case State.AnimateLift:
                                if (TimeSinceInitialFloatStart > LiftStageEndTime)
                                {
                                        PrepareForVortexAnimationEnded ();
                                        state = State.AnimateVortex;
                                }
                                break;

                        case State.AnimateVortex:
                                if (TimeSinceVortexStart < VortexAnimationDuration)
                                {
                                        AnimateVortex ();
                                } else {
                                        OnVortexAnimationEnded ();
                                        state = State.None;
                                }
                                break;
                }
        }

        #region Animate Vortex

        // Stable vortex (values found through experimentation)
        const float RadialSpringConstant = 100f;
        const float TangentVelocitySpringContant = 40f;
        const float MaxVelocity = 3f;

        const float MaxRandomVortexTorque = 0f;
        const double MaxRandomVortexForce = 0.2d;

        SCNVector3 lastFront = new SCNVector3 (0f, 0f, -1f);
        float lastVortexCenterY = 0f;
        float lastVortexHeight = 0f;
        float lastOuterRadius = 0f;

        public void AnimateVortex ()
        {
                if (Delegate is null)
                {
                        throw new Exception ("No delegate");
                }

                if (vortexCylinder is null)
                {
                        throw new Exception ("Vortex animation cylinder not set");
                }

                // Vortex shape from animation
                var vortexShape = vortexCylinder.PresentationNode.Scale;
                var vortexHeightDelta = vortexShape.Y - lastVortexHeight;
                lastVortexHeight = vortexShape.Y;

                var vortexCenterY = vortexCylinder.PresentationNode.WorldPosition.Y;
                var vortexCenterYDelta = vortexCenterY - lastVortexCenterY;
                lastVortexCenterY = vortexCenterY;

                // Deform shape over time
                var maxOuterRadius = vortexShape.X;
                var maxInnerRadius = maxOuterRadius * 0.2f; // 20 % from experiment
                var maxOuterRadiusDelta = maxOuterRadius - lastOuterRadius;
                lastOuterRadius = maxOuterRadius;

                // Orbital velocity
                var currentFront = vortexCylinder.PresentationNode.WorldFront;
                var orbitalMoveDelta = (currentFront - lastFront).Length * maxInnerRadius;
                lastFront = currentFront;

                var orbitalVelocityFactor = 5f;
                var orbitalVelocity = (orbitalMoveDelta / (float)GameTime.DeltaTime) * orbitalVelocityFactor;

                var topBound = vortexCenterY + vortexShape.Y * 0.5f;
                var bottomBound = vortexCenterY - vortexShape.Y * 0.5f;

                var blockObjects = Delegate.AllBlockObjects;
                var up = SCNVector3.UnitY;
                foreach (var block in blockObjects)
                {
                        if (block.PhysicsNode?.PhysicsBody is not null)
                        {
                                var position = block.PhysicsNode.PresentationNode.WorldPosition;
                                var positionWithoutY = new SCNVector3 (position.X, 0f, position.Z);
                                var distanceFromCenter = positionWithoutY.Length;
                                var directionToCenter = -SCNVector3.Normalize (positionWithoutY);

                                // Adjust radius into curve
                                // Equation representing a half radius chord of circle equation
                                var normalizedY = DigitExtensions.Clamp (position.Y / topBound, 0f, 1f);
                                var radiusFactor = (float)Math.Sqrt (4f - 3f * normalizedY * normalizedY) - 1f;
                                radiusFactor = radiusFactor * 0.8f + 0.2f;
                                var innerRadius = maxInnerRadius * radiusFactor;
                                var outerRadius = maxOuterRadius * radiusFactor;

                                // Cap velocity
                                var maxVelocity = 30f;
                                if (block.PhysicsNode.PhysicsBody.Velocity.Length > maxVelocity)
                                {
                                        block.PhysicsNode.PhysicsBody.Velocity = SCNVector3.Normalize (block.PhysicsNode.PhysicsBody.Velocity) * maxVelocity;
                                }

                                var force = SCNVector3.Zero;

                                // Stage specific manipulation
                                var vortexDirection = SCNVector3.Cross (directionToCenter, up);
                                var speedInVortexDirection = SCNVector3.Dot (block.PhysicsNode.PhysicsBody.Velocity, vortexDirection);

                                // Stable vortex pull
                                var pullForceMagnitude = (speedInVortexDirection * speedInVortexDirection) * (float)(block.PhysicsNode.PhysicsBody.Mass) / distanceFromCenter;
                                force += pullForceMagnitude * directionToCenter;

                                // Pull into outer radius
                                var radialInwardForceMagnitude = RadialSpringConstant * (float)Math.Max (0d, distanceFromCenter - outerRadius);
                                force += radialInwardForceMagnitude * directionToCenter;

                                // Pull away from inner radius
                                var radialOutwardForceMagnitude = RadialSpringConstant * (float)Math.Max (0d, innerRadius - distanceFromCenter);
                                force += -radialOutwardForceMagnitude * directionToCenter;

                                // Vortex velocity adjustment
                                if (distanceFromCenter > innerRadius)
                                {
                                        var tangentForceMagnitude = TangentVelocitySpringContant * (speedInVortexDirection - orbitalVelocity);
                                        force += -tangentForceMagnitude * vortexDirection * (0.5f + (float)(random.NextDouble () * 1d));
                                }

                                // Random forces/torque
                                force += force.Length * (float)((random.NextDouble () * 2d - 1d) * MaxRandomVortexForce) * up;
                                ApplyRandomTorque (block.PhysicsNode.PhysicsBody, MaxRandomVortexTorque);

                                // Top bound pull down
                                var topBoundForceMagnitude = RadialSpringConstant * (float)Math.Max (0d, position.Y - topBound);
                                force += topBoundForceMagnitude * -up;

                                // Bottom bound pull up
                                var bottomBoundForceMagnitude = RadialSpringConstant * (float)Math.Max (0d, bottomBound - position.Y);
                                force += bottomBoundForceMagnitude * up;

                                block.PhysicsNode.PhysicsBody.ApplyForce (force, false);

                                // Scale the vortex
                                // The higher position in the bound, more it should move upward to scale the vortex
                                var normalizedPositionInBoundY = DigitExtensions.Clamp ((position.Y - bottomBound) / vortexShape.Y, 0f, 1f);
                                var heightMoveFactor = Math.Abs (normalizedPositionInBoundY - 0.5f);
                                var newPositionY = position.Y + vortexCenterYDelta + vortexHeightDelta * heightMoveFactor;

                                var positionXZ = new SCNVector3 (position.X, 0f, position.Z);
                                var radialMoveFactor = DigitExtensions.Clamp (distanceFromCenter / outerRadius, 0f, 1f);
                                var newPositionXZ = positionXZ + maxOuterRadiusDelta * radialMoveFactor * -directionToCenter;

                                block.PhysicsNode.WorldPosition = new SCNVector3 (newPositionXZ.X, newPositionY, newPositionXZ.Z);
                                block.PhysicsNode.WorldOrientation = block.PhysicsNode.PresentationNode.WorldOrientation;
                                block.PhysicsNode.PhysicsBody.ResetTransform ();
                        }
                }
        }

        public void HandleTouch (TouchType type, Ray camera) { }

        #endregion

        #region Prepare for Vortex

        const float MaxInitialImpulse = 3f;
        const float MaxInitialTorque = 1f; // Found through experimentation

        void SetBlocksToNoGravity ()
        {
                EnumerateThroughBlocks ((physicsBody) =>
                {
                        physicsBody.AffectedByGravity = false;
                });
        }

        Random random = new Random ();

        void PrepareForVortexAnimationStart ()
        {
                EnumerateThroughBlocks ((physicsBody) =>
                {
                        physicsBody.AffectedByGravity = false;
                        var initialImpulse = MaxInitialImpulse * (float)(random.NextDouble () * 0.7d + 0.3d);
                        physicsBody.ApplyForce (new SCNVector3 (0f, initialImpulse, 0f), true);

                        ApplyRandomTorque (physicsBody, MaxInitialTorque);

                        physicsBody.Damping = 0.4f;
                });
        }

        void PrepareForVortexAnimationEnded ()
        {
                if (Delegate is null)
                {
                        throw new Exception ("No Delegate");
                }

                EnumerateThroughBlocks ((physicsBody) =>
                {
                        physicsBody.Damping = 0.1f;
                });

                // Chasm Expand Object (used for animation)
                var vortex = SCNNodeExtensions.LoadSCNAsset ("chasm_animation");
                vortexCylinder = vortex.FindChildNode ("Cylinder", true);
                if (vortexCylinder is null)
                {
                        throw new Exception ("Vortex has no cone");
                }

                Delegate.AddNodeToLevel (vortexCylinder);
                vortexCylinder.StopAllAnimations ();
                vortexCylinder.PlayAllAnimations ();
                vortexCylinder.Hidden = true; // Vortex Cylinder is only used for deriving the vortex shape animation

                // Chasm Expand Object (used for animation)
                chasmExpandObject = vortex.FindChildNode ("chasm", true);
                if (chasmExpandObject is null)
                {
                        throw new Exception ("Vortex has no chasm");
                }

                foreach (var child in chasmExpandObject.ChildNodes)
                {
                        if (!string.IsNullOrEmpty (child.Name) && child.Name.StartsWith ("occluder", StringComparison.Ordinal))
                        {
                                child.SetNodeToOccluder ();
                        }
                }

                Delegate.AddNodeToLevel (chasmExpandObject);
                chasmExpandObject.StopAllAnimations ();
                chasmExpandObject.PlayAllAnimations ();

                var vortexShape = vortexCylinder.PresentationNode.Scale;
                lastOuterRadius = vortexShape.X;
                lastVortexCenterY = vortexCylinder.PresentationNode.WorldPosition.Y;
                lastVortexHeight = vortexShape.Y;
                lastFront = vortexCylinder.PresentationNode.WorldFront;

                startVortexTime = GameTime.Time;
        }

        void OnVortexAnimationEnded ()
        {
                // Remove or hide everything
                vortexCylinder?.StopAllAnimations ();
                vortexCylinder?.RemoveFromParentNode ();
                chasmExpandObject?.RemoveFromParentNode ();

                if (Delegate is null)
                {
                        throw new Exception ("No delegate");
                }

                var blockObjects = Delegate.AllBlockObjects;
                foreach (var block in blockObjects)
                {
                        block.ObjectRootNode!.Hidden = true;
                }
        }

        void EnumerateThroughBlocks (Action<SCNPhysicsBody> physicsFunction)
        {
                if (Delegate is null)
                {
                        throw new Exception ("No delegate");
                }

                var blockObjects = Delegate.AllBlockObjects;
                foreach (var block in blockObjects)
                {
                        if (block.PhysicsNode?.PhysicsBody is not null)
                        {
                                physicsFunction (block.PhysicsNode.PhysicsBody);
                        }
                }
        }

        #endregion

        #region Helper Function

        void ApplyRandomTorque (SCNPhysicsBody physicsBody, float maxTorque)
        {
                var randomAxis = new SCNVector3 ((float)random.NextDouble (), (float)random.NextDouble (), (float)random.NextDouble ());
                randomAxis = SCNVector3.Normalize (randomAxis);

                var randomTorque = new SCNVector4 (randomAxis, (float)(random.NextDouble () * 2d - 1d) * maxTorque);
                physicsBody.ApplyTorque (randomTorque, true);
        }

        float StageProgress (double startTime, double endTime)
        {
                var progress = (float)((TimeSinceInitialFloatStart - startTime) / (endTime - startTime));
                return DigitExtensions.Clamp (progress, 0f, 1f);
        }

        #endregion

        public void DidCollision (SCNNode node, SCNNode otherNode, SCNVector3 position, float impulse) { }

        enum State
        {
                None,
                InitialWait,
                AnimateLift,
                AnimateVortex,
        }
}
