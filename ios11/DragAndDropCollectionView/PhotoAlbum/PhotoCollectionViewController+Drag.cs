using System;
using Foundation;
using UIKit;

namespace PhotoAlbum
{
    public partial class PhotoCollectionViewController
    {
		// Stores the album state when the drag begins.
		PhotoAlbum albumBeforeDrag;


		public UIDragItem[] GetItemsForBeginningDragSession(UICollectionView collectionView, IUIDragSession session, NSIndexPath indexPath)
		{
            var dragItem = DragItem(indexPath);
            return new UIDragItem[] { dragItem };
		}

        [Export("collectionView:itemsForAddingToDragSession:atIndexPath:point:")]
        public UIDragItem[] GetItemsForAddingToDragSession(UICollectionView collectionView, IUIDragSession session, Foundation.NSIndexPath indexPath, CoreGraphics.CGPoint point)
        {
            throw new System.NotImplementedException();
        }

		// Helper method to obtain a drag item for the photo at the index path.
        UIDragItem DragItem (NSIndexPath indexPath)
        {
            var photo = Photo(indexPath);
            var itemProvider = photo.ItemProvider;
            var dragItem = new UIDragItem(itemProvider);
            dragItem.LocalObject = photo;
            return dragItem;
        }

        [Export("collectionView:dragPreviewParametersForItemAtIndexPath:")]
        public UIDragPreviewParameters GetDragPreviewParameters(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return PreviewParameters(indexPath);
        }

	}
}
