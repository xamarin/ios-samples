using System;
using UIKit;
using CoreGraphics;

namespace SoZoomy
{
	public class FaceView : UIView
	{
		public Action<int, FaceView> Callback;

		public int Id;

		public FaceView () : base ()
		{
			ExclusiveTouch = true;
		}

		public FaceView (CGRect frame) : base (frame)
		{
			ExclusiveTouch = true;
		}

		public override void TouchesEnded (Foundation.NSSet touches, UIEvent evt)
		{
			if (Bounds.Contains ((touches.AnyObject as UITouch).LocationInView (this))) {
				base.TouchesEnded (touches, evt);

				if (Callback != null)
					Callback (Id, this);
			}
		}
	}
}

