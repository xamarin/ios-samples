/*
 SessionTransfer protocol defines the session transfer interface.
 Its extension implements the cancel method to cancel the transfer and notify UI.
 Used on both iOS and watchOS.
*/

namespace SimpleWatchConnectivity {
	using CoreFoundation;
	using Foundation;
	using WatchConnectivity;

	// Implement the cancel method to cancel the transfer and notify UI.
	//
	public class SessionTransfer {
		public WCSessionFileTransfer SessionFileTransfer { get; set; }

		public WCSessionUserInfoTransfer SessionUserInfoTransfer { get; set; }

		public TimedColor TimedColor {
			get {
				TimedColor result = null;
				if (this.SessionFileTransfer != null) {
					result = new TimedColor (this.SessionFileTransfer.File.Metadata);
				} else if (this.SessionUserInfoTransfer != null) {
					result = new TimedColor (this.SessionUserInfoTransfer.UserInfo);
				}

				return result;
			}
		}

		public void Cancel (Command command)
		{
			var commandStatus = new CommandStatus (command, Phrase.Canceled) { TimedColor = TimedColor };
			this.Cancel ();

			var dateFormatter = new NSDateFormatter { TimeStyle = NSDateFormatterStyle.Medium };
			if (commandStatus.TimedColor != null) {
				commandStatus.TimedColor.TimeStamp = dateFormatter.StringFor (new NSDate ());
			}

			DispatchQueue.MainQueue.DispatchAsync (() => {
				NSNotificationCenter.DefaultCenter.PostNotificationName (NotificationName.DataDidFlow, commandStatus);
			});
		}

		public void Cancel ()
		{
			if (this.SessionFileTransfer != null) {
				this.SessionFileTransfer.Cancel ();
			} else if (this.SessionUserInfoTransfer != null) {
				this.SessionUserInfoTransfer.Cancel ();
			}
		}
	}
}
