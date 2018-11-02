
namespace XamarinShot.Models
{
    using SceneKit;
    using XamarinShot.Utils;
    using System.Collections.Generic;

    public class CatapultRope
    {
        private const int NumStrapBones = 35;

        // hold onto the rope shapes, will copy transforms into these
        private readonly List<SCNNode> ropeShapes = new List<SCNNode>();

        private readonly SCNNode @base;

        private SlingShotSimulation rope;

        private double animatingLaunchTimestamp = 0d;

        private State state = State.None;

        public CatapultRope(SCNNode @base)
        {
            this.@base = @base;

            // setup the rope simulation
            // segmentRadius is size of the rigid bodies that represent the rope
            this.rope = new SlingShotSimulation(this.@base, NumStrapBones, 0.02f * 5f);

            // this will be set when projectile set onto catapult
            this.rope.BallRadius = 0.275f;

            // the projectile is resting atop the strap when inactive, will it settle okay
            // what about the orient of the projectile
            // The ball will jump as the strap is pulled

            this.rope.BallPosition = this.@base.FindChildNode("ballOriginInactiveBelow", true).WorldPosition;

            // grab an array of each bone from the rig, these are called sling0...sling<count>
            // these are in the inactive strap
            var originalTotalLength = 0f;
            var originalLeatherLength = 0f;
            for (int i = 0; i < NumStrapBones; i++)
            {
                this.ropeShapes.Add(this.@base.FindChildNode($"strap{i}", true));
                if (i > 1)
                {
                    // estimate with linear length
                    var delta = (this.ropeShapes[i].WorldPosition - this.ropeShapes[i - 1].WorldPosition).Length;
                    originalTotalLength += delta;
                    if (i >= 13 && i <= 21)
                    {
                        originalLeatherLength += delta;
                    }
                }
            }

            this.rope.OriginalTotalLength = originalTotalLength;
            this.rope.OriginalLeatherLength = originalLeatherLength;

            this.rope.SetInitialRestPose(this.@base, this.ropeShapes);
        }

        public void SetBallRadius(float ballRadius)
        {
            this.rope.BallRadius = ballRadius;
        }

        public void GrabBall(SCNVector3 ballPosition)
        {
            this.MoveBall(ballPosition);
        }

        public void MoveBall(SCNVector3 ballPosition)
        {
            // grab must be called prior, and it sets .move mode up
            this.state = State.Move;
            this.rope.BallPosition = ballPosition;

            // this is really the currently rotated space of the base
            var worldTransform = this.@base.WorldTransform;
            worldTransform.Transpose();
            this.rope.RestPoseSpace = worldTransform;

            // disables simulation, sets mask to zero, and drives it by the rig
            this.rope.EnableInputPose();
        }

        private bool useSim = true;

        public void LaunchBall()
        {
            this.state = State.Launch;
            this.animatingLaunchTimestamp = GameTime.Time;

            this.useSim = UserDefaults.ShowRopeSimulation;

            // this lets the rope fly
            if (this.useSim)
            {
                this.rope.StartLaunchSimulation();
            }
            else
            {
                this.InterpolateToRestPoseAnimation(this.rope.LinearLaunchAnimationTime);
            }
        }

        public void InterpolateToRestPoseAnimation(double duration)
        {
            this.rope.InterpolateToRestPoseAnimation(duration, this.ropeShapes);
        }

        /// <summary>
        /// Called at start of render phase, but before render scaling
        /// </summary>
        public void UpdateRopeModel()
        {
            if (this.state == State.None || (!this.useSim && this.state == State.Launch))
            {
                return;
            }

            var time = GameTime.Time;

            // this advances by at most a fixed timestep
            this.rope.SimulateStep(time);

            // copy the bone locations back to the rig
            // don't change the begin and end bones, those are static and will separate if changed
            for (var i = this.rope.BoneInset; i < NumStrapBones - this.rope.BoneInset; i++)
            {
                // presentation node actually has the results of the simulation
                this.ropeShapes[i].WorldTransform = this.rope.SimulatedTransformAsFloat4x4(i);
            }

            // reset the animation state back to none
            // this can't be longer than the cooldownTime - grow/drop time (around 2.5s)
            // but has to be long enough to let the rope sim settle back to reset pose
            var delta = time - this.animatingLaunchTimestamp;
            if (this.state == State.Launch && delta >= this.rope.SimLaunchAnimationTime)
            {
                this.state = State.None;
            }
        }

        enum State
        {
            None,
            Move,
            Launch,
        }
    }
}