using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace TicTacToe
{
	public class TTTCountView : UIView
	{
		const float LineWidth = 1f;
		const float LineMargin = 4f;
		const int LineGroupCount = 5;
		int count;

		public int Count {
			get { return count; }
			set { 
				if (count != value) {
					RectangleF oldRect = rectForCount (count);
					count = value;
					RectangleF newRect = rectForCount (count);
					RectangleF dirtyRect = RectangleF.Union (oldRect, newRect);
					SetNeedsDisplayInRect (dirtyRect);
				}
			}
		}

		public TTTCountView (RectangleF frame) : base (frame)
		{
			Opaque = false;
		}

		public override void Draw (RectangleF rect)
		{
			TintColor.SetColor ();
			RectangleF bounds = Bounds;
			float x = bounds.Right - LineWidth;

			for (int n = 0; n < Count; n++) {
				x -= LineMargin;
				if ((n + 1) % LineGroupCount == 0) {
					UIBezierPath path = new UIBezierPath ();
					path.MoveTo (
						new PointF (x + 0.5f * LineWidth, 
					             bounds.Top + 0.5f * LineWidth));
					path.AddLineTo (
						new PointF (x + 0.5f * LineWidth + LineGroupCount * LineMargin,
					             bounds.Bottom - 0.5f * LineWidth));
					path.Stroke ();
				} else {
					RectangleF lineRect = bounds;
					lineRect.X = x;
					lineRect.Width = LineWidth;
					UIGraphics.RectFill (lineRect);
				}
			}
		}

		RectangleF rectForCount (int count)
		{
			RectangleF bounds = Bounds;
			RectangleF rect = bounds;
			rect.Width = LineWidth + LineMargin * count;
			rect.X += bounds.Size.Width - rect.Size.Width;
			return rect;
		}

		public override void TintColorDidChange ()
		{
			base.TintColorDidChange ();

			SetNeedsDisplayInRect (rectForCount (Count));
		}

		public override bool IsAccessibilityElement {
			get {
				return true;
			}
		}

		public override string AccessibilityLabel {
			get {
				return Count.ToString ();
			}
		}

		public override UIAccessibilityTrait AccessibilityTraits {
			get {
				return UIAccessibilityTrait.Image;
			}
		}
	}
}

