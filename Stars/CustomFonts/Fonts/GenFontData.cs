using System;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;

namespace CustomFonts
{
	public class GenFontData : NSObject
	{
		public CGFont cgFont;
		public bool registered;
		public IntPtr data;
		public int length;

		public GenFontData ()
		{
		}
	}
}