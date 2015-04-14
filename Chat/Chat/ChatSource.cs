using System;

using UIKit;
using System.Collections.Generic;
using Foundation;
using CoreGraphics;
using ObjCRuntime;

namespace Chat
{
	public class ChatSource : UITableViewSource
	{
		static readonly NSString IncomingCellId = new NSString("Incoming");
		static readonly NSString OutgoingCellId = new NSString("Outgoing");

		IList<Message> messages;

		readonly BubbleCell[] sizingCells;
		readonly Action<UIGestureRecognizer> showMenuAction;

		public ChatSource(IList<Message> messages, Action<UIGestureRecognizer> showMenuAction)
		{
			if (messages == null)
				throw new ArgumentNullException ("messages");

			this.messages = messages;
			this.showMenuAction = showMenuAction;
			sizingCells = new BubbleCell[2];
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return messages.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			BubbleCell cell = null;
			Message msg = messages [indexPath.Row];

			cell = (BubbleCell)tableView.DequeueReusableCell (GetReuseId (msg.Type));

			bool isNew = cell.Message == null;
			if (isNew) {
				var doubleTap = new UITapGestureRecognizer (ShowMenu);
				doubleTap.NumberOfTapsRequired = 2;
				cell.MessageLbl.AddGestureRecognizer (doubleTap);

				var longPressTap = new UILongPressGestureRecognizer (ShowMenu);
				cell.MessageLbl.AddGestureRecognizer (longPressTap);
			}

			cell.Message = msg;

			return cell;
		}

		public override NSIndexPath WillSelectRow (UITableView tableView, NSIndexPath indexPath)
		{
			return null; // Reserve row selection #CopyMessage
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			Message msg = messages [indexPath.Row];
			return CalculateHeightFor (msg, tableView);
		}

		public override nfloat EstimatedHeight (UITableView tableView, NSIndexPath indexPath)
		{
			Message msg = messages [indexPath.Row];
			return CalculateHeightFor (msg, tableView);
		}

		nfloat CalculateHeightFor(Message msg, UITableView tableView)
		{
			var index = (int)msg.Type;
			BubbleCell cell = sizingCells [index];
			if (cell == null)
				cell = sizingCells [index] = (BubbleCell)tableView.DequeueReusableCell (GetReuseId (msg.Type));

			cell.Message = msg; 

			cell.SetNeedsLayout ();
			cell.LayoutIfNeeded ();
			CGSize size = cell.ContentView.SystemLayoutSizeFittingSize (UIView.UILayoutFittingCompressedSize);
			return NMath.Ceiling (size.Height) + 1;
		}

		NSString GetReuseId(MessageType msgType)
		{
			return msgType == MessageType.Incoming ? IncomingCellId : OutgoingCellId;
		}

		void ShowMenu(UITapGestureRecognizer tapGesture)
		{
			if (tapGesture.State == UIGestureRecognizerState.Ended && showMenuAction != null)
				showMenuAction (tapGesture);
		}

		void ShowMenu(UILongPressGestureRecognizer longPress)
		{
			if (longPress.State == UIGestureRecognizerState.Began && showMenuAction != null)
				showMenuAction (longPress);
		}
	}
}