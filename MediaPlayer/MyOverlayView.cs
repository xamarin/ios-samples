using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace MediaPlayer
{
	public partial class MyOverlayView : UIView {
		public MyOverlayView (IntPtr handle) : base (handle) {}

		//
		// The movieplayer rotates the content 90 degrees,
		// so the same for our view
		//
		public override void AwakeFromNib ()
		{
			var transform = Transform;			
			var bounds = UIScreen.MainScreen.Bounds;
			
			transform.Rotate ((float)(Math.PI / 2));
			transform.Translate ((float)(bounds.Height - bounds.Height/2), 0);
			Transform = transform;
			
			var newFrame = Frame;
			newFrame.X = 190;
			Frame = newFrame;
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			var touch = (UITouch) touches.AnyObject;
			if (touch.Phase == UITouchPhase.Began){
				Console.WriteLine ("Overlay view touched");
			}
		}
 
	}
}
