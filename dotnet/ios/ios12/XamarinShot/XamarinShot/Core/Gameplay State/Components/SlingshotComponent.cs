namespace XamarinShot.Models.GameplayState;

public class SlingshotComponent : GKComponent
{
        readonly SCNVector3 restPosition;
        readonly SCNNode catapult;

        SCNVector3 currentPosition;
        SCNVector3 velocity;
        bool physicsMode;

        public SlingshotComponent (SCNNode catapult) : base ()
        {
                this.catapult = catapult;
                restPosition = catapult.Position;
                currentPosition = restPosition;
                physicsMode = false; // Started off and gets turned on only if needed
                velocity = SCNVector3.Zero;
        }

        public SlingshotComponent (NSCoder coder) => throw new NotImplementedException ("init(coder:) has not been implemented");

        public void SetGrabMode (bool state)
        {
                physicsMode = !state;  // physics mode is off when grab mode is on
                if (physicsMode)
                {
                        currentPosition = catapult.Position;
                }
        }

        public override void Update (double deltaTimeInSeconds)
        {
                if (physicsMode)
                {
                        // add force in direction to rest point
                        var offset = restPosition - currentPosition;
                        var force = offset * 1000f - velocity * 10f;

                        velocity += force * (float)deltaTimeInSeconds;
                        currentPosition += velocity * (float)deltaTimeInSeconds;
                        catapult.Position = currentPosition;

                        // bring back to 0
                        catapult.EulerAngles = new SCNVector3 (catapult.EulerAngles.X * 0.9f,
                                catapult.EulerAngles.Y, catapult.EulerAngles.Z);
                }
        }
}
