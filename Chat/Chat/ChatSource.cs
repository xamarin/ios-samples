using System;

using UIKit;
using System.Collections.Generic;
using Foundation;

namespace Chat
{
	public class ChatDelegate : UITableViewSource
	{
		static readonly NSString CellId = "ChatCellId";

		IList<Message> messages;

		public ChatDelegate(IList<Message> messages)
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
			UITableViewCell cell = tableView.DequeueReusableCell (CellId);
			cell = cell ?? new UITableViewCell ();

			var msg = messages [indexPath.Row];
			cell.TextLabel.Text = msg.Text;
		}
	}
}

