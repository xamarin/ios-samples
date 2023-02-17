using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using Foundation;
using CoreAnimation;
using CoreFoundation;
using ObjCRuntime;

namespace DropDestination {
	/**
	 A DroppableImageGridViewController is a ImageGridViewController
	 that supports drop interactions. When one or more items
	 containing images are dropped within this view, it will insert
	 them as image views to its view hierarchy.
	 */
	public class DroppableImageGridViewController : ImageGridViewController, IUIDropInteractionDelegate {
		#region Computed Properties
		public Dictionary<UIDragItem, (DraggableImageView view, NSProgress progress)> ItemStates { get; set; } = new Dictionary<UIDragItem, (DraggableImageView, NSProgress)> ();
		#endregion

		#region Constructors
		public DroppableImageGridViewController ()
		{
		}

		public DroppableImageGridViewController (NSCoder coder) : base (coder)
		{
		}

		public DroppableImageGridViewController (CGSize cellSize) : base (cellSize)
		{
		}
		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Initialize
			View.UserInteractionEnabled = true;
			View.AddInteraction (new UIDropInteraction (this));
		}

		#region UIDropInteractionDelegate
		[Export ("dropInteraction:sessionDidUpdate:")]
		public UIDropProposal SessionDidUpdate (UIDropInteraction interaction, IUIDropSession session)
		{
			if (session.LocalDragSession == null && session.CanLoadObjects (typeof (UIImage))) {
				return new UIDropProposal (UIDropOperation.Copy);
			}
			return new UIDropProposal (UIDropOperation.Cancel);
		}

		[Export ("dropInteraction:performDrop:")]
		public void PerformDrop (UIDropInteraction interaction, IUIDropSession session)
		{
			session.ProgressIndicatorStyle = UIDropSessionProgressIndicatorStyle.None;

			foreach (UIDragItem item in session.Items) {
				if (item.ItemProvider.CanLoadObject (typeof (UIImage))) {
					var photoView = NextView ();
					var progress = item.ItemProvider.LoadObject<UIImage> ((img, err) => {
						DispatchQueue.MainQueue.DispatchAsync (() => {
							photoView.StopShowingProgress ();
							photoView.Image = img;
						});
					});
					ItemStates.Add (item, (photoView, progress));
				}
			}
		}

		[Export ("dropInteraction:item:willAnimateDropWithAnimator:")]
		public void WillAnimateDrop (UIDropInteraction interaction, UIDragItem item, IUIDragAnimating animator)
		{
			if (!ItemStates.ContainsKey (item)) return;
			var state = ItemStates [item];
			//if (state.view.Image == null) return;

			state.view.Alpha = 0f;
			state.view.StartShowingProgress (state.progress);
			animator.AddCompletion ((obj) => {
				state.view.Alpha = 1f;
			});
		}

		[Export ("dropInteraction:concludeDrop:")]
		public void ConcludeDrop (UIDropInteraction interaction, IUIDropSession session)
		{
			ItemStates.Clear ();
		}

		[Export ("dropInteraction:previewForDroppingItem:withDefault:")]
		public UITargetedDragPreview GetPreviewForDroppingItem (UIDropInteraction interaction, UIDragItem item, UITargetedDragPreview defaultPreview)
		{
			if (!ItemStates.ContainsKey (item)) return null;
			var state = ItemStates [item];
			var previewView = new ProgressSpinnerView (state.view.Frame, state.progress);
			var target = new UIDragPreviewTarget (ContainerView, state.view.Center);
			return new UITargetedDragPreview (previewView, new UIDragPreviewParameters (), target);
		}
		#endregion
	}
}
