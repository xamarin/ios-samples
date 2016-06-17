using System;

using CoreGraphics;
using Foundation;
using UIKit;

namespace StickyCorners {
	public partial class StickyCornersViewController : UIViewController, INSCoding {

		const float ItemAspectRatio = 0.7f;

		UIDynamicAnimator animator;
		StickyCornersBehavior stickyBehavior;
		UIView itemView;
		CGPoint offset = CGPoint.Empty;

		[Export ("initWithCoder:")]
		public StickyCornersViewController (NSCoder coder) : base (coder)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var screenBounds = UIScreen.MainScreen.Bounds;
			var length = Math.Floor (0.1 * Math.Max (screenBounds.Width, screenBounds.Height));

			itemView = new UIView (new CGRect (0.0, 0.0, length, Math.Floor (length / ItemAspectRatio))) {
				AutoresizingMask = UIViewAutoresizing.None,
				BackgroundColor = UIColor.Red
			};

			var panGestureRecognizer = new UIPanGestureRecognizer (Pan);
			itemView.AddGestureRecognizer (panGestureRecognizer);
			View.AddSubview (itemView);

			var longPressGestureRecognizer = new UILongPressGestureRecognizer (LongPress);
			View.AddGestureRecognizer (longPressGestureRecognizer);

			animator = new UIDynamicAnimator (View);
			stickyBehavior = new StickyCornersBehavior (itemView, (float)length * 0.5f);
			animator.AddBehavior (stickyBehavior);
		}

		public override void ViewWillTransitionToSize (CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
		{
			base.ViewWillTransitionToSize (toSize, coordinator);

			var corner = stickyBehavior.CurrentCorner;
			stickyBehavior.Enabled = false;
			stickyBehavior.UpdateFieldsInBounds (new CGRect (CGPoint.Empty, toSize));

			coordinator.AnimateAlongsideTransition (context => {
				itemView.Center = stickyBehavior.GetPositionForCorner (corner);
			}, context => {
				stickyBehavior.Enabled = true;
			});
		}

		void Pan (UIPanGestureRecognizer pan)
		{
			var location = pan.LocationInView (View);

			switch (pan.State) {
			case UIGestureRecognizerState.Began:
				// Capture the initial touch offset from the itemView's center.
				var center = itemView.Center;
				offset.X = location.X - center.X;
				offset.Y = location.Y - center.Y;

				// Disable the behavior while the item is manipulated by the pan recognizer.
				stickyBehavior.Enabled = false;
				break;
			case UIGestureRecognizerState.Changed:
				// Get reference bounds.
				var referenceBounds = View.Bounds;
				var referenceWidth = referenceBounds.Width;
				var referenceHeight = referenceBounds.Height;

				// Get item bounds.
				var itemBounds = itemView.Bounds;
				var itemHalfWidth = itemBounds.Width / 2f;
				var itemHalfHeight = itemBounds.Height / 2f;

				// Apply the initial offset.
				location.X -= offset.X;
				location.Y -= offset.Y;

				// Bound the item position inside the reference view.
				location.X = NMath.Max (itemHalfWidth, location.X);
				location.X = NMath.Min (referenceWidth - itemHalfWidth, location.X);
				location.Y = NMath.Max (itemHalfHeight, location.Y);
				location.Y = NMath.Min (referenceHeight - itemHalfHeight, location.Y);

				// Apply the resulting item center.
				itemView.Center = location;
				break;
			case UIGestureRecognizerState.Ended:
			case UIGestureRecognizerState.Cancelled:
				// Get the current velocity of the item from the pan gesture recognizer.
				var velocity = pan.VelocityInView (View);

				// Re-enable the stickyCornersBehavior.
				stickyBehavior.Enabled = true;

				// Add the current velocity to the sticky corners behavior.
				stickyBehavior.AddLinearVelocity (velocity);
				break;
			}
		}

		void LongPress (UILongPressGestureRecognizer longPress)
		{
			longPress.State = UIGestureRecognizerState.Began;
		}
	}
}
