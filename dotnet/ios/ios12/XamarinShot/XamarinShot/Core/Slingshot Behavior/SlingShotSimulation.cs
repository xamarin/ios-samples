namespace XamarinShot.Models;

public class SimulatedTransform
{
        public SimulatedTransform (SCNVector3 position, SCNQuaternion orientation, SCNVector3 velocity, bool dynamic)
        {
                Position = position;
                Orientation = orientation;
                Velocity = velocity;
                Dynamic = dynamic;
        }

        public SCNVector3 Position { get; set; }

        public SCNQuaternion Orientation { get; set; }

        public SCNVector3 Velocity { get; set; }

        public bool Dynamic { get; set; }
}

// Provides access to most variables used to describe the simulation parameters as well as
// the management of the SceneKit rigid bodies.
public class SlingShotSimulation : NSObject
{
        public double LinearLaunchAnimationTime { get; } = 0.5f;
        public double SimLaunchAnimationTime { get; } = 2.5f;  // needs more time to settle

        const float SimDamping = 0.85f; // the amount of damping to apply to the rigid 5odies
        const float SimBounce = 0f; // the amount of restitution to apply to the rigid bodies
        const float SimShotStrength = 20f; // the strength of the shot when released
        const float SimNeighborStrength = 350f; // the strength for the neighbor springs
        const float SimRestPoseStrength = 5f; // the strength for the restpose force
        const float SimBlend = 0.08f; // the amount of lerp to restPose per iteration,
                                              // higher ends faster, related to finishing sim before simLaunchAnimationTime expires
                                              // the collision plane distance
        const float CollisionPlane = 0.15f; // using half depth of catatpult at 0.25 / 2 ~ 0.125 + slop

        // smoothing the rope from 0 to 1
        const float SmoothRope = 0.25f;

        // the parent space of the slingshot
        SCNMatrix4 restPoseSpace = new SCNMatrix4 ();

        SCNMatrix4 originalRestPoseSpace = new SCNMatrix4 ();

        List<SCNMatrix4> restPoseTransforms = new List<SCNMatrix4> ();

        public SlingShotSimulation (SCNNode rootNode, int count, float segmentRadius) : base ()
        {
                InitializeHelpers ();

                for (var i = 0; i < count; i++)
                {
                        var isStatic = (i < BoneInset) || (i >= count - BoneInset);
                        var quaternion = SCNQuaternion.FromAxisAngle (SCNVector3.UnitX, 0);

                        var transform = new SimulatedTransform (SCNVector3.Zero, quaternion, SCNVector3.Zero, !isStatic);
                        simulatedTransforms.Add (transform);
                }
        }

        /// <summary>
        /// The rest pose of the slingshot in world space
        /// </summary>
        public SlingShotPose RestPose => computedRestPose.Value;

        ComputedValue<SlingShotPose> computedRestPose;

        public SCNVector3 FixturePositionL => RestPose.Positions.First ();

        public SCNVector3 FixturePositionR => RestPose.Positions.Last ();

        public SCNVector3 UpVector => SimdExtensions.CreateQuaternion (restPoseSpace).Act (SCNVector3.UnitY);

        // return the number of simulated transforms in this slingshot
        public int SimulatedTransformCount => simulatedTransforms.Count;

        /// <summary>
        /// ignore these bones
        /// weight the vertices at the edge of the rope on first/last bone 100% to avoid cord pulling out
        /// </summary>
        /// <value>The bone inset.</value>
        public int BoneInset { get; set; } = 1;

        /// <summary>
        /// Defines the original (unstretched) length of the rope
        /// </summary>
        public float OriginalTotalLength { get; set; } = 100f;

        /// <summary>
        /// Defines the original (unstretched) length of the leather portion of the rope
        /// </summary>
        public float OriginalLeatherLength { get; set; } = 45f;

        public SCNMatrix4 RestPoseSpace
        {
                get
                {
                        return restPoseSpace;
                }

                set
                {
                        restPoseSpace = value;
                        DirtyCachedValues ();
                }
        }

        /// <summary>
        /// Returns a simulated transform within this simulation
        /// </summary>
        public SimulatedTransform SimulatedTransform (int index)
        {
                return simulatedTransforms [index];
        }

        /// <summary>
        /// Returns a simulated transform within this simulation as float4x4
        /// </summary>
        public SCNMatrix4 SimulatedTransformAsFloat4x4 (int index)
        {
                var mSim = simulatedTransforms [index];
                var quaternion = mSim.Orientation.ToQuaternion ();
                var matrix = SCNMatrix4.Rotate (quaternion);

                matrix.M41 = mSim.Position.X;
                matrix.M42 = mSim.Position.Y;
                matrix.M43 = mSim.Position.Z;
                matrix.M44 = 1f;

                return matrix;
        }

        readonly List<SimulatedTransform> simulatedTransforms = new List<SimulatedTransform> ();

        double time;

        // the position of the ball - used to compute the input pose.
        SCNVector3 ballPosition = new SCNVector3 (0f, 0f, -175f);
        public SCNVector3 BallPosition
        {
                get
                {
                        return ballPosition;
                }

                set
                {
                        ballPosition = value;
                        DirtyCachedValues ();
                }
        }

        // the radius of the ball - used to compute the input pose
        float ballRadius = 6f;
        public float BallRadius
        {
                get
                {
                        return ballRadius;
                }

                set
                {
                        ballRadius = value;
                        DirtyCachedValues ();
                }
        }

        // the position on the left side of the ball where the straight part of the rope touches
        SCNVector3 TangentPositionL => computedTangentPositionL.Value;
        ComputedValue<SCNVector3> computedTangentPositionL;

        // the position on the right side of the ball where the straight part of the rope touches
        SCNVector3 TangentPositionR => computedTangentPositionR.Value;
        ComputedValue<SCNVector3> computedTangentPositionR;

        // the center position within the triangle spanned by the ball and both fixture positions
        SCNVector3 CenterPosition => computedCenterPosition.Value;
        ComputedValue<SCNVector3> computedCenterPosition;

        // the angle of the arc portion of the rope touching the ball
        public float BetaAngle => computedBetaAngle.Value;
        ComputedValue<float> computedBetaAngle;

        // the resulting input pose of the sling shot rope
        public SlingShotPose InputPose => computedInputPose.Value;
        ComputedValue<SlingShotPose> computedInputPose;

        void InitializeHelpers ()
        {
                computedInputPose = new ComputedValue<SlingShotPose> (() => ComputeInputPose ());
                computedTangentPositionR = new ComputedValue<SCNVector3> (() => TangentPosition (FixturePositionR));
                computedTangentPositionL = new ComputedValue<SCNVector3> (() => TangentPosition (FixturePositionL));

                computedBetaAngle = new ComputedValue<float> (() =>
                {
                        var d = SCNVector3.Normalize (ballPosition - CenterPosition);
                        var t = SCNVector3.Normalize (TangentPositionL - ballPosition);

                        var quaternion = SimdExtensions.CreateQuaternion (d, t);
                        quaternion.ToAxisAngle (out SCNVector3 _, out float angle);
                        return 2f * angle;
                });

                computedCenterPosition = new ComputedValue<SCNVector3> (() =>
                {
                        var direction = SCNVector3.Cross (UpVector, TangentPositionR - TangentPositionL);
                        return ballPosition - SCNVector3.Normalize (direction) * 1.25f * ballRadius;
                });

                computedRestPose = new ComputedValue<SlingShotPose> (() =>
                {
                        var data = new SlingShotPose ();

                        for (var i = 0; i < restPoseTransforms.Count; i++)
                        {
                                var transform = restPoseSpace * restPoseTransforms [i];
                                var p = new SCNVector3 (transform.Column3.X, transform.Column3.Y, transform.Column3.Z);
                                var t = SimdExtensions.CreateQuaternion (transform).Act (new SCNVector3 (1f, 0f, 0f));

                                var l = 0f;
                                if (i > 0)
                                {
                                        l = data.Lengths [i - 1] + (p - data.Positions [i - 1]).Length;
                                }

                                data.Positions.Add (p);
                                data.Tangents.Add (t);
                                data.Lengths.Add (l);
                        }

                        return data;
                });
        }

        public SlingShotPose ComputeInputPose ()
        {
                // note the -1 here differs from other usage
                var data = new SlingShotPose { UpVector = -UpVector /* negated because the strap Y-axis points down */ };

                var startBend = CurrentLengthL / CurrentTotalLength;
                var endBend = 1f - CurrentLengthR / CurrentTotalLength;
                var leatherOnStraights = OriginalLeatherLength - CurrentLengthOnBall;
                var segmentAStart = 0f;
                var segmentAEnd = CurrentLengthL - leatherOnStraights * 0.5f;
                var segmentCStart = segmentAEnd + OriginalLeatherLength;
                var segmentCEnd = CurrentTotalLength;
                var originalLeatherRange = OriginalLeatherLength / OriginalTotalLength;
                var currentLeatherRange = OriginalLeatherLength / CurrentTotalLength;

                for (var i = 0; i < SimulatedTransformCount; i++)
                {
                        var l = OriginalTotalLength * i / (float)(SimulatedTransformCount - 1f);
                        var u = l / OriginalTotalLength;

                        // remap the u value depending on the material (rubber vs leather)
                        var isRubber = Math.Abs (0.5f - u) > originalLeatherRange * 0.5f;
                        if (isRubber)
                        {
                                if (u < 0.5f)
                                {
                                        u = u / (0.5f - originalLeatherRange * 0.5f);
                                        u = (segmentAStart + (segmentAEnd - segmentAStart) * u) / CurrentTotalLength;
                                } else {
                                        u = 1f - (1f - u) / (0.5f - originalLeatherRange * 0.5f);
                                        u = (segmentCStart + (segmentCEnd - segmentCStart) * u) / CurrentTotalLength;
                                }
                        } else {
                                u = (startBend + endBend) * 0.5f - (0.5f - u) * (currentLeatherRange / originalLeatherRange);
                        }

                        var p = SCNVector3.Zero;
                        var t = SCNVector3.UnitX;
                        if (u < startBend)
                        {
                                // left straight
                                var value = u / startBend;
                                p = SimdExtensions.Mix (FixturePositionL,
                                        TangentPositionL, new SCNVector3 (value, value, value)); // left rubber band
                                t = SCNVector3.Normalize (TangentPositionL - FixturePositionL);
                        }
                        else if (u > endBend)
                        {
                                // right straight
                                var value = (1f - u) / (1f - endBend);
                                p = SimdExtensions.Mix (FixturePositionR,
                                        TangentPositionR, new SCNVector3 (value, value, value)); // right rubber band
                                t = SCNVector3.Normalize (FixturePositionR - TangentPositionR);
                        } else {
                                // on the ball
                                var upv = UpVector;
                                var rot = SCNQuaternion.FromAxisAngle (upv, -BetaAngle * (u - startBend) / (endBend - startBend));
                                p = ballPosition + rot.Act (TangentPositionL - ballPosition);
                                t = SCNVector3.Cross (upv, SCNVector3.Normalize (ballPosition - p));
                        }

                        data.Positions.Add (p);
                        data.Tangents.Add (t);
                        data.Lengths.Add (l);
                }

                return data;
        }

        /// <summary>
        /// Mark all computed values as dirty - to force a recompute
        /// </summary>
        void DirtyCachedValues ()
        {
                computedTangentPositionL.IsDirty = true;
                computedTangentPositionR.IsDirty = true;
                computedCenterPosition.IsDirty = true;
                computedBetaAngle.IsDirty = true;
                computedInputPose.IsDirty = true;
                computedRestPose.IsDirty = true;
        }

        /// <summary>
        /// Computes the tangent position of the rope based on a given fixture
        /// </summary>
        SCNVector3 TangentPosition (SCNVector3 fixture)
        {
                var r = ballRadius;
                var d = fixture - ballPosition;
                var alpha = (float)Math.Acos (r / d.Length);
                d = ballRadius * SCNVector3.Normalize (d);
                var rot = SCNQuaternion.FromAxisAngle (UpVector, fixture == FixturePositionL ? -alpha : alpha);
                var d_rotated = rot.Act (d);
                return d_rotated + ballPosition;
        }

        /// <summary>
        /// Sets the initial rest pose of the slingshot, all args are in global space
        /// </summary>
        void SetInitialRestPose (SCNMatrix4 slingshotSpace, List<SCNMatrix4> transforms)
        {
                originalRestPoseSpace = slingshotSpace;
                RestPoseSpace = slingshotSpace;

                restPoseTransforms = new List<SCNMatrix4> ();

                for (var i = 0; i < transforms.Count; i++)
                {
                        restPoseTransforms.Add (SCNMatrix4.Invert (originalRestPoseSpace) * transforms [i]);
                }

                computedRestPose.IsDirty = true;
        }

        /// <summary>
        /// Sets the initial rest pose from a series of scenekit nodes
        /// </summary>
        public void SetInitialRestPose (SCNNode slingshotBase, List<SCNNode> bones)
        {
                var space = slingshotBase.WorldTransform;
                space.Transpose ();

                var transforms = new List<SCNMatrix4> ();

                for (var i = 0; i < bones.Count; i++)
                {
                        var worldTransform = bones [i].WorldTransform;
                        worldTransform.Transpose ();
                        transforms.Add (worldTransform);
                }

                SetInitialRestPose (space, transforms);
        }

        // returns the length of the rope touching the ball
        public float CurrentLengthOnBall => BetaAngle * ballRadius;

        // returns the length of the straight portion of the rope on the left side of the ball
        public float CurrentLengthL => (TangentPositionL - FixturePositionL).Length;

        // returns the length of the straight portion of the rope on the right side of the ball
        public float CurrentLengthR => (TangentPositionR - FixturePositionR).Length;

        // returns the current total length of the rope
        public float CurrentTotalLength => CurrentLengthL + CurrentLengthOnBall + CurrentLengthR;

        /// <summary>
        /// Returns the interpolated transform on the restpose given a length l (from 0.0 to originalTotalLength)
        /// </summary>
        SCNMatrix4 RestPoseTransform (float l)
        {
                if (CurrentTotalLength == 0f)
                {
                        return new SCNMatrix4 ();
                }

                var normalizedL = RestPose.TotalLength * l / OriginalTotalLength;
                return RestPose.Transform (normalizedL);
        }

        /// <summary>
        /// Returns the interpolated transform on the input pose given a length l (from 0.0 to currentTotalLength)
        /// </summary>
        SCNMatrix4 InputPoseTransform (float l)
        {
                if (CurrentTotalLength == 0f)
                {
                        return new SCNMatrix4 ();
                }

                return InputPose.Transform (l);
        }

        public void InterpolateToRestPoseAnimation (double duration, List<SCNNode> ropeShapes)
        {
                // use relative tr[ansforms here, these are safer when the catapult gets hit
                if (duration == 0d)
                {
                        for (var i = BoneInset; i < ropeShapes.Count - BoneInset; i++)
                        {
                                var worldTransform = restPoseSpace * restPoseTransforms [i];
                                worldTransform.Transpose ();
                                ropeShapes [i].WorldTransform = worldTransform;
                        }
                } else {
                        SCNTransaction.Begin ();
                        SCNTransaction.AnimationDuration = duration;

                        for (var i = BoneInset; i < ropeShapes.Count - BoneInset; i++)
                        {
                                var worldTransform = restPoseSpace * restPoseTransforms [i];
                                worldTransform.Transpose ();
                                ropeShapes [i].WorldTransform = worldTransform;
                        }

                        SCNTransaction.Commit ();
                }
        }

        /// <summary>
        /// Disables the simulation on the slingshot and sets the rigid bodies to be driven by the input pose
        /// </summary>
        public void EnableInputPose ()
        {
                for (var i = 0; i < simulatedTransforms.Count; i++)
                {
                        var l = OriginalTotalLength * i / (simulatedTransforms.Count - 1);
                        var transform = InputPoseTransform (l);

                        var position = new SCNVector3 (transform.Column3.X, transform.Column3.Y, transform.Column3.Z);
                        simulatedTransforms [i].Position = position;
                        simulatedTransforms [i].Orientation = SimdExtensions.CreateQuaternion (transform);
                        simulatedTransforms [i].Velocity = SCNVector3.Zero;
                        simulatedTransforms [i].Dynamic = false;
                }
        }

        /// <summary>
        /// Starts the simulation for the slingshot
        /// </summary>
        public void StartLaunchSimulation ()
        {
                var center = (FixturePositionL + FixturePositionR) * 0.5f;
                var force = center - ballPosition;
                var strength = (float)SimShotStrength;

                for (var i = BoneInset; i < simulatedTransforms.Count - BoneInset; i++)
                {
                        simulatedTransforms [i].Dynamic = true;

                        // apply a force
                        var u = i / (float)(simulatedTransforms.Count - 1);
                        var restPoseFactorAlongRope = 1f - Math.Abs (u - 0.5f) / 0.5f;

                        simulatedTransforms [i].Velocity = force * restPoseFactorAlongRope * strength;
                }
        }

        /// <summary>
        /// Computes and applies the custom forces for the slingshot rope. 
        /// This should be called every frame.
        /// </summary>
        void ApplyForces ()
        {
                var b = new SCNVector3 (SimBlend, SimBlend, SimBlend);

                for (var i = BoneInset; i < simulatedTransforms.Count - BoneInset; i++)
                {
                        if (!simulatedTransforms [i].Dynamic)
                        {
                                continue;
                        }

                        var force = SCNVector3.Zero;

                        if (i > 0)
                        {
                                var restA = RestPose.Positions [i - 1];
                                var restB = RestPose.Positions [i];
                                var currentA = simulatedTransforms [i - 1].Position;
                                var currentB = simulatedTransforms [i].Position;
                                var restDistance = (restA - restB).Length;
                                var currentDistance = (currentA - currentB).Length;
                                force += SCNVector3.Normalize (currentA - currentB) * (currentDistance - restDistance) * SimNeighborStrength;
                        }

                        if (i < simulatedTransforms.Count - 1)
                        {
                                var restA = RestPose.Positions [i + 1];
                                var restB = RestPose.Positions [i];
                                var currentA = simulatedTransforms [i + 1].Position;
                                var currentB = simulatedTransforms [i].Position;
                                var restDistance = (restA - restB).Length;
                                var currentDistance = (currentA - currentB).Length;
                                force += SCNVector3.Normalize (currentA - currentB) * (currentDistance - restDistance) * SimNeighborStrength;
                        }

                        force += (RestPose.Positions [i] - simulatedTransforms [i].Position) * SimRestPoseStrength;


                        var vel = simulatedTransforms [i].Velocity;
                        simulatedTransforms [i].Velocity = SimdExtensions.Mix (vel, force, b);
                }
        }

        void AverageVelocities ()
        {
                var currentTransforms = new List<SimulatedTransform> ();
                currentTransforms.AddRange (simulatedTransforms);

                for (var i = BoneInset; i < SimulatedTransformCount - BoneInset; i++)
                {
                        if (!simulatedTransforms [i].Dynamic)
                        {
                                continue;
                        }

                        var a = currentTransforms [i - 1].Velocity;
                        var b = currentTransforms [i].Velocity;
                        var c = currentTransforms [i + 1].Velocity;
                        var ab = SimdExtensions.Mix (a, b, new SCNVector3 (0.5f, 0.5f, 0.5f));
                        var bc = SimdExtensions.Mix (b, c, new SCNVector3 (0.5f, 0.5f, 0.5f));
                        simulatedTransforms [i].Velocity = SimdExtensions.Mix (ab, bc, t: new SCNVector3 (0.5f, 0.5f, 0.5f));

                        var center = SimdExtensions.Mix (currentTransforms [i - 1].Position, currentTransforms [i + 1].Position, new SCNVector3 (0.5f, 0.5f, 0.5f));
                        simulatedTransforms [i].Position = SimdExtensions.Mix (simulatedTransforms [i].Position, center, new SCNVector3 (SmoothRope, SmoothRope, SmoothRope));
                }
        }

        void PerformPlaneCollision (List<SimulatedTransform> previousTransforms, float seconds)
        {
                for (var i = BoneInset; i < simulatedTransforms.Count - BoneInset; i++)
                {
                        if (!simulatedTransforms [i].Dynamic)
                        {
                                continue;
                        }

                        var p = simulatedTransforms [i].Position;
                        var v = simulatedTransforms [i].Velocity;

                        // project into the space of the base
                        var pM = SCNMatrix4.Identity.SetTranslation ((OpenTK.Vector3)p);

                        var pLocal = SCNMatrix4.Invert (restPoseSpace) * pM;

                        if (pLocal.Column3.Z <= CollisionPlane)
                        {
                                pLocal.M34 = CollisionPlane;
                                pM = restPoseSpace * pLocal;

                                var pOnPlane = new SCNVector3 (pM.Column3.X, pM.Column3.Y, pM.Column3.Z);

                                var blend = new SCNVector3 (0.3f, 0.3f, 0.3f);
                                simulatedTransforms [i].Position = SimdExtensions.Mix (p, pOnPlane, blend);

                                var correctedVelocity = (simulatedTransforms [i].Position - previousTransforms [i].Position) / seconds;
                                correctedVelocity = SCNVector3.Multiply (correctedVelocity, new SCNVector3 (0.7f, 0.1f, 0.7f));

                                // verlet integration
                                simulatedTransforms [i].Velocity = SimdExtensions.Mix (v, correctedVelocity, blend);

                                p = simulatedTransforms [i].Position;
                                v = simulatedTransforms [i].Velocity;
                        }

                        if (pLocal.Column3.Y <= CollisionPlane + 0.3f)
                        {
                                pLocal.M24 = CollisionPlane + 0.3f;
                                pM = restPoseSpace * pLocal;

                                var pOnPlane = new SCNVector3 (pM.Column3.X, pM.Column3.Y, pM.Column3.Z);

                                var blend = new SCNVector3 (0.3f, 0.3f, 0.3f);
                                simulatedTransforms [i].Position = SimdExtensions.Mix (p, pOnPlane, blend);

                                var correctedVelocity = (simulatedTransforms [i].Position - previousTransforms [i].Position) / seconds;

                                // verlet integration
                                simulatedTransforms [i].Velocity = SimdExtensions.Mix (v, correctedVelocity, blend);
                        }
                }
        }

        /// <summary>
        /// Aligns the rigid bodies by correcting their orienation 
        /// This should be called after the simulation step
        /// </summary>
        private void AlignBones ()
        {
                // orient the bodies accordingly
                for (var i = BoneInset; i < simulatedTransforms.Count - BoneInset; i++)
                {
                        if (!simulatedTransforms [i].Dynamic)
                        {
                                continue;
                        }

                        var a = simulatedTransforms [i - 1].Position;
                        var b = simulatedTransforms [i + 1].Position;

                        // this is the upVector computed for each bone of the rest pose
                        var transform = restPoseSpace * restPoseTransforms [i]; // todo: direction of multiply?
                        var y = SimdExtensions.CreateQuaternion (transform).Act (new SCNVector3 (0f, 1f, 0f));

                        var x = SCNVector3.Normalize (b - a);
                        var z = SCNVector3.Normalize (SCNVector3.Cross (x, y));
                        y = SCNVector3.Normalize (SCNVector3.Cross (z, x));

                        var rot = new OpenTK.Matrix3 (x.X, y.X, z.X, x.Y, y.Y, z.Y, x.Z, y.Z, z.Z);
                        simulatedTransforms [i].Orientation = new SCNQuaternion (rot.ToQuaternion ());
                }
        }

        public void SimulateStep (double time)
        {
                var minUpdateSeconds = 1f / 120f;
                var maxUpdateSeconds = 1f / 30f;
                var seconds = DigitExtensions.Clamp ((float)(time - time), minUpdateSeconds, maxUpdateSeconds);

                // could run multiple iterations if greater than maxUpdateSeconds, but for now just run one

                ApplyForces ();
                AverageVelocities ();

                // copy the current state
                var currentTransforms = new List<SimulatedTransform> ();
                currentTransforms.AddRange (simulatedTransforms);

                // simulate forward
                for (var i = BoneInset; i < simulatedTransforms.Count - BoneInset; i++)
                {
                        if (!currentTransforms [i].Dynamic)
                        {
                                continue;
                        }

                        var p = currentTransforms [i].Position;
                        var v = currentTransforms [i].Velocity;
                        p += v * seconds;

                        simulatedTransforms [i].Position = p;
                }

                PerformPlaneCollision (currentTransforms, seconds);
                AlignBones ();

                this.time = time;
        }
}
