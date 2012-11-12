using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SimpleCollectionView
{
    public class GridLayout : UICollectionViewFlowLayout
    {
        public GridLayout ()
        {
        }

        public override bool ShouldInvalidateLayoutForBoundsChange (RectangleF newBounds)
        {
            return true;
        }

        public override UICollectionViewLayoutAttributes LayoutAttributesForItem (NSIndexPath path)
        {
            return base.LayoutAttributesForItem (path);
        }

        public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect (RectangleF rect)
        {
            return base.LayoutAttributesForElementsInRect (rect);
        }

//        public override SizeF CollectionViewContentSize {
//            get {
//                return CollectionView.Bounds.Size;
//            }       
//        }
    }
}

