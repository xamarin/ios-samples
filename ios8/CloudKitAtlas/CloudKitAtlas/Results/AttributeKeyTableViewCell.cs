using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas
{
	public partial class AttributeKeyTableViewCell : UITableViewCell
	{
		[Outlet]
		public UILabel AttributeKey { get; set; }

		public AttributeKeyTableViewCell (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public AttributeKeyTableViewCell (NSCoder coder)
			: base (coder)
		{
		}
	}
}
