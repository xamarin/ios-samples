using System;
using UIKit;
using Foundation;

namespace CloudCaptions
{
	[Register("ExistingImageCollectionViewCell")]
	public class ExistingImageCollectionViewCell : UICollectionViewCell
	{
		[Outlet("thumbnailImage")]
		public UIImageView ThumbnailImage { get; set; }

		[Outlet("loadingIndicator")]
		UIActivityIndicatorView LoadingIndicator { get; set; }

		UIVisualEffectView blurSubview;

		public ExistingImageCollectionViewCell (IntPtr handle)
			: base (handle)
		{

		}

		public ExistingImageCollectionViewCell ()
		{
		}

		public void SetLoading (bool loading)
		{
			if(loading) {
				LoadingIndicator.StartAnimating ();

				UIBlurEffect blurEffect = UIBlurEffect.FromStyle (UIBlurEffectStyle.Light);
				blurSubview = new UIVisualEffectView (blurEffect);
				blurSubview.Frame = ThumbnailImage.Frame;
				ThumbnailImage.AddSubview (blurSubview);
			} else {
				if(blurSubview != null) {
					blurSubview.RemoveFromSuperview ();
					blurSubview = null;
				}
				LoadingIndicator.StopAnimating ();
			}
		}
	}
}

