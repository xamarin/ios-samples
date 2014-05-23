using System;
using System.Collections.Generic;
using CoreGraphics;

using Foundation;
using UIKit;

namespace CollectionViewTransition {

	public class APLStackLayout : UICollectionViewLayout {

		const int stackCount = 5;
		List<float> angles;
		List<UICollectionViewLayoutAttributes> attributesArray;

		public APLStackLayout ()
		{
			angles = new List<float> (stackCount * 10);
		}

		public override void PrepareLayout ()
		{
			CGSize size = CollectionView.Bounds.Size;
			CGPoint center = new CGPoint (size.Width / 2.0f, size.Height / 2.0f);

			int itemCount = (int) CollectionView.NumberOfItemsInSection (0);

			if (attributesArray == null) 
				attributesArray = new List<UICollectionViewLayoutAttributes> (itemCount);

			angles.Clear ();

			float maxAngle = (float) (1 / Math.PI / 3.0f);
			float minAngle = -maxAngle;
			float diff = maxAngle - minAngle;

			angles.Add (0);
			for (int i = 1; i < stackCount * 10; i++) {
				int hash = (int) (i * 2654435761 % 2 ^ 32);
				hash = (int)(hash * 2654435761 % 2 ^ 32);

				float currentAngle = (float) ((hash % 1000) / 1000.0 * diff) + minAngle;
				angles.Add (currentAngle);
			}

			for (int i = 0; i < itemCount; i++) {
				int angleIndex = i % (stackCount * 10);
				float angle = angles [angleIndex];
				var path = NSIndexPath.FromItemSection (i, 0);
				UICollectionViewLayoutAttributes attributes = UICollectionViewLayoutAttributes.CreateForCell (path);
				attributes.Size = new CGSize (150, 200);
				attributes.Center = center;
				attributes.Transform = CGAffineTransform.MakeRotation (angle);
				attributes.Alpha = (i > stackCount) ? 0.0f : 1.0f;
				attributes.ZIndex = (itemCount - i);
				attributesArray.Add (attributes);
			}
		}

		public override void InvalidateLayout ()
		{
			attributesArray.Clear ();
			attributesArray = null;
		}

		public override CGSize CollectionViewContentSize {
			get { return CollectionView.Bounds.Size; }
		}

		public override UICollectionViewLayoutAttributes LayoutAttributesForItem (NSIndexPath indexPath)
		{
			return attributesArray [(int)indexPath.Item];
		}

		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect (CGRect rect)
		{
			return attributesArray.ToArray ();
		}
	}
}