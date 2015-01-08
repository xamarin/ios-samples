using System;

using UIKit;
using Foundation;

namespace PhotoHandoff
{
	[Register("Cell")]
	public class Cell : UICollectionViewCell
	{
		UIColor labelColor;

		[Outlet("image")]
		public UIImageView Image { get; set; }

		[Outlet("label")]
		public UILabel Label { get; set; }

		public Cell (IntPtr handle)
			: base(handle)
		{
			SelectedBackgroundView = new CustomCellBackground ();
		}

		public override bool Selected {
			get {
				return base.Selected;
			}
			set {
				base.Selected = value;
				HandleStateFlag (value);
			}
		}

		public override bool Highlighted {
			get {
				return base.Highlighted;
			}
			set {
				base.Highlighted = value;
				HandleStateFlag (value);
			}
		}

		void HandleStateFlag (bool stateFlag)
		{
			if (stateFlag) {
				labelColor = Label.TextColor;
				Label.TextColor = UIColor.Black;
				SetNeedsDisplay ();
			} else if (labelColor != null) {
				Label.TextColor = labelColor;
			}
		}
	}
}

