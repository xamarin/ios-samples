using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

using UIKit;
using Foundation;

using ListerKit;
using CoreFoundation;

namespace Lister
{
	[Register ("DocumentsViewController")]
	public class DocumentsViewController : UITableViewController, IListsControllerDelegate, IUIDocumentMenuDelegate, IUIDocumentPickerDelegate
	{
		// Segue identifiers.
		const string ListDocumentSegueIdentifier = "showListDocument";
		const string NewListDocumentSegueIdentifier = "newListDocument";
		const string ContinueUserActivityToListViewControllerSegueIdentifier = "showListDocumentFromUserActivity";

		readonly NSString ListDocumentCellIdentifier = new NSString ("listDocumentCell");

		NSObject sizeChangedToken, finishGatheringToken, updateToken;
		NSUserActivity pendingUserActivity;

		ListsController listsController;

		public ListsController ListsController {
			get {
				return listsController;
			}
			set {
				if (listsController == value)
					return;

				listsController = value;
				listsController.Delegate = this;
			}
		}

		public DocumentsViewController (IntPtr handle)
			: base (handle)
		{

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			TableView.RowHeight = 44;

			NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes {
				Font = UIFont.PreferredHeadline,
				ForegroundColor = AppColors.ColorFrom (ListColor.Gray)
			};

			sizeChangedToken = UIApplication.Notifications.ObserveContentSizeCategoryChanged (HandleContentSizeCategoryDidChangeNotification);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			UIColor grayListColor = AppColors.ColorFrom (ListColor.Gray);
			NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes {
				Font = UIFont.PreferredHeadline,
				ForegroundColor = grayListColor
			};

			NavigationController.NavigationBar.TintColor = grayListColor;
			NavigationController.Toolbar.TintColor = grayListColor;
			TableView.TintColor = grayListColor;
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (pendingUserActivity != null)
				RestoreUserActivityState (pendingUserActivity);

			pendingUserActivity = null;
		}

		#region UIResponder

		public override void RestoreUserActivityState (NSUserActivity activity)
		{
			// If there is a list currently displayed; pop to the root view controller (this controller) and continue
			// the activity from there. Otherwise, continue the activity directly.
			if (NavigationController.TopViewController is UINavigationController) {
				NavigationController.PopToRootViewController (false);
				pendingUserActivity = activity;
				return;
			}

			// TODO: https://bugzilla.xamarin.com/show_bug.cgi?id=27189
			var handle = ObjCRuntime.Dlfcn.dlopen ("/System/Library/Frameworks/UIKit.framework/UIKit", 0);
			NSString key = ObjCRuntime.Dlfcn.GetStringConstant (handle, "NSUserActivityDocumentURLKey");
			NSUrl activityUrl = (NSUrl)activity.UserInfo [key];

			if (activityUrl == null)
				return;

			ListInfo activityListInfo = new ListInfo (activityUrl);

			var listInfoColorNumber = (NSNumber)activity.UserInfo [AppConfig.UserActivityListColorUserInfoKey];
			activityListInfo.Color = (ListColor)listInfoColorNumber.Int32Value;

			PerformSegue (ContinueUserActivityToListViewControllerSegueIdentifier, activityListInfo);
		}

		#endregion

		#region IBActions

		/// <summary>
		/// Note that the document picker requires that code signing, entitlements, and provisioning for
		/// the project have been configured before you run Lister. If you run the app without configuring
		/// entitlements correctly, an exception when this method is invoked (i.e. when the "+" button is
		/// clicked).
		/// </summary>
		[Export ("pickDocument:")]
		void pickDocument (UIBarButtonItem barButtonItem)
		{
			UIDocumentMenuViewController documentMenu = new UIDocumentMenuViewController (new string[]{ AppConfig.ListerFileUTI }, UIDocumentPickerMode.Import);
			documentMenu.Delegate = this;

			documentMenu.AddOption ("New List", null, UIDocumentMenuOrder.First, () => {
				// Show the NewListDocumentController.
				PerformSegue (NewListDocumentSegueIdentifier, this);
			});

			documentMenu.ModalInPopover = true;
			documentMenu.PopoverPresentationController.BarButtonItem = barButtonItem;

			PresentViewController (documentMenu, true, null);
		}

		#endregion

		#region IUIDocumentMenuDelegate

		public void DidPickDocumentPicker (UIDocumentMenuViewController documentMenu, UIDocumentPickerViewController documentPicker)
		{
			documentPicker.Delegate = this;
			PresentViewController (documentPicker, true, null);
		}

		public void WasCancelled (UIDocumentMenuViewController documentMenu)
		{
			// The user cancelled interacting with the document menu. In your own app, you may want to
			// handle this with other logic.
		}

		#endregion

		#region IUIDocumentPickerDelegate

		public void DidPickDocument (UIDocumentPickerViewController controller, NSUrl url)
		{
			// The user selected the document and it should be picked up by the ListsController.
		}

		[Export ("documentPickerWasCancelled:")]
		public void WasCancelled (UIDocumentPickerViewController controller)
		{
			// The user cancelled interacting with the document picker. In your own app, you may want to
			// handle this with other logic.
		}

		#endregion

		#region IListsControllerDelegate implementation

		public void WillChangeContent (ListsController listsController)
		{
			TableView.BeginUpdates ();
		}

		public void DidInsertListInfo (ListsController listsController, ListInfo listInfo, int index)
		{
			NSIndexPath indexPath = NSIndexPath.FromRowSection (index, 0);
			TableView.InsertRows (new NSIndexPath[]{ indexPath }, UITableViewRowAnimation.Automatic);
		}

		public void DidRemoveListInfo (ListsController listsController, ListInfo listInfo, int index)
		{
			NSIndexPath indexPath = NSIndexPath.FromRowSection (index, 0);
			TableView.DeleteRows (new NSIndexPath[]{ indexPath }, UITableViewRowAnimation.Automatic);
		}

		public void DidUpdateListInfo (ListsController listsController, ListInfo listInfo, int index)
		{
			NSIndexPath indexPath = NSIndexPath.FromRowSection (index, 0);

			ListCell cell = (ListCell)TableView.CellAt (indexPath);
			cell.Label.Text = listInfo.Name;

			listInfo.FetchInfoWithCompletionHandler (() => {
				// The fetchInfoWithCompletionHandler: method calls its completion handler on a background
				// queue, dispatch back to the main queue to make UI updates.
				DispatchQueue.MainQueue.DispatchAsync (() => {
					// Make sure that the list info is still visible once the color has been fetched.
					if (Array.IndexOf (TableView.IndexPathsForVisibleRows, indexPath) != -1)
						cell.ListColorView.BackgroundColor = AppColors.ColorFrom (listInfo.Color.Value);
				});
			});
		}

		public void DidChangeContent (ListsController listsController)
		{
			TableView.EndUpdates ();
		}

		public void DidFailCreatingListInfo (ListsController listsController, ListInfo listInfo, NSError error)
		{
			string title = "Failed to Create List";
			string message = error.LocalizedDescription;
			string okActionTitle = "OK";

			UIAlertController errorOutController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);
			errorOutController.AddAction (UIAlertAction.Create (okActionTitle, UIAlertActionStyle.Cancel, null));

			PresentViewController (errorOutController, true, null);
		}

		public void DidFailRemovingListInfo (ListsController listsController, ListInfo listInfo, NSError error)
		{
			string title = "Failed to Delete List";
			string message = error.LocalizedDescription;
			string okActionTitle = "OK";

			UIAlertController errorOutController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);
			errorOutController.AddAction (UIAlertAction.Create (okActionTitle, UIAlertActionStyle.Cancel, null));

			PresentViewController (errorOutController, true, null);
		}

		#endregion

		#region UITableViewDataSource

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return ListsController != null ? ListsController.Count : 0;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			return TableView.DequeueReusableCell (ListDocumentCellIdentifier, indexPath);
		}

		#endregion

		#region UITableViewDelegate

		public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			// Assert if attempting to configure an unknown or unsupported cell type.
			var listCell = cell as ListCell;
			if (listCell != null)
				throw new InvalidOperationException ();

			ListInfo listInfo = ListsController [indexPath.Row];

			listCell.Label.Text = listInfo.Name;
			listCell.Label.Font = UIFont.PreferredBody;
			listCell.ListColorView.BackgroundColor = UIColor.Clear;

			// Once the list info has been loaded, update the associated cell's properties.
			listInfo.FetchInfoWithCompletionHandler (() => {
				// The fetchInfoWithCompletionHandler: method calls its completion handler on a background
				// queue, dispatch back to the main queue to make UI updates.
				DispatchQueue.MainQueue.DispatchAsync (() => {
					// Make sure that the list info is still visible once the color has been fetched.
					if (Array.IndexOf (TableView.IndexPathsForVisibleRows, indexPath) != -1)
						listCell.ListColorView.BackgroundColor = AppColors.ColorFrom (listInfo.Color.Value);
				});
			});
		}

		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			return false;
		}

		public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
		{
			return false;
		}

		#endregion

		#region UIStoryboardSegue Handling

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == NewListDocumentSegueIdentifier) {
				var newListController = (NewDocumentController)segue.DestinationViewController;
				newListController.ListsController = ListsController;
			} else if (segue.Identifier == ListDocumentSegueIdentifier ||
			           segue.Identifier == ContinueUserActivityToListViewControllerSegueIdentifier) {
				var listNavigationController = (UINavigationController)segue.DestinationViewController;
				ListViewController listViewController = (ListViewController)listNavigationController.TopViewController;
				listViewController.ListsController = ListsController;

				listViewController.NavigationItem.LeftBarButtonItem = SplitViewController.DisplayModeButtonItem;
				listViewController.NavigationItem.LeftItemsSupplementBackButton = true;

				if (segue.Identifier == ListDocumentSegueIdentifier) {
					NSIndexPath indexPath = TableView.IndexPathForSelectedRow;
					listViewController.ConfigureWith (ListsController [indexPath.Row]);
				} else if (segue.Identifier == ContinueUserActivityToListViewControllerSegueIdentifier) {
					ListInfo userActivityListInfo = (ListInfo)sender;
					listViewController.ConfigureWith (userActivityListInfo);
				}
			}
		}

		#endregion

		#region Notifications

		void HandleContentSizeCategoryDidChangeNotification (object sender, UIContentSizeCategoryChangedEventArgs arg)
		{
			View.SetNeedsLayout ();
		}

		#endregion

	}
}