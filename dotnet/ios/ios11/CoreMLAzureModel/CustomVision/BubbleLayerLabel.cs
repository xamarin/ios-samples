using CoreAnimation;

namespace CustomVision;

public class BubbleLayerLabel : CATextLayer
{
	CGSize preferredSize = CGSize.Empty;
	CGSize PreferredSize
	{
		set
		{
			preferredSize = value;
			base.NeedsLayout ();
		}
	}

	UIFont? font;

	public void UpdatePreferredSize (float maxWidth)
	{
		var text = this.String;
		if (string.IsNullOrEmpty (text)) {
			preferredSize = CGSize.Empty;
			return;
		}

		if (this.WeakFont is string fontName) {
			if (font is null)
				font = UIFont.FromName (fontName, UIFont.ButtonFontSize);

			var attributes = new UIStringAttributes
			{
				Font = font
			};
			var nsString = new NSAttributedString (text, attributes);
			var fontBounds = nsString.GetBoundingRect (new CGSize (maxWidth, float.MaxValue),
													NSStringDrawingOptions.UsesLineFragmentOrigin, null);
			fontBounds.Size = new CGSize (fontBounds.Size.Width, fontBounds.Size.Height + Math.Abs (font.Descender));
			preferredSize = fontBounds.Size;
		} else {
			preferredSize = CGSize.Empty;
		}
	}

	public override CGSize PreferredFrameSize ()
	{
		return preferredSize;
	}
}
