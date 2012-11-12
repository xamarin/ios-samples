using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.CoreAnimation;
using MonoTouch.UIKit;

namespace SimpleCollectionView
{
	public class CircleLayout : UICollectionViewLayout
	{
		const float ItemSize = 70.0f;
		int cellCount = 20;
		float radius;
		PointF center;

		static NSString myDecorationViewId = new NSString ("MyDecorationView");

		public CircleLayout ()
		{
			RegisterClassForDecorationView (typeof(MyDecorationView), myDecorationViewId);
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

        public override bool ShouldInvalidateLayoutForBoundsChange (RectangleF newBounds)
        {
            return true;
        }

		public override UICollectionViewLayoutAttributes LayoutAttributesForItem (NSIndexPath path)
		{
			UICollectionViewLayoutAttributes attributes = UICollectionViewLayoutAttributes.CreateForCell (path);
			attributes.Size = new SizeF (ItemSize, ItemSize);
			attributes.Center = new PointF (center.X + radius * (float)Math.Cos (2 * path.Row * Math.PI / cellCount),
			                                center.Y + radius * (float)Math.Sin (2 * path.Row * Math.PI / cellCount));
			return attributes;
		}

		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect (RectangleF rect)
		{
			var attributes = new UICollectionViewLayoutAttributes [cellCount + 1];

			for (int i = 0; i < cellCount; i++) {
				NSIndexPath indexPath = NSIndexPath.FromItemSection (i, 0);
				attributes [i] = LayoutAttributesForItem (indexPath);
			}

            var decorationAttribs = UICollectionViewLayoutAttributes.CreateForDecorationView (myDecorationViewId, NSIndexPath.FromItemSection (0, 0));
            decorationAttribs.Size = CollectionView.Frame.Size;
			decorationAttribs.Center = CollectionView.Center;
			decorationAttribs.ZIndex = -1;
			attributes [cellCount] = decorationAttribs;

			return attributes;
		}

	}
	
	public class MyDecorationView : UICollectionReusableView
	{
		[Export ("initWithFrame:")]
		public MyDecorationView (System.Drawing.RectangleF frame) : base (frame)
		{
			BackgroundColor = UIColor.Red;
		}
	}
}
