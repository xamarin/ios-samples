
namespace XamarinShot.Models;

public class CatapultRope
{
        const int NumStrapBones = 35;

        // hold onto the rope shapes, will copy transforms into these
        readonly List<SCNNode> ropeShapes = new List<SCNNode> ();

        readonly SCNNode @base;

        SlingShotSimulation rope;

        double animatingLaunchTimestamp = 0d;

        State state = State.None;

        public CatapultRope (SCNNode @base)
        {
                this.@base = @base;

                // setup the rope simulation
                // segmentRadius is size of the rigid bodies that represent the rope
                rope = new SlingShotSimulation (this.@base, NumStrapBones, 0.02f * 5f);

                // this will be set when projectile set onto catapult
                rope.BallRadius = 0.275f;

                // the projectile is resting atop the strap when inactive, will it settle okay
                // what about the orient of the projectile
                // The ball will jump as the strap is pulled

                rope.BallPosition = @base.FindChildNode ("ballOriginInactiveBelow", true)?.WorldPosition ?? SCNVector3.One;

                // grab an array of each bone from the rig, these are called sling0...sling<count>
                // these are in the inactive strap
                var originalTotalLength = 0f;
                var originalLeatherLength = 0f;
                for (int i = 0; i < NumStrapBones; i++)
                {
                        ropeShapes.Add (@base.FindChildNode ($"strap{i}", true)!);
                        if (i > 1)
                        {
                                // estimate with linear length
                                var delta = (ropeShapes [i].WorldPosition - ropeShapes [i - 1].WorldPosition).Length;
                                originalTotalLength += delta;
                                if (i >= 13 && i <= 21)
                                {
                                        originalLeatherLength += delta;
                                }
                        }
                }

                rope.OriginalTotalLength = originalTotalLength;
                rope.OriginalLeatherLength = originalLeatherLength;

                rope.SetInitialRestPose (this.@base, ropeShapes);
        }

        public void SetBallRadius (float ballRadius)
        {
                rope.BallRadius = ballRadius;
        }

        public void GrabBall (SCNVector3 ballPosition)
        {
                MoveBall (ballPosition);
        }

        public void MoveBall (SCNVector3 ballPosition)
        {
                // grab must be called prior, and it sets .move mode up
                state = State.Move;
                rope.BallPosition = ballPosition;

                // this is really the currently rotated space of the base
                var worldTransform = @base.WorldTransform;
                worldTransform.Transpose ();
                rope.RestPoseSpace = worldTransform;

                // disables simulation, sets mask to zero, and drives it by the rig
                rope.EnableInputPose ();
        }

        bool useSim = true;

        public void LaunchBall ()
        {
                state = State.Launch;
                animatingLaunchTimestamp = GameTime.Time;

                useSim = UserDefaults.ShowRopeSimulation;

                // this lets the rope fly
                if (useSim)
                {
                        rope.StartLaunchSimulation ();
                } else {
                        InterpolateToRestPoseAnimation (rope.LinearLaunchAnimationTime);
                }
        }

        public void InterpolateToRestPoseAnimation (double duration)
        {
                rope.InterpolateToRestPoseAnimation (duration, ropeShapes);
        }

        /// <summary>
        /// Called at start of render phase, but before render scaling
        /// </summary>
        public void UpdateRopeModel ()
        {
                if (state == State.None || (!useSim && state == State.Launch))
                {
                        return;
                }

                var time = GameTime.Time;

                // this advances by at most a fixed timestep
                rope.SimulateStep (time);

                // copy the bone locations back to the rig
                // don't change the begin and end bones, those are static and will separate if changed
                for (var i = rope.BoneInset; i < NumStrapBones - rope.BoneInset; i++)
                {
                        // presentation node actually has the results of the simulation
                        ropeShapes [i].WorldTransform = rope.SimulatedTransformAsFloat4x4 (i);
                }

                // reset the animation state back to none
                // this can't be longer than the cooldownTime - grow/drop time (around 2.5s)
                // but has to be long enough to let the rope sim settle back to reset pose
                var delta = time - animatingLaunchTimestamp;
                if (state == State.Launch && delta >= rope.SimLaunchAnimationTime)
                {
                        state = State.None;
                }
        }

        enum State
        {
                None,
                Move,
                Launch,
        }
}
