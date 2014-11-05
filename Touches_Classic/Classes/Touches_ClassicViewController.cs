// 
// Touches_ClassicViewController.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2011 Xamarin <http://xamarin.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using CoreGraphics;
using System.Linq;
using UIKit;
using Foundation;

namespace Touches_Classic
{
	public partial class Touches_ClassicViewController : UIViewController
	{
		private float padding = 10f;

		public Touches_ClassicViewController (UIWindow window, string nibName, NSBundle bundle) : base (nibName, bundle)
		{
		}
		
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			
			// Release images
			firstImage.Dispose ();
			secondImage.Dispose ();
			thirdImage.Dispose ();
			
			// Release labels
			touchInfoLabel.Dispose ();
			touchPhaseLabel.Dispose ();
			touchInstructionLabel.Dispose ();
			touchTrackingLabel.Dispose ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}
		
		#region Touch handling
		bool piecesOnTop;

		public override void TouchesBegan (NSSet touchesSet, UIEvent evt)
		{
			var touches = touchesSet.ToArray<UITouch> ();
			touchPhaseLabel.Text = "Phase:Touches began";
			touchInfoLabel.Text = "";
		
			var numTaps = touches.Sum (t => t.TapCount);
			if (numTaps >= 2){
				touchInfoLabel.Text = string.Format ("{0} taps", numTaps);
				if (numTaps == 2 && piecesOnTop) {
					// recieved double tap -> align the three pieces diagonal.
					firstImage.Center = new CGPoint (padding + firstImage.Frame.Width / 2f,
						touchInfoLabel.Frame.Bottom + padding + firstImage.Frame.Height / 2f);
					secondImage.Center = new CGPoint (View.Bounds.Width / 2f, View.Bounds.Height / 2f);
					thirdImage.Center = new CGPoint (View.Bounds.Width - thirdImage.Frame.Width / 2f - padding,
						touchInstructionLabel.Frame.Top - thirdImage.Frame.Height);
					touchInstructionLabel.Text = "";
				}
					
			} else {
				touchTrackingLabel.Text = "";
			}
			foreach (var touch in touches) {
				// Send to the dispatch method, which will make sure the appropriate subview is acted upon
				DispatchTouchAtPoint (touch.LocationInView (View));
			}
		}
		
		// Checks which image the point is in & performs the opening animation (which makes the image a bit larger)
		void DispatchTouchAtPoint (CGPoint touchPoint)
		{
			if (firstImage.Frame.Contains (touchPoint))
				AnimateTouchDownAtPoint (firstImage, touchPoint);
			if (secondImage.Frame.Contains (touchPoint))
				AnimateTouchDownAtPoint (secondImage, touchPoint);
			if (thirdImage.Frame.Contains (touchPoint))
				AnimateTouchDownAtPoint (thirdImage, touchPoint);
		}
		
		// Handles the continuation of a touch
		public override void TouchesMoved (NSSet touchesSet, UIEvent evt)
		{
			var touches = touchesSet.ToArray<UITouch> ();
			touchPhaseLabel.Text = "Phase: Touches moved";
			
			foreach (var touch in touches) {
				// Send to the dispatch touch method, which ensures that the image is moved
				DispatchTouchEvent (touch.View, touch.LocationInView (View));
			}
			
			// When multiple touches, report the number of touches. 
			if (touches.Length > 1) {
				touchTrackingLabel.Text = string.Format ("Tracking {0} touches", touches.Length);
			} else {
				touchTrackingLabel.Text = "Tracking 1 touch";
			}
		}
			
		// Checks to see which view is touch point is in and sets the center of the moved view to the new position.
		void DispatchTouchEvent (UIView theView, CGPoint touchPoint)
		{
			if (firstImage.Frame.Contains (touchPoint))
				firstImage.Center = touchPoint;
			if (secondImage.Frame.Contains (touchPoint))
				secondImage.Center = touchPoint;
			if (thirdImage.Frame.Contains (touchPoint))
				thirdImage.Center = touchPoint;
		}
		
		public override void TouchesEnded (NSSet touchesSet, UIEvent evt)
		{
			touchPhaseLabel.Text = "Phase: Touches ended";
			foreach (var touch in touchesSet.ToArray<UITouch> ()) {
				DispatchTouchEndEvent (touch.View, touch.LocationInView (View));
			}
		}
		
		// Puts back the images to their original size
		void DispatchTouchEndEvent (UIView theView, CGPoint touchPoint)
		{
			if (firstImage.Frame.Contains (touchPoint))
				AnimateTouchUpAtPoint (firstImage, touchPoint);
			if (secondImage.Frame.Contains (touchPoint))
				AnimateTouchUpAtPoint (secondImage, touchPoint);
			if (thirdImage.Frame.Contains (touchPoint))
				AnimateTouchUpAtPoint (thirdImage, touchPoint);
			
			// If one piece obscures another, display a message so the user can move the pieces apart
			piecesOnTop = firstImage.Center == secondImage.Center ||
				firstImage.Center == thirdImage.Center ||
				secondImage.Center == thirdImage.Center;
			
			if (piecesOnTop)
				touchInstructionLabel.Text = @"Double tap the background to move the pieces apart.";

			touchTrackingLabel.Text = string.Empty;
		}
		
		public override void TouchesCancelled (NSSet touchesSet, UIEvent evt)
		{
			touchPhaseLabel.Text = "Phase: Touches cancelled";
			foreach (var touch in touchesSet.ToArray<UITouch> ()) {
				DispatchTouchEndEvent (touch.View, touch.LocationInView (View));
			}
		}
		#endregion
		
		#region Animating subviews
		const double GROW_ANIMATION_DURATION_SECONDS = 0.15;
		const double SHRINK_ANIMATION_DURATION_SECONDS = 0.15;
		
		// Scales up a image slightly
		void AnimateTouchDownAtPoint (UIImageView theView, CGPoint touchPoint)
		{
			theView.AnimationDuration = GROW_ANIMATION_DURATION_SECONDS;
			theView.Transform = CoreGraphics.CGAffineTransform.MakeScale (1.2f, 1.2f);
		}
		
		// Scales down a image slightly
		void AnimateTouchUpAtPoint (UIImageView theView, CGPoint touchPoint)
		{
			// Set the center to the touch position
			theView.Center = touchPoint;
			
			// Resets the transformation
			theView.AnimationDuration = SHRINK_ANIMATION_DURATION_SECONDS;
			theView.Transform = CoreGraphics.CGAffineTransform.MakeIdentity ();
		}
		#endregion
	}
}
