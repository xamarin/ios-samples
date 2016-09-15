// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SamplePhotoApp
{
	[Register ("AssetViewController")]
	partial class AssetViewController
	{
		[Outlet]
		UIKit.UIBarButtonItem EditButton { get; set; }

		[Outlet]
		UIKit.UIImageView ImageView { get; set; }

		[Outlet]
		PhotosUI.PHLivePhotoView LivePhotoView { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem PlayButton { get; set; }

		[Outlet]
		UIKit.UIProgressView ProgressView { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem Space { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem TrashButton { get; set; }

		[Action ("EditButtonClickHandler:")]
		partial void EditButtonClickHandler (Foundation.NSObject sender);

		[Action ("PlayButtonClickHandler:")]
		partial void PlayButtonClickHandler (Foundation.NSObject sender);

		[Action ("TrashButtonClickHandler:")]
		partial void TrashButtonClickHandler (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (LivePhotoView != null) {
				LivePhotoView.Dispose ();
				LivePhotoView = null;
			}

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
