using System;
using CoreGraphics;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Example_Touch.Screens.iPhone.SimpleTouch
{
	public partial class Touches_iPhone : UIViewController
	{
		protected bool touchStartedInside;
		protected bool imageHighlighted = false;

		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need
		// to be able to be created from a xib rather than from managed code

		public Touches_iPhone (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public Touches_iPhone (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public Touches_iPhone () : base("Touches_iPhone", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Title = "Touches";

			imgDragMe.Image = UIImage.FromBundle ("Images/DragMe.png");
			imgTouchMe.Image = UIImage.FromBundle ("Images/TouchMe.png");
			imgTapMe.Image = UIImage.FromBundle ("Images/DoubleTapMe.png");
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);

			// we can get the number of fingers from the touch count, but Multitouch must be enabled
			lblNumberOfFingers.Text = "Number of fingers: " + touches.Count.ToString();

			// get the touch
			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null) {

				Console.WriteLine("screen touched");

				//==== IMAGE TOUCH
				if (imgTouchMe.Frame.Contains (touch.LocationInView (View)))
					lblTouchStatus.Text = "TouchesBegan";

				//==== IMAGE DOUBLE TAP
				if(touch.TapCount == 2 && imgTapMe.Frame.Contains (touch.LocationInView (View))) {
					if (imageHighlighted)
						imgTapMe.Image = UIImage.FromBundle ("Images/DoubleTapMe.png");
					else
						imgTapMe.Image = UIImage.FromBundle ("Images/DoubleTapMe_Highlighted.png");
					imageHighlighted = !imageHighlighted;
				}

				//==== IMAGE DRAG
				// check to see if the touch started in the dragme image
				if (imgDragMe.Frame.Contains (touch.LocationInView (View)))
					touchStartedInside = true;
			}
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);
			// get the touch
			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null) {

				//==== IMAGE TOUCH
				if (imgTouchMe.Frame.Contains (touch.LocationInView (View)))
					lblTouchStatus.Text = "TouchesMoved";

				//==== IMAGE DRAG
				// check to see if the touch started in the dragme image
				if (touchStartedInside) {

					// move the shape
					nfloat offsetX = touch.PreviousLocationInView (View).X - touch.LocationInView(View).X;
					nfloat offsetY = touch.PreviousLocationInView (View).Y - touch.LocationInView(View).Y;
					imgDragMe.Frame = new CGRect (new CGPoint (imgDragMe.Frame.X - offsetX, imgDragMe.Frame.Y - offsetY), imgDragMe.Frame.Size);
				}
			}
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);
			// get the touch
			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null) {

				//==== IMAGE TOUCH
				if (imgTouchMe.Frame.Contains (touch.LocationInView (View)))
					lblTouchStatus.Text = "TouchesEnded";
			}
			// reset our tracking flags
			touchStartedInside = false;
		}

		public override void TouchesCancelled (NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled (touches, evt);
			// reset our tracking flags
			touchStartedInside = false;
		}
	}
}

