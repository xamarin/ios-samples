using System;
using AVFoundation;
using System.Threading.Tasks;

namespace PrivacyPrompts
{
	//Custom PrivacyDetailViewController demonstrating async AVCaptureDevice permissions
	public class VideoCapturePrivacyController : PrivacyDetailViewController
	{
		public VideoCapturePrivacyController ()
		{
			CheckAccess = CameraAccessStatus;
			RequestAccess = RequestCameraAccess;
		}

		protected string CameraAccessStatus()
		{
			return AVCaptureDevice.GetAuthorizationStatus (AVMediaType.Video).ToString ();
		}

		protected async void RequestCameraAccess()
		{
			await AVCaptureDevice.RequestAccessForMediaTypeAsync (AVMediaType.Video);
			// But the continuation is on a background thread, so use InvokeOnMainThread to update the UI
			UpdateStatus();
		}
	}
}

