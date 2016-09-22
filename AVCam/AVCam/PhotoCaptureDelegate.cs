using System;
using AVFoundation;
using Foundation;

namespace AVCam
{
	public class PhotoCaptureDelegate : IAVCapturePhotoCaptureDelegate
	{
		public AVCapturePhotoSettings RequestedPhotoSettings { get; private set; }

		Action willCapturePhotoAnimation;
		Action<bool> capturingLivePhoto;
		Action<PhotoCaptureDelegate> completed;

		NSData photoData;
		NSUrl livePhotoCompanionMovieURL; // TODO: rename URL -> Url

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
			var path = livePhotoCompanionMovieURL?.Path;
			if (path != null) {
				if (NSFileManager.DefaultManager.FileExists (path)) {
					NSError error;
					if (!NSFileManager.DefaultManager.Remove (path, out error))
						Console.WriteLine ($"Could not remove file at url: {path}");
				}
			}

			completed (this);
		}
	}
}
