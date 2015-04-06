using System;
using System.Threading.Tasks;

using AVFoundation;

namespace PrivacyPrompts
{
	public class VideoCapturePrivacyManager : IPrivacyManager
	{
		public Task RequestAccess ()
		{
			return AVCaptureDevice.RequestAccessForMediaTypeAsync (AVMediaType.Video);
		}

		public string CheckAccess ()
		{
			return AVCaptureDevice.GetAuthorizationStatus (AVMediaType.Video).ToString ();
		}
	}
}