using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreAnimation;

namespace SimpleCollectionView
{
	public class LineLayout : UICollectionViewFlowLayout
	{
		public const float ITEM_SIZE = 200.0f;
		public const int ACTIVE_DISTANCE = 200;
		public const float ZOOM_FACTOR = 0.3f;
		
		public LineLayout ()
		{	
			ItemSize = new SizeF (ITEM_SIZE, ITEM_SIZE);
			ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            SectionInset = new UIEdgeInsets (400,0,400,0);
			MinimumLineSpacing = 50.0f;		
		}

		public override bool ShouldInvalidateLayoutForBoundsChange (RectangleF newBounds)
		{
			return true;
		}
		
		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect (RectangleF rect)
		{
			var array = base.LayoutAttributesForElementsInRect (rect);
            var visibleRect = new RectangleF (CollectionView.ContentOffset, CollectionView.Bounds.Size);
						
			foreach (var attributes in array) {
				if (attributes.Frame.IntersectsWith (rect)) {
					float distance = visibleRect.GetMidX () - attributes.Center.X;
					float normalizedDistance = distance / ACTIVE_DISTANCE;
					if (Math.Abs (distance) < ACTIVE_DISTANCE) {
						float zoom = 1 + ZOOM_FACTOR * (1 - Math.Abs (normalizedDistance));
						attributes.Transform3D = CATransform3D.MakeScale (zoom, zoom, 1.0f);
						attributes.ZIndex = 1;											
					}
				}
			}
			return array;
		}
		
		public override PointF TargetContentOffset (PointF proposedContentOffset, PointF scrollingVelocity)
		{
			float offSetAdjustment = float.MaxValue;
			float horizontalCenter = (float)(proposedContentOffset.X + (this.CollectionView.Bounds.Size.Width / 2.0));
			RectangleF targetRect = new RectangleF (proposedContentOffset.X, 0.0f, this.CollectionView.Bounds.Size.Width, this.CollectionView.Bounds.Size.Height);
			var array = base.LayoutAttributesForElementsInRect (targetRect);
			foreach (var layoutAttributes in array) {
				float itemHorizontalCenter = layoutAttributes.Center.X;
				if (Math.Abs (itemHorizontalCenter - horizontalCenter) < Math.Abs (offSetAdjustment)) {
					offSetAdjustment = itemHorizontalCenter - horizontalCenter;
				}
			}
            return new PointF (proposedContentOffset.X + offSetAdjustment, proposedContentOffset.Y);
		}

	}
}

