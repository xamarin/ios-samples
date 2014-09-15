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

		public override void PrepareForReuse ()
		{
			Label.Text = string.Empty;
			ListColorView.BackgroundColor = UIColor.Clear;
		}
	}
}

