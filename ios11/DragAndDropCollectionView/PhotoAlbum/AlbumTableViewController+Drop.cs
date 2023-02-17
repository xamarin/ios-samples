using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace PhotoAlbum {
	public partial class AlbumTableViewController {

		public bool CanHandle (UITableView tableView, IUIDropSession session)
		{
			return session.HasConformingItems (UIImage.ReadableTypeIdentifiers);
		}

		[Export ("tableView:dropSessionDidUpdate:withDestinationIndexPath:")]
		public UITableViewDropProposal DropSessionDidUpdate (UITableView tableView, IUIDropSession session, Foundation.NSIndexPath destinationIndexPath)
		{
			if (tableView.Editing && tableView.HasActiveDrag) {
				return new UITableViewDropProposal (UIDropOperation.Move, UITableViewDropIntent.InsertAtDestinationIndexPath);
			} else if (tableView.Editing || tableView.HasActiveDrag) {
				return new UITableViewDropProposal (UIDropOperation.Forbidden);
			} else if (destinationIndexPath != null && destinationIndexPath.Row < albums.Count) {
				if (session.LocalDragSession != null) {
					return new UITableViewDropProposal (UIDropOperation.Move, UITableViewDropIntent.InsertIntoDestinationIndexPath);
				} else {
					return new UITableViewDropProposal (UIDropOperation.Copy, UITableViewDropIntent.InsertIntoDestinationIndexPath);
				}
			}
			return new UITableViewDropProposal (UIDropOperation.Cancel);
		}

		public void PerformDrop (UITableView tableView, IUITableViewDropCoordinator coordinator)
		{
			var destinationIndexPath = coordinator.DestinationIndexPath;
			if (destinationIndexPath.Row >= albums.Count) return;

			switch (coordinator.Proposal.Operation) {
			case UIDropOperation.Copy:
				LoadAndInsertItems (destinationIndexPath, coordinator);

				break;
			case UIDropOperation.Move:
				break;
			default:
				return;
			}
		}

		void LoadAndInsertItems (NSIndexPath destinationIndexPath, IUITableViewDropCoordinator coordinator)
		{
			var destinationAlbum = Album (destinationIndexPath);

			foreach (var item in coordinator.Items) {
				var dragItem = item.DragItem;
				if (dragItem.ItemProvider.CanLoadObject (typeof (UIImage))) {
					dragItem.ItemProvider.LoadObject<UIImage> ((droppedImage, err) => {
						var image = droppedImage as UIImage;
						if (image != null) {
							var photo = new Photo (image);
							UpdatePhotoLibrary ((photoLibrary) => {
								photoLibrary.Add (photo, destinationAlbum);
							});

							UpdateVisibleAlbumsAndPhotos ();
						}
					});

					var cell = TableView.CellAt (destinationIndexPath) as AlbumTableViewCell;
					if (cell != null) {
						var rect = cell.RectForAlbumThumbnail;
						if (!rect.HasValue) rect = new CGRect (cell.ContentView.Center, CGSize.Empty); //??
						coordinator.DropItemIntoRow (dragItem, destinationIndexPath, rect.Value);
					}
				}
			}
		}

		void MoveItems (NSIndexPath destinationIndexPath, IUITableViewDropCoordinator coordinator)
		{
			var destinationAlbum = Album (destinationIndexPath);

			foreach (var item in coordinator.Items) {
				var dragItem = item.DragItem;

				var photo = dragItem.LocalObject as Photo;
				if (photo is null) return;

				UpdatePhotoLibrary ((photoLibrary) => {
					photoLibrary.MovePhoto (photo, destinationAlbum);
				});

				var cell = TableView.CellAt (destinationIndexPath) as AlbumTableViewCell;
				if (cell != null) {
					var rect = cell.RectForAlbumThumbnail;
					if (!rect.HasValue) rect = new CGRect (cell.ContentView.Center, CGSize.Empty);
					coordinator.DropItemIntoRow (dragItem, destinationIndexPath, rect.Value);
				}
				UpdateVisibleAlbumsAndPhotos ();
			}
		}
	}
}
