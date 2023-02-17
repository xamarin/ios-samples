using System;
using AVFoundation;
using CoreMedia;
using Foundation;

namespace AVCam {
	public class AVCamPhotoCaptureDelegate : NSObject, IAVCapturePhotoCaptureDelegate {
		public AVCapturePhotoSettings RequestedPhotoSettings { get; set; }
		Action WillCapturePhotoAnimation { get; set; }
		Action<bool> LivePhotoCaptureHandler { get; set; }
		Action<AVCamPhotoCaptureDelegate> CompletionHandler { get; set; }
		NSData PhotoData { get; set; }
		NSUrl LivePhotoCompanionMovieUrl { get; set; }

		public AVCamPhotoCaptureDelegate (AVCapturePhotoSettings requestedPhotoSettings, Action willCapturePhotoAnimation, Action<bool> livePhotoCaptureHandler, Action<AVCamPhotoCaptureDelegate> completionHandler)
		{
			RequestedPhotoSettings = requestedPhotoSettings;
			WillCapturePhotoAnimation = willCapturePhotoAnimation;
			LivePhotoCaptureHandler = livePhotoCaptureHandler;
			CompletionHandler = completionHandler;
		}

		void DidFinish ()
		{
			if (LivePhotoCompanionMovieUrl != null && NSFileManager.DefaultManager.FileExists (LivePhotoCompanionMovieUrl.Path)) {
				NSError error;
				NSFileManager.DefaultManager.Remove (LivePhotoCompanionMovieUrl.Path, out error);

				if (error != null)
					Console.WriteLine ($"Could not remove file at url: {LivePhotoCompanionMovieUrl.Path}");
			}

			CompletionHandler (this);
		}

		[Export ("captureOutput:willBeginCaptureForResolvedSettings:")]
		public virtual void WillBeginCapture (AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings)
		{
			if ((resolvedSettings.LivePhotoMovieDimensions.Width > 0) && (resolvedSettings.LivePhotoMovieDimensions.Height > 0)) {
				LivePhotoCaptureHandler (true);
			}
		}

		[Export ("captureOutput:willCapturePhotoForResolvedSettings:")]
		public virtual void WillCapturePhoto (AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings)
		{
			WillCapturePhotoAnimation ();
		}

		[Export ("captureOutput:didFinishProcessingPhoto:error:")]
		public virtual void DidFinishProcessingPhoto (AVCapturePhotoOutput captureOutput, AVCapturePhoto photo, NSError error)
		{
			if (error != null) {
				Console.WriteLine ($"Error capturing photo: {error}", error);
				return;
			}
			PhotoData = photo.FileDataRepresentation ();
		}

		[Export ("captureOutput:didFinishRecordingLivePhotoMovieForEventualFileAtURL:resolvedSettings:")]
		public virtual void DidFinishRecordingLivePhotoMovie (AVCapturePhotoOutput captureOutput, NSUrl outputFileUrl, AVCaptureResolvedPhotoSettings resolvedSettings)
		{
			LivePhotoCaptureHandler (false);
		}

		[Export ("captureOutput:didFinishProcessingLivePhotoToMovieFileAtURL:duration:photoDisplayTime:resolvedSettings:error:")]
		public virtual void DidFinishProcessingLivePhotoMovie (AVCapturePhotoOutput captureOutput, NSUrl outputFileUrl, CMTime duration, CMTime photoDisplayTime, AVCaptureResolvedPhotoSettings resolvedSettings, NSError error)
		{
			if (error != null) {
				Console.WriteLine ($"Error processing live photo companion movie: {error}", error);
				return;
			}

			LivePhotoCompanionMovieUrl = outputFileUrl;
		}

		[Export ("captureOutput:didFinishCaptureForResolvedSettings:error:")]
		public virtual async void DidFinishCapture (AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings, NSError error)
		{
			if (error != null) {
				Console.WriteLine ($"Error capturing photo: {error}", error);
				DidFinish ();
				return;
			}

			if (PhotoData == null) {
				Console.WriteLine ("No photo data resource");
				DidFinish ();
				return;
			}

			var status = await Photos.PHPhotoLibrary.RequestAuthorizationAsync ();

			if (status == Photos.PHAuthorizationStatus.Authorized) {
				Photos.PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
					var options = new Photos.PHAssetResourceCreationOptions ();
					options.UniformTypeIdentifier = RequestedPhotoSettings.ProcessedFileType ();
					var creationRequest = Photos.PHAssetCreationRequest.CreationRequestForAsset ();
					creationRequest.AddResource (Photos.PHAssetResourceType.Photo, PhotoData, options);

					if (LivePhotoCompanionMovieUrl != null) {
						var livePhotoCompanionMovieResourceOptions = new Photos.PHAssetResourceCreationOptions ();
						livePhotoCompanionMovieResourceOptions.ShouldMoveFile = true;
						creationRequest.AddResource (Photos.PHAssetResourceType.PairedVideo, LivePhotoCompanionMovieUrl, livePhotoCompanionMovieResourceOptions);
					}
				}, (success, completeError) => {
					if (!success) {
						Console.WriteLine ($"Error occurred while saving photo to photo library: {error}");
					}

					DidFinish ();
				});
			} else {
				Console.WriteLine (@"Not authorized to save photo");
				DidFinish ();
			}
		}
	}
}
