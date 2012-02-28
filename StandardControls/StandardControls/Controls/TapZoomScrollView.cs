using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;

namespace Example_StandardControls.Controls
{
	[MonoTouch.Foundation.Register("TapZoomScrollView")]
	public class TapZoomScrollView : UIScrollView
	{
		public TapZoomScrollView (IntPtr handle) : base(handle) { }

		[Export("initWithCoder:")]
		public TapZoomScrollView (NSCoder coder) : base(coder) { }

		public TapZoomScrollView () { }
		
		public TapZoomScrollView (RectangleF frame) : base(frame) { }
		
		
		public override void TouchesBegan (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);
			
			UITouch touch = touches.AnyObject as UITouch;
			
			if (touch.TapCount == 2)
			{
				if (ZoomScale >= 2)
					SetZoomScale(1, true);
				else
					SetZoomScale(3, true);
			}
		}
	}
}

