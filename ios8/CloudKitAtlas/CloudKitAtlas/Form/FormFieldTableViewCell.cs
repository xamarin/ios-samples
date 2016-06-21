using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas
{
	public class FormFieldTableViewCell : UITableViewCell
	{
		[Outlet]
		public UILabel FieldLabel { get; set; }

		public FormFieldTableViewCell (IntPtr handle)
			: base (handle)
		{
		}
	}
}

