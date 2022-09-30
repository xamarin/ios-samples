namespace XamarinShot.Models;

public class BoardAnchor : ARAnchor
{
	public BoardAnchor (IntPtr handle) : base (handle) { }

	public BoardAnchor (SCNMatrix4 transform, CGSize size) : base ("Board", transform.ToNMatrix4 ())
	{
		Size = size;
	}

	[Export ("initWithCoder:")]
	public BoardAnchor (NSCoder coder) : base (coder)
	{
		var width = coder.DecodeFloat ("width");
		var height = coder.DecodeFloat ("height");
		Size = new CGSize (width, height);
	}

	// this is guaranteed to be called with something of the same class
	public BoardAnchor (ARAnchor anchor) : base (anchor)
	{
		if (anchor is BoardAnchor other)
			Size = other.Size;
	}

	public CGSize Size { get; private set; }

	public override NSObject Copy (NSZone? zone)
	{
		// required by objc method override
		if (base.Copy (zone) is BoardAnchor copy)
		{
			copy.Size = Size;
			return copy;
		}
		throw new NotImplementedException ("unknown zone type");
	}

	public override void EncodeTo (NSCoder encoder)
	{
		base.EncodeTo (encoder);
		encoder.Encode (Size.Width, "width");
		encoder.Encode (Size.Height, "height");
	}

	[Export ("supportsSecureCoding")]
	public static bool SupportsSecureCoding => true;
}
