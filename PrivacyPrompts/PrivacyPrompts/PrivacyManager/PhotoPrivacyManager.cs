using System;
using System.Threading.Tasks;
using Photos;

namespace PrivacyPrompts
{
	public class PhotoPrivacyManager : IPrivacyManager
	{
		public Task RequestAccess ()
		{
			var tcs = new TaskCompletionSource<object> ();
			PHPhotoLibrary.RequestAuthorization (_ => tcs.SetResult (null));
			return tcs.Task;
		}

		public string CheckAccess ()
		{
			return PHPhotoLibrary.AuthorizationStatus.ToString ();
		}
	}
}

