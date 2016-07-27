using System;

using Foundation;
using UIKit;

namespace RawExpose
{
	public partial class LightboxCollectionViewCell : UICollectionViewCell
	{
		public static new readonly string ReuseIdentifier = "LightboxCollectionViewCell";

		[Outlet ("imageView")]
		public UIImageView ImageView { get; set; }

		public LightboxCollectionViewCell (IntPtr handle)
			: base (handle)
		{
		}
	}
}
