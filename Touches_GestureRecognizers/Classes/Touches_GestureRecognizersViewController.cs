//
// Touches_GestureRecognizersViewController.cs
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
using ObjCRuntime;

namespace Touches_GestureRecognizers
{
	public partial class Touches_GestureRecognizersViewController : UIViewController
	{

		public Touches_GestureRecognizersViewController (UIWindow window, string nibName, NSBundle bundle) : base (nibName, bundle)
		{
		}

		#region Tearing down
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
		#endregion

		#region Set up
		class GestureDelegate : UIGestureRecognizerDelegate
		{
			Touches_GestureRecognizersViewController controller;

			public GestureDelegate (Touches_GestureRecognizersViewController controller)
			{
				this.controller = controller;
			}

			public override bool ShouldReceiveTouch(UIGestureRecognizer aRecogniser, UITouch aTouch)
			{
				return true;
			}

			// Ensure that the pinch, pan and rotate gestures are all recognized simultaneously
			public override bool ShouldRecognizeSimultaneously (UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
			{
				// if the gesture recognizers's view isn't one of our images don't recognize
				if (gestureRecognizer.View != controller.firstImage &&
					gestureRecognizer.View != controller.secondImage &&
					gestureRecognizer.View != controller.thirdImage)
					return false;

				// if the gesture recognizers views differ, don't recognize
				if (gestureRecognizer.View != otherGestureRecognizer.View)
					return false;

				// if either of the gesture recognizers is a long press, don't recognize
				if (gestureRecognizer is UILongPressGestureRecognizer || otherGestureRecognizer is UILongPressGestureRecognizer)
					return false;

				return true;
			}
		}

		// Add gesture recognizers to one of our images
		void AddGestureRecognizersToImage (UIImageView image)
		{
			image.UserInteractionEnabled = true;

			var rotationGesture = new UIRotationGestureRecognizer (RotateImage);
			image.AddGestureRecognizer (rotationGesture);

			var pinchGesture = new UIPinchGestureRecognizer (ScaleImage);
			pinchGesture.Delegate = new GestureDelegate (this);
			image.AddGestureRecognizer (pinchGesture);

			var panGesture = new UIPanGestureRecognizer (PanImage);
			panGesture.MaximumNumberOfTouches = 2;
			panGesture.Delegate = new GestureDelegate (this);
			image.AddGestureRecognizer (panGesture);

			var longPressGesture = new UILongPressGestureRecognizer (ShowResetMenu);
			image.AddGestureRecognizer (longPressGesture);
		}

		public override void ViewDidLoad ()
		{
			AddGestureRecognizersToImage (firstImage);
			AddGestureRecognizersToImage (secondImage);
			AddGestureRecognizersToImage (thirdImage);
		}
		#endregion

		#region Utility methods
		// Scale and rotation transforms are applied relative to the layer's anchor point.
		// This method moves a UIGestureRecognizer's view anchor point between the user's fingers
		void AdjustAnchorPointForGestureRecognizer (UIGestureRecognizer gestureRecognizer)
		{
			if (gestureRecognizer.State == UIGestureRecognizerState.Began) {
				var image = gestureRecognizer.View;
				var locationInView = gestureRecognizer.LocationInView (image);
				var locationInSuperview = gestureRecognizer.LocationInView (image.Superview);

				image.Layer.AnchorPoint = new CGPoint (locationInView.X / image.Bounds.Size.Width, locationInView.Y / image.Bounds.Size.Height);
				image.Center = locationInSuperview;
			}
		}

		// Reset image to the default anchor point and transform
		[Export("ResetImage:")]
		void ResetImage (UIMenuController controller)
		{
			var mid = new CGPoint ((imageForReset.Bounds.Left + imageForReset.Bounds.Right) / 2, (imageForReset.Bounds.Top + imageForReset.Bounds.Bottom) / 2);
			var locationInSuperview = imageForReset.ConvertPointToView (mid, imageForReset.Superview);
			imageForReset.Layer.AnchorPoint = new CGPoint (0.5f, 0.5f);
			imageForReset.Center =locationInSuperview;

			UIView.BeginAnimations (null, IntPtr.Zero);
			imageForReset.Transform = CoreGraphics.CGAffineTransform.MakeIdentity ();
			UIView.CommitAnimations ();
		}

		// UIMenuController requires that we can become first responder - otherwise it won't display
		public override bool CanBecomeFirstResponder {
			get {
				return true;
			}
		}
		#endregion

		#region Touch handling
		// Shift the image's center by the pan amount
		void PanImage (UIPanGestureRecognizer gestureRecognizer)
		{
			AdjustAnchorPointForGestureRecognizer (gestureRecognizer);
			var image = gestureRecognizer.View;
			if (gestureRecognizer.State == UIGestureRecognizerState.Began || gestureRecognizer.State == UIGestureRecognizerState.Changed) {
				var translation = gestureRecognizer.TranslationInView (View);
				image.Center = new CGPoint (image.Center.X + translation.X, image.Center.Y + translation.Y);
				// Reset the gesture recognizer's translation to {0, 0} - the next callback will get a delta from the current position.
				gestureRecognizer.SetTranslation (CGPoint.Empty, image);
			}
		}

		// Rotates the image by the current rotation
		void RotateImage (UIRotationGestureRecognizer gestureRecognizer)
		{
			AdjustAnchorPointForGestureRecognizer (gestureRecognizer);
			if (gestureRecognizer.State == UIGestureRecognizerState.Began || gestureRecognizer.State == UIGestureRecognizerState.Changed) {
				gestureRecognizer.View.Transform *= CGAffineTransform.MakeRotation (gestureRecognizer.Rotation);
				// Reset the gesture recognizer's rotation - the next callback will get a delta from the current rotation.
				gestureRecognizer.Rotation = 0;
			}
		}

		// Scales the image by the current scale
		void ScaleImage (UIPinchGestureRecognizer gestureRecognizer)
		{
			AdjustAnchorPointForGestureRecognizer (gestureRecognizer);
			if (gestureRecognizer.State == UIGestureRecognizerState.Began || gestureRecognizer.State == UIGestureRecognizerState.Changed) {
				gestureRecognizer.View.Transform *= CGAffineTransform.MakeScale (gestureRecognizer.Scale, gestureRecognizer.Scale);
				// Reset the gesture recognizer's scale - the next callback will get a delta from the current scale.
				gestureRecognizer.Scale = 1;
			}
		}

		UIView imageForReset;
		// Display a menu with a single item to allow the piece's transform to be reset
		[Export("ShowResetMenu:")]
		void ShowResetMenu (UILongPressGestureRecognizer gestureRecognizer)
		{
			if (gestureRecognizer.State == UIGestureRecognizerState.Began) {
				var menuController = UIMenuController.SharedMenuController;
				var resetMenuItem = new UIMenuItem ("Reset", new Selector ("ResetImage:"));
				var location = gestureRecognizer.LocationInView (gestureRecognizer.View);
				BecomeFirstResponder ();
				menuController.MenuItems = new [] { resetMenuItem };
				menuController.SetTargetRect (new CGRect (location.X, location.Y, 0, 0), gestureRecognizer.View);
				menuController.MenuVisible = true;
//				menuController.Animated = true;
				imageForReset = gestureRecognizer.View;
			}
		}
		#endregion
	}
}
