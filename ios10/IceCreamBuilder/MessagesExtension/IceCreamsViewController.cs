using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

namespace MessagesExtension {
	public partial class IceCreamsViewController : UICollectionViewController, IUICollectionViewDataSource {
		enum CollectionViewItem {
			IceCream,
			Create
		}

		public static readonly string StoryboardIdentifier = "IceCreamsViewController";

		public IIceCreamsViewControllerDelegate Builder { get; set; }

		List<KeyValuePair<CollectionViewItem, IceCream>> items;

		public IceCreamsViewController (IntPtr handle) : base (handle)
		{
			var history = IceCreamHistory.Load ();
			var reversedHistory = new List<IceCream> (history);
			reversedHistory.Reverse ();

			items = new List<KeyValuePair <CollectionViewItem, IceCream>> ();
			reversedHistory.ForEach (s => items.Add (
				new KeyValuePair<CollectionViewItem, IceCream> (CollectionViewItem.IceCream, s)
			));
			items.Insert (0, new KeyValuePair<CollectionViewItem, IceCream> (CollectionViewItem.Create, null));
		}

		[Export("collectionView:numberOfItemsInSection:")]
		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return items.Count;
		}

		[Export("collectionView:cellForItemAtIndexPath:")]
		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			UICollectionViewCell cell = null;
			var item = items [indexPath.Row];

			switch (item.Key) {
				case CollectionViewItem.IceCream:
					cell = DequeueIceCreamCell (item.Value, indexPath);
					break;
				case CollectionViewItem.Create:
					cell = DequeueIceCreamOutlineCell (indexPath);
					break;
			}

			return cell;
		}

		[Export("collectionView:didSelectItemAtIndexPath:")]
		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var item = items [indexPath.Row];

			if (item.Key == CollectionViewItem.Create)
				Builder?.DidSelectAdd (this);
		}

		UICollectionViewCell DequeueIceCreamCell (IceCream iceCream, NSIndexPath indexPath)
		{
			var cell = CollectionView.DequeueReusableCell (IceCreamCell.ReuseIdentifier, indexPath) as IceCreamCell;
			if (cell == null)
				throw new Exception ("Unable to dequeue am IceCreamCell");

			cell.RepresentedIceCream = iceCream;

			// Use a placeholder sticker while we fetch the real one from the cache.
			var cache = IceCreamStickerCache.Cache;
			cell.StickerView.Sticker = cache.PlaceholderSticker;

			// Fetch the sticker for the ice cream from the cache.
			cache.GetSticker (iceCream, (sticker) => {
				// If the cell is still showing the same ice cream, update its sticker view.
				if (cell.RepresentedIceCream == iceCream)
					cell.StickerView.Sticker = sticker;
			});

			return cell;
		}

		UICollectionViewCell DequeueIceCreamOutlineCell (NSIndexPath indexPath)
		{
			var cell = CollectionView.DequeueReusableCell (IceCreamOutlineCell.ReuseIdentifier, indexPath) as IceCreamOutlineCell;
			if (cell == null)
				throw new Exception ("Unable to dequeue a IceCreamOutlineCell");

			return cell;
		}
	}
}
