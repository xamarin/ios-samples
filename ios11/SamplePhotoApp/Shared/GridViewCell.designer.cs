using Foundation;

using UIKit;

namespace SamplePhotoApp
{
	[Register ("GridViewCell")]
	partial class GridViewCell
	{
		[Outlet]
		UIImageView ImageView { get; set; }

		[Outlet]
		UIImageView LivePhotoBadgeImageView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ImageView != null)
			{
				ImageView.Dispose ();
				ImageView = null;
			}

			if (LivePhotoBadgeImageView != null)
			{
				LivePhotoBadgeImageView.Dispose ();
				LivePhotoBadgeImageView = null;
			}
		}
	}
}
