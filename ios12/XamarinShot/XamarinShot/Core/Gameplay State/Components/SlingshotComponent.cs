
namespace XamarinShot.Models.GameplayState
{
    using Foundation;
    using GameplayKit;
    using SceneKit;
    using System;

    public class SlingshotComponent : GKComponent
    {
        private readonly SCNVector3 restPosition;
        private readonly SCNNode catapult;

        private SCNVector3 currentPosition;
        private SCNVector3 velocity;
        private bool physicsMode;

        public SlingshotComponent(SCNNode catapult) : base()
        {
            this.catapult = catapult;
            this.restPosition = catapult.Position;
            this.currentPosition = this.restPosition;
            this.physicsMode = false; // Started off and gets turned on only if needed
            this.velocity = SCNVector3.Zero;
        }

        public SlingshotComponent(NSCoder coder) => throw new NotImplementedException("init(coder:) has not been implemented");

        public void SetGrabMode(bool state)
        {
            this.physicsMode = !state;  // physics mode is off when grab mode is on
            if (this.physicsMode)
            {
                this.currentPosition = catapult.Position;
            }
        }

        public override void Update(double deltaTimeInSeconds)
        {
            if (this.physicsMode)
            {
                // add force in direction to rest point
                var offset = this.restPosition - this.currentPosition;
                var force = offset * 1000f - this.velocity * 10f;

                this.velocity += force * (float)deltaTimeInSeconds;
                this.currentPosition += this.velocity * (float)deltaTimeInSeconds;
                this.catapult.Position = this.currentPosition;

                // bring back to 0
                this.catapult.EulerAngles = new SCNVector3(this.catapult.EulerAngles.X * 0.9f,
                                                           this.catapult.EulerAngles.Y,
                                                           this.catapult.EulerAngles.Z);
            }
        }
    }
}