using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using CoreGraphics;

namespace Touch
{
	partial class TouchViewController : UIViewController
	{
		#region Private Variables
		private bool imageHighlighted = false;
		private bool touchStartedInside;
		#endregion

		#region Constructors
		public TouchViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Title = "Touches";

		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);

			// If Multitouch is enabled, report the number of fingers down
			TouchStatus.Text = string.Format ("Number of fingers {0}", touches.Count);

			// Get the current touch
			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null)
			{
				// Check to see if any of the images have been touched
				if (TouchImage.Frame.Contains(touch.LocationInView(TouchView)))
				{
					// Fist image touched
					TouchImage.Image = UIImage.FromBundle("TouchMe_Touched.png");
					TouchStatus.Text = "Touches Began";
				} else if (touch.TapCount == 2 && DoubleTouchImage.Frame.Contains(touch.LocationInView(TouchView)))
				{
					// Second image double-tapped, toggle bitmap
					if (imageHighlighted)
					{
						DoubleTouchImage.Image = UIImage.FromBundle("DoubleTapMe.png");
						TouchStatus.Text = "Double-Tapped Off";
					}
					else
					{
						DoubleTouchImage.Image = UIImage.FromBundle("DoubleTapMe_Highlighted.png");
						TouchStatus.Text = "Double-Tapped On";
					}
					imageHighlighted = !imageHighlighted;
				} else if (DragImage.Frame.Contains(touch.LocationInView(View)))
				{
					// Third image touched, prepare to drag
					touchStartedInside = true;
				}
			}
		}

		public override void TouchesCancelled(NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled(touches, evt);

			// reset our tracking flags
			touchStartedInside = false;
			TouchImage.Image = UIImage.FromBundle("TouchMe.png");
			TouchStatus.Text = "";
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);
			// get the touch
			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null)
			{
				//==== IMAGE TOUCH
				if (TouchImage.Frame.Contains(touch.LocationInView(TouchView)))
				{
					TouchImage.Image = UIImage.FromBundle("TouchMe.png");
					TouchStatus.Text = "Touches Ended";
				}
			}
			// reset our tracking flags
			touchStartedInside = false;
		}

		public override void TouchesMoved(NSSet touches, UIEvent evt)
		{
			base.TouchesMoved(touches, evt);
			// get the touch
			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null)
			{
				//==== IMAGE TOUCH
				if (TouchImage.Frame.Contains(touch.LocationInView(TouchView)))
				{
					TouchStatus.Text = "Touches Moved";
				}

				//==== IMAGE DRAG
				// check to see if the touch started in the dragme image
				if (touchStartedInside)
				{
					// move the shape
					nfloat offsetX = touch.PreviousLocationInView(View).X - touch.LocationInView(View).X;
					nfloat offsetY = touch.PreviousLocationInView(View).Y - touch.LocationInView(View).Y;
					DragImage.Frame = new CGRect(new CGPoint(DragImage.Frame.X - offsetX, DragImage.Frame.Y - offsetY), DragImage.Frame.Size);
				}
			}
		}
		#endregion
	}
}
