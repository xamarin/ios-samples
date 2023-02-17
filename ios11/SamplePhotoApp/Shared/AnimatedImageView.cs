using System;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace SamplePhotoApp {
	[Register (nameof (AnimatedImageView))]
	public class AnimatedImageView : UIView {
		AnimatedImage animatedImage;
		bool isPlaying;

		public AnimatedImage AnimatedImage {
			get { return animatedImage; }
			set {
				animatedImage = value;
				ResetAnimationState ();
				UpdateAnimation ();
				SetNeedsLayout ();
			}
		}

		public bool IsPlaying {
			get { return isPlaying; }
			set {
				if (isPlaying != value) {
					isPlaying = value;
					UpdateAnimation ();
				}
			}
		}

		CADisplayLink displayLink;
		int displayedIndex;
		UIView displayView;

		// Animation state
		bool hasStartedAnimating;
		bool hasFinishedAnimating;
		bool isInfiniteLoop;
		nint remainingLoopCount;
		double elapsedTime;
		double previousTime;

		[Export ("initWithCoder:")]
		public AnimatedImageView (NSCoder coder)
			: base (coder)
		{
		}

		public AnimatedImageView (IntPtr handle)
			: base (handle)
		{
		}

		protected override void Dispose (bool disposing)
		{
			displayLink?.Invalidate ();
			base.Dispose (disposing);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			nfloat viewAspect = 0.0f;
			if (Bounds.Height > 0.0) {
				viewAspect = Bounds.Width / Bounds.Height;
			}
			nfloat imageAspect = 0.0f;
			if (AnimatedImage != null) {
				var imageSize = AnimatedImage.Size;
				if (imageSize.Height > 0.0) {
					imageAspect = imageSize.Width / imageSize.Height;
				}
			}

			var viewFrame = new CGRect (0.0, 0.0, Bounds.Width, Bounds.Height);
			if (imageAspect < viewAspect) {
				viewFrame.Width = Bounds.Height * imageAspect;
				viewFrame.X = (Bounds.Width / 2.0f) - (0.5f * viewFrame.Width);
			} else if (imageAspect > 0.0) {
				viewFrame.Height = Bounds.Width / imageAspect;
				viewFrame.Y = (Bounds.Height / 2.0f) - (0.5f * viewFrame.Height);
			}

			if (AnimatedImage != null) {
				if (displayView == null) {
					var newView = new UIView (CGRect.Empty);
					AddSubview (newView);
					displayView = newView;
					UpdateImage ();
				}
			} else {
				displayView?.RemoveFromSuperview ();
				displayView = null;
			}

			if (displayView != null) {
				displayView.Frame = viewFrame;
			}
		}

		public override void MovedToWindow ()
		{
			base.MovedToWindow ();
			UpdateAnimation ();
		}

		public override void MovedToSuperview ()
		{
			base.MovedToSuperview ();
			UpdateAnimation ();
		}

		public override nfloat Alpha {
			get { return base.Alpha; }
			set {
				base.Alpha = value;
				UpdateAnimation ();
			}
		}

		public override bool Hidden {
			get { return base.Hidden; }
			set {
				base.Hidden = value;
				UpdateAnimation ();
			}
		}

		bool ShouldAnimate ()
		{
			var isShown = Window != null && Superview != null && !Hidden && Alpha > 0.0;
			return isShown && AnimatedImage != null && IsPlaying && !hasFinishedAnimating;
		}

		void ResetAnimationState ()
		{
			displayedIndex = 0;
			hasStartedAnimating = false;
			hasFinishedAnimating = false;
			isInfiniteLoop = true;
			remainingLoopCount = 0;
			if (AnimatedImage != null) {
				isInfiniteLoop = AnimatedImage?.FrameCount == 0;
				remainingLoopCount = AnimatedImage.LoopCount;
			}
			elapsedTime = 0.0;
			previousTime = 0.0;
		}

		void UpdateAnimation ()
		{
			if (ShouldAnimate ()) {
				displayLink = CADisplayLink.Create (TimerFired);
				displayLink.AddToRunLoop (NSRunLoop.Main, NSRunLoopMode.Common);
				displayLink.PreferredFramesPerSecond = 60;
			} else {
				displayLink?.Invalidate ();
				displayLink = null;
			}
		}

		void UpdateImage ()
		{
			var image = AnimatedImage?.ImageAtIndex (displayedIndex);
			if (image != null && displayView != null) {
				displayView.Layer.Contents = image;
			}
		}

		void TimerFired ()
		{
			if (!ShouldAnimate () || AnimatedImage == null)
				return;

			var timestamp = displayLink.Timestamp;

			// If this is the first callback, set things up
			if (!hasStartedAnimating) {
				elapsedTime = 0.0;
				previousTime = timestamp;
				hasStartedAnimating = true;
			}

			var currentDelayTime = AnimatedImage.DelayAtIndex (displayedIndex);
			elapsedTime += timestamp - previousTime;
			previousTime = timestamp;

			// Aaccount for big gaps in playback by just resuming from now
			// e.g. user presses home button and comes back after a while.
			// Allow for the possibility of the current delay time being relatively long
			if (elapsedTime >= Math.Max (10.0, currentDelayTime + 1.0)) {
				elapsedTime = 0.0;
			}

			var changedFrame = false;
			while (elapsedTime >= currentDelayTime) {
				elapsedTime -= currentDelayTime;
				displayedIndex += 1;
				changedFrame = true;
				if (displayedIndex >= AnimatedImage.FrameCount) {
					// Time to loop. Start infinite loops over, otherwise decrement loop count and stop if done
					if (isInfiniteLoop) {
						displayedIndex = 0;
					} else {
						remainingLoopCount -= 1;
						if (remainingLoopCount == 0) {
							hasFinishedAnimating = true;
							DispatchQueue.MainQueue.DispatchAsync (() => {
								UpdateAnimation ();
							});
						} else {
							displayedIndex = 0;
						}
					}
				}
			}

			if (changedFrame) {
				UpdateImage ();
			}
		}
	}
}
