using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

using UIKit;
using Foundation;

using Common;
using ListerKit;

namespace Lister
{
	[Register("DocumentsViewController")]
	public class DocumentsViewController : UITableViewController
	{
		// User defaults keys.
		const string StorageOptionUserDefaultsKey = "StorageOptionKey";
		const string StorageOptionUserDefaultsLocal = "StorageOptionLocal";
		const string StorageOptionUserDefaultsCloud = "StorageOptionCloud";

		// Segue identifiers.
		const string ListDocumentSegueIdentifier = "showListDocument";
		const string NewListDocumentSegueIdentifier = "newListDocument";
		readonly NSString ListDocumentCellIdentifier = new NSString("listDocumentCell");

		NSObject sizeChangedToken, finishGatheringToken, updateToken;

		List<ListInfo> listInfos = new List<ListInfo>();
		NSMetadataQuery documentMetadataQuery;

		public DocumentsViewController(IntPtr handle)
			: base(handle)
		{

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			SetNeedsStatusBarAppearanceUpdate ();

			SetupTextAttributes ();

			sizeChangedToken = UIApplication.Notifications.ObserveContentSizeCategoryChanged (HandleContentSizeCategoryDidChangeNotification);

			// When the desired storage chages, start the query.
			ListCoordinator.SharedListCoordinator.StorageChoiceChanged += OnStorageChoiceChanged;

			StartQuery ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			SetupTextAttributes ();

			NavigationController.NavigationBar.TintColor = AppColors.ColorFrom(ListColor.Gray);
			NavigationController.Toolbar.TintColor = AppColors.ColorFrom(ListColor.Gray);
			TableView.TintColor = AppColors.ColorFrom(ListColor.Gray);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			SetupUserStoragePreferences ();
		}

		void SetupTextAttributes ()
		{
			NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes {
				Font = UIFont.PreferredHeadline,
				ForegroundColor = AppColors.ColorFrom (ListColor.Gray)
			};
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);

			ListCoordinator.SharedListCoordinator.StorageChoiceChanged -= OnStorageChoiceChanged;

			sizeChangedToken.Dispose ();
			updateToken.Dispose ();
			finishGatheringToken.Dispose ();
		}

		#region Setup

		void SelectListWithListInfo(ListInfo listInfo)
		{
			UISplitViewController splitViewController = SplitViewController;

			Action<ListViewController> ConfigureListViewController = listViewController => {
				listViewController.ConfigureWith(listInfo);
				listViewController.MasterController = this;
			};

			if (splitViewController.Collapsed) {
				ListViewController listViewController = (ListViewController)Storyboard.InstantiateViewController ("listViewController");
				ConfigureListViewController(listViewController);
				ShowViewController (listViewController, this);
			} else {
				UINavigationController navigationController = (UINavigationController)Storyboard.InstantiateViewController ("listViewNavigationController");
				ListViewController listViewController = (ListViewController)navigationController.TopViewController;
				ConfigureListViewController(listViewController);
				SplitViewController.ViewControllers = new UIViewController[] {
					SplitViewController.ViewControllers [0],
					new UIViewController ()
				};
				ShowDetailViewController (navigationController, this);
			}
		}

		void SetupUserStoragePreferences()
		{
			StorageState storageState = AppConfig.SharedAppConfiguration.StorageState;

			if (storageState.AccountDidChange)
				NotifyUserOfAccountChange ();

			if (storageState.CloudAvailable) {
				if (storageState.StorageOption == StorageType.NotSet)
					PromptUserForStorageOption ();
			} else {
				AppConfig.SharedAppConfiguration.StorageOption = StorageType.NotSet;
				NeedCloudAlert ();
			}
		}

		#endregion

		#region UITableViewDataSource

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return listInfos.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			ListInfo listInfo = listInfos [indexPath.Row];
			ListCell cell = (ListCell)tableView.DequeueReusableCell(ListDocumentCellIdentifier, indexPath);

			Configure (cell, listInfo);

			return cell;
		}

		async void Configure(ListCell cell, ListInfo listInfo)
		{
			// Show an empty string as the text since it may need to load.
			cell.Label.Text = string.Empty;
			cell.Label.Font = UIFont.PreferredBody;
			cell.ListColorView.BackgroundColor = UIColor.Clear;
			cell.BackgroundColor = UIColor.Cyan;

			await listInfo.FetchInfoAsync ();

			cell.Label.Text = listInfo.Name;
			cell.ListColorView.BackgroundColor = AppColors.ColorFrom (listInfo.Color.Value);
		}

		#endregion

		#region UITableViewDelegate

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			ListInfo listInfo = listInfos[indexPath.Row];
			SelectListWithListInfo (listInfo);
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

		public void ListViewControllerDidDeleteList (ListViewController listViewController)
		{
			if (listViewController == null)
				throw new ArgumentNullException ("listViewController");

			TableView.DeselectRow (TableView.IndexPathForSelectedRow, false);
			DeleteListAtUrl (listViewController.DocumentURL);
		}

		#region UIStoryboardSegue Handling

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == NewListDocumentSegueIdentifier) {
				var newListController = (NewDocumentController)segue.DestinationViewController;
				newListController.MasterController = this;
			}
		}

		public void OnNewListInfo (ListInfo listInfo)
		{
			InsertListInfo(listInfo, index => {
				NSIndexPath indexPathForInsertedRow = NSIndexPath.FromRowSection(index, 0);
				TableView.InsertRows(new NSIndexPath[] {indexPathForInsertedRow }, UITableViewRowAnimation.Automatic);
			});
		}

		#endregion

		#region Convenience

		void DeleteListAtUrl(NSUrl url)
		{
			// Asynchonously delete the document.
			ThreadPool.QueueUserWorkItem(_=> {
				ListCoordinator.SharedListCoordinator.DeleteFileAtURL(url);
			});

			// Update the document list and remove the row from the table view.
			RemoveListInfo (url, index => {
				NSIndexPath indexPathForRemoval = NSIndexPath.FromRowSection(index, 0);
				TableView.DeleteRows(new NSIndexPath[] { indexPathForRemoval }, UITableViewRowAnimation.Automatic);
			});
		}

		#endregion

		#region List Management

		void StartQuery()
		{
			if(documentMetadataQuery != null)
				documentMetadataQuery.StopQuery ();

			if (AppConfig.SharedAppConfiguration.StorageOption == StorageType.Cloud)
				StartMetadataQuery ();
			else
				StartLocalQuery ();
		}

		void StartLocalQuery()
		{
			NSUrl docDir = ListCoordinator.SharedListCoordinator.DocumentsDirectory;

			NSFileManager defaultManager = NSFileManager.DefaultManager;

			// Fetch the list documents from container documents directory.
			NSError error;
			NSUrl[] localDocuments = defaultManager.GetDirectoryContent (docDir, null,
				                         NSDirectoryEnumerationOptions.SkipsPackageDescendants, out error);

			ProcessURLs (localDocuments);
		}

		void ProcessURLs(NSUrl[] results)
		{
			listInfos.Clear ();

			Array.Sort (results, (lhs, rhs) => {
				return lhs.LastPathComponent.CompareTo(rhs.LastPathComponent);
			});

			foreach (NSUrl url in results) {
				string ext = Path.GetExtension (url.AbsoluteString);
				ext = ext.Remove(0, 1); // remove lead dot ".list" become "list"
				if (ext == AppConfig.ListerFileExtension)
					InsertListInfoWithProvider (url, null);
			}

			TableView.ReloadData ();
		}

		void ProcessMetadataItems()
		{
			listInfos.Clear ();
			NSMetadataItem[] results = documentMetadataQuery.Results;

			// This is debug output, just to show when ProcessMetadataItems is called
			foreach (var item in results)
				PrintMetadataItem (item);

			Array.Sort (results, (left, right) => {
				string lName = (string)(NSString)left.ValueForAttribute (NSMetadataQuery.ItemFSNameKey);
				string rName = (string)(NSString)right.ValueForAttribute (NSMetadataQuery.ItemFSNameKey);
				return lName.CompareTo (rName);
			});

			foreach (NSMetadataItem item in results)
				InsertListInfoWithProvider (item, null);

			listInfos.Sort ((left, right) => {
				return left.Url.LastPathComponent.CompareTo(right.Url.LastPathComponent);
			});

			TableView.ReloadData ();
		}

		void PrintMetadataItem(NSMetadataItem item)
		{
			Console.WriteLine ("path={0}, display={1}, url={2}, name={3}, downloaded={4}, downloading={5}, uploaded={6}, uploading={7}, createData={8}, updateDate={9},",
				item.Path, item.DisplayName, item.Url, item.FileSystemName, item.UbiquitousItemIsUploaded,
				item.UbiquitousItemIsUploading, item.UbiquitousItemIsUploaded, item.UbiquitousItemIsUploading,
				item.FileSystemCreationDate, item.FileSystemContentChangeDate);
		}

		void StartMetadataQuery()
		{
			if (documentMetadataQuery == null) {
				NSMetadataQuery metadataQuery = new NSMetadataQuery {
					SearchScopes = new NSObject[] {
						NSMetadataQuery.UbiquitousDocumentsScope
					},
					Predicate = NSPredicate.FromFormat ("(%K.pathExtension = %@)",
						NSMetadataQuery.ItemFSNameKey,
						(NSString)AppConfig.ListerFileExtension)
				};
				documentMetadataQuery = metadataQuery;

				NSNotificationCenter notificationCenter = NSNotificationCenter.DefaultCenter;
				// TODO: use typed overload https://trello.com/c/1RyX6cJL
				finishGatheringToken = notificationCenter.AddObserver (NSMetadataQuery.DidFinishGatheringNotification,
					HandleMetadataQueryUpdates, metadataQuery);
				updateToken = notificationCenter.AddObserver (NSMetadataQuery.DidUpdateNotification,
					HandleMetadataQueryUpdates, metadataQuery);
			}

			documentMetadataQuery.StartQuery ();
		}

		void HandleMetadataQueryUpdates(NSNotification notification)
		{
			documentMetadataQuery.DisableUpdates ();
			ProcessMetadataItems ();
			documentMetadataQuery.EnableUpdates ();
		}

		void InsertListInfo(ListInfo listInfo, Action<int> completionHandler)
		{
			ComparisionComparer<ListInfo> comparer = new ComparisionComparer<ListInfo>((left, right) => {
				return left.Name.CompareTo(right.Name);
			});
			// read more about return value http://msdn.microsoft.com/en-us/library/ftfdbfx6(v=vs.110).aspx
			int index = listInfos.BinarySearch(listInfo, comparer);
			index = index >= 0 ? index : ~index;

			listInfos.Insert (index, listInfo);

			if (completionHandler != null)
				completionHandler(index);
		}

		void RemoveListInfo(NSUrl url, Action<int> completionHandler)
		{
			ListInfo listInfo = new ListInfo (url);
			RemoveListInfo (listInfo, completionHandler);
		}

		void RemoveListInfo(ListInfo listInfo, Action<int> completionHandler)
		{
			int index = listInfos.IndexOf (listInfo);
			if (index < 0)
				return;

			listInfos.RemoveAt (index);

			if (completionHandler != null)
				completionHandler(index);
		}

		// ListInfo objects may originate from local URLs or NSMetadataItems representing document in the cloud.
		void InsertListInfoWithProvider(NSMetadataItem item, Action<int> completionHandler)
		{
			InsertListInfoWithProvider ((NSUrl)item.ValueForAttribute (NSMetadataQuery.ItemURLKey), completionHandler);
		}

		// ListInfo objects may originate from local URLs or NSMetadataItems representing document in the cloud.
		void InsertListInfoWithProvider(NSUrl provider, Action<int> completionHandler)
		{
			ListInfo listInfo = new ListInfo (provider);
			InsertListInfo (listInfo, completionHandler);
		}

		#endregion

		#region Notifications

		public void UpdateDocumentColor(NSUrl documentUrl, ListColor newColor)
		{
			ListInfo listInfo = new ListInfo (documentUrl);

			int index = listInfos.IndexOf(listInfo);
			if (index != -1) {
				listInfo = listInfos[index];
				listInfo.Color = newColor;

				NSIndexPath indexPath = NSIndexPath.FromRowSection (index, 0);
				ListCell cell = (ListCell)TableView.CellAt (indexPath);
				cell.ListColorView.BackgroundColor = AppColors.ColorFrom (newColor);
			}
		}

		void HandleContentSizeCategoryDidChangeNotification(object sender, UIContentSizeCategoryChangedEventArgs arg)
		{
			View.SetNeedsLayout ();
		}

		void OnStorageChoiceChanged(object sender, EventArgs arg)
		{
			StartQuery ();
		}

		#endregion

		#region User Storage Preference Related Alerts

		void NotifyUserOfAccountChange()
		{
			string title = "iCloud Sign Out";
			string message = "You have signed out of the iCloud account previously used to store documents. Sign back in to access those documents.";
			string okActionTitle = "OK";

			UIAlertController signedOutController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);
			signedOutController.AddAction (UIAlertAction.Create (okActionTitle, UIAlertActionStyle.Cancel, null));

			PresentViewController (signedOutController, true, null);
		}

		void PromptUserForStorageOption()
		{
			string title = "Choose Storage Option";
			string message = "Do you want to store documents in iCloud or only on this device?";
			string localOnlyActionTitle = "Local Only";
			string cloudActionTitle = "iCloud";

			UIAlertController storageController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);

			UIAlertAction localOption = UIAlertAction.Create (localOnlyActionTitle, UIAlertActionStyle.Default, action => {
				AppConfig.SharedAppConfiguration.StorageOption = StorageType.Local;
			});
			storageController.AddAction (localOption);

			UIAlertAction cloudOption = UIAlertAction.Create (cloudActionTitle, UIAlertActionStyle.Default, action => {
				AppConfig.SharedAppConfiguration.StorageOption = StorageType.Cloud;
				AppConfig.SharedAppConfiguration.StoreUbiquityIdentityToken();
			});
			storageController.AddAction (cloudOption);

			PresentViewController (storageController, true, null);
		}

		void NeedCloudAlert()
		{
			var title = "iCloud needed";
			var message = "For this sample you need enable iCloud Drive and restart app";
			UIAlertController cloudController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);
			cloudController.AddAction (UIAlertAction.Create ("Ok", UIAlertActionStyle.Default, null));

			PresentViewController (cloudController, true, null);
		}
		#endregion

	}
}

