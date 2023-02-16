
namespace SimpleWatchConnectivity {
	using CoreGraphics;
	using Foundation;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UIKit;
	using WatchConnectivity;

	/// <summary>
	/// UserInfoTransfersViewController manages the UserInfo transfer of the iOS app.
	/// </summary>
	public partial class UserInfoTransfersViewController : UIViewController, IUITableViewDelegate, IUITableViewDataSource {
		public UserInfoTransfersViewController (IntPtr handle) : base (handle) { }

		~UserInfoTransfersViewController ()
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (this);
		}

		[Outlet]
		protected UITableView TableView { get; set; }

		public Command Command { get; set; }

		// Outstanding transfers can change in the background so make a copy (cache) to
		// make sure the data doesn't change during the table view loading cycle.
		// Subclasses can override the computed property to provide the right transfers.
		protected List<SessionTransfer> TransfersStore { get; set; }

		protected virtual List<SessionTransfer> Transfers {
			get {
				if (this.TransfersStore == null) {
					List<WCSessionUserInfoTransfer> transfersStore = null;
					if (this.Command == Command.TransferUserInfo) {
						transfersStore = WCSession.DefaultSession.OutstandingUserInfoTransfers.Where (transfer => !transfer.CurrentComplicationInfo).ToList ();
					} else if (this.Command == Command.TransferCurrentComplicationUserInfo) {
						transfersStore = WCSession.DefaultSession.OutstandingUserInfoTransfers.Where (transfer => transfer.CurrentComplicationInfo).ToList ();
					}

					this.TransfersStore = transfersStore.Select (transfer => new SessionTransfer { SessionUserInfoTransfer = transfer }).ToList ();
				}

				return this.TransfersStore;
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.TableView.Delegate = this;
			this.TableView.DataSource = this;
			NSNotificationCenter.DefaultCenter.AddObserver (NotificationName.DataDidFlow, this.DataDidFlow);
		}

		#region Actions handlers.

		partial void Dismiss (UIButton sender)
		{
			this.WillMoveToParentViewController (null);
			this.View.RemoveFromSuperview ();
			this.RemoveFromParentViewController ();
		}

		private void Cancel (UIButton sender)
		{
			var buttonOrigin = sender.ConvertPointToView (CGPoint.Empty, this.TableView);
			var indexPath = this.TableView.IndexPathForRowAtPoint (buttonOrigin);
			if (indexPath != null) {
				var transfer = this.Transfers [indexPath.Row];
				transfer.Cancel (this.Command);
			}
		}

		private void DataDidFlow (NSNotification notification)
		{
			if (notification.Object is CommandStatus commandStatus) {
				// Invalidate the cached transfers and reload the table view with animation
				// if the notification command is relevant and is not failed.
				if (commandStatus.Command == this.Command && commandStatus.Phrase != Phrase.Failed) {
					this.TransfersStore = null;
					this.TableView.ReloadSections (new NSIndexSet (0), UITableViewRowAnimation.Automatic);
				}
			}
		}

		#endregion

		#region UITableViewDelegate, UITableViewDataSource

		public nint RowsInSection (UITableView tableView, nint section)
		{
			return this.Transfers.Count;
		}

		public virtual UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("TransferCell", indexPath);

			var transfer = this.Transfers [indexPath.Row];
			cell.AccessoryView = this.NewAccessoryView (transfer.TimedColor.Color);
			cell.TextLabel.Text = transfer.TimedColor.TimeStamp;
			cell.TextLabel.TextColor = transfer.TimedColor.Color;
			cell.DetailTextLabel.Text = string.Empty;

			return cell;
		}

		private UIButton NewAccessoryView (UIColor titleColor)
		{
			var button = new UIButton (UIButtonType.RoundedRect);
			button.SetTitle ("  X   ", UIControlState.Normal);
			button.SetTitleColor (titleColor, UIControlState.Normal);
			button.SizeToFit ();
			button.AddTarget ((sender, e) => this.Cancel (sender as UIButton), UIControlEvent.TouchUpInside);

			return button;
		}

		#endregion
	}
}
