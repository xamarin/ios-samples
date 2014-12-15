using System;
using MonoTouch.AddressBook;
using MonoTouch.Foundation;

namespace PrivacyPrompts
{
	public class AddressBookPrivacyController : PrivacyDetailViewController
	{
		ABAddressBook addressBook;

		public AddressBookPrivacyController ()
		{
			CheckAccess = CheckAddressBookAccess;
			RequestAccess = RequestAddressBookAccess;
		}

		public string CheckAddressBookAccess ()
		{
			return ABAddressBook.GetAuthorizationStatus ().ToString ();
		}

		public void RequestAddressBookAccess ()
		{
			NSError error;
			addressBook = ABAddressBook.Create (out error);

			if (addressBook != null) {
				addressBook.RequestAccess (delegate (bool granted, NSError accessError) {
					InvokeOnMainThread(() => accessStatus.Text = "Access " + (granted ? "allowed" : "denied"));
				});
			}
		}

	}
}

