using Foundation;
using System;
using UIKit;
using System.Collections.Generic;

namespace PhotoAlbum {
	public partial class AlbumTableViewController : UITableViewController, IUITableViewDragDelegate, IUITableViewDropDelegate {
		public AlbumTableViewController () { }
		public AlbumTableViewController (IntPtr handle) : base (handle)
		{
		}

		List<PhotoAlbum> albums = PhotoLibrary.SharedInstance.Albums;

		PhotoAlbum Album (NSIndexPath atPath)
		{
			return albums [atPath.Row];
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			TableView.DataSource = this;
			TableView.Delegate = this;

			TableView.DragDelegate = this;
			TableView.DropDelegate = this;

			NavigationItem.RightBarButtonItem = EditButtonItem;
			//TODO: TableView.IsSpringLoaded = true;
		}





		public override bool Editing {
			get {
				return base.Editing;
			}
			set {
				//TODO: TableView.SpringLoaded = !value;
				base.Editing = value;
			}
		}

		/// <summary>
		/// Performs updates to the photo library backing store, then loads the latest album values from it.
		/// </summary>
		void UpdatePhotoLibrary (Action<PhotoLibrary> updates)
		{
			updates (PhotoLibrary.SharedInstance);
			ReloadAlbumsFromPhotoLibrary ();
		}

		/// <summary>
		/// Loads the latest album values from the photo library backing store.
		/// </summary>
		public void ReloadAlbumsFromPhotoLibrary ()
		{
			albums = PhotoLibrary.SharedInstance.Albums;
		}

		/// <summary>
		/// Updates the visible cells to display the latest values for albums.
		/// </summary>
		public void UpdateVisibleAlbumCells ()
		{
			var visibleIndexPaths = TableView.IndexPathsForVisibleRows;
			if (visibleIndexPaths is null) return;

			foreach (var indexPath in visibleIndexPaths) {
				var cell = TableView.CellAt (indexPath) as AlbumTableViewCell;
				if (cell != null) {
					cell.Configure (Album (indexPath));
					cell.SetNeedsLayout ();
				}
			}
		}

		void UpdateVisibleAlbumsAndPhotos ()
		{
			UpdateVisibleAlbumCells ();
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			var selectedIndexPath = TableView.IndexPathForSelectedRow;
			var navigationController = segue.DestinationViewController as UINavigationController;
			var photosViewController = navigationController.TopViewController as PhotoCollectionViewController;

			if (selectedIndexPath is null || navigationController is null || photosViewController is null) return;

			// Load the selected album in the collection view to display its photos.
			var album = Album (selectedIndexPath);
			photosViewController.LoadAlbum (album, this);
		}


		#region MARK UITableViewDataSource & UITableViewDelegate

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return albums.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (AlbumTableViewCell.Identifier, indexPath) as AlbumTableViewCell;
			cell.Configure (albums [indexPath.Row]);
			return cell;
		}

		public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return UITableViewCellEditingStyle.None;
		}

		public override bool ShouldIndentWhileEditing (UITableView tableView, NSIndexPath indexPath)
		{
			return false;
		}

		/// <summary>
		/// Implement reordering by simply updating the photo library backing store.
		/// </summary>
		public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		{
			UpdatePhotoLibrary ((photoLibrary) => {
				photoLibrary.MoveAlbum (sourceIndexPath.Row, destinationIndexPath.Row);
			});
		}
		#endregion

	}
}
