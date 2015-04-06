using System;
using Accounts;
using Foundation;

namespace PrivacyPrompts
{
	public class SocialNetworkPrivacyManager : IPrivacyManager
	{
		readonly ACAccountStore accountStore;
		readonly NSString socialNetwork;
		readonly IPrivacyViewController viewController;

		public SocialNetworkPrivacyManager (NSString socialNetworkName, IPrivacyViewController vc)
		{
			socialNetwork = socialNetworkName;
			viewController = vc;
			accountStore = new ACAccountStore ();
		}

		public void RequestAccess ()
		{
			AccountStoreOptions options = GetOptions ();
			ACAccountType account = accountStore.FindAccountType (socialNetwork);
			accountStore.RequestAccess (account, options, RequestCompletionHandler);
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

		void RequestCompletionHandler (bool granted, NSError error)
		{
			accountStore.InvokeOnMainThread (() => {
				viewController.AccessStatus.Text = CheckAccess ();
			});
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