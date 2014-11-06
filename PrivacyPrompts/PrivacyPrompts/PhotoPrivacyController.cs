using System;
using MonoTouch.AssetsLibrary;
using MonoTouch.UIKit;
using MonoTouch.Photos;

namespace PrivacyPrompts
{
	public class PhotoPrivacyController : PrivacyDetailViewController
	{
		public PhotoPrivacyController ()
		{
			this.CheckAccess = CheckPhotosAuthorizationStatus;
			this.RequestAccess = RequestPhotoAccess;
		}

		string CheckPhotosAuthorizationStatus ()
		{
			return PHPhotoLibrary.AuthorizationStatus.ToString ();
		}

		void RequestPhotoAccess ()
		{
			PHPhotoLibrary.RequestAuthorization ((_) => UpdateStatus());
		}

	}
}

