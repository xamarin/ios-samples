using System;

using UIKit;
using Foundation;

namespace Emporium
{
	[Register ("ProductCell")]
	public class ProductCell : UICollectionViewCell
	{
		[Outlet ("imageView")]
		public UIImageView ImageView { get; set; }

		[Outlet ("titleLabel")]
		public UILabel TitleLabel { get; set; }

		[Outlet ("priceLabel")]
		public UILabel PriceLabel { get; set; }

		[Outlet ("subtitleLabel")]
		public UILabel SubtitleLabel { get; set; }

		public ProductCell (IntPtr handle)
			: base (handle)
		{
		}
	}
}