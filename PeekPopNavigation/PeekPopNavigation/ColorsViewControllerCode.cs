using CoreGraphics;
using System;
using UIKit;

namespace PeekPopNavigation {
	/// <summary>
	/// A subclass of `ColorsViewControllerBase` that adds support for Peek and Pop with code by calling
	/// registerForPreviewing(with:sourceView:) and implementing UIViewControllerPreviewingDelegate.
	/// </summary>
	public partial class ColorsViewControllerCode : ColorsViewControllerBase, IUIViewControllerPreviewingDelegate {
		public ColorsViewControllerCode (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			base.RegisterForPreviewingWithDelegate (this, base.TableView);
		}

		#region IUIViewControllerPreviewingDelegate

		public void CommitViewController (IUIViewControllerPreviewing previewingContext, UIViewController viewControllerToCommit)
		{
			// Push the configured view controller onto the navigation stack.
			base.NavigationController.PushViewController (viewControllerToCommit, true);
		}

		public UIViewController GetViewControllerForPreview (IUIViewControllerPreviewing previewingContext, CGPoint location)
		{
			UIViewController result = null;

			// First, get the index path and view for the previewed cell.
			var indexPath = base.TableView.IndexPathForRowAtPoint (location);
			if (indexPath != null) {
				var cell = base.TableView.CellAt (indexPath);
				if (cell != null) {
					// Enable blurring of other UI elements, and a zoom in animation while peeking.
					previewingContext.SourceRect = cell.Frame;

					// Create and configure an instance of the color item view controller to show for the peek.
					var viewController = base.Storyboard.InstantiateViewController ("ColorItemViewController") as ColorItemViewController;

					// Pass over a reference to the ColorData object and the specific ColorItem being viewed.
					viewController.ColorData = ColorData;
					viewController.ColorItem = ColorData.Colors [indexPath.Row];

					result = viewController;
				}
			}

			return result;
		}

		#endregion
	}
}
