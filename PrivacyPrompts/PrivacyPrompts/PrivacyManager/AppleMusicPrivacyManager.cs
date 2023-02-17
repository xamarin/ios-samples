using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Foundation;
using StoreKit;

namespace PrivacyPrompts {
	public class AppleMusicPrivacyManager : IPrivacyManager {

		public Task RequestAccess ()
		{
			var tcs = new TaskCompletionSource<object> ();

			SKCloudServiceController.RequestAuthorization (_ => {
				tcs.SetResult (null);
			});

			return tcs.Task;
		}

		public string CheckAccess ()
		{
			return SKCloudServiceController.AuthorizationStatus.ToString ();
		}
	}
}
