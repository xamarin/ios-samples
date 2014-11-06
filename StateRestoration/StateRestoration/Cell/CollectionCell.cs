using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace StateRestoration
{
	public partial class CollectionCell : UICollectionViewCell
	{
		UIColor labelColor;

		public override bool Selected {
			get {
				return base.Selected;
			}
			set {
				base.Selected = value;
				if (value)
					UpdateLabelColor ();
				else
					ResetLabelColor ();
			}
		}

		public override bool Highlighted {
			get {
				return base.Highlighted;
			}
			set {
				base.Highlighted = value;
				if (value)
					UpdateLabelColor ();
				else
					ResetLabelColor ();
			}
		}

		public CollectionCell (IntPtr handle)
			: base (handle)
		{
			SelectedBackgroundView = CustomCellBackground.CreateCustomCellBackground (Frame);
		}

		void UpdateLabelColor()
		{
			labelColor = Label.TextColor;
			Label.TextColor = UIColor.Black;
			SetNeedsDisplay ();
		}

		void ResetLabelColor()
		{
			if (labelColor != null)
				Label.TextColor = labelColor;
		}
	}
}