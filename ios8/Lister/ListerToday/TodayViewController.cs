using System;
using System.Drawing;

using NotificationCenter;
using Foundation;
using UIKit;

using Common;
using ListerKit;
using System.Collections.Generic;
using CoreGraphics;

namespace ListerToday
{
	[Register("TodayViewController")]
	public class TodayViewController : UITableViewController, INCWidgetProviding
	{
		const float TodayRowHeight = 44;
		const int TodayBaseRowCount = 5;

		static readonly NSString ContentCellIdentifier = new NSString("todayViewCell");
		static readonly NSString MessageCellIdentifier = new NSString("messageCell");

		ListDocument document;
		List empty = new List();
		List List {
			get {
				return document != null ? document.List : empty;
			}
		}

		NSMetadataQuery documentMetadataQuery;

		bool showingAll;
		bool ShowingAll {
			get {
				return showingAll;
			}
			set {
				if (showingAll == value)
					return;

				showingAll = value;
				ResetContentSize ();
			}
		}

		NSObject finishGatheringToken;

		bool IsCloudAvailable {
			get {
				return AppConfig.SharedAppConfiguration.IsCloudAvailable;
			}
		}

		bool IsTodayAvailable {
			get {
				return IsCloudAvailable && document != null && List != null;
			}
		}

		float PreferredViewHeight {
			get {
				int itemCount = IsTodayAvailable && List.Count > 0 ? List.Count : 1;
				int rowCount = ShowingAll ? itemCount : Math.Min (itemCount, TodayBaseRowCount + 1);
				return rowCount * TodayRowHeight;
			}
		}

		public TodayViewController (IntPtr handle)
			: base (handle)
		{
			Console.WriteLine (IntPtr.Size);
			Console.WriteLine ("TodayViewController");
		}

		public override void ViewDidLoad ()
		{
			Console.WriteLine ("TodayViewController ViewDidLoad");
			base.ViewDidLoad ();

			TableView.BackgroundColor = UIColor.Clear;

			if (IsCloudAvailable)
				StartQuery ();

			ResetContentSize ();
			TableView.ReloadData ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			if(finishGatheringToken != null)
				finishGatheringToken.Dispose ();

			if (IsTodayAvailable)
				document.Close (null);
		}

		#region NCWidgetProviding

		public void WidgetPerformUpdate (Action<NCUpdateResult> completionHandler)
		{
			Console.WriteLine ("WidgetPerformUpdate");
			if (completionHandler != null)
				completionHandler (NCUpdateResult.NewData);
		}

		public UIEdgeInsets GetWidgetMarginInsets (UIEdgeInsets defaultMarginInsets)
		{
			Console.WriteLine ("GetWidgetMarginInsets");
			return new UIEdgeInsets (defaultMarginInsets.Top, 27, defaultMarginInsets.Bottom, defaultMarginInsets.Right);
		}

		#endregion

		#region Query Management

		void StartQuery()
		{
			if (documentMetadataQuery == null) {
				documentMetadataQuery = new NSMetadataQuery ();
				documentMetadataQuery.SearchScopes = new NSObject[]{ NSMetadataQuery.UbiquitousDocumentsScope };

				string todayListName = AppConfig.SharedAppConfiguration.TodayDocumentNameAndExtension;
				documentMetadataQuery.Predicate = NSPredicate.FromFormat ("(%K = %@)", NSMetadataQuery.ItemFSNameKey,
					(NSString)todayListName);

				NSNotificationCenter notificationCenter = NSNotificationCenter.DefaultCenter;
				// TODO: subscribtion https://trello.com/c/1RyX6cJL
				finishGatheringToken = notificationCenter.AddObserver (NSMetadataQuery.DidFinishGatheringNotification,
					HandleMetadataQueryUpdates, documentMetadataQuery);
			}

			documentMetadataQuery.StartQuery ();
		}

		void HandleMetadataQueryUpdates(NSNotification notification)
		{
			documentMetadataQuery.DisableUpdates ();
			ProcessMetadataItems ();
			documentMetadataQuery.EnableUpdates();
		}

		async void ProcessMetadataItems()
		{
			Console.WriteLine ("ProcessMetadataItems");
			NSMetadataItem[] metadataItems = documentMetadataQuery.Results;

			// We only expect a single result to be returned by our NSMetadataQuery since we query for a specific file.
			if (metadataItems.Length == 1) {
				var url = (NSUrl)metadataItems [0].ValueForAttribute (NSMetadataQuery.ItemURLKey);

				if (document != null)
					document.Close (null);
				document = new ListDocument (url);

				var success = await document.OpenAsync ();
				if (!success) {
					Console.WriteLine ("Couldn't open document: {0}.", document.FileUrl.AbsoluteString);
					return;
				}

				ResetContentSize();
				TableView.ReloadData();
			}
		}

		#endregion

		#region UITableViewDataSource

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			if (!IsTodayAvailable)
				return 1;

			return ShowingAll ? List.Count : Math.Min(List.Count, TodayBaseRowCount + 1);
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			if (!IsCloudAvailable) {
				UITableViewCell cell = tableView.DequeueReusableCell (MessageCellIdentifier, indexPath);
				cell.TextLabel.Text = "Today requires iCloud";

				return cell;
			}

			int itemCount = List.Count;

			if (itemCount > 0) {
				ListItem item = List[indexPath.Row];

				if (ShowingAll && indexPath.Row == TodayBaseRowCount && itemCount != TodayBaseRowCount + 1) {
					UITableViewCell cell = tableView.DequeueReusableCell (MessageCellIdentifier, indexPath);
					cell.TextLabel.Text = "Show All...";

					return cell;
				} else {
					var cell = (CheckBoxCell)tableView.DequeueReusableCell (ContentCellIdentifier, indexPath);
					ConfigureListItemCell (cell, List.Color, item);

					return cell;
				}
			}

			var msgCell = tableView.DequeueReusableCell(MessageCellIdentifier, indexPath);
			msgCell.TextLabel.Text = IsTodayAvailable ? "No items in today's list" : string.Empty;

			return msgCell;
		}

		public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			cell.Layer.BackgroundColor = UIColor.Clear.CGColor;
		}

		void ConfigureListItemCell(CheckBoxCell itemCell, ListColor color, ListItem item)
		{
			itemCell.CheckBox.TintColor = AppColors.ColorFrom(color);
			itemCell.CheckBox.Checked = item.IsComplete;
			itemCell.Label.Text = item.Text;

			itemCell.Label.TextColor = UIColor.White;

			// Configure a completed list item cell.
			if (item.IsComplete)
				itemCell.Label.TextColor = UIColor.LightGray;
		}

		#endregion

		#region UITableViewDelegate

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			// Show all of the cells if the user taps the "Show All..." row.
			if (IsTodayAvailable && !ShowingAll && indexPath.Row == TodayBaseRowCount) {
				ShowingAll = true;

				TableView.BeginUpdates ();

				NSIndexPath indexPathForRemoval = NSIndexPath.FromRowSection (TodayBaseRowCount, 0);
				TableView.DeleteRows (new NSIndexPath[] { indexPathForRemoval }, UITableViewRowAnimation.Fade);

				int count = Math.Max (List.Count - TodayBaseRowCount, 0);
				NSIndexPath[] inserted = new NSIndexPath[count];
				for (int i = 0; i < count; i++)
					inserted[i] = NSIndexPath.FromRowSection (i + TodayBaseRowCount, 0);

				TableView.InsertRows (inserted, UITableViewRowAnimation.Fade);
				TableView.EndUpdates ();

				return;
			}

			// Open the main app if an item is tapped.
			NSUrl url = NSUrl.FromString ("lister://today");
			ExtensionContext.OpenUrl (url, null);
		}

		#endregion

		#region IBActions

		[Export("checkBoxTapped:")]
		public void CheckBoxTapped(CheckBox sender)
		{
			NSIndexPath indexPath = IndexPathForView (sender);

			ListItem item = List[indexPath.Row];
			ListOperationInfo info = List.ToggleItem (item, -1);
			if (info.FromIndex == info.ToIndex) {
				TableView.ReloadRows (new NSIndexPath[]{ indexPath }, UITableViewRowAnimation.Automatic);
			} else {
				int itemCount = List.Count;

				if (!ShowingAll && itemCount != TodayBaseRowCount && info.ToIndex > TodayBaseRowCount - 1) {
					// Completing has moved an item off the bottom of the short list.
					// Delete the completed row and insert a new row above "Show All...".
					NSIndexPath targetIndexPath = NSIndexPath.FromRowSection (TodayBaseRowCount - 1, 0);

					TableView.BeginUpdates ();
					TableView.DeleteRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Automatic);
					TableView.InsertRows (new NSIndexPath[]{ targetIndexPath }, UITableViewRowAnimation.Automatic);
					TableView.EndUpdates ();
				} else {
					// Need to animate the row up or down depending on its completion state.
					NSIndexPath targetIndexPath = NSIndexPath.FromRowSection (info.ToIndex, 0);

					TableView.BeginUpdates ();
					TableView.MoveRow (indexPath, targetIndexPath);
					TableView.EndUpdates ();
					TableView.ReloadRows (new NSIndexPath[] { targetIndexPath }, UITableViewRowAnimation.Automatic);
				}
			}

			// Notify the document of a change.
			document.UpdateChangeCount (UIDocumentChangeKind.Done);
		}

		#endregion

		#region Convenience

		void ResetContentSize()
		{
			CGSize preferredSize = PreferredContentSize;
			preferredSize.Height = PreferredViewHeight;
			PreferredContentSize = preferredSize;
		}

		NSIndexPath IndexPathForView(UIView view)
		{
			CGPoint viewOrigin = view.Bounds.Location;
			CGPoint viewLocation = TableView.ConvertPointFromView (viewOrigin, view);

			return TableView.IndexPathForRowAtPoint (viewLocation);
		}

		#endregion

	}
}

