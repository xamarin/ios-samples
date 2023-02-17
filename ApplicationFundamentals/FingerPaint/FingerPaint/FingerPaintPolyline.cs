using CoreGraphics;
using System.Collections.Generic;

namespace FingerPaint {
	class FingerPaintPolyline {
		public FingerPaintPolyline ()
		{
			Path = new CGPath ();
		}

		public CGColor Color { set; get; }

		public float StrokeWidth { set; get; }

		public CGPath Path { private set; get; }
	}
}
