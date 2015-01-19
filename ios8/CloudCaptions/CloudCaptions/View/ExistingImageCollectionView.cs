using System;
using System.Collections.Generic;

using UIKit;
using CoreGraphics;
using CloudKit;
using Foundation;
using CoreFoundation;

namespace CloudCaptions
{
	[Register("ExistingImageCollectionView")]
	public class ExistingImageCollectionView : UICollectionView, INSCoding
	{
		class CollectionViewDataSource : UICollectionViewDataSource
		{
			readonly ExistingImageCollectionView view;

			public CollectionViewDataSource(ExistingImageCollectionView view)
			{
				this.view = view;
			}

			public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
			{
				var cell = (ExistingImageCollectionViewCell)collectionView.DequeueReusableCell (cellReuseIdentifier, indexPath);
				cell = cell ?? new ExistingImageCollectionViewCell ();

				cell.ThumbnailImage.Image = view.imageRecords [indexPath.Row].Thumbnail;
				bool isLoading = indexPath != null && indexPath.Equals (view.currentLoadingIndex);
				cell.SetLoading (isLoading);

				return cell;
			}

			public override nint GetItemsCount (UICollectionView collectionView, nint section)
			{
				return view.imageRecords.Count;
			}
		}

		static readonly NSString cellReuseIdentifier = new NSString("imageCell");

		List<Image> imageRecords;
		DispatchQueue updateArrayQueue;
		NSIndexPath currentLoadingIndex;

		int Count {
			get {
				return imageRecords.Count;
			}
		}

		public ExistingImageCollectionView(IntPtr handle)
			: base(handle)
		{
			Initialize ();
		}

		public ExistingImageCollectionView(NSCoder coder)
			: base(coder)
		{
			Initialize ();
		}

		void Initialize ()
		{
			DataSource = new CollectionViewDataSource (this);
			imageRecords = new List<Image> ();
			updateArrayQueue = new DispatchQueue ("UpdateCollectionViewQueue");
		}

		public void AddImageFromRecord(CKRecord toAdd)
		{
			Image fetchedImage = new Image (toAdd);
			// Ensures that only one object will be added to the imageRecords array at a time
			updateArrayQueue.DispatchAsync (() => {
				imageRecords.Add (fetchedImage);
				InvokeOnMainThread (ReloadData);
			});
		}

		public void SetLoadingFlag(NSIndexPath index, bool isLoading)
		{
			currentLoadingIndex = isLoading ? index : null;
			InvokeOnMainThread (ReloadData);
		}

		public CKRecordID GetRecordId (NSIndexPath index)
		{
			// returns the recordID of the item in imageRecords at the given index
			Image img = imageRecords[index.Row];
			return img.Record.Id;
		}
	}
}

