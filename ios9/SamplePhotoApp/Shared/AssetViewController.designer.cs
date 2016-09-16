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
		[Outlet]
		UIImageView ImageView { get; set; }

		#if __IOS__
		[Outlet]
		PHLivePhotoView LivePhotoView { get; set; }
		#endif

		[Outlet]
		UIBarButtonItem EditButton { get; set; }

		[Outlet]
		UIProgressView ProgressView { get; set; }

		[Outlet]
		UIBarButtonItem PlayButton { get; set; }

		[Outlet]
		UIBarButtonItem Space { get; set; }

		[Outlet]
		UIBarButtonItem TrashButton { get; set; }

		[Outlet]
		UIBarButtonItem FavoriteButton { get; set; }

		// TODO: check selector
		[Action ("EditButtonClickHandler:")]
		partial void EditButtonClickHandler (UIBarButtonItem sender);

		[Action ("PlayButtonClickHandler:")]
		partial void PlayButtonClickHandler (NSObject sender);

		[Action ("TrashButtonClickHandler:")]
		partial void RemoveAsset (NSObject sender);

		// TODO: check selector
		[Action ("play:")]
		partial void Play (NSObject sender);

		// TODO: check selector
		[Action ("ToggleFavorite:")]
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
