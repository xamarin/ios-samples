using System;

using UIKit;
using Foundation;
using CoreGraphics;
using NotificationCenter;

using ListerKit;
using CoreFoundation;

namespace Lister
{
	[Register ("ListViewController")]
	public class ListViewController : UITableViewController, IListPresenterDelegate, IListColorCellDelegate, IListDocumentDelegate, IUITextFieldDelegate
	{
		//		const string EmptyViewControllerStoryboardIdentifier = "emptyViewController";

		// Notification User Info Keys
		//		public static readonly NSString ListDidUpdateColorUserInfoKey = new NSString("ListDidUpdateColorUserInfoKey");
		//		public static readonly NSString ListDidUpdateURLUserInfoKey = new NSString("ListDidUPdateURLUserInfoKey");

		// UITableViewCell Identifiers
		static readonly NSString ListItemCellIdentifier = new NSString ("listItemCell");
		static readonly NSString ListColorCellIdentifier = new NSString ("listColorCell");

		NSObject docStateChangeToken;
		UITextField activeTextField;
		ListInfo listInfo;

		// Return the toolbar items since they are used in edit mode.
		UIBarButtonItem[] ListToolbarItems {
			get {
				UIBarButtonItem flexibleSpace = new UIBarButtonItem (UIBarButtonSystemItem.FixedSpace);

				string title = "Delete List";
				UIBarButtonItem deleteList = new UIBarButtonItem (title, UIBarButtonItemStyle.Plain, DeleteList);
				deleteList.TintColor = UIColor.Red;

				if (DocumentURL.LastPathComponent == AppConfig.SharedAppConfiguration.TodayDocumentNameAndExtension)
					deleteList.Enabled = false;

				return new UIBarButtonItem[]{ flexibleSpace, deleteList, flexibleSpace };
			}
		}

		AllListItemsPresenter ListPresenter {
			get {
				return (AllListItemsPresenter)document.ListPresenter;
			}
		}

		public override NSUndoManager UndoManager {
			get {
				return document.UndoManager;
			}
		}

		NSUrl DocumentURL {
			get {
				return Document.FileUrl;
			}
		}

		ListDocument document;

		public ListDocument Document {
			get {
				return document;
			}
			private set {
				document = value;
				document.Delegate = this;
				ListPresenter.UndoManager = UndoManager;
				ListPresenter.Delegate = this;
			}
		}

		public ListsController ListsController { get; set; }

		UIStringAttributes textAttributes;

		public UIStringAttributes TextAttributes {
			get {
				return textAttributes;
			}
			private set {
				textAttributes = value;

				if (IsViewLoaded)
					UpdateInterfaceWithTextAttributes ();
			}
		}

		public override bool CanBecomeFirstResponder {
			get {
				return true;
			}
		}

		public ListViewController (IntPtr handle)
			: base (handle)
		{
		}

		#region View Life Cycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			TableView.RowHeight = 44.0;

			UpdateInterfaceWithTextAttributes ();

			// Use the edit button item provided by the table view controller.
			NavigationItem.RightBarButtonItem = EditButtonItem;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			Document.Open ((success) => {
				if (!success) {
					// In your app you should handle this gracefully.
					Console.WriteLine ("Couldn't open document: {0}.", DocumentURL.AbsoluteString);
					throw new InvalidProgramException ();
				}

				TextAttributes = new UIStringAttributes {
					Font = UIFont.PreferredHeadline,
					ForegroundColor = AppColors.ColorFrom (ListPresenter.Color)
				};
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			});

			// TODO: use strong type subscribtion version https://trello.com/c/1RyX6cJL
			docStateChangeToken = NSNotificationCenter.DefaultCenter.AddObserver (UIDocument.StateChangedNotification,
				HandleDocumentStateChangedNotification, Document);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			ResignFirstResponder ();

			Document.Delegate = null;
			Document.Close (null);

			docStateChangeToken.Dispose ();

			// Hide the toolbar so the list can't be edited.
			NavigationController.SetToolbarHidden (true, animated);
		}

		#endregion

		#region Setup

		public void ConfigureWith (ListInfo listInfo)
		{
			this.listInfo = listInfo;

			AllListItemsPresenter listPresenter = new AllListItemsPresenter ();
			Document = new ListDocument (listInfo.Url, listPresenter);

			NavigationItem.Title = listInfo.Name;

			TextAttributes = new UIStringAttributes {
				Font = UIFont.PreferredHeadline,
				ForegroundColor = AppColors.ColorFrom (listInfo.Color ?? AppColors.GrayColor) 
			};
		}

		#endregion

		#region Notifications

		void HandleDocumentStateChangedNotification (NSNotification notification)
		{
			UIDocumentState state = Document.DocumentState;
			if ((state & UIDocumentState.InConflict) != 0)
				ResolveConflicts ();

			// In order to update the UI, dispatch back to the main queue as there are no promises about the queue this will be called on.
			DispatchQueue.MainQueue.DispatchAsync (TableView.ReloadData);
		}

		#endregion

		#region UIViewController Overrides

		public override void SetEditing (bool editing, bool animated)
		{
			base.SetEditing (editing, animated);

			// Prevent navigating back in edit mode.
			NavigationItem.SetHidesBackButton (editing, animated);

			// Make sure to resign first responder on the active text field if needed.
			activeTextField.EndEditing (false);

			// Reload the first row to switch from "Add Item" to "Change Color"
			NSIndexPath indexPath = NSIndexPath.FromRowSection (0, 0);
			TableView.ReloadRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Automatic);

			// If moving out of edit mode, notify observers about the list color and trigger a save.
			if (!editing) {
				listInfo = listInfo ?? new ListInfo (DocumentURL);
				listInfo.Color = ListPresenter.Color;
				ListsController.SetListInfoHasNewContents (listInfo);

				TriggerNewDataForWidget ();
			}

			NavigationController.SetToolbarHidden (!editing, animated);
			NavigationController.Toolbar.SetItems (ListToolbarItems, animated);
		}

		#endregion

		#region UITableViewDataSource

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			// Don't show anything if the document hasn't been loaded.
			// Show the items in a list, plus a separate row that lets users enter a new item.
			return Document == null ? 0 : ListPresenter.Count + 1;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			NSString id = Editing && indexPath.Row == 0 ? ListColorCellIdentifier : ListItemCellIdentifier;
			return tableView.DequeueReusableCell (id, indexPath);
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

			ListItem listItem = ListPresenter.PresentedListItems [indexPath.Row - 1];
			ListPresenter.RemoveListItem (listItem);
		}

		public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		{
			ListItem item = ListPresenter.PresentedListItems [sourceIndexPath.Row - 1];

			// destinationIndexPath.Row will never be `0` since we don't allow moving to the zeroth row (it's the color selection row).
			ListPresenter.MoveListItem (item, destinationIndexPath.Row - 1);
		}

		#endregion

		#region UITableViewDelegate

		public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			var colorCell = cell as ListColorCell;
			var itemCell = cell as ListItemCell;

			if (colorCell != null) {
				colorCell.Configure ();
				colorCell.SelectedColor = ListPresenter.Color;
				colorCell.Delegate = this;
			} else if (itemCell != null) {
				ConfigureListItemCell (itemCell, indexPath.Row);
			} else {
				throw ArgumentException ();
			}
		}

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

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true);
		}

		public override NSIndexPath CustomizeMoveTarget (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath proposedIndexPath)
		{
			ListItem listItem = ListPresenter.PresentedListItems [sourceIndexPath.Row - 1];

			if (proposedIndexPath.Row == 0)
				return sourceIndexPath;
			else if (ListPresenter.CanMove (listItem, proposedIndexPath.Row - 1))
				return proposedIndexPath;

			return sourceIndexPath;
		}

		#endregion

		#region UITextFieldDelegate

		[Export ("textFieldDidBeginEditing:")]
		public void EditingStarted (UITextField textField)
		{
			activeTextField = textField;
		}

		[Export ("textFieldDidEndEditing:")]
		public void EditingEnded (UITextField textField)
		{
			NSIndexPath indexPath = IndexPathForView (textField);

			if (indexPath != null && indexPath.Row > 0) {
				ListItem listItem = ListPresenter.PresentedListItems [indexPath.Row - 1];
				ListPresenter.Update (listItem, textField.Text.Trim ());
			} else if (textField.Text.Length > 0) {
				ListItem listItem = new ListItem (textField.Text);
				ListPresenter.InsertListItem (listItem);
			}

			activeTextField = null;
		}

		[Export ("textFieldShouldReturn:")]
		public bool ShouldReturn (UITextField textField)
		{
			NSIndexPath indexPath = IndexPathForView (textField);

			if (string.IsNullOrEmpty (textField.Text) || indexPath.Row == 0) {
				textField.ResignFirstResponder ();
				return true;
			}

			return false;
		}

		#endregion

		#region IListColorCellDelegate implementation

		public void DidChangeSelectedColor (ListColorCell listColorCell)
		{
			ListPresenter.Color = listColorCell.SelectedColor;
		}

		#endregion

		#region IBActions

		void DeleteList (object sender, EventArgs e)
		{
			ListsController.RemoveListInfo (listInfo);
			HideViewControllerAfterListWasDeleted ();
		}

		[Export ("checkBoxTapped:")]
		public void CheckBoxTapped (CheckBox sender)
		{
			NSIndexPath indexPath = IndexPathForView (sender);

			// Check to see if the tapped row is within the list item rows.
			if (indexPath.Row >= 1 && indexPath.Row <= ListPresenter.Count) {
				ListItem listItem = ListPresenter.PresentedListItems [indexPath.Row - 1];
				ListPresenter.ToggleListItem (listItem);
			}
		}

		#endregion

		#region IListDocumentDelegate implementation

		public void WasDeleted (ListDocument document)
		{
			HideViewControllerAfterListWasDeleted ();
		}

		#endregion

		#region Convenience

		void TriggerNewDataForWidget ()
		{
			string todayDocumentName = AppConfig.SharedAppConfiguration.TodayDocumentName;

			if (Document.LocalizedName != todayDocumentName)
				return;

			// You can use hardcoded value for widget bundle identifier
			// But it is a good idea to move this value to some kind of setting
			// Info.plist works for this purpose
			var widgetBundleId = (string)(NSString)NSBundle.MainBundle.ObjectForInfoDictionary ("WidgetBundleIdentifier");
			NCWidgetController.GetWidgetController ().SetHasContent (true, widgetBundleId);
		}

		void UpdateInterfaceWithTextAttributes ()
		{
			NavigationController.NavigationBar.TitleTextAttributes = TextAttributes;

			UIColor color = TextAttributes.ForegroundColor;
			NavigationController.NavigationBar.TintColor = color;
			NavigationController.Toolbar.TintColor = color;
			TableView.TintColor = color;
		}

		void HideViewControllerAfterListWasDeleted ()
		{
//			if (self.splitViewController && self.splitViewController.isCollapsed) {
//				UINavigationController *controller = self.navigationController.navigationController ?: self.navigationController;
//				[controller popViewControllerAnimated:YES];
//			}
//			else {
//				UINavigationController *emptyViewController = (UINavigationController *)[self.storyboard instantiateViewControllerWithIdentifier:AAPLAppDelegateMainStoryboardEmptyViewControllerIdentifier];
//				emptyViewController.topViewController.navigationItem.leftBarButtonItem = [self.splitViewController displayModeButtonItem];
//
//				self.splitViewController.viewControllers = @[self.splitViewController.viewControllers.firstObject, emptyViewController];
//			}
		}


		void ResolveConflicts ()
		{
			// Any automatic merging logic or presentation of conflict resolution UI should go here.
			// For this sample, just pick the current version and mark the conflict versions as resolved.
			NSError error = null;
			NSFileVersion.RemoveOtherVersions (DocumentURL, out error);

			NSFileVersion[] conflictVersions = NSFileVersion.GetUnresolvedConflictVersions (DocumentURL);
			foreach (NSFileVersion fileVersion in conflictVersions)
				fileVersion.Resolved = true;
		}

		NSIndexPath IndexPathForView (UIView view)
		{
			CGPoint viewOrigin = view.Bounds.Location;
			CGPoint viewLocation = TableView.ConvertPointFromView (viewOrigin, view);

			return TableView.IndexPathForRowAtPoint (viewLocation);
		}

		void ConfigureListItemCell (ListItemCell itemCell, int row)
		{
			itemCell.TextField.Font = UIFont.PreferredBody;
			itemCell.TextField.WeakDelegate = this;

			if (row == 0) {
				// Configure an "Add Item" list item cell.
				itemCell.TextField.Placeholder = "Add Item";
				itemCell.CheckBox.Hidden = true;
			} else {
				ListItem item = List [row - 1];

				itemCell.Completed = item.IsComplete;
				itemCell.TextField.Text = item.Text;
			}
		}

		#endregion

		#region IListPresenterDelegate implementation

		public void DidRefreshCompleteLayout (IListPresenter presenter)
		{
			throw new NotImplementedException ();
		}

		public void WillChangeListLayout (IListPresenter listPresenter, bool isInitialLayout)
		{
			throw new NotImplementedException ();
		}

		public void DidInsertListItem (IListPresenter listPresenter, ListItem listItem, int atIndex)
		{
			throw new NotImplementedException ();
		}

		public void DidRemoveListItem (IListPresenter listPresenter, ListItem listItem, int atIndex)
		{
			throw new NotImplementedException ();
		}

		public void DidUpdateListItem (IListPresenter listPresenter, ListItem listItem, int atIndex)
		{
			throw new NotImplementedException ();
		}

		public void DidMoveListItem (IListPresenter listPresenter, ListItem listItem, int fromIndex, int toIndex)
		{
			throw new NotImplementedException ();
		}

		public void DidUpdateListColorWithColor (IListPresenter listPresenter, ListColor color)
		{
			throw new NotImplementedException ();
		}

		public void DidChangeListLayout (IListPresenter listPresenter, bool isInitialLayout)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}
