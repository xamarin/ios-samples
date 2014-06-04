using System;
using CoreGraphics;
using Foundation;
using UIKit;

using CoreAnimation;

namespace LineLayout
{
	public partial class LineLayout : UICollectionViewFlowLayout
	{
		public const float ITEM_SIZE = 200.0f;
		public const int ACTIVE_DISTANCE = 200;
		public const float ZOOM_FACTOR = 0.3f;
		
		public LineLayout ()
		{	
			ItemSize = new CGSize (ITEM_SIZE, ITEM_SIZE);
			ScrollDirection = UICollectionViewScrollDirection.Horizontal;
			SectionInset = new UIEdgeInsets (200, 0.0f, 200, 0.0f);
			MinimumLineSpacing = 50.0f;		
		}

		public override bool ShouldInvalidateLayoutForBoundsChange (CGRect newBounds)
		{
			return true;
		}		
		
		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect (CGRect rect)
		{
			var array = base.LayoutAttributesForElementsInRect (rect);
			var visibleRect = new CGRect (CollectionView.ContentOffset, CollectionView.Bounds.Size);
						
			foreach (var attributes in array) {
				if (attributes.Frame.IntersectsWith (rect)) {
					nfloat distance = visibleRect.GetMidX () - attributes.Center.X;
					nfloat normalizedDistance = distance / ACTIVE_DISTANCE;
					if (Math.Abs (distance) < ACTIVE_DISTANCE) {
						nfloat zoom = 1 + (nfloat)ZOOM_FACTOR * (1 - (nfloat)Math.Abs (normalizedDistance));
						attributes.Transform3D = CATransform3D.MakeScale (zoom, zoom, 1.0f);
						attributes.ZIndex = 1;											
					}
				}
			}
			return array;
		}
		
		public override CGPoint TargetContentOffset (CGPoint proposedContentOffset, CGPoint scrollingVelocity)
		{
			nfloat offSetAdjustment = nfloat.MaxValue;
			nfloat horizontalCenter = (nfloat)(proposedContentOffset.X + (this.CollectionView.Bounds.Size.Width / 2.0));
			CGRect targetRect = new CGRect (proposedContentOffset.X, 0.0f, this.CollectionView.Bounds.Size.Width, this.CollectionView.Bounds.Size.Height);
			var array = base.LayoutAttributesForElementsInRect (targetRect);
			foreach (var layoutAttributes in array) {
				nfloat itemHorizontalCenter = layoutAttributes.Center.X;
				if (Math.Abs (itemHorizontalCenter - horizontalCenter) < Math.Abs (offSetAdjustment)) {
					offSetAdjustment = itemHorizontalCenter - horizontalCenter;
				}
			}
			return new CGPoint (proposedContentOffset.X + offSetAdjustment, proposedContentOffset.Y);
		}
	}
}