using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas
{
	public partial class ResultTableViewCell : UITableViewCell
	{
		[Outlet]
		public UILabel ResultLabel { get; set; }

		[Outlet]
		public UILabel ChangeLabel { get; set; }

		[Outlet]
		public NSLayoutConstraint ChangeLabelWidthConstraint { get; set; }

		public ResultTableViewCell (IntPtr handle)
			: base (handle)
		{
		}
	}
}
