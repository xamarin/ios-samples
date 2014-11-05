using System;
using Foundation;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe
{
	public class TTTMessageServer
	{
		public const string DidAddMessagesNotification = "DidAddMessagesNotification";
		public const string AddedMessageIndexesUserInfoKey = "AddedMessageIndexesUserInfoKey";
		List<TTTMessage> messages;
		List<TTTMessage> favoriteMessages;

		public TTTMessageServer ()
		{
			messages = new List<TTTMessage> ();
			favoriteMessages = new List<TTTMessage> ();

			ReadMessages ();
		}

		static TTTMessageServer sharedMessageServer;

		public static TTTMessageServer SharedMessageServer {
			get {
				if (sharedMessageServer == null)
					sharedMessageServer = new TTTMessageServer ();

				return sharedMessageServer;
			}
		}

		public NSUrl MessagesUrl {
			get {
				NSError error;
				return NSFileManager.DefaultManager.GetUrl (NSSearchPathDirectory.DocumentDirectory,
				                                                 NSSearchPathDomain.User, null, true, out error);
			}
		}

		public void ReadMessages ()
		{
			NSData data = NSData.FromUrl (MessagesUrl);
			if (data != null)
				messages = (List<TTTMessage>)TTTProfile.ByteArrayToObject (data.ToArray ());
		}

		public void WriteMessages ()
		{
			NSError error;

			byte[] bytes = TTTProfile.ObjectToByteArray (messages);
			NSData data = NSData.FromArray (bytes);
			data.Save (MessagesUrl, false, out error);
		}

		public int NumberOfMessages {
			get {
				return messages.Count;
			}
		}

		public TTTMessage MessageAtIndex (int index)
		{
			return messages [index];
		}

		public void AddMessage (TTTMessage message)
		{
			int messageIndex = 0;
			messages.Insert (0, message);

			NSDictionary userInfo = 
				NSDictionary.FromObjectAndKey (NSArray.FromNSObjects (new NSNumber (messageIndex)),
			                                new NSString (AddedMessageIndexesUserInfoKey));
			NSNotificationCenter.DefaultCenter.PostNotificationName (DidAddMessagesNotification,
			                                                         null, userInfo);
			WriteMessages ();
		}

		public bool IsFavoriteMessage (TTTMessage message)
		{
			return favoriteMessages.Contains (message);
		}

		public void SetFavorite (bool favorite, TTTMessage message)
		{
			if (favorite)
				favoriteMessages.Add (message);
			else
				favoriteMessages.Remove (message);
		}
	}
}

