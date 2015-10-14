using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using CoreGraphics;

namespace Touch
{
	partial class GestureViewController : UIViewController
	{
		#region Private Variables
		private bool imageHighlighted = false;
		private CGRect originalImageFrame = CGRect.Empty;
		#endregion

		#region Constructors
		public GestureViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Title = "Gesture Recognizers";

			// Save initial state
			originalImageFrame = DragImage.Frame;

			WireUpTapGestureRecognizer();
			WireUpDragGestureRecognizer();
		}
		#endregion

		#region Private Methods
		private void HandleDrag(UIPanGestureRecognizer recognizer)
		{
			// If it's just began, cache the location of the image
			if (recognizer.State == UIGestureRecognizerState.Began)
			{
				originalImageFrame = DragImage.Frame;
			}

			// Move the image if the gesture is valid
			if (recognizer.State != (UIGestureRecognizerState.Cancelled | UIGestureRecognizerState.Failed
				| UIGestureRecognizerState.Possible))
			{
				// Move the image by adding the offset to the object's frame
				CGPoint offset = recognizer.TranslationInView(DragImage);
				CGRect newFrame = originalImageFrame;
				newFrame.Offset(offset.X, offset.Y);
				DragImage.Frame = newFrame;
			}
		}

		private void WireUpDragGestureRecognizer()
		{
			// Create a new tap gesture
			UIPanGestureRecognizer gesture = new UIPanGestureRecognizer();

			// Wire up the event handler (have to use a selector)
			gesture.AddTarget(() => HandleDrag(gesture));

			// Add the gesture recognizer to the view
			DragImage.AddGestureRecognizer(gesture);
		}
			
		private void WireUpTapGestureRecognizer()
		{
			// Create a new tap gesture
			UITapGestureRecognizer tapGesture = null;

			// Report touch
			Action action = () => { 
				TouchStatus.Text = string.Format("Image touched at: {0}",tapGesture.LocationOfTouch(0, DoubleTouchImage));

				// Toggle the image
				if (imageHighlighted)
				{
					DoubleTouchImage.Image = UIImage.FromBundle("DoubleTapMe.png");
				}
				else
				{
					DoubleTouchImage.Image = UIImage.FromBundle("DoubleTapMe_Highlighted.png");
				}
				imageHighlighted = !imageHighlighted;
			};

			tapGesture = new UITapGestureRecognizer(action);

			// Configure it
			tapGesture.NumberOfTapsRequired = 2;

			// Add the gesture recognizer to the view
			DoubleTouchImage.AddGestureRecognizer(tapGesture);
		}
		#endregion
	}
}
