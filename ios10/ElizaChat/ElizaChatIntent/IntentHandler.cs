using System;
using System.Collections.Generic;
using Foundation;
using Intents;
using ElizaCore;

namespace ElizaChatIntent
{
	// As an example, this class is set up to handle Message intents.
	// You will want to replace this or add other intents as appropriate.
	// The intents you wish to handle must be declared in the extension's Info.plist.

	// You can test your example integration by saying things to Siri like:
	// "Send a message using <myApp>"
	// "<myApp> John saying hello"
	// "Search for messages in <myApp>"
	[Register ("IntentHandler")]
	public class IntentHandler : INExtension, IINSendMessageIntentHandling, IINSearchForMessagesIntentHandling, IINSetMessageAttributeIntentHandling
	{
		#region Computed Properties
		public ElizaAddressBook AddressBook { get; set; } = new ElizaAddressBook ();
		public ElizaMain Eliza { get; set; } = new ElizaMain ();
		#endregion

		#region Constructors
		protected IntentHandler (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}
		#endregion

		public override NSObject GetHandler (INIntent intent)
		{
			// Load the address book
			AddressBook.LoadAddressBook ();

			// Return this class instance as the Intents handler
			return this;
		}

		// Implement resolution methods to provide additional information about your intent (optional).
		[Export ("resolveRecipientsForSearchForMessages:withCompletion:")]
		public void ResolveRecipients (INSendMessageIntent intent, Action<INPersonResolutionResult []> completion)
		{
			var resolutionResults = new List<INPersonResolutionResult> ();
			var matchingContacts = new List<INPerson> ();

			// Verify Recipients
			var recipients = intent.Recipients;
			if (recipients.Length == 0) {
				// We must have at least one recipient
				resolutionResults.Add (INPersonResolutionResult.NeedsValue);
			} else {
				// Scan all recipients
				foreach (var recipient in recipients) {
					// Know user to ElizaChat
					if (AddressBook.FindUser (recipient.Identifier) == null) {
						matchingContacts.Add (recipient);
					}
				}

				// Take action based on the found recipients
				switch (matchingContacts.Count) {
				case 0:
					// We have no contacts matching the description provided
					resolutionResults.Add (INPersonResolutionResult.Unsupported);
					break;
				case 1:
					// We have exactly one matching contact
					resolutionResults.Add (INPersonResolutionResult.GetSuccess (matchingContacts[0]));
					break;
				default:
					// We need Siri's help to ask user to pick one from the matches.
					resolutionResults.Add (INPersonResolutionResult.GetDisambiguation (matchingContacts.ToArray ()));
					break;
				}
			}

			// Report results to Siri
			completion (resolutionResults.ToArray ());
		}

		[Export ("resolveContentForSendMessage:withCompletion:")]
		public void ResolveContent (INSendMessageIntent intent, Action<INStringResolutionResult> completion)
		{
			var text = intent.Content;
			if (!string.IsNullOrEmpty (text))
				completion (INStringResolutionResult.GetSuccess (text));
			else
				completion (INStringResolutionResult.NeedsValue);
		}

		// Once resolution is completed, perform validation on the intent and provide confirmation (optional).
		[Export ("confirmSendMessage:completion:")]
		public void ConfirmSendMessage (INSendMessageIntent intent, Action<INSendMessageIntentResponse> completion)
		{
			// Verify user is authenticated and your app is ready to send a message.

			var userActivity = new NSUserActivity (nameof (INSendMessageIntent));
			var response = new INSendMessageIntentResponse (INSendMessageIntentResponseCode.Ready, userActivity);
			completion (response);
		}

		// Handle the completed intent (required).
		public void HandleSendMessage (INSendMessageIntent intent, Action<INSendMessageIntentResponse> completion)
		{
			// Implement your application logic to send a message here.

			//var userActivity = new NSUserActivity (nameof (INSendMessageIntent));
			var userActivity = new NSUserActivity ("com.appracatappra.askquestion");

			// Define details
			var info = new NSMutableDictionary ();
			info.Add (new NSString ("question"), new NSString (intent.Content));

			// Populate Activity
			userActivity.Title = "Ask Eliza a Question";
			userActivity.UserInfo = info;

			// Add App Search ability
			userActivity.EligibleForHandoff = true;
			userActivity.EligibleForSearch = true;
			userActivity.EligibleForPublicIndexing = true;
			userActivity.BecomeCurrent ();

			// Assemble response and send it
			var response = new INSendMessageIntentResponse (INSendMessageIntentResponseCode.InProgress, userActivity);
			completion (response);
		}
		// Implement handlers for each intent you wish to handle.
		// As an example for messages, you may wish to add HandleSearchForMessages and HandleSetMessageAttribute.

		public void HandleSearchForMessages (INSearchForMessagesIntent intent, Action<INSearchForMessagesIntentResponse> completion)
		{
			// Implement your application logic to find a message that matches the information in the intent.

			var userActivity = new NSUserActivity (nameof (INSearchForMessagesIntent));
			var response = new INSearchForMessagesIntentResponse (INSearchForMessagesIntentResponseCode.Success, userActivity);

			// Initialize with found message's attributes
			var sender = new INPerson (new INPersonHandle ("sarah@example.com", INPersonHandleType.EmailAddress), null, "Sarah", null, null, null);
			var recipient = new INPerson (new INPersonHandle ("+1-415-555-5555", INPersonHandleType.PhoneNumber), null, "John", null, null, null);
			var message = new INMessage ("identifier", "I am so excited about SiriKit!", NSDate.Now, sender, new INPerson [] { recipient });
			response.Messages = new INMessage [] { message };
			completion (response);
		}

		public void HandleSetMessageAttribute (INSetMessageAttributeIntent intent, Action<INSetMessageAttributeIntentResponse> completion)
		{
			// Implement your application logic to set the message attribute here.

			var userActivity = new NSUserActivity (nameof (INSetMessageAttributeIntent));
			var response = new INSetMessageAttributeIntentResponse (INSetMessageAttributeIntentResponseCode.Success, userActivity);
			completion (response);
		}
	}
}
