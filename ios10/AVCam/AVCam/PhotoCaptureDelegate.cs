using System;

using Foundation;
using AVFoundation;
using CoreMedia;
using Photos;

namespace AVCam
{
	public class PhotoCaptureDelegate : NSObject, IAVCapturePhotoCaptureDelegate
	{
		public AVCapturePhotoSettings RequestedPhotoSettings { get; private set; }

		Action willCapturePhotoAnimation;
		Action<bool> capturingLivePhoto;
		Action<PhotoCaptureDelegate> completed;

		NSData photoData;
		NSUrl livePhotoCompanionMovieUrl;

		public PhotoCaptureDelegate (AVCapturePhotoSettings settings,
									 Action willCapturePhotoAnimation,
									 Action<bool> capturingLivePhoto,
									 Action<PhotoCaptureDelegate> completed)
		{
			RequestedPhotoSettings = settings;
			this.willCapturePhotoAnimation = willCapturePhotoAnimation;
			this.capturingLivePhoto = capturingLivePhoto;
			this.completed = completed;
		}

		void DidFinish ()
		{
			var path = livePhotoCompanionMovieUrl?.Path;
			if (path != null) {
				if (NSFileManager.DefaultManager.FileExists (path)) {
					NSError error;
					if (!NSFileManager.DefaultManager.Remove (path, out error))
						Console.WriteLine ($"Could not remove file at url: {path}");
				}
			}

			completed (this);
		}

		[Export ("captureOutput:willBeginCaptureForResolvedSettings:")]
		public void WillBeginCapture (AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings)
		{
			if (resolvedSettings.LivePhotoMovieDimensions.Width > 0 && resolvedSettings.LivePhotoMovieDimensions.Height > 0)
				capturingLivePhoto (true);
		}

		[Export ("captureOutput:willCapturePhotoForResolvedSettings:")]
		public void WillCapturePhoto (AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings)
		{
			willCapturePhotoAnimation ();
		}

		[Export ("captureOutput:didFinishProcessingPhotoSampleBuffer:previewPhotoSampleBuffer:resolvedSettings:bracketSettings:error:")]
		public void DidFinishProcessingPhoto (AVCapturePhotoOutput captureOutput, CMSampleBuffer photoSampleBuffer, CMSampleBuffer previewPhotoSampleBuffer, AVCaptureResolvedPhotoSettings resolvedSettings, AVCaptureBracketedStillImageSettings bracketSettings, NSError error)
		{
			if (photoSampleBuffer != null)
				photoData = AVCapturePhotoOutput.GetJpegPhotoDataRepresentation (photoSampleBuffer, previewPhotoSampleBuffer);
			else
				Console.WriteLine ($"Error capturing photo: {error.LocalizedDescription}");
		}

		[Export ("captureOutput:didFinishRecordingLivePhotoMovieForEventualFileAtURL:resolvedSettings:")]
		public void DidFinishRecordingLivePhotoMovie (AVCapturePhotoOutput captureOutput, NSUrl outputFileUrl, AVCaptureResolvedPhotoSettings resolvedSettings)
		{
			capturingLivePhoto (false);
		}

		[Export ("captureOutput:didFinishProcessingLivePhotoToMovieFileAtURL:duration:photoDisplayTime:resolvedSettings:error:")]
		public void DidFinishProcessingLivePhotoMovie (AVCapturePhotoOutput captureOutput, NSUrl outputFileUrl, CMTime duration, CMTime photoDisplayTime, AVCaptureResolvedPhotoSettings resolvedSettings, NSError error)
		{
			if (error != null) {
				Console.WriteLine ($"Error processing live photo companion movie: {error.LocalizedDescription})");
				return;
			}

			livePhotoCompanionMovieUrl = outputFileUrl;
		}

		[Export ("captureOutput:didFinishCaptureForResolvedSettings:error:")]
		public void DidFinishCapture (AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings, NSError error)
		{
			if (error != null) {
				Console.WriteLine ($"Error capturing photo: {error.LocalizedDescription})");
				DidFinish ();
				return;
			}

			if (photoData == null) {
				Console.WriteLine ("No photo data resource");
				DidFinish ();
				return;
			}

			PHPhotoLibrary.RequestAuthorization (status => {
				if (status == PHAuthorizationStatus.Authorized) {
					PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
						var creationRequest = PHAssetCreationRequest.CreationRequestForAsset ();
						creationRequest.AddResource (PHAssetResourceType.Photo, photoData, null);

						var url = livePhotoCompanionMovieUrl;
						if (url != null) {
							var livePhotoCompanionMovieFileResourceOptions = new PHAssetResourceCreationOptions {
								ShouldMoveFile = true
							};
							creationRequest.AddResource (PHAssetResourceType.PairedVideo, url, livePhotoCompanionMovieFileResourceOptions);
						}
					}, (success, err) => {
						if (err != null)
							Console.WriteLine ($"Error occurered while saving photo to photo library: {error.LocalizedDescription}");
						DidFinish ();
					});
				} else {
					DidFinish ();
				}
			});
		}
	}
}
