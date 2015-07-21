using System;

using Foundation;
using UIKit;

namespace PhotoProgress {
	public partial class PhotosCollectionViewController : UICollectionViewController, INSCoding {

		Album album;
		public Album Album {
			get {
				return album;
			}
			set {
				album = value;
				CollectionView?.ReloadData ();
			}
		}

		[Export ("initWithCoder:")]
		public PhotosCollectionViewController (NSCoder coder) : base (coder)
		{
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return (album.Photos != null) ? album.Photos.Count : 0;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (PhotoCollectionViewCell)CollectionView.DequeueReusableCell ("Photo", indexPath);
			cell.Photo = album.Photos [indexPath.Row];
			return cell;
		}
	}
}
