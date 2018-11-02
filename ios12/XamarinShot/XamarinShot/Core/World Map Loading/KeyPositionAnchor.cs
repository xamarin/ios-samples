
namespace XamarinShot.Models
{
    using ARKit;
    using Foundation;
    using System;
    using UIKit;

    public class KeyPositionAnchor : ARAnchor
    {
        public KeyPositionAnchor(UIImage image, OpenTK.NMatrix4 transform, ARWorldMappingStatus mappingStatus) : base("KeyPosition", transform)
        {
            this.Image = image;
            this.MappingStatus = mappingStatus;
        }

        // this is guaranteed to be called with something of the same class
        public KeyPositionAnchor(ARAnchor anchor) : base(anchor)
        {
            var other = anchor as KeyPositionAnchor;
            this.Image = other.Image;
            this.MappingStatus = other.MappingStatus;
        }

        [Export("initWithCoder:")]
        public KeyPositionAnchor(NSCoder coder) : base(coder)
        {
            if (coder.DecodeObject("image") is UIImage image)
            {
                this.Image = image;
                var mappingValue = coder.DecodeInt("mappingStatus");
                this.MappingStatus = (ARWorldMappingStatus)mappingValue;
            }
            else
            {
                throw new Exception();
            }
        }

        public UIImage Image { get; private set; }

        public ARWorldMappingStatus MappingStatus { get; private set; }

        public override void EncodeTo(NSCoder encoder)
        {
            base.EncodeTo(encoder);
            encoder.Encode(this.Image, "image");
            encoder.Encode((int)this.MappingStatus, "mappingStatus");
        }

        public override NSObject Copy(NSZone zone)
        {
            // required by objc method override
            var copy = base.Copy(zone) as KeyPositionAnchor;
            copy.Image = this.Image;
            copy.MappingStatus = this.MappingStatus;
            return copy;
        }

        [Export("supportsSecureCoding")]
        public static bool SupportsSecureCoding => true;
    }
}