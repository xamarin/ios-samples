using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas {
	public partial class AttributeTableViewCell : UITableViewCell {
		[Outlet]
		public UILabel AttributeKey { get; set; }

		[Outlet]
		public UILabel AttributeValue { get; set; }

		public AttributeTableViewCell (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public AttributeTableViewCell (NSCoder coder)
			: base (coder)
		{
		}
	}
}
