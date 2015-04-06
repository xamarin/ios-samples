using System;
using AddressBook;
using Foundation;

namespace PrivacyPrompts
{
	public class AddressBookPrivacyController : PrivacyDetailViewController
	{
		/*
		protected override string CheckAccess ()
		{
			return ABAddressBook.GetAuthorizationStatus ().ToString ();
		}

		protected override void RequestAccess ()
		{
			NSError error;
			ABAddressBook addressBook = ABAddressBook.Create (out error);

			if (addressBook == null)
				return;

			addressBook.RequestAccess ((granted, accessError) => {
				string text = string.Format ("Access {0}", granted ? "allowed" : "denied");
				InvokeOnMainThread (() => AccessStatus.Text = text);
			});
		}
		*/
	}
}

