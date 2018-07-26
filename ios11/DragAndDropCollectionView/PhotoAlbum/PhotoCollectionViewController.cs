using Foundation;
using System;
using UIKit;
using ObjCRuntime;

namespace PhotoAlbum
{
    /// <summary>
    /// A collection view controller that displays the photos in a photo album. Supports drag and drop and reordering of photos in the album.
    /// </summary>
    public partial class PhotoCollectionViewController : UICollectionViewController, IUICollectionViewDelegateFlowLayout, IUICollectionViewDragDelegate, IUICollectionViewDropDelegate
    {
        public PhotoCollectionViewController(IntPtr handle) : base(handle)
        {
        }

        WeakReference<AlbumTableViewController> albumTableViewController;

        PhotoAlbum album;
        PhotoAlbum Album
        {
            get { return album; }
            set
            {
                album = value;
                Title = album?.Title;
            }
        }

        Photo Photo(NSIndexPath atPath)
        {
            return Album?.Photos[(int)atPath.Item];
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CollectionView.DragDelegate = this;
            CollectionView.DropDelegate = this;

            UpdateRightBarButtonItem();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            StopInsertions();
        }

        bool isPerformingAutomaticInsertions = false;

        void UpdateRightBarButtonItem()
        {
            var startInsertionsBarButtonItem = new UIBarButtonItem("Start Insertions", UIBarButtonItemStyle.Plain, this, new Selector("startInsertions"));
            var stopInsertionsBarButtonItem = new UIBarButtonItem("Stop Insertions", UIBarButtonItemStyle.Done, this, new Selector("stopInsertions"));
            NavigationItem.RightBarButtonItem = isPerformingAutomaticInsertions ? stopInsertionsBarButtonItem : startInsertionsBarButtonItem;
        }

        [Export("startInsertions")]
        public void StartInsertions()
        {
            if (album == null) return;
            PhotoLibrary.SharedInstance.StartAutomaticInsertions(album, this);
            isPerformingAutomaticInsertions = true;
            UpdateRightBarButtonItem();
        }

        [Export("stopInsertions")]
        void StopInsertions()
        {
            PhotoLibrary.SharedInstance.StopAutomaticInsertions();
            isPerformingAutomaticInsertions = false;
            UpdateRightBarButtonItem();
        }

        public void LoadAlbum(PhotoAlbum album, AlbumTableViewController albumTableViewController)
        {
            this.Album = album;
            this.albumTableViewController = new WeakReference<AlbumTableViewController>(albumTableViewController);

            if (albumBeforeDrag == null)
            {
                CollectionView?.ReloadData();
            }

        }

        /// <summary>
        /// Performs updates to the photo library backing store, then loads the latest album and photo values from it.
        /// </summary>
        void UpdatePhotoLibrary(Action<PhotoLibrary> updates)
        {
            updates(PhotoLibrary.SharedInstance);
            ReloadAlbumFromPhotoLibrary();
        }

		/// <summary>
		/// Loads the latest album & photo values from the photo library backing store.
		/// </summary>
		void ReloadAlbumFromPhotoLibrary()
        {
            var albumIdentifier = album?.Identifier;
            if (albumIdentifier != null)
                Album = PhotoLibrary.SharedInstance.Album(albumIdentifier.Value);

            if (albumTableViewController.TryGetTarget(out var ctrl))
            {
                ctrl.ReloadAlbumsFromPhotoLibrary();
                ctrl.UpdateVisibleAlbumCells();
            }
        }

		/// <summary>
		/// Called when an photo has been automatically inserted into the album this collection view is displaying.
		/// </summary>
		public void InsertedItem(int atIndex)
        {
            CollectionView?.PerformBatchUpdates( () =>{
                ReloadAlbumFromPhotoLibrary();
                CollectionView.InsertItems(new NSIndexPath[] { NSIndexPath.FromItemSection(atIndex, 0)});
            },(finished) => {
                
            });
        }

        #region UICollectionViewDataSource & UICollectionViewDelegate

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            if (album == null) return 0;
            return album.Photos.Count;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(PhotoCollectionViewCell.Identifier, indexPath) as PhotoCollectionViewCell;
            cell.Configure(Photo(indexPath));
            return cell;
        }

        #endregion


    }
}