
namespace XamarinShot.Models
{
    using ARKit;
    using CoreGraphics;
    using Foundation;
    using SceneKit;
    using XamarinShot.Utils;
    using System;

    public class BoardAnchor : ARAnchor
    {
        public BoardAnchor(IntPtr handle) : base(handle) { }

        public BoardAnchor(SCNMatrix4 transform, CGSize size) : base("Board", transform.ToNMatrix4())
        {
            this.Size = size;
        }

        [Export("initWithCoder:")]
        public BoardAnchor(NSCoder coder) : base(coder)
        {
            var width = coder.DecodeFloat("width");
            var height = coder.DecodeFloat("height");
            this.Size = new CGSize(width, height);
        }

        // this is guaranteed to be called with something of the same class
        public BoardAnchor(ARAnchor anchor) : base(anchor)
        {
            var other = anchor as BoardAnchor;
            this.Size = other.Size;
        }

        public CGSize Size { get; private set; }

        public override NSObject Copy(NSZone zone)
        {
            // required by objc method override
            var copy = base.Copy(zone) as BoardAnchor;
            copy.Size = this.Size;
            return copy;
        }

        public override void EncodeTo(NSCoder encoder)
        {
            base.EncodeTo(encoder);
            encoder.Encode(this.Size.Width, "width");
            encoder.Encode(this.Size.Height, "height");
        }

        [Export("supportsSecureCoding")]
        public static bool SupportsSecureCoding => true;
    }
}