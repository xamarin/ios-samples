namespace XamarinShot.Models;

public class KeyPositionAnchor : ARAnchor
{
        public KeyPositionAnchor (UIImage image, OpenTK.NMatrix4 transform, ARWorldMappingStatus mappingStatus) : base ("KeyPosition", transform)
        {
                Image = image;
                MappingStatus = mappingStatus;
        }

        // this is guaranteed to be called with something of the same class
        public KeyPositionAnchor (ARAnchor anchor) : base (anchor)
        {
                var other = anchor as KeyPositionAnchor;
                if (other is null)
                        throw new Exception ("unexpexted anchor type");
                Image = other.Image;
                MappingStatus = other.MappingStatus;
        }

        [Export ("initWithCoder:")]
        public KeyPositionAnchor (NSCoder coder) : base (coder)
        {
                if (coder.DecodeObject ("image") is UIImage image)
                {
                        Image = image;
                        var mappingValue = coder.DecodeInt ("mappingStatus");
                        MappingStatus = (ARWorldMappingStatus)mappingValue;
                } else {
                        throw new Exception ();
                }
        }

        public UIImage Image { get; private set; }

        public ARWorldMappingStatus MappingStatus { get; private set; }

        public override void EncodeTo (NSCoder encoder)
        {
                base.EncodeTo (encoder);
                encoder.Encode (Image, "image");
                encoder.Encode ((int)MappingStatus, "mappingStatus");
        }

        public override NSObject Copy (NSZone? zone)
        {
                // required by objc method override
                var copy = base.Copy (zone) as KeyPositionAnchor;
                if (copy is null)
                        throw new Exception ("unexpected anchor type");
                copy.Image = Image;
                copy.MappingStatus = MappingStatus;
                return copy;
        }

        [Export ("supportsSecureCoding")]
        public static bool SupportsSecureCoding => true;
}
