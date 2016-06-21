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

		public override void SetSelected (bool selected, bool animated)
		{
			base.SetSelected (selected, animated);
			// Configure the view for the selected state
		}
	}
}
