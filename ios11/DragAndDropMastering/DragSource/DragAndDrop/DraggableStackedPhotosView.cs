using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using CoreLocation;
using Foundation;

namespace DragSource
{
	/**
	 A DraggableStackedPhotosView is a StackedPhotosView that
	 supports dragging. When dragging, this view vends a list
	 of drag items constructed from each image.
	 */
	public class DraggableStackedPhotosView : StackedPhotosView, IUIDragInteractionDelegate
	{
		#region Computed Properties
		public UIDragItem[] DragItems {
			get {
				var results = new List<UIDragItem>();
				foreach(UIImageView imageView in ImageViews) {
					var itemProvider = new NSItemProvider(imageView.Image);
					var item = new UIDragItem(itemProvider) {
						LocalObject = imageView
					};
					results.Add(item);
				}
				return results.ToArray();
			}
		}
		#endregion

		#region Constructors
		public DraggableStackedPhotosView()
		{
		}

		public DraggableStackedPhotosView(NSCoder coder) : base(coder)
		{
		}

		public DraggableStackedPhotosView(UIImage[] images) : base(images)
		{
			// Initialize
			UserInteractionEnabled = true;
			AddInteraction(new UIDragInteraction(this));
		}
		#endregion

		#region UIDragInteractionDelegate
		public UIDragItem[] GetItemsForBeginningSession(UIDragInteraction interaction, IUIDragSession session)
		{
			return DragItems;
		}

		[Export("dragInteraction:itemsForAddingToSession:withTouchAtPoint:")]
		public UIDragItem[] GetItemsForAddingToSession(UIDragInteraction interaction, IUIDragSession session, CGPoint point)
		{
			return DragItems;
		}

		[Export("dragInteraction:previewForLiftingItem:session:")]
		public UITargetedDragPreview GetPreviewForLiftingItem(UIDragInteraction interaction, UIDragItem item, IUIDragSession session)
		{
			var imageView = item.LocalObject as UIImageView;
			if (imageView == null) return null;
			return new UITargetedDragPreview(imageView);
		}
		#endregion
	}
}
