using System;
using System.Collections.Generic;

using CoreGraphics;
using UIKit;

namespace StickyCorners {
	public enum StickyCorner {
		None = 0,
		TopLeft,
		BottomLeft,
		BottomRight,
		TopRight
	}

	public sealed class StickyCornersBehavior : UIDynamicBehavior {

		float cornerInset;
		UIDynamicItemBehavior itemBehavior;
		UICollisionBehavior collisionBehavior;
		IUIDynamicItem item;
		List<UIFieldBehavior> fieldBehaviors;
			
		public StickyCorner CurrentCorner {
			get {
				var bounds = DynamicAnimator.ReferenceView.Bounds;
				var position = item.Center;

				var halfWidth = bounds.Width / 2.0;
				var halfHeight = bounds.Height / 2.0;

				var rect = new CGRect (CGPoint.Empty, new CGSize (halfWidth, halfHeight));

				// Top left.
				if (rect.Contains (position))
					return StickyCorner.TopLeft;
				
				// Bottom left.
				rect.Location = new CGPoint (0, halfHeight);
				if (rect.Contains (position))
					return StickyCorner.BottomLeft;

				// Bottom right.
				rect.Location = new CGPoint (halfWidth, halfHeight);
				if (rect.Contains (position))
					return StickyCorner.BottomRight;

				// Top right.
				rect.Location = new CGPoint (halfWidth, 0);
				return rect.Contains (position) ? StickyCorner.TopRight : StickyCorner.None;
			}
		}

		bool enabled = true;
		public bool Enabled {
			get {
				return enabled;
			}
			set {
				enabled = value;

				if (enabled) {
					foreach (var fieldBehavior in fieldBehaviors)
						fieldBehavior.AddItem (item);

					collisionBehavior.AddItem (item);
					itemBehavior.AddItem (item);
				} else {
					foreach (var fieldBehavior in fieldBehaviors)
						fieldBehavior.RemoveItem (item);

					collisionBehavior.RemoveItem (item);
					itemBehavior.RemoveItem (item);
				}
			}
		}

		public StickyCornersBehavior (IUIDynamicItem stickyCornerItem, float stickyCornerInset)
		{
			item = stickyCornerItem;
			cornerInset = stickyCornerInset;

			fieldBehaviors = new List<UIFieldBehavior> ();

			collisionBehavior = new UICollisionBehavior (item) {
				TranslatesReferenceBoundsIntoBoundary = true
			};

			itemBehavior = new UIDynamicItemBehavior (item) {
				Density = 0.01f,
				Resistance = 10f,
				Friction = 0f,
				AllowsRotation = false
			};

			AddChildBehavior (collisionBehavior);
			AddChildBehavior (itemBehavior);

			for (int i = 0; i <= 3; i++) {
				var fieldBehavior = UIFieldBehavior.CreateSpringField ();
				fieldBehavior.AddItem (item);
				fieldBehaviors.Add (fieldBehavior);
				AddChildBehavior (fieldBehavior);
			}
		}

		public override void WillMoveToAnimator (UIDynamicAnimator targetAnimator)
		{
			base.WillMoveToAnimator (targetAnimator);
			var bounds = targetAnimator.ReferenceView.Bounds;
			UpdateFieldsInBounds (bounds);
		}

		public CGPoint GetPositionForCorner (StickyCorner corner)
		{
			return fieldBehaviors [(int)corner - 1].Position;
		}

		public void UpdateFieldsInBounds (CGRect bounds)
		{
			if (bounds == CGRect.Empty)
				return;

			var itemBounds = item.Bounds;
			var dx = cornerInset + itemBounds.Width / 2f;
			var dy = cornerInset + itemBounds.Height / 2f;

			var w = bounds.Width;
			var h = bounds.Height;

			var topLeft = new CGPoint (dx, dy);
			var bottomLeft = new CGPoint (dx, h - dy);
			var bottomRight = new CGPoint (w - dx, h - dy);
			var topRight = new CGPoint (w - dx, dy);

			var regionRect = new CGRect (dx, dy, w, h);
			UpdateFieldBehaviour (fieldBehaviors [(int)StickyCorner.TopLeft - 1], topLeft, regionRect);
			UpdateFieldBehaviour (fieldBehaviors [(int)StickyCorner.BottomLeft - 1], bottomLeft, regionRect);
			UpdateFieldBehaviour (fieldBehaviors [(int)StickyCorner.BottomRight - 1], bottomRight, regionRect);
			UpdateFieldBehaviour (fieldBehaviors [(int)StickyCorner.TopRight - 1], topRight, regionRect);
		}

		public void AddLinearVelocity (CGPoint velocity)
		{
			itemBehavior.AddLinearVelocityForItem (velocity, item);
		}

		void UpdateFieldBehaviour (UIFieldBehavior fieldBehaviour, CGPoint point, CGRect regionRect)
		{
			fieldBehaviour.Position = new CGPoint (point);
			fieldBehaviour.Region = new UIRegion (new CGSize (regionRect.Width / 2f - regionRect.X, regionRect.Height / 2f - regionRect.Y));
		}
	}
}
