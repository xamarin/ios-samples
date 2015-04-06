using System;
using Accounts;
using Foundation;

namespace PrivacyPrompts
{
	public class SocialNetworkPrivacyController : PrivacyDetailViewController
	{
		static ACAccountStore accountStore = new ACAccountStore ();

		readonly string socialNetwork;

		public SocialNetworkPrivacyController (string socialNetworkName)
		{
			socialNetwork = socialNetworkName;
		}

		/*
		protected override string CheckAccess ()
		{
			ACAccountType socialAccount = accountStore.FindAccountType (socialNetwork);
			return socialAccount.AccessGranted ? "granted" : "denied";
		}

		protected override void RequestAccess ()
		{
			switch (socialNetwork) {
				case "com.apple.facebook":
					RequestFacebookAccess ();
					break;
				case "com.apple.twitter":
					RequestTwitterAccess ();
					break;
				case "com.apple.sinaweibo":
					RequestSinaWeiboAccess ();
					break;
				case "com.apple.account.tencentweibo":
					RequestTencentWeiboAccess ();
					break;
				default:
					throw new ArgumentOutOfRangeException ();
			}
		}
		*/

		void RequestFacebookAccess ()
		{
			ACAccountType facebookAccount = accountStore.FindAccountType (ACAccountType.Facebook);

			AccountStoreOptions options = new AccountStoreOptions () { FacebookAppId = "MY_CODE" };
			options.SetPermissions (ACFacebookAudience.Friends, new [] {
				"email",
				"user_about_me"
			});

			SocialNetworkPrivacyController.accountStore.RequestAccess (facebookAccount, options, (s, e) => PrivacyManager.CheckAccess ());
		}

		void RequestTwitterAccess ()
		{
			ACAccountType twitterAccount = accountStore.FindAccountType (ACAccountType.Twitter);
			SocialNetworkPrivacyController.accountStore.RequestAccess (twitterAccount, null, (s, e) => PrivacyManager.CheckAccess ());
		}

		void RequestSinaWeiboAccess ()
		{
			ACAccountType sinaWeiboAccount = accountStore.FindAccountType (ACAccountType.SinaWeibo);
			SocialNetworkPrivacyController.accountStore.RequestAccess (sinaWeiboAccount, null, (s, e) => PrivacyManager.CheckAccess ());
		}

		void RequestTencentWeiboAccess ()
		{
			ACAccountType tencentWeiboAccount = accountStore.FindAccountType (ACAccountType.TencentWeibo);

			AccountStoreOptions options = new AccountStoreOptions ();
			options.TencentWeiboAppId = "MY_ID";

			SocialNetworkPrivacyController.accountStore.RequestAccess (tencentWeiboAccount, options, (s, e) => PrivacyManager.CheckAccess ());
		}
	}
}

