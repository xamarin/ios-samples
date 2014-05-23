using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using CoreAnimation;
using UIKit;
using ObjCRuntime;

namespace SimpleCollectionView
{
	public class CircleData {
		public nint CellCount = 20;
		public nfloat Radius;
		public nfloat ItemSize = 70.0f;
		public CGPoint Center;
	}

	public class CircleLayout : UICollectionViewLayout
	{
		Random random = new Random ();
		CircleData data = new CircleData ();

		[Export("layoutAttributesClass")]
		public static new Class LayoutAttributesClass {	
			get {
				return new Class (typeof (CustomCollectionViewLayoutAttributes));
			}
		}

		public override void PrepareLayout ()
		{
			base.PrepareLayout ();

			CGSize size = CollectionView.Frame.Size;

			data.CellCount = CollectionView.NumberOfItemsInSection (0);
			data.Center = new CGPoint (size.Width / 2.0f, size.Height / 2.0f);
			data.Radius = (nfloat)Math.Min (size.Width, size.Height) / 2.5f;	
		}
			
		public override CGSize CollectionViewContentSize {
			get {
				return CollectionView.Frame.Size;
			}
		}

        public override bool ShouldInvalidateLayoutForBoundsChange (CGRect newBounds)
        {
            return true;
        }

		public override UICollectionViewLayoutAttributes LayoutAttributesForItem (NSIndexPath path)
		{
			// If the following line fails to compile (this will happen with Xamarin.iOS before 6.4), you can
			// use the workaround here: https://bugzilla.xamarin.com/show_bug.cgi?id=10877#c6
			var attributes = CustomCollectionViewLayoutAttributes.CreateForCell<CustomCollectionViewLayoutAttributes> (path);

			attributes.Size = new CGSize (data.ItemSize, data.ItemSize);

			// Create a random value around 1.0f
			attributes.Distance = 1.0f + ((float) random.NextDouble () - 0.5f) / 5.0f;
			attributes.Row = (int)path.Row;
			attributes.Data = data;

			return attributes;
		}

		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect (CGRect rect)
		{
			var attributes = new UICollectionViewLayoutAttributes [data.CellCount];

			for (int i = 0; i < data.CellCount; i++) {
				NSIndexPath indexPath = NSIndexPath.FromItemSection (i, 0);
				attributes [i] = LayoutAttributesForItem (indexPath);
			}

			return attributes;
		}
	}

	public class CustomCollectionViewLayoutAttributes : UICollectionViewLayoutAttributes
	{
		public int Row { get; set; }
		public CircleData Data { get; set; }
		// "distance from center" multiplier.	
		public float Distance { get; set; }
		
		public override NSObject Copy ()
		{
			// We must override Copy since iOS will try to clone objects using the NSCopying protocol.
			var obj = base.Copy () as CustomCollectionViewLayoutAttributes;
			
			if (obj != null) {
				obj.Row = Row;
				obj.Data = Data;
				obj.Distance = Distance;
			}
			
			return obj;
		}
	}
}
