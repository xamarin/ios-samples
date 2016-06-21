using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas
{
	public partial class BooleanFieldTableViewCell : FormFieldTableViewCell
	{
		[Outlet]
		public UISwitch BooleanField { get; set; }

		public BooleanInput BooleanInput { get; set; }

		public BooleanFieldTableViewCell (IntPtr handle)
			: base (handle)
		{
		}
	}
}
