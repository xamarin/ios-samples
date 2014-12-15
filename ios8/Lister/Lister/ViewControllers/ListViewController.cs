using System;

using UIKit;
using Foundation;

using Common;
using ListerKit;
using NotificationCenter;
using System.Drawing;
using CoreGraphics;

namespace Lister
{
	[Register("ListViewController")]
	public class ListViewController : UITableViewController, IUITextFieldDelegate
	{
		const string EmptyViewControllerStoryboardIdentifier = "emptyViewController";

		// Notification User Info Keys
		public static readonly NSString ListDidUpdateColorUserInfoKey = new NSString("ListDidUpdateColorUserInfoKey");
		public static readonly NSString ListDidUpdateURLUserInfoKey = new NSString("ListDidUPdateURLUserInfoKey");

		// UITableViewCell Identifiers
		static readonly NSString ListItemCellIdentifier = new NSString("listItemCell");
		static readonly NSString ListColorCellIdentifier = new NSString("listColorCell");

		ListDocument document;
		UIBarButtonItem[] listToolbarItems;

		public NSUrl DocumentURL {
			get {
				return document.FileUrl;
			}
		}

		List List {
			get {
				return document.List;
			}
		}

		public DocumentsViewController MasterController { get; set; }

		NSObject docStateChangeToken;

		UIStringAttributes textAttributes;
		UIStringAttributes TextAttributes {
			get {
				return textAttributes;
			}
			set {
				textAttributes = value;

				if(IsViewLoaded)
					UpdateInterfaceWithTextAttributes ();
			}
		}

		public ListViewController(IntPtr handle)
			: base (handle)
		{
			string title = "Delete List";
			UIBarButtonItem deleteList = new UIBarButtonItem (title, UIBarButtonItemStyle.Plain, DeleteList);

			UIBarButtonItem flexibleSpace = new UIBarButtonItem (UIBarButtonSystemItem.FixedSpace);

			listToolbarItems = new UIBarButtonItem[] {
				flexibleSpace,
				deleteList,
				flexibleSpace
			};
		}

		#region View Life Cycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			UpdateInterfaceWithTextAttributes ();

			// Use the edit button item provided by the table view controller.
			NavigationItem.RightBarButtonItem = EditButtonItem;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			UpdateList ();

			// TODO: use strong type subscribtion version https://trello.com/c/1RyX6cJL
			docStateChangeToken = NSNotificationCenter.DefaultCenter.AddObserver (UIDocument.StateChangedNotification,
				HandleDocumentStateChangedNotification, document);
		}

		async void UpdateList ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;

			var success = await document.OpenAsync ();
			if (!success) {
				// In your app you should handle this gracefully.
				Console.WriteLine ("Couldn't open document: {0}.", DocumentURL.AbsoluteString);
				throw new InvalidProgramException ();
			}
			TableView.ReloadData ();

			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			if(document != null)
				document.Close (null);

			// Unsibscribe from UIDocument.StateChangedNotification
			if(docStateChangeToken != null)
				docStateChangeToken.Dispose ();

			// Hide the toolbar so the list can't be edited.
			NavigationController.SetToolbarHidden (true, animated);
		}

		#endregion

		#region Setup

		public async void ConfigureWith(ListInfo listInfo)
		{
			await listInfo.FetchInfoAsync ();

			if (document != null) {
				document.DocumentDeleted -= OnListDocumentWasDeleted;
				document.Close (null);
			}

			document = new ListDocument(listInfo.Url);
			document.DocumentDeleted += OnListDocumentWasDeleted;

			NavigationItem.Title = listInfo.Name;

			TextAttributes = new UIStringAttributes {
				Font = UIFont.PreferredHeadline,
				ForegroundColor = AppColors.ColorFrom(listInfo.Color.Value)
			};
		}

		#endregion

		#region Notifications

		void HandleDocumentStateChangedNotification(NSNotification notification)
		{
			UIDocumentState state = document.DocumentState;

			if ((state & UIDocumentState.InConflict) != 0)
				ResolveConflicts ();

			InvokeOnMainThread (() => {
				TableView.ReloadData();
			});
		}

		#endregion

		#region UIViewController Overrides

		public override void SetEditing (bool editing, bool animated)
		{
			base.SetEditing (editing, animated);

			// Prevent navigating back in edit mode.
			NavigationItem.SetHidesBackButton (editing, animated);

			// Reload the first row to switch from "Add Item" to "Change Color"
			NSIndexPath indexPath = NSIndexPath.FromRowSection (0, 0);
			TableView.ReloadRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Automatic);

			// If moving out of edit mode, notify observers about the list color and trigger a save.
			if (!editing) {
				// Notify the document of a change.
				document.UpdateChangeCount (UIDocumentChangeKind.Done);

				MasterController.UpdateDocumentColor (DocumentURL, List.Color);

				TriggerNewDataForWidget ();
			}

			NavigationController.SetToolbarHidden (!editing, animated);
			NavigationController.Toolbar.SetItems (listToolbarItems, animated);
		}

		#endregion

		#region UITableViewDataSource

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			// Don't show anything if the document hasn't been loaded.
			// We show the items in a list, plus a separate row that lets users enter a new item.
			return document == null || List == null ? 0 : List.Count + 1;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			if (Editing && indexPath.Row == 0) {
				var colorCell = (ListColorCell)tableView.DequeueReusableCell (ListColorCellIdentifier, indexPath);

				colorCell.Configure ();
				colorCell.ViewController = this;

				return colorCell;
			} else {
				var itemCell = (ListItemCell)tableView.DequeueReusableCell (ListItemCellIdentifier, indexPath);
				ConfigureListItemCell (itemCell, indexPath.Row);

				return itemCell;
			}
		}

		void ConfigureListItemCell(ListItemCell itemCell, int row)
		{
			itemCell.TextField.Font = UIFont.PreferredBody;
			itemCell.TextField.WeakDelegate = this;

			if (row == 0) {
				// Configure an "Add Item" list item cell.
				itemCell.TextField.Placeholder = "Add Item";
				itemCell.CheckBox.Hidden = true;
			} else {
				ListItem item = List[row - 1];

				itemCell.Completed = item.IsComplete;
				itemCell.TextField.Text = item.Text;
			}
		}

		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			// The initial row is reserved for adding new items so it can't be deleted or edited.
			return indexPath.Row != 0;
		}

		public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
		{
			// The initial row is reserved for adding new items so it can't be moved.
			return indexPath.Row != 0;
		}

		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (editingStyle != UITableViewCellEditingStyle.Delete)
				return;

			ListItem item = List[indexPath.Row - 1];
			List.RemoveItems (new ListItem[]{ item });

			TableView.DeleteRows (new NSIndexPath[]{ indexPath }, UITableViewRowAnimation.Automatic);

			TriggerNewDataForWidget ();

			// Notify the document of a change.
			document.UpdateChangeCount (UIDocumentChangeKind.Done);
		}

		public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		{
			ListItem item = List[sourceIndexPath.Row - 1];
			List.MoveItem (item, destinationIndexPath.Row - 1);

			// Notify the document of a change.
			document.UpdateChangeCount (UIDocumentChangeKind.Done);
		}

		#endregion

		#region UITableViewDelegate

		public override void WillBeginEditing (UITableView tableView, NSIndexPath indexPath)
		{
			// When the user swipes to show the delete confirmation, don't enter editing mode.
			// UITableViewController enters editing mode by default so we override without calling super.
		}

		public override void DidEndEditing (UITableView tableView, NSIndexPath indexPath)
		{
			// When the user swipes to hide the delete confirmation, no need to exit edit mode because we didn't enter it.
			// UITableViewController enters editing mode by default so we override without calling super.
		}

		public override NSIndexPath CustomizeMoveTarget (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath proposedIndexPath)
		{
			ListItem item = List[sourceIndexPath.Row - 1];

			int row;
			if (proposedIndexPath.Row == 0) {
				row = item.IsComplete ? List.IndexOfFirstCompletedItem() + 1 : 1;
				return NSIndexPath.FromRowSection (row, 0);
			} else if (List.CanMoveItem(item, proposedIndexPath.Row - 1, false)) {
				return proposedIndexPath;
			} else if (item.IsComplete) {
				row = List.IndexOfFirstCompletedItem () + 1;
				return NSIndexPath.FromRowSection (row, 0);
			} else {
				row = List.IndexOfFirstCompletedItem ();
				return NSIndexPath.FromRowSection (row, 0);
			}
		}

		#endregion

		#region UITextFieldDelegate

		[Export ("textFieldDidEndEditing:")]
		public void EditingEnded (UITextField textField)
		{
			NSIndexPath indexPath = IndexPathForView (textField);

			if (indexPath.Row > 0) {
				// Edit the item in place.
				ListItem item = List[indexPath.Row - 1];

				// If the contents of the text field at the end of editing is the same as it started, don't trigger an update.
				if (item.Text != textField.Text) {
					item.Text = textField.Text;

					TriggerNewDataForWidget ();

					// Notify the document of a change.
					document.UpdateChangeCount (UIDocumentChangeKind.Done);
				}
			} else if (textField.Text.Length > 0) {
				// Adds the item to the top of the list.
				ListItem item = new ListItem (textField.Text);
				int insertedIndex = List.InsertItem (item);

				// Update the edit row to show the check box.
				ListItemCell itemCell = (ListItemCell)TableView.CellAt (indexPath);
				itemCell.CheckBox.Hidden = false;

				// Insert a new add item row into the table view.
				TableView.BeginUpdates ();

				NSIndexPath targetIndexPath = NSIndexPath.FromRowSection (insertedIndex, 0);
				TableView.InsertRows (new NSIndexPath[] { targetIndexPath }, UITableViewRowAnimation.Automatic);

				TableView.EndUpdates ();

				TriggerNewDataForWidget ();

				// Notify the document of a change.
				document.UpdateChangeCount (UIDocumentChangeKind.Done);
			}
		}

		[Export ("textFieldShouldReturn:")]
		public bool ShouldReturn (UITextField textField)
		{
			NSIndexPath indexPath = IndexPathForView(textField);

			// An item must have text to dismiss the keyboard.
			if (!string.IsNullOrEmpty(textField.Text) || indexPath.Row == 0) {
				textField.ResignFirstResponder ();
				return true;
			}

			return false;
		}

		#endregion

		public void OnListColorCellDidChangeSelectedColor (ListColor color)
		{
			List.Color = color;

			TextAttributes = new UIStringAttributes {
				Font = UIFont.PreferredHeadline,
				ForegroundColor = AppColors.ColorFrom (List.Color)
			};

			NSIndexPath[] indexPaths = TableView.IndexPathsForVisibleRows;
			TableView.ReloadRows (indexPaths, UITableViewRowAnimation.None);
		}

		#region IBActions

		void DeleteList(object sender, EventArgs e)
		{
			if (SplitViewController.Collapsed) {
				NavigationController.PopViewController (true);
			} else {
				UIViewController emptyViewController = (UIViewController)Storyboard.InstantiateViewController (EmptyViewControllerStoryboardIdentifier);
				SplitViewController.ShowDetailViewController (emptyViewController, null);
			}

			MasterController.ListViewControllerDidDeleteList (this);
		}

		[Export("checkBoxTapped:")]
		public void CheckBoxTapped(CheckBox sender)
		{
			NSIndexPath indexPath = IndexPathForView (sender);

			if (indexPath.Row >= 1 && indexPath.Row <= List.Count) {
				ListItem item = List[indexPath.Row - 1];
				ListOperationInfo info = List.ToggleItem (item, -1);

				if (info.FromIndex == info.ToIndex) {
					TableView.ReloadRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Automatic);
				} else {
					// Animate the row up or down depending on whether it was complete/incomplete.
					NSIndexPath target = NSIndexPath.FromRowSection (info.ToIndex + 1, 0);

					TableView.BeginUpdates ();
					TableView.MoveRow (indexPath, target);
					TableView.EndUpdates ();
					TableView.ReloadRows (new NSIndexPath[] { target }, UITableViewRowAnimation.Automatic);
				}

				TriggerNewDataForWidget ();

				// notify the document that we've made a change
				document.UpdateChangeCount (UIDocumentChangeKind.Done);
			}
		}

		#endregion

		void OnListDocumentWasDeleted (object sender, EventArgs e)
		{
			var document = (ListDocument)sender;
			document.DocumentDeleted -= OnListDocumentWasDeleted;

			InvokeOnMainThread (() => {
				DismissViewController(true, null);
			});
		}

		#region Convenience

		void TriggerNewDataForWidget()
		{
			string todayDocumentName = AppConfig.SharedAppConfiguration.TodayDocumentName;

			if (document.LocalizedName != todayDocumentName)
				return;

			// You can use hardcoded value for widget bundle identifier
			// But it is a good idea to move this value to some kind of setting
			// Info.plist works for this purpose
			var widgetBundleId = (string)(NSString)NSBundle.MainBundle.ObjectForInfoDictionary ("WidgetBundleIdentifier");
			NCWidgetController.GetWidgetController ().SetHasContent (true, widgetBundleId);
		}

		void UpdateInterfaceWithTextAttributes()
		{
			NavigationController.NavigationBar.TitleTextAttributes = TextAttributes;

			UIColor color = TextAttributes.ForegroundColor;
			NavigationController.NavigationBar.TintColor = color;
			NavigationController.Toolbar.TintColor = color;
			TableView.TintColor = color;
		}

		void ResolveConflicts()
		{
			// Any automatic merging logic or presentation of conflict resolution UI should go here.
			// For this sample, just pick the current version and mark the conflict versions as resolved.
			NSError error = null;
			NSFileVersion.RemoveOtherVersions (DocumentURL, out error);

			NSFileVersion[] conflictVersions = NSFileVersion.GetUnresolvedConflictVersions (DocumentURL);
			foreach (NSFileVersion fileVersion in conflictVersions)
				fileVersion.Resolved = true;
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
