using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace PhotoAlbum
{
    public partial class PhotoCollectionViewController
    {
        public UIDragItem[] GetItemsForBeginningDragSession(UICollectionView collectionView, IUIDragSession session, NSIndexPath indexPath)
		{
			var dragItem = DragItem(indexPath);
			return new UIDragItem[] { dragItem };
		}

		[Export("collectionView:itemsForAddingToDragSession:atIndexPath:point:")]
		public UIDragItem[] GetItemsForAddingToDragSession(UICollectionView collectionView, IUIDragSession session, Foundation.NSIndexPath indexPath, CoreGraphics.CGPoint point)
		{
            var dragItem = DragItem(indexPath);
            return new UIDragItem[] { dragItem };
		}

		// Helper method to obtain a drag item for the photo at the index path.
		UIDragItem DragItem(NSIndexPath indexPath)
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

		/// <summary>Stores the album state when the drag begins.</summary>
		PhotoAlbum albumBeforeDrag;


        [Export("collectionView:dragSessionWillBegin:")]
        public void DragSessionWillBegin(UICollectionView collectionView, IUIDragSession session)
        {
            albumBeforeDrag = album;
        }

        [Export("collectionView:dragSessionDidEnd:")]
        public void DragSessionDidEnd(UICollectionView collectionView, IUIDragSession session)
        {
            ReloadAlbumFromPhotoLibrary();
            DeleteItems(collectionView);
            albumBeforeDrag = null;
        }

		/// <summary>
		/// Compares the album state before and after the drag to delete items in the collection view that represent photos moved elsewhere.
		/// </summary>
		void DeleteItems (UICollectionView collectionView)
        {
            var albumAfterDrag = album;
            if (albumBeforeDrag is null || albumAfterDrag is null) return;

            var indexPathsToDelete = new List<NSIndexPath> ();

            for (var i = 0; i < albumBeforeDrag.Photos.Count; i++)
            {
                var photo = albumBeforeDrag.Photos[i];
                if (!albumAfterDrag.Contains(photo))
                {
                    indexPathsToDelete.Add(NSIndexPath.FromItemSection(i, 0));
                }
            }
            if (indexPathsToDelete.Count > 0)
            {
                collectionView.PerformBatchUpdates(() =>{
                    collectionView.DeleteItems(indexPathsToDelete.ToArray());
                }, (finished) => {});
            }
        }

		

	}
}
