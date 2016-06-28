using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas
{
	public partial class ImageTableViewCell : NestedAttributeTableViewCell
	{
		[Outlet]
		public UIImageView AssetImage { get; set; }

		public ImageTableViewCell (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public ImageTableViewCell (NSCoder coder)
			: base (coder)
		{
		}
	}
}
