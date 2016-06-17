using System;
using System.Drawing;
using Foundation;
using UIKit;
using CoreGraphics;

namespace UIKit
{
	[Register("TVFont")]
	public class TVFont : NSObject
	{
		#region Static constants
		public static float LabelFontSize = 16f;
		#endregion

		#region Computed Properties
		public UIFont UIFont { get; set; }
		#endregion

		#region Type Conversion
		public static implicit operator UIFont(TVFont font) {
			return font.UIFont;
		}

		public static implicit operator TVFont(UIFont font) {
			return new TVFont(font);
		}
		#endregion

		#region Constructors
		public TVFont (UIFont font)
		{
			// Intialize
			this.UIFont = font;
		}
		#endregion

		#region Static Methods
		public static UIFont BoldSystemFontOfSize(nfloat size) {
			return UIFont.BoldSystemFontOfSize (size);
		}

		public static UIFont SystemFontOfSize(nfloat size) {
			return UIFont.SystemFontOfSize (size);
		}

		public static UIFont FromName(string name, nfloat size) {
			return UIFont.FromName(name,size);
		}
		#endregion
	}
}

