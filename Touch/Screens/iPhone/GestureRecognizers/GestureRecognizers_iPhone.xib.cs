using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;

namespace Example_Touch.Screens.iPhone.GestureRecognizers
{
	public partial class GestureRecognizers_iPhone : UIViewController
	{		
		System.Drawing.RectangleF originalImageFrame = System.Drawing.RectangleF.Empty;
			
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public GestureRecognizers_iPhone (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public GestureRecognizers_iPhone (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public GestureRecognizers_iPhone () : base("GestureRecognizers_iPhone", null)
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
			
			Title = "Gesture Recognizers";
			
			imgDragMe.Image = UIImage.FromBundle ("Images/DragMe.png");
			imgTapMe.Image = UIImage.FromBundle ("Images/DoubleTapMe.png");

			originalImageFrame = imgDragMe.Frame;
			
			WireUpTapGestureRecognizer ();
			WireUpDragGestureRecognizer ();
			
		}

		#region -= tap me button =-
		
		protected void WireUpTapGestureRecognizer ()
		{
			// create a new tap gesture
			UITapGestureRecognizer tapGesture = null;
			
			NSAction action = () => {
				lblGestureStatus.Text = "tap me image tapped @" + tapGesture.LocationOfTouch (0, imgTapMe).ToString ();
			};
			
			tapGesture = new UITapGestureRecognizer (action);
			// configure it
			tapGesture.NumberOfTapsRequired = 2;
			// add the gesture recognizer to the view
			imgTapMe.AddGestureRecognizer (tapGesture);
		}
		
		#endregion
		
		#region -= drag me button =-
		
		protected void WireUpDragGestureRecognizer ()
		{
			// create a new tap gesture
			UIPanGestureRecognizer gesture = new UIPanGestureRecognizer ();
			// wire up the event handler (have to use a selector)
			gesture.AddTarget ( () => { HandleDrag (gesture); });
			// add the gesture recognizer to the view
			imgDragMe.AddGestureRecognizer (gesture);
		}
		
		protected void HandleDrag (UIPanGestureRecognizer recognizer)
		{
			// if it's just began, cache the location of the image
			if (recognizer.State == UIGestureRecognizerState.Began)
				originalImageFrame = imgDragMe.Frame;
			
			if (recognizer.State != (UIGestureRecognizerState.Cancelled | UIGestureRecognizerState.Failed 
				| UIGestureRecognizerState.Possible)) {
				
				// move the shape by adding the offset to the object's frame
				System.Drawing.PointF offset = recognizer.TranslationInView (imgDragMe);
				System.Drawing.RectangleF newFrame = originalImageFrame;
				newFrame.Offset (offset.X, offset.Y);			
				imgDragMe.Frame = newFrame;
			}
		}
				
		#endregion
		
	}
}

