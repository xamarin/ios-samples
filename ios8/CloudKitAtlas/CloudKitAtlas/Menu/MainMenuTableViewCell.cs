using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas
{
	public partial class MainMenuTableViewCell : UITableViewCell
	{
		[Outlet]
		public UIImageView MenuIcon { get; set; }

		[Outlet]
		public UILabel MenuLabel { get; set; }

		[Outlet]
		public UILabel BadgeLabel { get; set; }

		public MainMenuTableViewCell (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainMenuTableViewCell (NSCoder coder)
			: base (coder)
		{
		}
	}
}
