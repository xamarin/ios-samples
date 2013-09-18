using System;
using MonoTouch.UIKit;
using System.Drawing;

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

		public FaceView (RectangleF frame) : base (frame)
		{
			ExclusiveTouch = true;
		}

		public override void TouchesEnded (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			if (Bounds.Contains ((touches.AnyObject as UITouch).LocationInView (this))) {
				base.TouchesEnded (touches, evt);

				if (Callback != null)
					Callback (Id, this);
			}
		}
	}
}

