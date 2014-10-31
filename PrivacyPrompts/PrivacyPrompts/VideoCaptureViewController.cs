using System;
using AVFoundation;
using System.Threading.Tasks;

namespace PrivacyPrompts
{
	//Custom PrivacyDetailViewController demonstrating async AVCaptureDevice permissions
	public class VideoCaptureViewController : PrivacyDetailViewController
	{
		public VideoCaptureViewController () : base(null, null)
		{
			CheckAccess = CameraAccessStatus;
			RequestAccess = RequestCameraAccess;
		}

		protected string CameraAccessStatus()
		{
			return AVCaptureDevice.GetAuthorizationStatus (AVMediaType.Video).ToString ();
		}

		//This method is synchronous...
		protected void RequestCameraAccess()
		{
			//But inside, use an async lambda to request permission...
			Task.Run( async () => 
				{
					var _ = await AVCaptureDevice.RequestAccessForMediaTypeAsync (AVMediaType.Video);
					//...But the continuation is on a background thread, so use InvokeOnMainThread to update the UI
					InvokeOnMainThread( () => accessStatus.Text = CheckAccess.Invoke() );
				});
		}
	}
}

