using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas
{
	public partial class ImageFieldTableViewCell : FormFieldTableViewCell
	{
		public ImageInput ImageInput { get; set; }

		[Outlet]
		public UIImageView AssetView { get; set; }

		public ImageFieldTableViewCell (IntPtr handle)
			: base (handle)
		{
		}
	}
}
