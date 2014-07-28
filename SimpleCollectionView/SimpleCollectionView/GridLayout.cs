using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace SimpleCollectionView
{
    public class GridLayout : UICollectionViewFlowLayout
    {
        public GridLayout ()
        {
        }

        public override bool ShouldInvalidateLayoutForBoundsChange (CGRect newBounds)
        {
            return true;
        }

        public override UICollectionViewLayoutAttributes LayoutAttributesForItem (NSIndexPath path)
        {
            return base.LayoutAttributesForItem (path);
        }

        public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect (CGRect rect)
        {
            return base.LayoutAttributesForElementsInRect (rect);
        }

//        public override CGSize CollectionViewContentSize {
//            get {
//                return CollectionView.Bounds.Size;
//            }       
//        }
    }
}

