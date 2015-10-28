using Foundation;
using UIKit;

namespace UICatalog {
	public partial class FocusGuidesViewController : UIViewController {

		UIFocusGuide focusGuide;

		[Export ("initWithCoder:")]
		public FocusGuidesViewController (NSCoder coder): base (coder)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			focusGuide = new UIFocusGuide ();
			View.AddLayoutGuide (focusGuide);

			focusGuide.LeftAnchor.ConstraintEqualTo (TopRightButton.LeftAnchor).Active = true;
			focusGuide.TopAnchor.ConstraintEqualTo (BottomLeftButton.TopAnchor).Active = true;

			// Anchor the width and height of the focus guide.
			focusGuide.WidthAnchor.ConstraintEqualTo (BottomLeftButton.WidthAnchor).Active = true;
			focusGuide.HeightAnchor.ConstraintEqualTo (BottomLeftButton.HeightAnchor).Active = true;
		}

		public override void DidUpdateFocus (UIFocusUpdateContext context, UIFocusAnimationCoordinator coordinator)
		{
			base.DidUpdateFocus (context, coordinator);

			var nextFocusedView = context.NextFocusedView;
			if (nextFocusedView == null)
				return;

			if (nextFocusedView == TopRightButton)
				focusGuide.PreferredFocusedView = BottomLeftButton;
			else if (nextFocusedView == BottomLeftButton)
				focusGuide.PreferredFocusedView = TopRightButton;
			else
				focusGuide.PreferredFocusedView = null;
		}
	}
}
