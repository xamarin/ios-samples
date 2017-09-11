
using UIKit;
using Foundation;
using PhotosUI;

namespace SamplePhotoApp
{
	[Register ("AssetViewController")]
	partial class AssetViewController
	{
		[Outlet ("imageView")]
		UIImageView ImageView { get; set; }

		[Outlet ("livePhotoView")]
		PHLivePhotoView LivePhotoView { get; set; }

		[Outlet ("animatedImageView")]
		AnimatedImageView AnimatedImageView { get; set; }

		[Outlet ("editButton")]
		UIBarButtonItem EditButton { get; set; }

		[Outlet ("progressView")]
		UIProgressView ProgressView { get; set; }

#if __TVOS__
		[Outlet ("livePhotoPlayButton")]
		UIBarButtonItem LivePhotoPlayButton { get; set; }
#endif

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

		[Action ("playLivePhoto:")]
		partial void PlayLivePhoto (NSObject sender);

		[Action ("play:")]
		partial void Play (NSObject sender);

		[Action ("removeAsset:")]
		partial void RemoveAsset (NSObject sender);

		[Action ("toggleFavorite:")]
		partial void ToggleFavorite (UIBarButtonItem sender);

		void ReleaseDesignerOutlets ()
		{
			if (AnimatedImageView != null)
			{
				AnimatedImageView.Dispose ();
				AnimatedImageView = null;
			}

			if (LivePhotoView != null)
			{
				LivePhotoView.Dispose ();
				LivePhotoView = null;
			}

#if __TVOS__
			if (LivePhotoPlayButton != null)
			{
				LivePhotoPlayButton.Dispose ();
				LivePhotoPlayButton = null;
			}
#endif

			if (ImageView != null)
			{
				ImageView.Dispose ();
				ImageView = null;
			}

			if (Space != null)
			{
				Space.Dispose ();
				Space = null;
			}

			if (TrashButton != null)
			{
				TrashButton.Dispose ();
				TrashButton = null;
			}

			if (PlayButton != null)
			{
				PlayButton.Dispose ();
				PlayButton = null;
			}

			if (EditButton != null)
			{
				EditButton.Dispose ();
				EditButton = null;
			}

			if (ProgressView != null)
			{
				ProgressView.Dispose ();
				ProgressView = null;
			}
		}
	}
}
