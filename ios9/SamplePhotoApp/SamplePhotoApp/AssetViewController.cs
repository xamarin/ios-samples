using System;

using AVFoundation;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using CoreImage;
using Foundation;
using Photos;
using PhotosUI;
using UIKit;

namespace SamplePhotoApp {
	public partial class AssetViewController : UIViewController, IPHPhotoLibraryChangeObserver, IPHLivePhotoViewDelegate {
		const string AdjustmentFormatIdentifier = "com.xamarin.SamplePhotosApp";

		AVPlayerLayer playerLayer;
		bool playingHint;

		public PHAssetCollection AssetCollection { get; set; }

		public PHAsset Asset { get; set; }

		CGSize TargetSize {
			get {
				nfloat scale = UIScreen.MainScreen.Scale;
				var targetSize = new CGSize (ImageView.Bounds.Width * scale, ImageView.Bounds.Height * scale);
				return targetSize;
			}
		}

		[Export ("initWithCoder:")]
		public AssetViewController (NSCoder coder) : base (coder)
		{
		}

		public AssetViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			LivePhotoView.Delegate = this;
			PHPhotoLibrary.SharedPhotoLibrary.RegisterChangeObserver (this);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Set the appropriate toolbarItems based on the mediaType of the asset.
			if (Asset.MediaType == PHAssetMediaType.Video)
				ShowPlaybackToolbar ();
			else
				ShowStaticToolbar ();

			// Enable the edit button if the asset can be edited.
			bool isEditable = Asset.CanPerformEditOperation (PHAssetEditOperation.Properties) ||
				Asset.CanPerformEditOperation (PHAssetEditOperation.Content);
			EditButton.Enabled = isEditable;

			// Enable the trash button if the asset can be deleted.
			bool isTrashable = AssetCollection != null ?
				AssetCollection.CanPerformEditOperation (PHCollectionEditOperation.RemoveContent) :
				Asset.CanPerformEditOperation (PHAssetEditOperation.Delete);

			TrashButton.Enabled = isTrashable;
			UpdateImage ();
			View.LayoutIfNeeded ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			RemovePlayerLayer ();
		}

		protected override void Dispose (bool disposing)
		{
			PHPhotoLibrary.SharedPhotoLibrary.UnregisterChangeObserver (this);
			base.Dispose (disposing);
		}

		public void PhotoLibraryDidChange (PHChange changeInstance)
		{
			// Call might come on any background queue. Re-dispatch to the main queue to handle it.
			DispatchQueue.MainQueue.DispatchAsync (() => {
				// Check if there are changes to the asset we're displaying.
				PHObjectChangeDetails changeDetails = changeInstance.GetObjectChangeDetails (Asset);
				if (changeDetails == null)
					return;

				// Get the updated asset.
				// TODO check return type. Catch! ObjectAfterChanges should be PHObject instead of NSObject https://bugzilla.xamarin.com/show_bug.cgi?id=35540
				Asset = (PHAsset)changeDetails.ObjectAfterChanges;

				// If the asset's content changed, update the image and stop any video playback.
				if (changeDetails.AssetContentChanged) {
					UpdateImage ();
					RemovePlayerLayer ();
				}
			});
		}

		partial void EditButtonClickHandler (NSObject sender)
		{
			// Use a UIAlertController to display the editing options to the user.
			var alertController = UIAlertController.Create (null, null, UIAlertControllerStyle.ActionSheet);
			alertController.ModalPresentationStyle = UIModalPresentationStyle.Popover;
			if (alertController.PopoverPresentationController != null) {
				alertController.PopoverPresentationController.BarButtonItem = (UIBarButtonItem)sender;
				alertController.PopoverPresentationController.PermittedArrowDirections = UIPopoverArrowDirection.Up;
			}

			// Add an action to dismiss the UIAlertController.
			alertController.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));

			// If PHAsset supports edit operations, allow the user to toggle its favorite status.
			if (Asset.CanPerformEditOperation (PHAssetEditOperation.Properties)) {
				var favoriteActionTitle = Asset.Favorite ? "Unfavorite" : "Favorite";
				alertController.AddAction (UIAlertAction.Create (favoriteActionTitle, UIAlertActionStyle.Default, actions =>
					ToggleFavoriteState ()
				));
			}

			// Only allow editing if the PHAsset supports edit operations and it is not a Live Photo.
			if (Asset.CanPerformEditOperation (PHAssetEditOperation.Content) && Asset.MediaSubtypes != PHAssetMediaSubtype.PhotoLive) {
				// Allow filters to be applied if the PHAsset is an image.
				if (Asset.MediaType == PHAssetMediaType.Image) {
					alertController.AddAction (UIAlertAction.Create ("Sepia", UIAlertActionStyle.Default, action =>
						ApplyFilter (new CISepiaTone())
					));

					alertController.AddAction (UIAlertAction.Create ("Chrome", UIAlertActionStyle.Default, action =>
						ApplyFilter (new CIPhotoEffectChrome ())
					));
				}

				// Add actions to revert any edits that have been made to the PHAsset.
				alertController.AddAction (UIAlertAction.Create ("Revert", UIAlertActionStyle.Default, action =>
					RevertToOriginal ()
				));
			}

			// Present the UIAlertController.
			PresentViewController (alertController, true, null);
		}

		void RevertToOriginal ()
		{
			PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
				var request = PHAssetChangeRequest.ChangeRequest (Asset);
				request.RevertAssetContentToOriginal ();
			}, (success, error) => {
				if (!success)
					Console.WriteLine ("Error: {0}", error.LocalizedDescription);
			});
		}

		void ApplyFilter (CIFilter filter)
		{
			// Prepare the options to pass when requesting to edit the image.
			var options = new PHContentEditingInputRequestOptions ();
			options.SetCanHandleAdjustmentDataHandler (adjustmentData => {
				bool result = false;
				InvokeOnMainThread (() => {
					result = adjustmentData.FormatIdentifier == AdjustmentFormatIdentifier && adjustmentData.FormatVersion == "1.0";
				});

				return result;
			});

			Asset.RequestContentEditingInput (options,(contentEditingInput, requestStatusInfo) => {
				// Create a CIImage from the full image representation.
				var url = contentEditingInput.FullSizeImageUrl;
				int orientation = (int)contentEditingInput.FullSizeImageOrientation;
				var inputImage = CIImage.FromUrl (url);
				inputImage = inputImage.CreateWithOrientation ((CIImageOrientation)orientation);

				// Create the filter to apply.
				filter.SetDefaults ();
				filter.Image = inputImage;

				// Apply the filter.
				CIImage outputImage = filter.OutputImage;

				// Create a PHAdjustmentData object that describes the filter that was applied.
				var adjustmentData = new PHAdjustmentData (
					AdjustmentFormatIdentifier,
					"1.0",
					NSData.FromString (filter.Name, NSStringEncoding.UTF8)
				);

				var contentEditingOutput = new PHContentEditingOutput (contentEditingInput);
				NSData jpegData = outputImage.GetJpegRepresentation (0.9f);
				jpegData.Save (contentEditingOutput.RenderedContentUrl, true);
				contentEditingOutput.AdjustmentData = adjustmentData;

				// Ask the shared PHPhotoLinrary to perform the changes.
				PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
					var request = PHAssetChangeRequest.ChangeRequest (Asset);
						request.ContentEditingOutput = contentEditingOutput;
					}, (success, error) => {
					if (!success)
						Console.WriteLine ("Error: {0}", error.LocalizedDescription);
				});
			});
		}

		void ToggleFavoriteState ()
		{
			PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
				var request = PHAssetChangeRequest.ChangeRequest (Asset);
				request.Favorite = !Asset.Favorite;
			}, (success, error) => {
				if (!success)
					Console.WriteLine ("Error: {0}", error.LocalizedDescription);
			});
		}

		partial void PlayButtonClickHandler (NSObject sender)
		{
			if (LivePhotoView.LivePhoto != null) {
				// We're displaying a live photo, begin playing it.
				LivePhotoView.StartPlayback (PHLivePhotoViewPlaybackStyle.Full);
			} else if (playerLayer != null) {
				// An AVPlayerLayer has already been created for this asset.
				playerLayer.Player.Play ();
			} else {
				// Request an AVAsset for the PHAsset we're displaying.
				PHImageManager.DefaultManager.RequestAvAsset (Asset, null, (asset, audioMix, info) =>
					DispatchQueue.MainQueue.DispatchAsync (() => {
						if (playerLayer == null) {
							CALayer viewLayer = View.Layer;

							// Create an AVPlayerItem for the AVAsset.
							var playerItem = new AVPlayerItem (asset);
							playerItem.AudioMix = audioMix;

							// Create an AVPlayer with the AVPlayerItem.
							var player = new AVPlayer (playerItem);

							// Create an AVPlayerLayer with the AVPlayer.
							playerLayer = AVPlayerLayer.FromPlayer (player);

							// Configure the AVPlayerLayer and add it to the view.
							playerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;
							playerLayer.Frame = new CGRect (0, 0, viewLayer.Bounds.Width, viewLayer.Bounds.Height);

							viewLayer.AddSublayer (playerLayer);
							playerLayer.Player.Play ();
						}
				}));
			}
		}

		partial void TrashButtonClickHandler (NSObject sender)
		{
			Action<bool, NSError> completionHandler = (success, error) => {
				if (success) {
					DispatchQueue.MainQueue.DispatchAsync (() =>
						NavigationController.PopViewController (true)
					);
				} else {
					Console.WriteLine (error.LocalizedDescription);
				}
			};

			if (AssetCollection != null) {
				// Remove asset from album
				PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
					var changeRequest = PHAssetCollectionChangeRequest.ChangeRequest (AssetCollection);
					changeRequest.RemoveAssets (new PHObject[] { Asset });
				}, completionHandler);
			} else {
				// Delete asset from library
				PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() =>
					PHAssetChangeRequest.DeleteAssets (new [] { Asset }), completionHandler);
			}
		}

		[Export ("livePhotoView:didEndPlaybackWithStyle:")]
		public virtual void DidEndPlayback (PHLivePhotoView livePhotoView, PHLivePhotoViewPlaybackStyle playbackStyle)
		{
			Console.WriteLine ("Did End Playback of Live Photo...");
			playingHint = false;
		}

		[Export ("livePhotoView:willBeginPlaybackWithStyle:")]
		public virtual void WillBeginPlayback (PHLivePhotoView livePhotoView, PHLivePhotoViewPlaybackStyle playbackStyle)
		{
			Console.WriteLine ("Will Beginning Playback of Live Photo...");
		}

		void ShowLivePhotoView ()
		{
			LivePhotoView.Hidden = false;
			ImageView.Hidden = true;
		}

		void ShowStaticPhotoView ()
		{
			LivePhotoView.Hidden = true;
			ImageView.Hidden = false;
		}

		void ShowPlaybackToolbar ()
		{
			ToolbarItems = new [] {
				PlayButton, Space, TrashButton
			};
		}

		void ShowStaticToolbar ()
		{
			ToolbarItems = new [] {
				Space, TrashButton
			};
		}

		void UpdateImage ()
		{
			// Check the asset's `mediaSubtypes` to determine if this is a live photo or not.
			bool assetHasLivePhotoSubType = Asset.MediaSubtypes == PHAssetMediaSubtype.PhotoLive;

			if (assetHasLivePhotoSubType)
				UpdateLiveImage ();
			else
				UpdateStaticImage ();
		}

		void UpdateLiveImage ()
		{
			// Prepare the options to pass when fetching the live photo.
			var livePhotoOptions = new PHLivePhotoRequestOptions {
				DeliveryMode = PHImageRequestOptionsDeliveryMode.HighQualityFormat,
				NetworkAccessAllowed = true
			};

			livePhotoOptions.ProgressHandler = (double progress, NSError error, out bool stop, NSDictionary info) => {
				stop = false;
				DispatchQueue.MainQueue.DispatchAsync (() => {
					ProgressView.Progress = (float)progress;
				});
			};

			// Request the live photo for the asset from the default PHImageManager.
			PHImageManager.DefaultManager.RequestLivePhoto (Asset, TargetSize, PHImageContentMode.AspectFit, livePhotoOptions, (livePhoto, info) => {
				// Hide the progress view now the request has completed.
				ProgressView.Hidden = true;
				// Check if the request was successful.
				if (livePhoto == null)
					return;

				Console.WriteLine ("Got a live photo");

				// Show the PHLivePhotoView and use it to display the requested image.
				ShowLivePhotoView ();
				LivePhotoView.LivePhoto = livePhoto;

				var value = (NSNumber)info.ObjectForKey (PHImageKeys.ResultIsDegraded);
				if (value.BoolValue && !playingHint) {
					// Playback a short section of the live photo; similar to the Photos share sheet.
					Console.WriteLine ("playing hint...");
					playingHint = true;
					LivePhotoView.StartPlayback (PHLivePhotoViewPlaybackStyle.Hint);

					// Update the toolbar to show the correct items for a live photo.
					ShowPlaybackToolbar ();
				}
			});
		}

		void UpdateStaticImage ()
		{
			// Prepare the options to pass when fetching the live photo.
			var options = new PHImageRequestOptions {
				DeliveryMode = PHImageRequestOptionsDeliveryMode.HighQualityFormat,
				NetworkAccessAllowed = true
			};

			options.ProgressHandler = (double progress, NSError error, out bool stop, NSDictionary info) => {
				stop = false;
				DispatchQueue.MainQueue.DispatchAsync (() => {
					ProgressView.Progress = (float)progress;
				});
			};

			PHImageManager.DefaultManager.RequestImageForAsset (Asset, TargetSize, PHImageContentMode.AspectFit, options, (result, info) => {
				// Hide the progress view now the request has completed.
				ProgressView.Hidden = true;

				// Check if the request was successful.
				if (result == null)
					return;

				// Show the UIImageView and use it to display the requested image.
				ShowStaticPhotoView ();
				ImageView.Image = result;
			});
		}

		void RemovePlayerLayer ()
		{
			if (playerLayer != null) {
				playerLayer.Player.Pause ();
				playerLayer.RemoveFromSuperLayer ();
				playerLayer.Dispose ();
			}
		}
	}
}