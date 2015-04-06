using System;
using System.Threading.Tasks;
using AddressBook;
using Foundation;

namespace PrivacyPrompts
{
	public class AddressBookPrivacyManager : IPrivacyManager, IDisposable
	{
		ABAddressBook addressBook;

		public AddressBookPrivacyManager ()
		{
			NSError error;
			addressBook = ABAddressBook.Create (out error);
		}

		public Task RequestAccess ()
		{
			if (addressBook == null)
				return Task.FromResult<object>(null);

			var tcs = new TaskCompletionSource<object> ();
			addressBook.RequestAccess ((granted, accessError) => tcs.SetResult (null));
			return tcs.Task;
		}

		public string CheckAccess ()
		{
			return ABAddressBook.GetAuthorizationStatus ().ToString ();
		}

		public void Dispose ()
		{
			addressBook.Dispose ();
		}
	}
}

