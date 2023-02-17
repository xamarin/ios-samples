
namespace ExceptionalAccessibility {
	using System;
	using UIKit;

	/// <summary>
	/// This modal view displays a gallery of images for the dog, and has a dark transparent background so that
	/// you can still see the content of the view below it.It is meant to be a full screen modal view,
	/// but it creates problems for VoiceOver because it isn't a view controller that is presented modally.
	/// Because it's simply a view that's added on top of everything else, and because it has a transparent background so the
	/// views behind are still visible, VoiceOver doesn't inherently know that the views behind it should no longer
	/// be accessible.So the user can still swipe to access those views behind it while this view is presented.
	/// This creates a confusing, bad experience, so we override `accessibilityViewIsModal` to indicate
	/// this view and its contents are the only thing on screen VoiceOver should currently care about.
	/// </summary>
	public partial class DogModalView : UIView {
		public DogModalView (IntPtr handle) : base (handle) { }

		public override bool AccessibilityViewIsModal { get => true; set { /* ignore */ _ = value; } }

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			this.closeButton.Layer.CornerRadius = this.closeButton.Bounds.Width / 2f;
			this.closeButton.Layer.BorderWidth = 1f;
			this.closeButton.Layer.BorderColor = UIColor.LightGray.CGColor;
		}

		partial void closeButtonTapped (UIButton sender)
		{
			UIView.AnimateNotify (0.25d, () => {
				this.Alpha = 0f;
			}, (bool finished) => {
				if (finished) {
					this.RemoveFromSuperview ();
				}
			});
		}
	}
}
