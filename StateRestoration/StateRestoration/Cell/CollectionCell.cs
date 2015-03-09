using System;
using CoreGraphics;

using Foundation;
using UIKit;

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

		void UpdateLabelColor ()
		{
			labelColor = Label.TextColor;
			Label.TextColor = UIColor.Black;
			SetNeedsDisplay ();
		}

		void ResetLabelColor ()
		{
			if (labelColor != null)
				Label.TextColor = labelColor;
		}
	}
}
