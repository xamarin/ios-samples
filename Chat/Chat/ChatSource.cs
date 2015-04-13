using System;

using UIKit;
using System.Collections.Generic;
using Foundation;

namespace Chat
{
	public class ChatSource : UITableViewSource
	{
		static readonly NSString IncomingCellId = new NSString("Incoming");
		static readonly NSString OutgoingCellId = new NSString("Outgoing");

		IList<Message> messages;

		public ChatSource(IList<Message> messages)
		{
			if (messages == null)
				throw new ArgumentNullException ("messages");

			this.messages = messages;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return messages.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = null;
			var msg = messages [indexPath.Row];

			if (msg.Type == MessageType.Incoming)
				cell = tableView.DequeueReusableCell (IncomingCellId);
			else
				cell = tableView.DequeueReusableCell (OutgoingCellId);

			cell.PrepareForReuse ();
			cell.TextLabel.Text = msg.Text;

			return cell;
		}
	}
}

