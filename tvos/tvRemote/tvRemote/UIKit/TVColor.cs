using System;
using System.Drawing;
using Foundation;
using UIKit;
using CoreGraphics;

namespace UIKit
{
	public class TVColor : NSObject
	{
		#region Computed Properties
		public UIColor UIColor { get; set; }

		public CGColor CGColor {
			get { return this.UIColor.CGColor; }
		}
		#endregion

		#region Type Conversion
		public static implicit operator UIColor(TVColor color) {
			return color.UIColor;
		}

		public static implicit operator TVColor(UIColor color) {
			return new TVColor(color);
		}
		#endregion

		#region Constructors
		public TVColor(UIColor color) : base() {
			// Initialize
			this.UIColor = color;
		}

		public TVColor(nfloat red, nfloat green, nfloat blue, nfloat alpha) : base() {
			// Initialize
			this.UIColor = UIColor.FromRGBA (red, green, blue, alpha);
		}

		public TVColor (NSObjectFlag x) : base(x) {
		}

		public TVColor (IntPtr handle) : base(handle) {
		}
		#endregion

		#region Static Methods
		public static TVColor FromRGBA(nfloat red, nfloat green, nfloat blue, nfloat alpha) {

			return new TVColor (UIColor.FromRGBA (red, green, blue, alpha));
		}
		#endregion

		#region Public Methods
		public void SetStroke() {
			this.UIColor.SetStroke ();
		}

		public void SetFill() {
			// Send color change to the String drawing routines
			UIStringDrawing.FillColor = this.UIColor;

			this.UIColor.SetFill ();
		}

		public void GetRGBA (out nfloat red, out nfloat green, out nfloat blue, out nfloat alpha){
			this.UIColor.GetRGBA (out red, out green, out blue, out alpha);
		}

		public UIColor ColorWithAlpha(nfloat alpha) {
			return this.UIColor.ColorWithAlpha (alpha);
		}

		public UIColor BlendedColor(nfloat fraction, UIColor color2)
		{
			var rgba1 = new nfloat[4];
			var rgba2 = new nfloat[4];

			this.UIColor.GetRGBA(out rgba1 [0], out rgba1 [1], out rgba1 [2], out rgba1 [3]);
			color2.GetRGBA(out rgba2 [0], out rgba2 [1], out rgba2 [2], out rgba2 [3]);

			return new UIColor (
				rgba1 [0] * (1 - fraction) + rgba2 [0] * fraction,
				rgba1 [1] * (1 - fraction) + rgba2 [1] * fraction,
				rgba1 [2] * (1 - fraction) + rgba2 [2] * fraction,
				rgba1 [3] * (1 - fraction) + rgba2 [3] * fraction);
		}
		#endregion
	}
}

