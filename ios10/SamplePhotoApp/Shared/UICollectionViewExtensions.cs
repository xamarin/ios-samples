using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;
using CoreGraphics;

namespace SamplePhotoApp
{
	public static class UICollectionViewExtensions
	{
		public static IEnumerable<NSIndexPath> GetIndexPaths (this UICollectionView collectionView, CGRect rect)
		{
			return collectionView.CollectionViewLayout
								 .LayoutAttributesForElementsInRect (rect)
								 .Select (attr => attr.IndexPath);
		}
	}
}

