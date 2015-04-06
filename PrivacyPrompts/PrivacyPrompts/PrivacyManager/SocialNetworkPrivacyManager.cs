using System;
using System.Threading.Tasks;

using Accounts;
using Foundation;

namespace PrivacyPrompts
{
	public class SocialNetworkPrivacyManager : IPrivacyManager, IDisposable
	{
		readonly ACAccountStore accountStore;
		readonly NSString socialNetwork;

		public SocialNetworkPrivacyManager (NSString socialNetworkName)
		{
			socialNetwork = socialNetworkName;
			accountStore = new ACAccountStore ();
		}

		public Task RequestAccess ()
		{
			AccountStoreOptions options = GetOptions ();
			ACAccountType account = accountStore.FindAccountType (socialNetwork);

			var tcs = new TaskCompletionSource<object> ();
			accountStore.RequestAccess (account, options, (granted, error) => tcs.SetResult(null));
			return tcs.Task;
		}

		AccountStoreOptions GetOptions()
		{
			if (socialNetwork == ACAccountType.Facebook)
				return GetFacebookOptions ();

			if (socialNetwork == ACAccountType.TencentWeibo)
				return GetTencentWeibo ();

			return null;
		}

		AccountStoreOptions GetFacebookOptions()
		{
			AccountStoreOptions options = new AccountStoreOptions () { FacebookAppId = "MY_CODE" };
			options.SetPermissions (ACFacebookAudience.Friends, new [] {
				"email",
				"user_about_me"
			});

			return options;
		}

		AccountStoreOptions GetTencentWeibo()
		{
			return new AccountStoreOptions {
				TencentWeiboAppId = "MY_ID"
			};
		}

		public string CheckAccess ()
		{
			ACAccountType socialAccount = accountStore.FindAccountType (socialNetwork);
			return socialAccount.AccessGranted ? "granted" : "denied";
		}

		public void Dispose ()
		{
			accountStore.Dispose ();
		}
	}
}