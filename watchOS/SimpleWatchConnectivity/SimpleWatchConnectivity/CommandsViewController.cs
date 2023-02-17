
namespace SimpleWatchConnectivity {
	using CoreFoundation;
	using CoreGraphics;
	using Foundation;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UIKit;
	using WatchConnectivity;

	public partial class CommandsViewController : UITableViewController {
		// List the supported methods, shown in the main table.
		private readonly List<Command> commands;

		private Command currentCommand = Command.UpdateAppContext; // Default to .updateAppContext.

		private UIColor currentColor;

		public CommandsViewController (IntPtr handle) : base (handle)
		{
			this.commands = Enum.GetValues (typeof (Command)).Cast<Command> ().ToList ();
		}

		~CommandsViewController ()
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (this);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.TableView.RowHeight = 42f;
			NSNotificationCenter.DefaultCenter.AddObserver (NotificationName.DataDidFlow, this.DataDidFlow);
		}

		private void DataDidFlow (NSNotification notification)
		{
			if (notification.Object is CommandStatus commandStatus) {
				this.currentCommand = commandStatus.Command;
				this.currentColor = commandStatus.TimedColor?.Color;
				this.TableView.ReloadData ();
			}
		}

		#region UITableViewDelegate and UITableViewDataSoruce.

		/// <summary>
		/// Create a button for the specified command and with the title color.
		/// The button is used as the accessory view of the table cell.
		/// </summary>
		private UIButton NewAccessoryView (Command cellCommand, UIColor titleColor)
		{
			var transferCount = 0;

			// Retrieve the transfer count for the command.
			if (cellCommand == Command.TransferFile) {
				transferCount = WCSession.DefaultSession.OutstandingFileTransfers?.Length ?? 0;
			} else if (cellCommand == Command.TransferUserInfo) {
				transferCount = WCSession.DefaultSession.OutstandingUserInfoTransfers?.Count (transfer => !transfer.CurrentComplicationInfo) ?? 0;
			} else if (cellCommand == Command.TransferCurrentComplicationUserInfo) {
				transferCount = WCSession.DefaultSession.OutstandingUserInfoTransfers?.Count (transfer => transfer.CurrentComplicationInfo) ?? 0;
			}

			// Create and configure the button.
			var button = new UIButton (UIButtonType.RoundedRect);
			button.AddTarget ((sender, args) => this.ShowTransfers (sender as UIButton), UIControlEvent.TouchUpInside);
			button.SetTitleColor (titleColor, UIControlState.Normal);
			button.SetTitle ($" {transferCount} ", UIControlState.Normal);
			button.SizeToFit ();
			return button;
		}

		/// <summary>
		/// Action handler of the accessory view. Present the view controller for the current command.
		/// </summary>
		private void ShowTransfers (UIButton sender)
		{
			var buttonPosition = sender.ConvertPointToView (CGPoint.Empty, this.TableView);
			var indexPath = this.TableView.IndexPathForRowAtPoint (buttonPosition);
			if (indexPath != null) {
				var storyboard = UIStoryboard.FromName ("Main", null);
				var command = this.commands [indexPath.Row];

				UIViewController childViewController;

				if (command == Command.TransferFile) {
					var viewController = storyboard.InstantiateViewController ("FileTransfersViewController");
					if (viewController is FileTransfersViewController transfersViewController) {
						transfersViewController.Command = command;
						childViewController = transfersViewController;
					} else {
						throw new Exception ("View controller (FileTransfersViewController) deson't have a right class!");
					}
				} else {
					var viewController = storyboard.InstantiateViewController ("UserInfoTransfersViewController");
					if (viewController is UserInfoTransfersViewController transfersViewController) {
						transfersViewController.Command = command;
						childViewController = transfersViewController;
					} else {
						throw new Exception ("View controller (UserInfoTransfersViewController) deson't have a right class!");
					}
				}

				this.AddChildViewController (childViewController);
				childViewController.View.Frame = this.View.ConvertRectToView (this.TableView.Bounds, this.TableView);
				this.View.AddSubview (childViewController.View);
				childViewController.DidMoveToParentViewController (this);
			}
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return this.commands.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("CommandCell", indexPath);

			var cellCommand = this.commands [indexPath.Row];
			cell.TextLabel.Text = cellCommand.ToString ();

			UIColor textColor = cellCommand == currentCommand ? this.currentColor : null;
			cell.TextLabel.TextColor = textColor;
			cell.DetailTextLabel.TextColor = textColor;

			cell.DetailTextLabel.Text = string.Empty;
			cell.AccessoryView = null;

			if (cellCommand == Command.TransferFile ||
				cellCommand == Command.TransferUserInfo ||
				cellCommand == Command.TransferCurrentComplicationUserInfo) {
				cell.AccessoryView = this.NewAccessoryView (cellCommand, textColor);
			}

			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			this.currentCommand = this.commands [indexPath.Row];
			switch (this.currentCommand) {
			case Command.UpdateAppContext:
				SessionCommands.UpdateAppContext (TestDataProvider.AppContext);
				break;
			case Command.SendMessage:
				SessionCommands.SendMessage (TestDataProvider.Message);
				break;
			case Command.SendMessageData:
				SessionCommands.SendMessageData (TestDataProvider.MessageData);
				break;
			case Command.TransferUserInfo:
				SessionCommands.TransferUserInfo (TestDataProvider.UserInfo);
				break;
			case Command.TransferFile:
				SessionCommands.TransferFile (TestDataProvider.File, TestDataProvider.FileMetaData);
				break;
			case Command.TransferCurrentComplicationUserInfo:
				SessionCommands.TransferCurrentComplicationUserInfo (TestDataProvider.CurrentComplicationInfo);
				break;
			}
		}

		#endregion
	}
}
