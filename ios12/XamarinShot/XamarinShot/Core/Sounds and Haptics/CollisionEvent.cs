
namespace XamarinShot.Models
{
    public class CollisionEvent
    {
        public byte Note { get; set; }

        public byte Velocity { get; set; }

        /// <summary>
        /// only requires 7-bit accuracy in range 0..1
        /// </summary>
        /// <value>The mod wheel.</value>
        public float ModWheel { get; set;  }
    }
}
