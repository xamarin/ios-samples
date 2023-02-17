using System;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using CoreImage;
using Foundation;
using Photos;
using PhotosUI;
using UIKit;

namespace SamplePhotoApp {
	public partial class AssetViewController : UIViewController, IPHPhotoLibraryChangeObserver, IPHLivePhotoViewDelegate {
		AVPlayerLayer playerLayer;
		AVPlayerLooper playerLooper;
		bool isPlayingHint;

		readonly string formatIdentifier = NSBundle.MainBundle.BundleIdentifier;
		readonly string formatVersion = "1.0";
		readonly CIContext ciContext = CIContext.Create ();

		public PHAsset Asset { get; set; }
		public PHAssetCollection AssetCollection { get; set; }

		[Export ("initWithCoder:")]
		public AssetViewController (NSCoder coder)
			: base (coder)
		{
		}

		public AssetViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			LivePhotoView.Delegate = this;
			PHPhotoLibrary.SharedPhotoLibrary.RegisterChangeObserver (this);

			LivePhotoView.Hidden = true;
			AnimatedImageView.Hidden = true;
		}

		protected override void Dispose (bool disposing)
		{
			PHPhotoLibrary.SharedPhotoLibrary.UnregisterChangeObserver (this);
			base.Dispose (disposing);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			UpdateToolbars ();

			// Make sure the view layout happens before requesting an image sized to fit the view.
			View.LayoutIfNeeded ();
			UpdateContent ();
		}

		#region UI Actions

		partial void EditAsset (UIBarButtonItem sender)
		{
			// Use a UIAlertController to display editing options to the user.
			var alertController = UIAlertController.Create (null, null, UIAlertControllerStyle.ActionSheet);
#if !__TVOS__
			alertController.ModalPresentationStyle = UIModalPresentationStyle.Popover;
			var popoverController = alertController.PopoverPresentationController;
			if (popoverController != null) {
				popoverController.BarButtonItem = sender;
				popoverController.PermittedArrowDirections = UIPopoverArrowDirection.Up;
			}
#endif
			// Add a Cancel action to dismiss the alert without doing anything.
			alertController.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));

			// Allow editing only if the PHAsset supports edit operations.
			if (Asset.CanPerformEditOperation (PHAssetEditOperation.Content)) {
				// Add actions for some canned filters.
				alertController.AddAction (UIAlertAction.Create ("Sepia Tone", UIAlertActionStyle.Default, _ => {
					ApplyFilter (new CISepiaTone ());
				}));
				alertController.AddAction (UIAlertAction.Create ("Chrome", UIAlertActionStyle.Default, _ => {
					ApplyFilter (new CIPhotoEffectChrome ());
				}));

				// Add actions to revert any edits that have been made to the PHAsset.
				alertController.AddAction (UIAlertAction.Create ("Revert", UIAlertActionStyle.Default, RevertAsset));
			}
			// Present the UIAlertController.
			PresentViewController (alertController, true, null);
		}

#if __TVOS__

		partial void PlayLivePhoto (NSObject sender)
		{
			LivePhotoView.StartPlayback (PHLivePhotoViewPlaybackStyle.Full);
		}

#endif

		partial void Play (NSObject sender)
		{
			// An AVPlayerLayer has already been created for this asset; just play it.
			if (playerLayer != null) {
				playerLayer.Player.Play ();
			} else {
				var options = new PHVideoRequestOptions {
					NetworkAccessAllowed = true,
					DeliveryMode = PHVideoRequestOptionsDeliveryMode.Automatic,
					ProgressHandler = (double progress, NSError error, out bool stop, NSDictionary info) => {
						stop = false;

						// Handler might not be called on the main queue, so re-dispatch for UI work.
						DispatchQueue.MainQueue.DispatchSync (() => {
							ProgressView.Progress = (float) progress;
						});
					}
				};

				// Request an AVAsset for the displayed PHAsset and set up a layer for playing it.
				PHImageManager.DefaultManager.RequestPlayerItem (Asset, options, (playerItem, info) => {
					DispatchQueue.MainQueue.DispatchSync (() => {
						if (this.playerLayer != null || playerItem == null)
							return;

						// Create an AVPlayer and AVPlayerLayer with the AVPlayerItem.
						AVPlayer player;

						if (Asset.PlaybackStyle == PHAssetPlaybackStyle.VideoLooping) {
							var queuePlayer = AVQueuePlayer.FromItems (new [] { playerItem });
							playerLooper = AVPlayerLooper.FromPlayer (queuePlayer, playerItem);
							player = queuePlayer;
						} else {
							player = AVPlayer.FromPlayerItem (playerItem);
						}

						var playerLayer = AVPlayerLayer.FromPlayer (player);

						// Configure the AVPlayerLayer and add it to the view.
						playerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;
						playerLayer.Frame = View.Layer.Bounds;
						View.Layer.AddSublayer (playerLayer);

						player.Play ();

						// Refer to the player layer so we can remove it later.
						this.playerLayer = playerLayer;
					});
				});
			}
		}

		partial void RemoveAsset (NSObject sender)
		{
			Action<bool, NSError> completion = (success, error) => {
				if (success) {
					PHPhotoLibrary.SharedPhotoLibrary.UnregisterChangeObserver (this);
					DispatchQueue.MainQueue.DispatchSync (() => NavigationController.PopViewController (true));
				} else {
					Console.WriteLine ($"can't remove asset: {error.LocalizedDescription}");
				}
			};

			if (AssetCollection != null) {
				// Remove asset from album
				PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
					var request = PHAssetCollectionChangeRequest.ChangeRequest (AssetCollection);
					request.RemoveAssets (new PHObject [] { Asset });
				}, completion);
			} else {
				// Delete asset from library
				PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
					PHAssetChangeRequest.DeleteAssets (new [] { Asset });
				}, completion);
			}
		}

		partial void ToggleFavorite (UIBarButtonItem sender)
		{
			PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
				var request = PHAssetChangeRequest.ChangeRequest (Asset);
				request.Favorite = !Asset.Favorite;
			}, (success, error) => {
				// Original sample updates FavoriteButton.Title here
				// but this doesn't work, because you have to wait PhotoLibraryDidChange notification.
				// At this point you just know is there any issue or everthing is ok.
				if (!success) {
					Console.WriteLine ($"can't set favorite: {error.LocalizedDescription}");
				}
			});
		}

		#endregion

		#region Image display

		CGSize GetTargetSize ()
		{
			nfloat scale = UIScreen.MainScreen.Scale;
			return new CGSize (ImageView.Bounds.Width * scale, ImageView.Bounds.Height * scale);
		}

		void UpdateContent ()
		{
			switch (Asset.PlaybackStyle) {
			case PHAssetPlaybackStyle.Unsupported:
				var alertController = UIAlertController.Create ("Unsupported Format", null, UIAlertControllerStyle.Alert);
				alertController.AddAction (UIAlertAction.Create ("Ok", UIAlertActionStyle.Default, null));
				PresentViewController (alertController, true, null);
				break;

			case PHAssetPlaybackStyle.Image:
				UpdateStillImage ();
				break;

			case PHAssetPlaybackStyle.LivePhoto:
				UpdateLivePhoto ();
				break;

			case PHAssetPlaybackStyle.ImageAnimated:
				UpdateAnimatedImage ();
				break;

			case PHAssetPlaybackStyle.Video:
				UpdateStillImage (); // as a placeholder until play is tapped
				break;

			case PHAssetPlaybackStyle.VideoLooping:
				Play (this);
				break;
			}
		}

		void UpdateToolbars ()
		{
			// Enable editing buttons if the asset can be edited.
			EditButton.Enabled = Asset.CanPerformEditOperation (PHAssetEditOperation.Content);
			FavoriteButton.Enabled = Asset.CanPerformEditOperation (PHAssetEditOperation.Properties);
			FavoriteButton.Title = Asset.Favorite ? "♥︎" : "♡";

			// Enable the trash button if the asset can be deleted.
			if (AssetCollection != null)
				TrashButton.Enabled = AssetCollection.CanPerformEditOperation (PHCollectionEditOperation.RemoveContent);
			else
				TrashButton.Enabled = Asset.CanPerformEditOperation (PHAssetEditOperation.Delete);

			// Set the appropriate toolbarItems based on the mediaType of the asset.
			if (Asset.MediaType == PHAssetMediaType.Video) {
#if __TVOS__
				NavigationItem.LeftBarButtonItems = new UIBarButtonItem[] { PlayButton, FavoriteButton, TrashButton };
#elif __IOS__
				ToolbarItems = new UIBarButtonItem[] { FavoriteButton, Space, PlayButton, Space, TrashButton };
				if (NavigationController != null)
					NavigationController.ToolbarHidden = false;
#endif
			} else {
#if __TVOS__
				// In tvOS, PHLivePhotoView doesn't do playback gestures,
				// so add a play button for Live Photos.
				if (Asset.PlaybackStyle == PHAssetPlaybackStyle.LivePhoto)
					NavigationItem.LeftBarButtonItems = new UIBarButtonItem[] { LivePhotoPlayButton, FavoriteButton, TrashButton };
				else
					NavigationItem.LeftBarButtonItems = new UIBarButtonItem[] { FavoriteButton, TrashButton };
#elif __IOS__
				// In iOS, present both stills and Live Photos the same way, because
				// PHLivePhotoView provides the same gesture-based UI as in Photos app.
				ToolbarItems = new UIBarButtonItem[] { FavoriteButton, Space, TrashButton };
				if (NavigationController != null)
					NavigationController.ToolbarHidden = false;
#endif
			}
		}

		void UpdateStillImage ()
		{
			// Prepare the options to pass when fetching the (photo, or video preview) image.
			var options = new PHImageRequestOptions {
				DeliveryMode = PHImageRequestOptionsDeliveryMode.HighQualityFormat,
				NetworkAccessAllowed = true,
				ProgressHandler = (double progress, NSError error, out bool stop, NSDictionary info) => {
					stop = false;
					// Handler might not be called on the main queue, so re-dispatch for UI work.
					DispatchQueue.MainQueue.DispatchSync (() => {
						ProgressView.Progress = (float) progress;
					});
				}
			};

			ProgressView.Hidden = false;
			PHImageManager.DefaultManager.RequestImageForAsset (Asset, GetTargetSize (), PHImageContentMode.AspectFit, options, (image, info) => {
				// Hide the progress view now the request has completed.
				ProgressView.Hidden = true;

				// If successful, show the image view and display the image.
				if (image == null)
					return;

				// Now that we have the image, show it.
				ImageView.Hidden = false;
				ImageView.Image = image;
			});
		}

		void UpdateLivePhoto ()
		{
			// Prepare the options to pass when fetching the live photo.
			var options = new PHLivePhotoRequestOptions {
				DeliveryMode = PHImageRequestOptionsDeliveryMode.HighQualityFormat,
				NetworkAccessAllowed = true,
				ProgressHandler = (double progress, NSError error, out bool stop, NSDictionary dictionary) => {
					stop = false;
					// Handler might not be called on the main queue, so re-dispatch for UI work.
					DispatchQueue.MainQueue.DispatchSync (() => ProgressView.Progress = (float) progress);
				}
			};

			ProgressView.Hidden = false;
			// Request the live photo for the asset from the default PHImageManager.
			PHImageManager.DefaultManager.RequestLivePhoto (Asset, GetTargetSize (), PHImageContentMode.AspectFit, options, (livePhoto, info) => {
				// Hide the progress view now the request has completed.
				ProgressView.Hidden = true;

				// If successful, show the live photo view and display the live photo.
				if (livePhoto == null)
					return;

				// Now that we have the Live Photo, show it.
				ImageView.Hidden = true;
				AnimatedImageView.Hidden = true;
				LivePhotoView.Hidden = false;
				LivePhotoView.LivePhoto = livePhoto;

				// Playback a short section of the live photo; similar to the Photos share sheet.
				if (!isPlayingHint) {
					isPlayingHint = true;
					LivePhotoView.StartPlayback (PHLivePhotoViewPlaybackStyle.Hint);
				}
			});
		}

		void UpdateAnimatedImage ()
		{
			// Prepare the options to pass when fetching the (photo, or video preview) image.
			var options = new PHImageRequestOptions {
				DeliveryMode = PHImageRequestOptionsDeliveryMode.HighQualityFormat,
				Version = PHImageRequestOptionsVersion.Original,
				NetworkAccessAllowed = true,
				ProgressHandler = (double progress, NSError error, out bool stop, NSDictionary info) => {
					stop = false;
					// Handler might not be called on the main queue, so re-dispatch for UI work.
					DispatchQueue.MainQueue.DispatchSync (() => {
						ProgressView.Progress = (float) progress;
					});
				}
			};

			ProgressView.Hidden = false;
			PHImageManager.DefaultManager.RequestImageData (Asset, options, (data, dataUti, orientation, info) => {
				// Hide the progress view now the request has completed.
				ProgressView.Hidden = true;

				// If successful, show the image view and display the image.
				if (data == null)
					return;

				var animatedImage = new AnimatedImage (data);

				LivePhotoView.Hidden = true;
				ImageView.Hidden = true;
				AnimatedImageView.Hidden = false;
				AnimatedImageView.AnimatedImage = animatedImage;
				AnimatedImageView.IsPlaying = true;
			});
		}

		#endregion

		#region Asset editing

		void RevertAsset (UIAlertAction action)
		{
			PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
				var request = PHAssetChangeRequest.ChangeRequest (Asset);
				request.RevertAssetContentToOriginal ();
			}, (success, error) => {
				if (!success)
					Console.WriteLine ($"can't revert asset: {error.LocalizedDescription}");
			});
		}

		void ApplyFilter (CIFilter filter)
		{
			// Set up a handler to make sure we can handle prior edits.
			var options = new PHContentEditingInputRequestOptions ();
			options.CanHandleAdjustmentData = (adjustmentData => {
				return adjustmentData.FormatIdentifier == formatIdentifier && adjustmentData.FormatVersion == formatVersion;
			});

			// Prepare for editing.
			Asset.RequestContentEditingInput (options, (input, requestStatusInfo) => {
				if (input == null)
					throw new InvalidProgramException ($"can't get content editing input: {requestStatusInfo}");

				// This handler gets called on the main thread; dispatch to a background queue for processing.
				DispatchQueue.GetGlobalQueue (DispatchQueuePriority.Default).DispatchAsync (() => {
					// Create a PHAdjustmentData object that describes the filter that was applied.
					var adjustmentData = new PHAdjustmentData (
							formatIdentifier,
							formatVersion,
							NSData.FromString (filter.Name, NSStringEncoding.UTF8));

					// NOTE:
					// This app's filter UI is fire-and-forget. That is, the user picks a filter, 
					// and the app applies it and outputs the saved asset immediately. There's 
					// no UI state for having chosen but not yet committed an edit. This means
					// there's no role for reading adjustment data -- you do that to resume
					// in-progress edits, and this sample app has no notion of "in-progress".
					//
					// However, it's still good to write adjustment data so that potential future
					// versions of the app (or other apps that understand our adjustement data
					// format) could make use of it.

					// Create content editing output, write the adjustment data.
					var output = new PHContentEditingOutput (input) {
						AdjustmentData = adjustmentData
					};

					// Select a filtering function for the asset's media type.
					Action<CIFilter, PHContentEditingInput, PHContentEditingOutput, Action> applyFunc;
					if (Asset.MediaSubtypes.HasFlag (PHAssetMediaSubtype.PhotoLive))
						applyFunc = ApplyLivePhotoFilter;
					else if (Asset.MediaType == PHAssetMediaType.Image)
						applyFunc = ApplyPhotoFilter;
					else
						applyFunc = ApplyVideoFilter;

					// Apply the filter.
					applyFunc (filter, input, output, () => {
						// When rendering is done, commit the edit to the Photos library.
						PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
							var request = PHAssetChangeRequest.ChangeRequest (Asset);
							request.ContentEditingOutput = output;
						}, (success, error) => {
							if (!success)
								Console.WriteLine ($"can't edit asset: {error.LocalizedDescription}");
						});
					});
				});
			});
		}

		void ApplyPhotoFilter (CIFilter filter, PHContentEditingInput input, PHContentEditingOutput output, Action completion)
		{
			// Load the full size image.
			var inputImage = new CIImage (input.FullSizeImageUrl);
			if (inputImage == null)
				throw new InvalidProgramException ("can't load input image to edit");

			// Apply the filter.
			filter.Image = inputImage.CreateWithOrientation (input.FullSizeImageOrientation);
			var outputImage = filter.OutputImage;

			// Write the edited image as a JPEG.
			// TODO: https://bugzilla.xamarin.com/show_bug.cgi?id=44503
			NSError error;
			if (!ciContext.WriteJpegRepresentation (outputImage, output.RenderedContentUrl, inputImage.ColorSpace, new NSDictionary (), out error))
				throw new InvalidProgramException ($"can't apply filter to image: {error.LocalizedDescription}");

			completion ();
		}

		void ApplyLivePhotoFilter (CIFilter filter, PHContentEditingInput input, PHContentEditingOutput output, Action completion)
		{
			// This app filters assets only for output. In an app that previews
			// filters while editing, create a livePhotoContext early and reuse it
			// to render both for previewing and for final output.
			var livePhotoContext = new PHLivePhotoEditingContext (input);

			livePhotoContext.FrameProcessor2 = (IPHLivePhotoFrame frame, ref NSError _) => {
				filter.Image = frame.Image;
				return filter.OutputImage;
			};
			livePhotoContext.SaveLivePhoto (output, (PHLivePhotoEditingOption) null, (success, error) => {
				if (success)
					completion ();
				else
					Console.WriteLine ("can't output live photo");
			});
			// Applying edits to a Live Photo currently crashes
			// TODO: https://bugzilla.xamarin.com/show_bug.cgi?id=58227
		}

		void ApplyVideoFilter (CIFilter filter, PHContentEditingInput input, PHContentEditingOutput output, Action completion)
		{
			// Load AVAsset to process from input.
			var avAsset = input.AudiovisualAsset;
			if (avAsset == null)
				throw new InvalidProgramException ("can't get AV asset to edit");

			// Set up a video composition to apply the filter.
			var composition = AVVideoComposition.CreateVideoComposition (avAsset, request => {
				filter.Image = request.SourceImage;
				var filtered = filter.OutputImage;
				request.Finish (filtered, null);
			});

			// Export the video composition to the output URL.
			var export = new AVAssetExportSession (avAsset, AVAssetExportSessionPreset.HighestQuality) {
				OutputFileType = AVFileType.QuickTimeMovie,
				OutputUrl = output.RenderedContentUrl,
				VideoComposition = composition
			};
			export.ExportAsynchronously (completion);
		}

		#endregion

		#region PHPhotoLibraryChangeObserver

		public void PhotoLibraryDidChange (PHChange changeInstance)
		{
			// Call might come on any background queue. Re-dispatch to the main queue to handle it.
			DispatchQueue.MainQueue.DispatchAsync (() => {
				// Check if there are changes to the asset we're displaying.
				PHObjectChangeDetails changeDetails = changeInstance.GetObjectChangeDetails (Asset);
				if (changeDetails == null)
					return;

				// Get the updated asset.
				var assetAfterChanges = changeDetails.ObjectAfterChanges as PHAsset;
				if (assetAfterChanges == null)
					return;

				Asset = (PHAsset) assetAfterChanges;

				if (Asset != null) {
					FavoriteButton.Title = Asset.Favorite ? "♥︎" : "♡";
				}

				// If the asset's content changed, update the image and stop any video playback.
				if (changeDetails.AssetContentChanged) {
					UpdateContent ();

					playerLayer?.RemoveFromSuperLayer ();
					playerLayer = null;
					playerLooper = null;
				}
			});
		}

		#endregion

		#region PHLivePhotoViewDelegate

		[Export ("livePhotoView:didEndPlaybackWithStyle:")]
		public virtual void DidEndPlayback (PHLivePhotoView livePhotoView, PHLivePhotoViewPlaybackStyle playbackStyle)
		{
			isPlayingHint = (playbackStyle == PHLivePhotoViewPlaybackStyle.Hint);
		}

		[Export ("livePhotoView:willBeginPlaybackWithStyle:")]
		public virtual void WillBeginPlayback (PHLivePhotoView livePhotoView, PHLivePhotoViewPlaybackStyle playbackStyle)
		{
			isPlayingHint = (playbackStyle == PHLivePhotoViewPlaybackStyle.Hint);
		}

		#endregion
	}
}
