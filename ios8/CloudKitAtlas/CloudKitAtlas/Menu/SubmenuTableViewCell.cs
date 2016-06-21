using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas
{
	public partial class SubmenuTableViewCell : UITableViewCell
	{
		[Outlet]
		public UILabel SubmenuLabel { get; set; }

		public SubmenuTableViewCell (IntPtr handle)
			: base (handle)
		{
		}
	}
}
