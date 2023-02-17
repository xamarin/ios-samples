using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Foundation;
using Security;

namespace AddingTheSignInWithAppleFlowToYourApp {
	public class KeychainItem {
		#region Properties

		public string Service { get; set; }
		public string Account { get; private set; }
		public string AccessGroup { get; set; }

		#endregion

		#region Constructors

		public KeychainItem (string service, string account, string accessGroup = null)
		{
			Service = service;
			Account = account;
			AccessGroup = accessGroup;
		}

		#endregion

		#region Keychain access

		public string ReadItem ()
		{
			//Build a record to find the item that matches the service, account and access group.
			var record = CreateKeychainRecord (Service, Account, AccessGroup);

			// Try to fetch the existing keychain item that matches the query.
			record = SecKeyChain.QueryAsRecord (record, out SecStatusCode statusCode);

			// Check the return status and throw an error if appropriate.
			if (statusCode == SecStatusCode.ItemNotFound)
				throw new KeychainNoPasswordException (statusCode);

			if (statusCode != SecStatusCode.Success)
				throw new KeychainUnhandledErrorException (statusCode);

			// Parse the password string from the query result.
			if (!(record.ValueData is NSData passwordData))
				throw new KeychainUnexpectedPasswordDataException (statusCode);

			return passwordData.ToString ();
		}

		public void SaveItem (string password)
		{
			// Encode the password into an Data object.
			var encodedPassword = NSData.FromString (password, NSStringEncoding.UTF8);
			var record = CreateKeychainRecord (Service, Account, AccessGroup);

			try {
				// Check for an existing item in the keychain.
				var _ = ReadItem ();

				// Update the existing item with the new password.
				var statusCode = SecKeyChain.Update (record, new SecRecord { ValueData = encodedPassword });

				// Throw an error if an unexpected status was returned.
				if (statusCode != SecStatusCode.Success)
					throw new KeychainUnhandledErrorException (statusCode);
			} catch (KeychainNoPasswordException) {
				//No password was found in the keychain. Create a dictionary to save
				//as a new keychain item.
				record.ValueData = encodedPassword;

				// Add a the new item to the keychain.
				var statusCode = SecKeyChain.Add (record);

				// Throw an error if an unexpected status was returned.
				if (statusCode != SecStatusCode.Success)
					throw new KeychainUnhandledErrorException (statusCode);
			}
		}

		public void DeleteItem ()
		{
			// Delete the existing item from the keychain.
			var record = CreateKeychainRecord (Service, Account, AccessGroup);
			var statusCode = SecKeyChain.Remove (record);

			// Throw an error if an unexpected status was returned.
			if (statusCode != SecStatusCode.Success || statusCode != SecStatusCode.ItemNotFound)
				throw new KeychainUnhandledErrorException (statusCode);
		}

		#endregion

		#region Convenience

		static SecRecord CreateKeychainRecord (string service, string account = null, string accessGroup = null)
		{
			return new SecRecord (SecKind.GenericPassword) {
				Service = service,
				Account = account,
				AccessGroup = accessGroup
			};
		}

		public static string CurrentUserIdentifier {
			get {
				try {
					return new KeychainItem ("com.xamarin.AddingTheSignInWithAppleFlowToYourApp", "userIdentifier").ReadItem ();
				} catch (Exception) {
					return "";
				}
			}
		}

		public static void DeleteUserIdentifierFromKeychain ()
		{
			try {
				new KeychainItem ("com.xamarin.AddingTheSignInWithAppleFlowToYourApp", "userIdentifier").DeleteItem ();
			} catch (Exception) {
				Console.WriteLine ("Unable to delete userIdentifier from keychain");
			}
		}

		#endregion
	}
}
