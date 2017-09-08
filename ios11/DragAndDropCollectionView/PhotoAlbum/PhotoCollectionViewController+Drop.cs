using Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using UIKit;
using System.Linq;
using CoreFoundation;

namespace PhotoAlbum
{
    public partial class PhotoCollectionViewController
    {
        [Export("collectionView:canHandleDropSession:")]
        public bool CanHandleDropSession(UICollectionView collectionView, IUIDropSession session)
        {
			    if (album == null) return false;
			    return session.HasConformingItems(UIImage.ReadableTypeIdentifiers);
		}

        [Export("collectionView:dropSessionDidUpdate:withDestinationIndexPath:")]
        public UICollectionViewDropProposal DropSessionDidUpdate(UICollectionView collectionView, IUIDropSession session, NSIndexPath destinationIndexPath)
        {
            if (session.LocalDragSession != null)
            {
                return new UICollectionViewDropProposal(UIDropOperation.Move, UICollectionViewDropIntent.InsertAtDestinationIndexPath);
            }
            else
            {
                return new UICollectionViewDropProposal(UIDropOperation.Copy, UICollectionViewDropIntent.InsertAtDestinationIndexPath);
            }
        }


		public void PerformDrop (UICollectionView collectionView, IUICollectionViewDropCoordinator coordinator)
		{
            if (album == null) return;

            var destinationIndexPath = coordinator.DestinationIndexPath ?? NSIndexPath.FromRowSection(0, 0);

            switch (coordinator.Proposal.Operation)
            {
                case UIDropOperation.Copy:
					// Receiving items from another app.
					LoadAndInsertItems(destinationIndexPath, coordinator);
                    break;
                case UIDropOperation.Move:
                    var items = coordinator.Items;
                    var hasSourceIndex = items.FirstOrDefault(i => i.SourceIndexPath != null);
                    var containsSourceIndex = items.ToList().Contains(hasSourceIndex); // HACK: not sure about this port of `if items.contains(where: { $0.sourceIndexPath != nil }) `
					if (hasSourceIndex != null)
					{
                        if (items.Length == 1) {
                            // Reordering a single item from this collection view.
							var item = items[0];
                            Reorder(item, destinationIndexPath, coordinator);
                        }
                    }
                    else
                    {
						// Moving items from somewhere else in this app.
						MoveItems(destinationIndexPath, coordinator);    
                    }

                    break;
                default:
                    return;
            }
		}

		/// <summary>
		/// Loads data using the item provider and inserts a new item in the collection view for each item in the drop session, using placeholders while the data loads asynchronously.
		/// </summary>
		void LoadAndInsertItems (NSIndexPath destinationPath, IUICollectionViewDropCoordinator coordinator)
        {
            if (album == null) return;

            foreach (var item in coordinator.Items)
            {
                var dragItem = item.DragItem;
                if (dragItem.ItemProvider.CanLoadObject(typeof(UIImage)))
                {
                    IUICollectionViewDropPlaceholderContext placeholderContext = null;

                    // Start loading the image for this drag item.
                    var progress = dragItem.ItemProvider.LoadObject(typeof(UIImage),
                                (droppedImage, _) =>
                                {
                                    DispatchQueue.MainQueue.DispatchAsync(() =>
                                    {
                                        var image = droppedImage as UIImage;
                                        if (image != null)
                                        {   //The image loaded successfully, commit the insertion to exchange the placeholder for the final cell.
                                            placeholderContext?.CommitInsertion((insertionIndexPath) =>
                                            {   // Update the photo library backing store to insert the new photo, using the insertionIndexPath passed into the closure.
												var photo = new Photo(image);
                                                var insertionIndex = insertionIndexPath.Item;
                                                UpdatePhotoLibrary((photoLibrary) =>
                                                {
                                                    photoLibrary.Insert(photo, album, (int)insertionIndex);
                                                });
                                            });
                                        }
                                        else
                                        {   // The data transfer for this item was canceled or failed, delete the placeholder.
											placeholderContext?.DeletePlaceholder();
                                        }
                                    });
                    });

					// Insert and animate to a placeholder for this item, configuring the placeholder cell to display the progress of the data transfer for this item.
					var placeholder = new UICollectionViewDropPlaceholder(destinationPath, PhotoPlaceholderCollectionViewCell.Identifier);
                    placeholder.CellUpdateHandler = (cell) => {
                        var placeholderCell = cell as PhotoPlaceholderCollectionViewCell;
                        if (cell is null) return;
                        placeholderCell.Configure(progress);
                    };
                    placeholderContext = coordinator.DropItemToPlaceholder(dragItem, placeholder);
                }
            }
        }

        public UIDragPreviewParameters GetDropPreviewParameters (UICollectionView collectionView, NSIndexPath indexPath)
        {
            return PreviewParameters(indexPath);
        }

        UIDragPreviewParameters PreviewParameters (NSIndexPath indexPath)
        {
            var cell = CollectionView.CellForItem(indexPath) as PhotoCollectionViewCell;
            var previewParameters = new UIDragPreviewParameters();
            previewParameters.VisiblePath = UIBezierPath.FromRect(cell.ClippingRectForPhoto);
            return previewParameters;
        }

		/// <summary>
		/// Moves an item (photo) in this collection view from one index path to another index path.
		/// </summary>
		void Reorder (IUICollectionViewDropItem item, NSIndexPath destinationIndexPath, IUICollectionViewDropCoordinator coordinator)
        {
            var sourceIndexPath = item?.SourceIndexPath;
            if (CollectionView is null || album is null || sourceIndexPath is null) return;

			// Perform batch updates to update the photo library backing store and perform the delete & insert on the collection view.
			CollectionView.PerformBatchUpdates(() =>
            {
                UpdatePhotoLibrary( (photoLibrary) => {
                    photoLibrary.MovePhoto(album, (int)sourceIndexPath.Item, (int)destinationIndexPath.Item);
                });
				
				CollectionView.DeleteItems(new NSIndexPath[] { sourceIndexPath});
                CollectionView.InsertItems(new NSIndexPath[] { destinationIndexPath });
            }, (finished) => {
                
            });

            coordinator.DropItemToItem(item.DragItem, destinationIndexPath);
        }

		/// <summary>
		/// Moves one or more photos from a different album into this album by inserting items into this collection view.
		/// </summary>
		void MoveItems (NSIndexPath destinationIndexPath, IUICollectionViewDropCoordinator coordinator)
        {
            if (CollectionView is null || album is null) return;

            var destinationIndex = destinationIndexPath.Item;

            foreach (var item in coordinator.Items)
            {
                var photo = item.DragItem.LocalObject as Photo;
                if (!album.Contains(photo))
                {
                    var insertionIndexPath = NSIndexPath.FromRowSection(destinationIndex, 0);

                    CollectionView.PerformBatchUpdates(() => {
                        UpdatePhotoLibrary((photoLibrary) =>
                        {
                            photoLibrary.MovePhoto (photo, album, (int)destinationIndex);
                        });
                        try
                        {
                            CollectionView.InsertItems(new NSIndexPath[] { insertionIndexPath });
                        } catch (Exception e){
                            Console.WriteLine(e);
                        }
                    }, (finished) => {});


                    coordinator.DropItemToItem(item.DragItem, insertionIndexPath);
                    destinationIndex += 1;
                }
            }
        }
    }
}
