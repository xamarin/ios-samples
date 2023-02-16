
namespace SimpleWatchConnectivity {
	using CoreFoundation;
	using Foundation;
	using WatchConnectivity;

	/// <summary>
	/// Every command handles the communication and notifies clients
	/// when WCSession status changes or data flows. Shared by the iOS app and watchOS app.
	/// </summary>
	public static class SessionCommands {
		/// <summary>
		/// Update app context if the session is activated and update UI with the command status.
		/// </summary>
		public static void UpdateAppContext (NSDictionary<NSString, NSObject> context)
		{
			var commandStatus = new CommandStatus (Command.UpdateAppContext, Phrase.Updated) { TimedColor = new TimedColor (context) };

			if (WCSession.DefaultSession.ActivationState == WCSessionActivationState.Activated) {
				WCSession.DefaultSession.UpdateApplicationContext (context, out NSError error);
				if (error != null) {
					commandStatus.Phrase = Phrase.Failed;
					commandStatus.ErrorMessage = error.LocalizedDescription;
				}

				PostNotificationOnMainQueueAsync (NotificationName.DataDidFlow, commandStatus);
			} else {
				HandleSessionUnactivated (commandStatus);
			}
		}

		/// <summary>
		/// Send a message if the session is activated and update UI with the command status.
		/// </summary>
		public static void SendMessage (NSDictionary<NSString, NSObject> message)
		{
			var commandStatus = new CommandStatus (Command.SendMessage, Phrase.Sent) { TimedColor = new TimedColor (message) };

			if (WCSession.DefaultSession.ActivationState == WCSessionActivationState.Activated) {
				WCSession.DefaultSession.SendMessage (message, (replyMessage) => {
					commandStatus.Phrase = Phrase.Replied;
					commandStatus.TimedColor = new TimedColor (replyMessage);
					PostNotificationOnMainQueueAsync (NotificationName.DataDidFlow, commandStatus);

				}, (error) => {
					commandStatus.Phrase = Phrase.Failed;
					commandStatus.ErrorMessage = error.LocalizedDescription;
					PostNotificationOnMainQueueAsync (NotificationName.DataDidFlow, commandStatus);
				});

				PostNotificationOnMainQueueAsync (NotificationName.DataDidFlow, commandStatus);

			} else {
				HandleSessionUnactivated (commandStatus);
			}
		}

		/// <summary>
		/// Send  a piece of message data if the session is activated and update UI with the command status.
		/// </summary>
		public static void SendMessageData (NSData messageData)
		{
			var commandStatus = new CommandStatus (Command.SendMessageData, Phrase.Sent) { TimedColor = TimedColor.Create (messageData) };

			if (WCSession.DefaultSession.ActivationState == WCSessionActivationState.Activated) {
				WCSession.DefaultSession.SendMessage (messageData, (replyData) => {
					commandStatus.Phrase = Phrase.Replied;
					commandStatus.TimedColor = TimedColor.Create (replyData);
					PostNotificationOnMainQueueAsync (NotificationName.DataDidFlow, commandStatus);
				}, (error) => {
					commandStatus.Phrase = Phrase.Failed;
					commandStatus.ErrorMessage = error.LocalizedDescription;
					PostNotificationOnMainQueueAsync (NotificationName.DataDidFlow, commandStatus);
				});

				PostNotificationOnMainQueueAsync (NotificationName.DataDidFlow, commandStatus);

			} else {
				HandleSessionUnactivated (commandStatus);
			}
		}

		/// <summary>
		/// Transfer a piece of user info if the session is activated and update UI with the command status.
		/// A WCSessionUserInfoTransfer object is returned to monitor the progress or cancel the operation.
		/// </summary>
		public static void TransferUserInfo (NSDictionary<NSString, NSObject> userInfo)
		{
			var commandStatus = new CommandStatus (Command.TransferUserInfo, Phrase.Transferring) { TimedColor = new TimedColor (userInfo) };

			if (WCSession.DefaultSession.ActivationState == WCSessionActivationState.Activated) {
				commandStatus.UserInfoTranser = WCSession.DefaultSession.TransferUserInfo (userInfo);
				PostNotificationOnMainQueueAsync (NotificationName.DataDidFlow, commandStatus);
			} else {
				HandleSessionUnactivated (commandStatus);
			}
		}

		/// <summary>
		/// Transfer a file if the session is activated and update UI with the command status.
		/// A WCSessionFileTransfer object is returned to monitor the progress or cancel the operation.
		/// </summary>
		public static void TransferFile (NSUrl file, NSDictionary<NSString, NSObject> metadata)
		{
			var commandStatus = new CommandStatus (Command.TransferFile, Phrase.Transferring) { TimedColor = new TimedColor (metadata) };

			if (WCSession.DefaultSession.ActivationState == WCSessionActivationState.Activated) {
				commandStatus.FileTransfer = WCSession.DefaultSession.TransferFile (file, metadata);
				PostNotificationOnMainQueueAsync (NotificationName.DataDidFlow, commandStatus);
			} else {
				HandleSessionUnactivated (commandStatus);
			}
		}

		/// <summary>
		/// Transfer a piece fo user info for current complications if the session is activated and update UI with the command status.
		/// A WCSessionUserInfoTransfer object is returned to monitor the progress or cancel the operation.
		/// </summary>
		public static void TransferCurrentComplicationUserInfo (NSDictionary<NSString, NSObject> userInfo)
		{
			var commandStatus = new CommandStatus (Command.TransferCurrentComplicationUserInfo, Phrase.Failed) { TimedColor = new TimedColor (userInfo) };

			if (WCSession.DefaultSession.ActivationState == WCSessionActivationState.Activated) {
				commandStatus.ErrorMessage = "Not supported on watchOS!";

#if __IOS__
                if (WCSession.DefaultSession.ComplicationEnabled)
                {
                    var userInfoTranser = WCSession.DefaultSession.TransferCurrentComplicationUserInfo(userInfo);
                    commandStatus.Phrase = Phrase.Transferring;
                    commandStatus.ErrorMessage = null;
                    commandStatus.UserInfoTranser = userInfoTranser;
                } 
                else 
                {
                    commandStatus.ErrorMessage = "\nComplication is not enabled!";
                }
#endif

				PostNotificationOnMainQueueAsync (NotificationName.DataDidFlow, commandStatus);
			} else {
				HandleSessionUnactivated (commandStatus);
			}
		}

		/// <summary>
		/// Post a notification on the main thread asynchronously.
		/// </summary>
		private static void PostNotificationOnMainQueueAsync (string name, CommandStatus @object)
		{
			DispatchQueue.MainQueue.DispatchAsync (() => {
				NSNotificationCenter.DefaultCenter.PostNotificationName (name, @object);
			});
		}

		/// <summary>
		/// Handle the session unactived error. WCSession commands require an activated session.
		/// </summary>
		private static void HandleSessionUnactivated (CommandStatus commandStatus)
		{
			var mutableStatus = commandStatus;
			mutableStatus.Phrase = Phrase.Failed;
			mutableStatus.ErrorMessage = "WCSession is not activeted yet!";
			PostNotificationOnMainQueueAsync (NotificationName.DataDidFlow, commandStatus);
		}
	}
}
