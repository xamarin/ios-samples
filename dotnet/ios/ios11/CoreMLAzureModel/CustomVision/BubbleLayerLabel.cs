using System;
using CoreAnimation;
using CoreGraphics;
using CoreText;
using Foundation;
using UIKit;

namespace CustomVision
{
    public class BubbleLayerLabel : CATextLayer
    {
        CGSize preferredSize = CGSize.Empty;
        CGSize PreferredSize 
        {
            set {
				preferredSize = value;
                base.NeedsLayout();
            }    
        }

		UIFont font;

        public void UpdatePreferredSize(float maxWidth)
        {
            var text = this.String;
            if (string.IsNullOrEmpty(text))
            {
                preferredSize = CGSize.Empty;
                return;
            }

            // TODO: test .Font property
            var fontName = this.WeakFont as string;
            if (fontName == null)
            {
                Console.WriteLine("Trying to update label size without font");
                preferredSize = CGSize.Empty;
                return;
            }
			if (font == null)
			{
				font = UIFont.FromName(fontName, UIFont.ButtonFontSize);
			}

            var attributes = new UIStringAttributes
            {
                Font = font
            };
            var nsString = new NSAttributedString(text, attributes);
            var fontBounds = nsString.GetBoundingRect(new CGSize(maxWidth, float.MaxValue),
                                                      NSStringDrawingOptions.UsesLineFragmentOrigin, null);
            fontBounds.Size = new CGSize(fontBounds.Size.Width, fontBounds.Size.Height + Math.Abs(font.Descender));
            preferredSize = fontBounds.Size;
        }

        public override CGSize PreferredFrameSize()
        {
            return preferredSize;
        }
    }
}
