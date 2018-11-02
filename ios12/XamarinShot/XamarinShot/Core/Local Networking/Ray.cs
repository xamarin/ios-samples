
namespace XamarinShot.Models
{
    using SceneKit;

    public class Ray
    {
        public Ray(SCNVector3 position, SCNVector3 direction)
        {
            this.Position = position;
            this.Direction = direction;
        }

        public SCNVector3 Position { get; set; }

        public SCNVector3 Direction { get; set; }

        public static Ray Zero => new Ray(SCNVector3.Zero, SCNVector3.Zero);
    }
}