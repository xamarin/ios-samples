using System;

using UIKit;
using Foundation;

namespace Lister
{
	[Register("ListCell")]
	public class ListCell : UITableViewCell
	{
		[Outlet("label")]
		public UILabel Label { get; set; }

		[Outlet("listColorView")]
		public UIView ListColorView { get; set; }

		public ListCell(IntPtr handle)
			: base(handle)
		{
		}

		public override void SetHighlighted (bool highlighted, bool animated)
		{
			UIColor color = ListColorView.BackgroundColor;
			base.SetHighlighted (highlighted, animated);

			// Reset the background color for the list color; the default implementation makes it clear.
			ListColorView.BackgroundColor = color;
		}

		public override void SetSelected (bool selected, bool animated)
		{
			UIColor color = ListColorView.BackgroundColor;
			base.SetSelected (selected, animated);

			// Reset the background color for the list color; the default implementation makes it clear.
			ListColorView.BackgroundColor = color;

			// Ensure that tapping on a selected cell doesn't re-trigger the display of the document.
			UserInteractionEnabled = !selected;
		}

		public override void PrepareForReuse ()
		{
			Label.Text = string.Empty;
			ListColorView.BackgroundColor = UIColor.Clear;
		}
	}
}

