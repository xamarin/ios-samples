using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.CoreAnimation;
using MonoTouch.UIKit;

namespace CircleLayout
{
	public class CircleLayout : UICollectionViewLayout
	{
		const float ItemSize = 70.0f;

		int cellCount = 20;
		float radius;
		PointF center;

		public CircleLayout ()
		{
		}

		public override void PrepareLayout ()
		{
			base.PrepareLayout ();

			SizeF size = CollectionView.Frame.Size;
			cellCount = CollectionView.NumberOfItemsInSection (0);
			center = new PointF (size.Width / 2.0f, size.Height / 2.0f);
			radius = Math.Min (size.Width, size.Height) / 2.5f;
		}
			
		public override SizeF CollectionViewContentSize {
			get {
				return CollectionView.Frame.Size;
			}
		}

		public override UICollectionViewLayoutAttributes LayoutAttributesForItem (NSIndexPath path)
		{
			UICollectionViewLayoutAttributes attributes = UICollectionViewLayoutAttributes.CreateForCell (path);
			attributes.Size = new SizeF (ItemSize, ItemSize);
			attributes.Center = new PointF (center.X + radius * (float) Math.Cos (2 * path.Row * Math.PI / cellCount),
			                                center.Y + radius * (float) Math.Sin (2 * path.Row * Math.PI / cellCount));
			return attributes;
		}

		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect (RectangleF rect)
		{
			var attributes = new UICollectionViewLayoutAttributes [cellCount];

			for (int i = 0; i < cellCount; i++) {
				NSIndexPath indexPath = NSIndexPath.FromItemSection (i, 0);
				attributes [i] = LayoutAttributesForItem (indexPath);
			}
		
			return attributes;
		}
#if false
		// that's part of the original sample - but never called

		public override UICollectionViewLayoutAttributes InitialLayoutAttributesForInsertedItem (NSIndexPath itemIndexPath)
		{
			var attributes = LayoutAttributesForItem (itemIndexPath);
			attributes.Alpha = 0;
			attributes.Center = new PointF (center.X, center.Y);
				
			return attributes;
		}

		public override UICollectionViewLayoutAttributes FinalLayoutAttributesForDeletedItem (NSIndexPath itemIndexPath)
		{
			var attributes = LayoutAttributesForItem (itemIndexPath);
			attributes.Alpha = 0;
			attributes.Center = new PointF (center.X, center.Y);
			attributes.Transform3D = CATransform3D.MakeScale (0.1f, 0.1f, 1);
				
			return attributes;
		}
#endif
	}
}
