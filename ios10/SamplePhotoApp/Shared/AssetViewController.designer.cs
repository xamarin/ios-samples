using System.CodeDom.Compiler;

using UIKit;
using Foundation;
#if __IOS__
using PhotosUI;
#endif

namespace SamplePhotoApp
{
	[Register ("AssetViewController")]
	partial class AssetViewController
	{
		[Outlet ("imageView")]
		UIImageView ImageView { get; set; }

		#if __IOS__
		[Outlet ("livePhotoView")]
		PHLivePhotoView LivePhotoView { get; set; }
		#endif

		[Outlet ("editButton")]
		UIBarButtonItem EditButton { get; set; }

		[Outlet ("progressView")]
		UIProgressView ProgressView { get; set; }

		[Outlet ("playButton")]
		UIBarButtonItem PlayButton { get; set; }

		[Outlet ("space")]
		UIBarButtonItem Space { get; set; }

		[Outlet ("trashButton")]
		UIBarButtonItem TrashButton { get; set; }

		[Outlet ("favoriteButton")]
		UIBarButtonItem FavoriteButton { get; set; }

		[Action ("editAsset:")]
		partial void EditAsset (UIBarButtonItem sender);

		[Action ("removeAsset:")]
		partial void RemoveAsset (NSObject sender);

		[Action ("play:")]
		partial void Play (NSObject sender);

		[Action ("toggleFavorite:")]
		partial void ToggleFavorite (UIBarButtonItem sender);

		void ReleaseDesignerOutlets ()
		{
#if __IOS__
			if (LivePhotoView != null) {
				LivePhotoView.Dispose ();
				LivePhotoView = null;
			}
#endif

			if (ImageView != null) {
				ImageView.Dispose ();
				ImageView = null;
			}

			if (Space != null) {
				Space.Dispose ();
				Space = null;
			}

			if (TrashButton != null) {
				TrashButton.Dispose ();
				TrashButton = null;
			}

			if (PlayButton != null) {
				PlayButton.Dispose ();
				PlayButton = null;
			}

			if (EditButton != null) {
				EditButton.Dispose ();
				EditButton = null;
			}

			if (ProgressView != null) {
				ProgressView.Dispose ();
				ProgressView = null;
			}
		}
	}
}
