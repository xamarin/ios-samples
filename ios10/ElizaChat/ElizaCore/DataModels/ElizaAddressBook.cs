using System;
using System.Linq;
using System.Collections.Generic;
using Foundation;
using Intents;

namespace ElizaCore {
	public class ElizaAddressBook : NSObject {
		#region Computed Properties
		public List<ElizaUser> Contacts { get; set; } = new List<ElizaUser> ();
		#endregion

		#region Constructors
		public ElizaAddressBook ()
		{
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Populates the address book with the default users of the ElizaChat app.
		/// </summary>
		public void LoadAddressBook ()
		{
			// Add Eliza and the human user
			Contacts.Add (new ElizaUser ("Eliza", "Eliza", "Chatbot"));
			Contacts.Add (new ElizaUser ("Human", "Human", "User"));
		}

		public NSOrderedSet<NSString> ContactScreenNames ()
		{
			var nicknames = new NSMutableOrderedSet<NSString> ();

			// Sort contacts by the last time used
			var query = Contacts.OrderBy (contact => contact.LastMessagedOn);

			// Assemble ordered list of nicknames by most used to least
			foreach (ElizaUser contact in query) {
				nicknames.Add (new NSString (contact.ScreenName));
			}

			// Return names
			return new NSOrderedSet<NSString> (nicknames.AsSet ());
		}

		public void UpdateUserSpecificVocabulary ()
		{
			// Clear any existing vocabulary
			INVocabulary.SharedVocabulary.RemoveAllVocabularyStrings ();

			// Register new vocabulary
			INVocabulary.SharedVocabulary.SetVocabularyStrings (ContactScreenNames (), INVocabularyStringType.ContactName);
			Console.WriteLine ("ElizaChat: User Vocabulary Sent to Siri");
		}

		public ElizaUser FindUser (string name)
		{
			// Search addressbook for the specific user
			foreach (ElizaUser contact in Contacts) {
				if (contact.ScreenName == name || contact.FirstName == name) return contact;
			}

			// Not found
			return null;
		}
		#endregion
	}
}
