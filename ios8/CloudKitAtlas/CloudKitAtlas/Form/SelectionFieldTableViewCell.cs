using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas
{
	public partial class SelectionFieldTableViewCell : FormFieldTableViewCell
	{
		[Outlet]
		public UILabel SelectedItemLabel { get; set; }

		public SelectionInput SelectionInput { get; set; }

		public SelectionFieldTableViewCell (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public SelectionFieldTableViewCell (NSCoder coder)
			: base (coder)
		{
		}
	}
}
