using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CollectionViewTransition {

	public class APLTransitionLayout : UICollectionViewTransitionLayout {

		public APLTransitionLayout(UICollectionViewLayout currentLayout, UICollectionViewLayout newLayout) :
			base (currentLayout, newLayout)
		{
		}

		public UIOffset Offset { get; private set; }

		public void SetTransitionProgress (float transitionProgress)
		{
			base.TransitionProgress = transitionProgress;
			float offsetH = GetValueForAnimatedKey ("offsetH");
			float offsetV = GetValueForAnimatedKey ("offsetV");
			Offset = new UIOffset (offsetH, offsetV);
		}

		public void SetOffset (UIOffset offset)
		{
			UpdateValue (offset.Horizontal, "offsetH");
			UpdateValue (offset.Vertical, "offsetV");
			Offset = offset;
		}

		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect (RectangleF rect)
		{
			UICollectionViewLayoutAttributes[] attributes = base.LayoutAttributesForElementsInRect (rect);
			foreach (var attribute in attributes) {
				PointF center = attribute.Center;
				attribute.Center = new PointF (center.X + Offset.Horizontal, center.Y + Offset.Vertical);
			}
			return attributes;
		}

		public override UICollectionViewLayoutAttributes LayoutAttributesForItem (NSIndexPath indexPath)
		{
			UICollectionViewLayoutAttributes attributes = base.LayoutAttributesForItem (indexPath);
			PointF center = attributes.Center;
			attributes.Center = new PointF (center.X + Offset.Horizontal, center.Y + Offset.Vertical);
			return attributes;
		}
	}
}