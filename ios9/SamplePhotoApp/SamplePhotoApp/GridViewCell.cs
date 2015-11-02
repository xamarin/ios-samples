using System;

using Foundation;
using UIKit;

namespace SamplePhotoApp {
	public partial class GridViewCell : UICollectionViewCell {

		UIImage thumbnailImage;
		public UIImage ThumbnailImage {
			get {
				return thumbnailImage;
			}
			set {
				thumbnailImage = value;
				ImageView.Image = thumbnailImage;
			}
		}

		UIImage livePhotoBadgeImage;
		public UIImage LivePhotoBadgeImage {
			get {
				return livePhotoBadgeImage;
			}
			set {
				livePhotoBadgeImage = value;
				LivePhotoBadgeImageView.Image = livePhotoBadgeImage;
			}
		}

		public string RepresentedAssetIdentifier { get; set; }

		[Export ("initWithCoder:")]
		public GridViewCell (NSCoder coder) : base (coder)
		{
		}

		public GridViewCell (IntPtr handle) : base (handle)
		{
		}

		public override void PrepareForReuse ()
		{
			base.PrepareForReuse ();
			ImageView.Image = null;
			LivePhotoBadgeImageView.Image = null;
		}
	}
}
