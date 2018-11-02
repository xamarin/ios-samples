
namespace XamarinShot.Models
{
    using Foundation;
    using SceneKit;
    using XamarinShot.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SimulatedTransform
    {
        public SimulatedTransform(SCNVector3 position, SCNQuaternion orientation, SCNVector3 velocity, bool dynamic)
        {
            this.Position = position;
            this.Orientation = orientation;
            this.Velocity = velocity;
            this.Dynamic = dynamic;
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

        private const float SimDamping = 0.85f; // the amount of damping to apply to the rigid 5odies
        private const float SimBounce = 0f; // the amount of restitution to apply to the rigid bodies
        private const float SimShotStrength = 20f; // the strength of the shot when released
        private const float SimNeighborStrength = 350f; // the strength for the neighbor springs
        private const float SimRestPoseStrength = 5f; // the strength for the restpose force
        private const float SimBlend = 0.08f; // the amount of lerp to restPose per iteration,
                                              // higher ends faster, related to finishing sim before simLaunchAnimationTime expires
                                              // the collision plane distance
        private const float CollisionPlane = 0.15f; // using half depth of catatpult at 0.25 / 2 ~ 0.125 + slop

        // smoothing the rope from 0 to 1
        private const float SmoothRope = 0.25f;

        // the parent space of the slingshot
        private SCNMatrix4 restPoseSpace = new SCNMatrix4();

        private SCNMatrix4 originalRestPoseSpace = new SCNMatrix4();

        private List<SCNMatrix4> restPoseTransforms = new List<SCNMatrix4>();

        public SlingShotSimulation(SCNNode rootNode, int count, float segmentRadius) : base()
        {
            this.InitializeHelpers();

            for (var i = 0; i < count; i++)
            {
                var isStatic = (i < this.BoneInset) || (i >= count - this.BoneInset);
                var quaternion = SCNQuaternion.FromAxisAngle(SCNVector3.UnitX, 0);

                var transform = new SimulatedTransform(SCNVector3.Zero, quaternion, SCNVector3.Zero, !isStatic);
                this.simulatedTransforms.Add(transform);
            }
        }

        /// <summary>
        /// The rest pose of the slingshot in world space
        /// </summary>
        public SlingShotPose RestPose => this.computedRestPose.Value;

        private ComputedValue<SlingShotPose> computedRestPose;

        public SCNVector3 FixturePositionL => this.RestPose.Positions.First();

        public SCNVector3 FixturePositionR => this.RestPose.Positions.Last();

        public SCNVector3 UpVector => SimdExtensions.CreateQuaternion(this.restPoseSpace).Act(SCNVector3.UnitY);

        // return the number of simulated transforms in this slingshot
        public int SimulatedTransformCount => this.simulatedTransforms.Count;

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
                return this.restPoseSpace;
            }

            set
            {
                this.restPoseSpace = value;
                this.DirtyCachedValues();
            }
        }

        /// <summary>
        /// Returns a simulated transform within this simulation
        /// </summary>
        public SimulatedTransform SimulatedTransform(int index)
        {
            return this.simulatedTransforms[index];
        }

        /// <summary>
        /// Returns a simulated transform within this simulation as float4x4
        /// </summary>
        public SCNMatrix4 SimulatedTransformAsFloat4x4(int index)
        {
            var mSim = this.simulatedTransforms[index];
            var quaternion = mSim.Orientation.ToQuaternion();
            var matrix = SCNMatrix4.Rotate(quaternion);

            matrix.M41 = mSim.Position.X;
            matrix.M42 = mSim.Position.Y;
            matrix.M43 = mSim.Position.Z;
            matrix.M44 = 1f;

            return matrix;
        }

        private readonly List<SimulatedTransform> simulatedTransforms = new List<SimulatedTransform>();

        private double time;

        // the position of the ball - used to compute the input pose.
        private SCNVector3 ballPosition = new SCNVector3(0f, 0f, -175f);
        public SCNVector3 BallPosition
        {
            get
            {
                return this.ballPosition;
            }

            set
            {
                this.ballPosition = value;
                this.DirtyCachedValues();
            }
        }

        // the radius of the ball - used to compute the input pose
        private float ballRadius = 6f;
        public float BallRadius
        {
            get
            {
                return this.ballRadius;
            }

            set
            {
                this.ballRadius = value;
                this.DirtyCachedValues();
            }
        }

        // the position on the left side of the ball where the straight part of the rope touches
        private SCNVector3 TangentPositionL => this.computedTangentPositionL.Value;
        private ComputedValue<SCNVector3> computedTangentPositionL;

        // the position on the right side of the ball where the straight part of the rope touches
        private SCNVector3 TangentPositionR => this.computedTangentPositionR.Value;
        private ComputedValue<SCNVector3> computedTangentPositionR;

        // the center position within the triangle spanned by the ball and both fixture positions
        private SCNVector3 CenterPosition => this.computedCenterPosition.Value;
        private ComputedValue<SCNVector3> computedCenterPosition;

        // the angle of the arc portion of the rope touching the ball
        public float BetaAngle => this.computedBetaAngle.Value;
        private ComputedValue<float> computedBetaAngle;

        // the resulting input pose of the sling shot rope
        public SlingShotPose InputPose => this.computedInputPose.Value;
        private ComputedValue<SlingShotPose> computedInputPose;

        private void InitializeHelpers()
        {
            this.computedInputPose = new ComputedValue<SlingShotPose>(() => this.ComputeInputPose());
            this.computedTangentPositionR = new ComputedValue<SCNVector3>(() => this.TangentPosition(this.FixturePositionR));
            this.computedTangentPositionL = new ComputedValue<SCNVector3>(() => this.TangentPosition(this.FixturePositionL));

            this.computedBetaAngle = new ComputedValue<float>(() =>
            {
                var d = SCNVector3.Normalize(this.ballPosition - this.CenterPosition);
                var t = SCNVector3.Normalize(this.TangentPositionL - this.ballPosition);

                var quaternion = SimdExtensions.CreateQuaternion(d, t);
                quaternion.ToAxisAngle(out SCNVector3 _, out float angle);
                return 2f * angle;
            });

            this.computedCenterPosition = new ComputedValue<SCNVector3>(() =>
            {
                var direction = SCNVector3.Cross(this.UpVector, this.TangentPositionR - this.TangentPositionL);
                return this.ballPosition - SCNVector3.Normalize(direction) * 1.25f * this.ballRadius;
            });

            this.computedRestPose = new ComputedValue<SlingShotPose>(() =>
            {
                var data = new SlingShotPose();

                for (var i = 0; i < this.restPoseTransforms.Count ; i++)
                {
                    var transform = this.restPoseSpace * this.restPoseTransforms[i];
                    var p = new SCNVector3(transform.Column3.X, transform.Column3.Y, transform.Column3.Z);
                    var t = SimdExtensions.CreateQuaternion(transform).Act(new SCNVector3(1f, 0f, 0f));

                    var l = 0f;
                    if (i > 0)
                    {
                        l = data.Lengths[i - 1] + (p - data.Positions[i - 1]).Length;
                    }

                    data.Positions.Add(p);
                    data.Tangents.Add(t);
                    data.Lengths.Add(l);
                }

                return data;
            });
        }

        public SlingShotPose ComputeInputPose() 
        {
            // note the -1 here differs from other usage
            var data = new SlingShotPose { UpVector = -this.UpVector /* negated because the strap Y-axis points down */ };

            var startBend = this.CurrentLengthL / this.CurrentTotalLength;
            var endBend = 1f - this.CurrentLengthR / this.CurrentTotalLength;
            var leatherOnStraights = this.OriginalLeatherLength - this.CurrentLengthOnBall;
            var segmentAStart = 0f;
            var segmentAEnd = this.CurrentLengthL - leatherOnStraights * 0.5f;
            var segmentCStart = segmentAEnd + this.OriginalLeatherLength;
            var segmentCEnd = this.CurrentTotalLength;
            var originalLeatherRange = this.OriginalLeatherLength / this.OriginalTotalLength;
            var currentLeatherRange = this.OriginalLeatherLength / this.CurrentTotalLength;

            for (var i = 0; i < this.SimulatedTransformCount; i++)
            {
                var l = this.OriginalTotalLength * (float)i / (float)(this.SimulatedTransformCount - 1f);
                var u = l / this.OriginalTotalLength;

                // remap the u value depending on the material (rubber vs leather)
                var isRubber = Math.Abs(0.5f - u) > originalLeatherRange * 0.5f;
                if (isRubber)
                {
                    if (u< 0.5f)
                    {
                        u = u / (0.5f - originalLeatherRange * 0.5f);
                        u = (segmentAStart + (segmentAEnd - segmentAStart) * u) / this.CurrentTotalLength;
                    }
                    else 
                    {
                        u = 1f - (1f - u) / (0.5f - originalLeatherRange * 0.5f);
                        u = (segmentCStart + (segmentCEnd - segmentCStart) * u) / this.CurrentTotalLength;
                    }
                }
                else 
                {
                    u = (startBend + endBend) * 0.5f - (0.5f - u) * (currentLeatherRange / originalLeatherRange);
                }

                var p = SCNVector3.Zero;
                var t = SCNVector3.UnitX;
                if (u < startBend)
                {
                    // left straight
                    var value = u / startBend;
                    p = SimdExtensions.Mix(this.FixturePositionL, 
                                           this.TangentPositionL, 
                                           new SCNVector3(value, value, value)); // left rubber band
                    t = SCNVector3.Normalize(this.TangentPositionL - this.FixturePositionL);
                }
                else if (u > endBend)
                { 
                    // right straight
                    var value = (1f - u) / (1f - endBend);
                    p = SimdExtensions.Mix(this.FixturePositionR, 
                                           this.TangentPositionR,
                                           new SCNVector3(value, value, value)); // right rubber band
                    t = SCNVector3.Normalize(this.FixturePositionR - this.TangentPositionR);
                } 
                else 
                { 
                    // on the ball
                    var upv = this.UpVector;
                    var rot = SCNQuaternion.FromAxisAngle(upv , - this.BetaAngle * (u - startBend) / (endBend - startBend));
                    p = this.ballPosition + rot.Act(this.TangentPositionL - this.ballPosition);
                    t = SCNVector3.Cross(upv, SCNVector3.Normalize(this.ballPosition - p));
                }

                data.Positions.Add(p);
                data.Tangents.Add(t);
                data.Lengths.Add(l);
            }

            return data;
        }

        /// <summary>
        /// Mark all computed values as dirty - to force a recompute
        /// </summary>
        private void DirtyCachedValues()
        {
            this.computedTangentPositionL.IsDirty = true;
            this.computedTangentPositionR.IsDirty = true;
            this.computedCenterPosition.IsDirty = true;
            this.computedBetaAngle.IsDirty = true;
            this.computedInputPose.IsDirty = true;
            this.computedRestPose.IsDirty = true;
        }

        /// <summary>
        /// Computes the tangent position of the rope based on a given fixture
        /// </summary>
        private SCNVector3 TangentPosition(SCNVector3 fixture)
        {
            var r = this.ballRadius;
            var d = fixture - this.ballPosition;
            var alpha = (float)Math.Acos(r / d.Length);
            d = this.ballRadius * SCNVector3.Normalize(d);
            var rot = SCNQuaternion.FromAxisAngle(this.UpVector, fixture == this.FixturePositionL ? -alpha : alpha);
            var d_rotated = rot.Act(d);
            return d_rotated + this.ballPosition;
        }

        /// <summary>
        /// Sets the initial rest pose of the slingshot, all args are in global space
        /// </summary>
        private void SetInitialRestPose(SCNMatrix4 slingshotSpace, List<SCNMatrix4> transforms)
        {
            this.originalRestPoseSpace = slingshotSpace;
            this.RestPoseSpace = slingshotSpace;

            this.restPoseTransforms = new List<SCNMatrix4>();

            for (var i = 0; i < transforms.Count; i++)
            {
                this.restPoseTransforms.Add(SCNMatrix4.Invert(this.originalRestPoseSpace) * transforms[i]);
            }

            this.computedRestPose.IsDirty = true;
        }

        /// <summary>
        /// Sets the initial rest pose from a series of scenekit nodes
        /// </summary>
        public void SetInitialRestPose(SCNNode slingshotBase, List<SCNNode> bones)
        {
            var space = slingshotBase.WorldTransform;
            space.Transpose();

            var transforms = new List<SCNMatrix4>();

            for (var i = 0; i < bones.Count; i++)
            {
                var worldTransform = bones[i].WorldTransform;
                worldTransform.Transpose();
                transforms.Add(worldTransform);
            }

            this.SetInitialRestPose(space, transforms);
        }

        // returns the length of the rope touching the ball
        public float CurrentLengthOnBall => this.BetaAngle * this.ballRadius;

        // returns the length of the straight portion of the rope on the left side of the ball
        public float CurrentLengthL => (this.TangentPositionL - this.FixturePositionL).Length;

        // returns the length of the straight portion of the rope on the right side of the ball
        public float CurrentLengthR => (this.TangentPositionR - this.FixturePositionR).Length;

        // returns the current total length of the rope
        public float CurrentTotalLength => this.CurrentLengthL + this.CurrentLengthOnBall + this.CurrentLengthR;

        /// <summary>
        /// Returns the interpolated transform on the restpose given a length l (from 0.0 to originalTotalLength)
        /// </summary>
        private SCNMatrix4 RestPoseTransform(float l)
        {
            if (this.CurrentTotalLength == 0f)
            {
                return new SCNMatrix4();
            }

            var normalizedL = this.RestPose.TotalLength * l / this.OriginalTotalLength;
            return this.RestPose.Transform(normalizedL);
        }

        /// <summary>
        /// Returns the interpolated transform on the input pose given a length l (from 0.0 to currentTotalLength)
        /// </summary>
        private SCNMatrix4 InputPoseTransform(float l)
        {
            if (this.CurrentTotalLength == 0f)
            {
                return new SCNMatrix4();
            }

            return this.InputPose.Transform(l);
        }

        public void InterpolateToRestPoseAnimation(double duration, List<SCNNode> ropeShapes)
        {
            // use relative tr[ansforms here, these are safer when the catapult gets hit
            if (duration == 0d)
            {
                for (var i = this.BoneInset; i < ropeShapes.Count - this.BoneInset; i++)
                {
                    var worldTransform = this.restPoseSpace * this.restPoseTransforms[i];
                    worldTransform.Transpose();
                    ropeShapes[i].WorldTransform = worldTransform;
                }
            }
            else
            {
                SCNTransaction.Begin();
                SCNTransaction.AnimationDuration = duration;

                for (var i = this.BoneInset; i < ropeShapes.Count - this.BoneInset; i++)
                {
                    var worldTransform = this.restPoseSpace * this.restPoseTransforms[i];
                    worldTransform.Transpose();
                    ropeShapes[i].WorldTransform = worldTransform;
                }

                SCNTransaction.Commit();
            }
        }

        /// <summary>
        /// Disables the simulation on the slingshot and sets the rigid bodies to be driven by the input pose
        /// </summary>
        public void EnableInputPose()
        {
            for (var i = 0; i < this.simulatedTransforms.Count; i++)
            {
                var l = this.OriginalTotalLength * (float)i / (float)(this.simulatedTransforms.Count - 1);
                var transform = this.InputPoseTransform(l);

                var position = new SCNVector3(transform.Column3.X, transform.Column3.Y, transform.Column3.Z);
                this.simulatedTransforms[i].Position = position;
                this.simulatedTransforms[i].Orientation = SimdExtensions.CreateQuaternion(transform);
                this.simulatedTransforms[i].Velocity = SCNVector3.Zero;
                this.simulatedTransforms[i].Dynamic = false;
            }
        }

        /// <summary>
        /// Starts the simulation for the slingshot
        /// </summary>
        public void StartLaunchSimulation()
        {
            var center = (this.FixturePositionL + this.FixturePositionR) * 0.5f;
            var force = center - this.ballPosition;
            var strength = (float)SimShotStrength;

            for (var i = this.BoneInset; i < this.simulatedTransforms.Count - this.BoneInset; i++)
            {
                this.simulatedTransforms[i].Dynamic = true;

                // apply a force
                var u = (float)i / (float)(this.simulatedTransforms.Count - 1);
                var restPoseFactorAlongRope = 1f - Math.Abs(u - 0.5f) / 0.5f;

                this.simulatedTransforms[i].Velocity = force * restPoseFactorAlongRope * strength;
            }
        }

        /// <summary>
        /// Computes and applies the custom forces for the slingshot rope. 
        /// This should be called every frame.
        /// </summary>
        private void ApplyForces()
        {
            var b = new SCNVector3(SimBlend, SimBlend, SimBlend);

            for (var i = this.BoneInset; i < this.simulatedTransforms.Count - this.BoneInset; i++)
            {
                if (!this.simulatedTransforms[i].Dynamic)
                {
                    continue;
                }

                var force = SCNVector3.Zero;

                if (i > 0)
                {
                    var restA = this.RestPose.Positions[i - 1];
                    var restB = this.RestPose.Positions[i];
                    var currentA = this.simulatedTransforms[i - 1].Position;
                    var currentB = this.simulatedTransforms[i].Position;
                    var restDistance = (restA - restB).Length;
                    var currentDistance = (currentA - currentB).Length;
                    force += SCNVector3.Normalize(currentA - currentB) * (currentDistance - restDistance) * SimNeighborStrength;
                }

                if (i < this.simulatedTransforms.Count - 1)
                {
                    var restA = this.RestPose.Positions[i + 1];
                    var restB = this.RestPose.Positions[i];
                    var currentA = this.simulatedTransforms[i + 1].Position;
                    var currentB = this.simulatedTransforms[i].Position;
                    var restDistance = (restA - restB).Length;
                    var currentDistance = (currentA - currentB).Length;
                    force += SCNVector3.Normalize(currentA - currentB) * (currentDistance - restDistance) * SimNeighborStrength;
                }

                force += (this.RestPose.Positions[i] - this.simulatedTransforms[i].Position) * SimRestPoseStrength;


                var vel = this.simulatedTransforms[i].Velocity;
                this.simulatedTransforms[i].Velocity = SimdExtensions.Mix(vel, force, b);
            }
        }

        private void AverageVelocities()
        {
            var currentTransforms = new List<SimulatedTransform>();
            currentTransforms.AddRange(this.simulatedTransforms);

            for (var i = this.BoneInset; i < this.SimulatedTransformCount - this.BoneInset; i++)
            {
                if (!this.simulatedTransforms[i].Dynamic)
                {
                    continue;
                }

                var a = currentTransforms[i - 1].Velocity;
                var b = currentTransforms[i].Velocity;
                var c = currentTransforms[i + 1].Velocity;
                var ab = SimdExtensions.Mix(a, b, new SCNVector3(0.5f, 0.5f, 0.5f));
                var bc = SimdExtensions.Mix(b, c, new SCNVector3(0.5f, 0.5f, 0.5f));
                this.simulatedTransforms[i].Velocity = SimdExtensions.Mix(ab, bc, t: new SCNVector3(0.5f, 0.5f, 0.5f));

                var center = SimdExtensions.Mix(currentTransforms[i - 1].Position, currentTransforms[i + 1].Position, new SCNVector3(0.5f, 0.5f, 0.5f));
                this.simulatedTransforms[i].Position = SimdExtensions.Mix(this.simulatedTransforms[i].Position, center, new SCNVector3(SmoothRope, SmoothRope, SmoothRope));
            }
        }

        private void PerformPlaneCollision(List<SimulatedTransform> previousTransforms, float seconds)
        {
            for (var i = this.BoneInset; i < this.simulatedTransforms.Count - this.BoneInset; i++)
            {
                if (!this.simulatedTransforms[i].Dynamic)
                {
                    continue;
                }

                var p = this.simulatedTransforms[i].Position;
                var v = this.simulatedTransforms[i].Velocity;

                // project into the space of the base
                var pM = SCNMatrix4.Identity.SetTranslation((OpenTK.Vector3)p);

                var pLocal = SCNMatrix4.Invert(this.restPoseSpace) * pM;

                if (pLocal.Column3.Z <= CollisionPlane)
                {
                    pLocal.M34 = CollisionPlane;
                    pM = this.restPoseSpace * pLocal;

                    var pOnPlane = new SCNVector3(pM.Column3.X, pM.Column3.Y, pM.Column3.Z);

                    var blend = new SCNVector3(0.3f, 0.3f, 0.3f);
                    this.simulatedTransforms[i].Position = SimdExtensions.Mix(p, pOnPlane, blend);

                    var correctedVelocity = (this.simulatedTransforms[i].Position - previousTransforms[i].Position) / seconds;
                    correctedVelocity = SCNVector3.Multiply(correctedVelocity, new SCNVector3(0.7f, 0.1f, 0.7f));

                    // verlet integration
                    this.simulatedTransforms[i].Velocity = SimdExtensions.Mix(v, correctedVelocity, blend);

                    p = this.simulatedTransforms[i].Position;
                    v = this.simulatedTransforms[i].Velocity;
                }

                if (pLocal.Column3.Y <= CollisionPlane + 0.3f)
                {
                    pLocal.M24 = CollisionPlane + 0.3f;
                    pM = this.restPoseSpace * pLocal;

                    var pOnPlane = new SCNVector3(pM.Column3.X, pM.Column3.Y, pM.Column3.Z);

                    var blend = new SCNVector3(0.3f, 0.3f, 0.3f);
                    this.simulatedTransforms[i].Position = SimdExtensions.Mix(p, pOnPlane, blend);

                    var correctedVelocity = (this.simulatedTransforms[i].Position - previousTransforms[i].Position) / seconds;

                    // verlet integration
                    this.simulatedTransforms[i].Velocity = SimdExtensions.Mix(v, correctedVelocity, blend);
                }
            }
        }

        /// <summary>
        /// Aligns the rigid bodies by correcting their orienation 
        /// This should be called after the simulation step
        /// </summary>
        private void AlignBones()
        {
            // orient the bodies accordingly
            for (var i = this.BoneInset; i < this.simulatedTransforms.Count - this.BoneInset; i++)
            {
                if (!this.simulatedTransforms[i].Dynamic)
                {
                    continue;
                }

                var a = this.simulatedTransforms[i - 1].Position;
                var b = this.simulatedTransforms[i + 1].Position;

                // this is the upVector computed for each bone of the rest pose
                var transform = this.restPoseSpace * this.restPoseTransforms[i]; // todo: direction of multiply?
                var y = SimdExtensions.CreateQuaternion(transform).Act(new SCNVector3(0f, 1f, 0f));

                var x = SCNVector3.Normalize(b - a);
                var z = SCNVector3.Normalize(SCNVector3.Cross(x, y));
                y = SCNVector3.Normalize(SCNVector3.Cross(z, x));

                var rot = new OpenTK.Matrix3(x.X, y.X, z.X, x.Y, y.Y, z.Y, x.Z, y.Z, z.Z);
                this.simulatedTransforms[i].Orientation = new SCNQuaternion(rot.ToQuaternion());
            }
        }

        public void SimulateStep(double time)
        {
            var minUpdateSeconds = 1f / 120f;
            var maxUpdateSeconds = 1f / 30f;
            var seconds = DigitExtensions.Clamp((float)(time - this.time), minUpdateSeconds, maxUpdateSeconds);

            // could run multiple iterations if greater than maxUpdateSeconds, but for now just run one

            this.ApplyForces();
            this.AverageVelocities();

            // copy the current state
            var currentTransforms = new List<SimulatedTransform>();
            currentTransforms.AddRange(this.simulatedTransforms);

            // simulate forward
            for (var i = this.BoneInset; i < this.simulatedTransforms.Count - this.BoneInset; i++)
            {
                if (!currentTransforms[i].Dynamic)
                {
                    continue;
                }

                var p = currentTransforms[i].Position;
                var v = currentTransforms[i].Velocity;
                p += v * seconds;

                this.simulatedTransforms[i].Position = p;
            }

            this.PerformPlaneCollision(currentTransforms, seconds);
            this.AlignBones();

            this.time = time;
        }
    }
}