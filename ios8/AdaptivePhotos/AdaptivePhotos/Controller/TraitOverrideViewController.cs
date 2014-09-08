using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;

namespace AdaptivePhotos
{
	public class TraitOverrideViewController : CustomViewController
	{
		UITraitCollection forcedTraitCollection = new UITraitCollection ();
		UIViewController viewController;

		public UITraitCollection ForcedTraitCollection {
			get {
				return forcedTraitCollection;
			}

			set {
				if (value != forcedTraitCollection) {
					forcedTraitCollection = value;
					UpdateForcedTraitCollection ();
				}
			}
		}

		public override bool ShouldAutomaticallyForwardAppearanceMethods {
			get {
				return true;
			}
		}

		public override bool ShouldAutomaticallyForwardRotationMethods {
			get {
				return true;
			}
		}

		public UIViewController ViewController {
			set {
				if (viewController != value) {
					if (viewController != null) {
						viewController.WillMoveToParentViewController (null);
						SetOverrideTraitCollection (null, viewController);
						viewController.View.RemoveFromSuperview ();
						viewController.RemoveFromParentViewController ();
					}

					if (value != null)
						AddChildViewController (value);

					viewController = value;

					if (viewController != null) {
						var view = viewController.View;
						view.TranslatesAutoresizingMaskIntoConstraints = false;
						View.Add (view);
						NSDictionary views = NSDictionary.FromObjectAndKey (view, new NSString ("view"));
						View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("|[view]|", 
							NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views));
						View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("V:|[view]|", 
							NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views));
						viewController.DidMoveToParentViewController (this);

						UpdateForcedTraitCollection ();
					}
				}
			}
		}

		public override void ViewWillTransitionToSize (CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
		{
			if (toSize.Width > 320.0f) {
				ForcedTraitCollection = UITraitCollection.FromHorizontalSizeClass (UIUserInterfaceSizeClass.Regular);
			} else {
				ForcedTraitCollection = new UITraitCollection ();
			}

			base.ViewWillTransitionToSize (toSize, coordinator);
		}

		public void UpdateForcedTraitCollection ()
		{
			SetOverrideTraitCollection (forcedTraitCollection, viewController);
		}
	}
}

