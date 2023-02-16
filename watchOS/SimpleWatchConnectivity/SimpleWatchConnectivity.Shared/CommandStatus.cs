
namespace SimpleWatchConnectivity {
	using System;
	using Foundation;
	using UIKit;
	using WatchConnectivity;

	/// <summary>
	/// Constants to identify the Watch Connectivity methods, also used as user-visible strings in UI.
	/// </summary>
	public enum Command {
		UpdateAppContext,
		SendMessage,
		SendMessageData,
		TransferFile,
		TransferUserInfo,
		TransferCurrentComplicationUserInfo
	}

	/// <summary>
	/// Constants to identify the phrases of a Watch Connectivity communication.
	/// </summary>
	public enum Phrase {
		Updated,
		Sent,
		Received,
		Replied,
		Transferring,
		Canceled,
		Finished,
		Failed,
	}

	/// <summary>
	/// Wrap a timed color payload dictionary with a stronger type.
	/// </summary>
	public class TimedColor {
		public TimedColor (NSDictionary timedColor)
		{
			if (timedColor.TryGetValue (PayloadKey.TimeStamp, out NSObject timeStamp) &&
			   timedColor.TryGetValue (PayloadKey.ColorData, out NSObject colorData)) {
				this.TimeStamp = (timeStamp as NSString).ToString ();
				this.ColorData = colorData as NSData;
			} else {
				throw new Exception ("Timed color dictionary doesn't have right keys!");
			}
		}

		public static TimedColor Create (NSData timedColor)
		{
			var data = NSKeyedUnarchiver.UnarchiveTopLevelObject (timedColor, out NSError error);
			if (data is NSDictionary dictionary) {
				return new TimedColor (dictionary);
			} else {
				throw new Exception ("Failed to unarchive a timedColor dictionary!");
			}
		}

		public string TimeStamp { get; set; }

		public NSData ColorData { get; set; }

		public UIColor Color {
			get {
				var optional = NSKeyedUnarchiver.GetUnarchivedObject (typeof (UIColor), this.ColorData, out NSError error);
				if (!(optional is UIColor color)) {
					throw new Exception ("Failed to unarchive a UIClor object!");
				}

				return color;
			}
		}
	}

	/// <summary>
	/// Wrap the command status to bridge the commands status and UI.
	/// </summary>
	public class CommandStatus : NSObject {
		public CommandStatus (Command command, Phrase phrase)
		{
			this.Command = command;
			this.Phrase = phrase;
		}

		public Command Command { get; set; }
		public Phrase Phrase { get; set; }
		public TimedColor TimedColor { get; set; }
		public WCSessionFileTransfer FileTransfer { get; set; }
		public WCSessionFile File { get; set; }
		public WCSessionUserInfoTransfer UserInfoTranser { get; set; }
		public string ErrorMessage { get; set; }
	}
}
