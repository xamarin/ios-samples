using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.CoreText;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;

namespace CustomFonts
{
	[Register ("CTLabel")]
	public class CTLabel : UIView
	{
		const int DEFAULT_FONT_SIZE = 36;

		public string text;
		public CTFont ctFont;
		float fontSize;
		CTFontDescriptor sCascadeDescriptor = null;

		public CTLabel(IntPtr handle) : base (handle)
		{
			if (sCascadeDescriptor == null) {
				var theCascadeList = new List<CTFontDescriptor>();
				var testFallbackFont = FontLoader.SharedFontLoader.HiddenFontWithName ("FallbackTestFont", DEFAULT_FONT_SIZE);

				if (testFallbackFont != null) {
					theCascadeList.Add (testFallbackFont.GetFontDescriptor ());
					testFallbackFont.Dispose ();
				}

				theCascadeList.Add (new CTFontDescriptor ("LastResort", DEFAULT_FONT_SIZE));

				var dictionary = NSDictionary.FromObjectAndKey (NSObject.FromObject (theCascadeList), CTFontDescriptorAttributeKey.CascadeList);
				sCascadeDescriptor = new CTFontDescriptor (new CTFontDescriptorAttributes (dictionary));
			}

			text = "";
			ctFont = null;
			fontSize = DEFAULT_FONT_SIZE;					
		}

		public override void Draw (RectangleF rect)
		{
			var bounds = Bounds;
			var context = UIGraphics.GetCurrentContext ();

			context.TranslateCTM (0, bounds.Size.Height);
			context.ScaleCTM (1, -1);

			context.TextMatrix = CGAffineTransform.MakeIdentity ();
			context.SetTextDrawingMode (CGTextDrawingMode.Fill);

			var baseFont = ctFont;

			var aFont = baseFont.WithAttributes (baseFont.Size, sCascadeDescriptor);


			//var attributes = NSDictionary.FromObjectsAndKeys(aFont, CTFontDescriptorAttributeKey.Name, null);
			var attributes = NSDictionary.FromObjectAndKey (NSObject.FromObject (aFont), CTFontDescriptorAttributeKey.Name);
			aFont.Dispose ();

			var attributedString = new NSAttributedString (text, attributes);
			var ctLine = new CTLine (attributedString);

			if (ctLine != null) {
				var lineBounds = ctLine.GetBounds (CTLineBoundsOptions.UseOpticalBounds); //0
				context.TextPosition = new PointF (bounds.Size.Width / 2 - lineBounds.Size.Width / 2,
				                                   (bounds.Size.Height / 2 - lineBounds.Size.Height / 2) + ctFont.DescentMetric);
				ctLine.Draw (context);
				ctLine.Dispose ();
			}
		}
	}
}