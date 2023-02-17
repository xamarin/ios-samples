using System;

using UIKit;
using Foundation;

namespace RawExpose {
	public partial class AlbumCollectionViewCell : UICollectionViewCell {
		public static new readonly string ReuseIdentifier = "AlbumCollectionViewCell";

		[Outlet ("imageView")]
		public UIImageView ImageView { get; set; }

		[Outlet ("label")]
		public UILabel Label { get; set; }

		public AlbumCollectionViewCell (IntPtr handle)
			: base (handle)
		{
		}
	}
}
