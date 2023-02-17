using System;
using System.Linq;

using CoreGraphics;
using Foundation;
using UIKit;

namespace MessagesExtension {
	[Register ("IceCreamPartCollectionViewLayout")]
	public class IceCreamPartCollectionViewLayout : UICollectionViewFlowLayout {

		public IceCreamPartCollectionViewLayout (IntPtr handle) : base (handle)
		{
		}

		public override CGPoint TargetContentOffset (CGPoint proposedContentOffset, CGPoint scrollingVelocity)
		{
			if (CollectionView == null)
				return proposedContentOffset;

			var halfWidth = CollectionView.Bounds.Width / 2f;
			var targetIndexPath = IndexPathForVisibleItemClosest (proposedContentOffset.X + halfWidth);
			if (targetIndexPath == null)
				return proposedContentOffset;

			var itemAttributes = LayoutAttributesForItem (targetIndexPath);
			if (itemAttributes == null)
				return proposedContentOffset;

			return new CGPoint (itemAttributes.Center.X - halfWidth, proposedContentOffset.Y);
		}

		public NSIndexPath IndexPathForVisibleItemClosest (nfloat offset)
		{
			if (CollectionView == null)
				return null;

			var layoutAttributes = LayoutAttributesForElementsInRect (CollectionView.Bounds);

			if (layoutAttributes.Length == 0)
				return null;

			var closestAttributes = layoutAttributes.OrderBy (a => Math.Abs (a.Center.X - offset)).FirstOrDefault ();
			return closestAttributes?.IndexPath;
		}
	}
}

