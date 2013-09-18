using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace TextKitDemo
{
	public static class Font
	{
		public static UIFont GetPreferredFont (NSString textStyle, float scale)
		{
			UIFontDescriptor tmp = UIFontDescriptor.GetPreferredDescriptorForTextStyle (textStyle);
			UIFontDescriptor newBaseDescriptor = tmp.CreateWithSize (tmp.PointSize * scale);

			return UIFont.FromDescriptor (newBaseDescriptor, newBaseDescriptor.PointSize);
		}
	}
}

