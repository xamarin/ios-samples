using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using Foundation;
using CoreAnimation;

namespace DropDestination {
	/**
	 A DroppableDeleteView is a DeleteView that invokes
	 its delegate when a drop occurs on it. All local
	 objects that represent views in the list of dropped
	 items are to be removed.
	 */
	public class DroppableDeleteView : DeleteView, IUIDropInteractionDelegate {
		#region Constructors
		public DroppableDeleteView ()
		{
		}

		public DroppableDeleteView (NSCoder coder) : base (coder)
		{
		}

		public DroppableDeleteView (string labelText, DidDeleteViewsDelegate deleteDelegate) : base (labelText)
		{
			// Initialize
			this.DidDeleteViews += deleteDelegate;
			UserInteractionEnabled = true;
			AddInteraction (new UIDropInteraction (this));
		}
		#endregion

		#region Public Methods
		public List<UIView> LocalViewsForSession (IUIDropSession session)
		{
			var views = new List<UIView> ();

			if (session.LocalDragSession != null) {
				foreach (UIDragItem item in session.Items) {
					var view = item.LocalObject as UIView;
					if (view != null) views.Add (view);
				}
			}

			return views;
		}
		#endregion

		#region UIDropInteractionDelegate
		[Export ("dropInteraction:previewForDroppingItem:withDefault:")]
		public UITargetedDragPreview GetPreviewForDroppingItem (UIDropInteraction interaction, UIDragItem item, UITargetedDragPreview defaultPreview)
		{
			var target = new UIDragPreviewTarget (this, IconView.Center, CGAffineTransform.Scale (new CGAffineTransform (), 0.1f, 0.1f));
			return defaultPreview.GetRetargetedPreview (target);
		}

		[Export ("dropInteraction:sessionDidUpdate:")]
		public UIDropProposal SessionDidUpdate (UIDropInteraction interaction, IUIDropSession session)
		{
			if (session.LocalDragSession == null) return new UIDropProposal (UIDropOperation.Forbidden);
			return new UIDropProposal (UIDropOperation.Move);
		}

		[Export ("dropInteraction:performDrop:")]
		public void PerformDrop (UIDropInteraction interaction, IUIDropSession session)
		{
			RaiseDidDeleteViews (LocalViewsForSession (session));
		}

		[Export ("dropInteraction:sessionDidEnter:")]
		public void SessionDidEnter (UIDropInteraction interaction, IUIDropSession session)
		{
			Alpha = 1.0f;
		}

		[Export ("dropInteraction:sessionDidExit:")]
		public void SessionDidExit (UIDropInteraction interaction, IUIDropSession session)
		{
			Alpha = 0.5f;
		}

		[Export ("dropInteraction:sessionDidEnd:")]
		public void SessionDidEnd (UIDropInteraction interaction, IUIDropSession session)
		{
			Alpha = 0.5f;
		}
		#endregion
	}
}
