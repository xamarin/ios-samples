using System;

using UIKit;
using System.Collections.Generic;
using Foundation;
using CoreGraphics;

namespace Chat
{
	public class ChatSource : UITableViewSource
	{
		static readonly NSString IncomingCellId = new NSString("Incoming");
		static readonly NSString OutgoingCellId = new NSString("Outgoing");

		IList<Message> messages;

		OutgoingCell outgoingSizingCell;
		IncomingCell incomingSizingCell;

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
			Message msg = messages [indexPath.Row];

			if (msg.Type == MessageType.Incoming)
				cell = tableView.DequeueReusableCell (IncomingCellId);
			else
				cell = tableView.DequeueReusableCell (OutgoingCellId);

			cell.PrepareForReuse ();
			cell.TextLabel.Text = msg.Text;

			return cell;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			Message msg = messages [indexPath.Row];
			return CalculateHeightFor (msg, tableView);
		}

		nfloat CalculateHeightFor(Message msg, UITableView tableView)
		{
			UITableViewCell cell;
			switch (msg.Type) {
				case MessageType.Incoming:
					incomingSizingCell = incomingSizingCell ?? (IncomingCell)tableView.DequeueReusableCell (IncomingCellId);
					cell = incomingSizingCell;
					break;

				case MessageType.Outgoing:
					outgoingSizingCell = outgoingSizingCell ?? (OutgoingCell)tableView.DequeueReusableCell (OutgoingCellId);
					cell = outgoingSizingCell;
					break;

				default:
					throw new NotImplementedException ();
			}

			cell.TextLabel.Text = msg.Text;

			cell.SetNeedsLayout ();
			cell.LayoutIfNeeded ();
			CGSize size = cell.ContentView.SystemLayoutSizeFittingSize (UIView.UILayoutFittingCompressedSize);
			return NMath.Ceiling (size.Height);
		}

		public override nfloat EstimatedHeight (UITableView tableView, NSIndexPath indexPath)
		{
			Message msg = messages [indexPath.Row];
			return CalculateHeightFor (msg, tableView);
		}
	}
}

