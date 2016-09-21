using System;
using AVFoundation;

namespace AVCam
{
	public class PhotoCaptureDelegate : IAVCapturePhotoCaptureDelegate
	{
		public AVCapturePhotoSettings RequestedPhotoSettings { get; private set; } 

		public PhotoCaptureDelegate (AVCapturePhotoSettings settings, Action a1, Action<bool> a2, Action<PhotoCaptureDelegate> a3)
		{
		}
	}
}
