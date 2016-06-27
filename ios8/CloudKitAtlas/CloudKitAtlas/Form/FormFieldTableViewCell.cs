using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas
{
	[Register("FormFieldTableViewCell")]
	public class FormFieldTableViewCell : UITableViewCell
	{
		[Outlet]
		public UILabel FieldLabel { get; set; }

		public FormFieldTableViewCell (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public FormFieldTableViewCell (NSCoder coder)
			: base (coder)
		{
		}
	}
}

