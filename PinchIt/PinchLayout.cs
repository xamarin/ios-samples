using System;
using CoreGraphics;

using UIKit;
using Foundation;
using CoreAnimation;

namespace PinchIt
{
	public class PinchLayout : UICollectionViewFlowLayout
	{
		float pinchedCellScale;
		CGPoint pinchedCellCenter ;
		public NSIndexPath pinchedCellPath { get; set; }

		public void applyPinchToLayoutAttributes (UICollectionViewLayoutAttributes layoutAttributes)
		{
			if (layoutAttributes.IndexPath.Equals (pinchedCellPath)) {
				layoutAttributes.Transform3D = CATransform3D.MakeScale (pinchedCellScale, pinchedCellScale, 1.0f);
				layoutAttributes.Center = pinchedCellCenter;
				layoutAttributes.ZIndex = 1;
			}
		}

		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect (CGRect rect)
		{
			var array = base.LayoutAttributesForElementsInRect (rect);

			foreach (var cellAttributes in array)
				applyPinchToLayoutAttributes (cellAttributes);
			
			return array;
		}

		public override UICollectionViewLayoutAttributes LayoutAttributesForItem (NSIndexPath indexPath)
		{
			var attributes = base.LayoutAttributesForItem(indexPath);

			applyPinchToLayoutAttributes (attributes);

			return attributes;
		}

		public void setPinchedCellScale (float scale)
		{
			pinchedCellScale = scale;
			InvalidateLayout();
		}

		public void setPinchedCellCenter (CGPoint origin)
		{
			pinchedCellCenter = origin;
			InvalidateLayout();
		}
	}
}

