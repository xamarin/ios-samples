using System;
using AssetsLibrary;
using UIKit;
using Photos;

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

