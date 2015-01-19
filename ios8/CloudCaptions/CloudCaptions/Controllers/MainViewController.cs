using System;

using UIKit;
using CloudKit;
using System.Threading.Tasks;
using Foundation;
using CoreGraphics;

namespace CloudCaptions
{
	[Register("MainViewController")]
	public class MainViewController : UITableViewController, IUIScrollViewDelegate, IUISearchBarDelegate
	{
		readonly NSString cellReuseIdentifier = new NSString("post");

		PostManager postManager;

		public MainViewController (IntPtr handle)
			: base(handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			((AppDelegate)UIApplication.SharedApplication.Delegate).TableController = this;

			postManager = new PostManager (() => {
				InvokeOnMainThread(() => { TableView.ReloadData(); });
			});
			postManager.LoadBatch ();

			RefreshControl = new UIRefreshControl ();
			RefreshControl.ValueChanged += LoadNewPosts;
			postManager.RefreshControl = RefreshControl;

			UIActivityIndicatorView indicator = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.Gray);
			indicator.StartAnimating ();
			NavigationItem.LeftBarButtonItem = new SubscriptionController (indicator);
			NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Compose, OnNewPostClicked);
		}

		public async void LoadNewPostsWithRecordID(CKRecordID recordID)
		{
			// Called when AppDelegate receives a push notification
			// The post that triggered the push may not be indexed yet, so a fetch on predicate might not see it.
			// We can still fetch by recordID though
			CKDatabase publicDB = CKContainer.DefaultContainer.PublicCloudDatabase;
			try {
				CKRecord record = await publicDB.FetchRecordAsync (recordID);
				Post postThatFiredPush = new Post(record);
				postThatFiredPush.LoadImage(null, TableView.ReloadData);
				postManager.LoadNewPosts(postThatFiredPush);
			} catch(NSErrorException ex) {
				Console.WriteLine (ex.Error);
			}
		}

		void LoadNewPosts (object sender, EventArgs e)
		{
			postManager.LoadNewPosts ();
		}

		void OnNewPostClicked (object sender, EventArgs e)
		{
			// Shows the user options for selecting an image to post

			string alertTitle = "How do you want to get your photo?";
			UIAlertController picMethod = UIAlertController.Create (alertTitle, null, UIAlertControllerStyle.ActionSheet);
			picMethod.ModalPresentationStyle = UIModalPresentationStyle.Popover;
			UIPopoverPresentationController popPresenter = picMethod.PopoverPresentationController;
			if(popPresenter != null)
				popPresenter.BarButtonItem = NavigationItem.RightBarButtonItem;

			string takePhotoButton = "Take Photo";
			UIAlertAction takePhoto = UIAlertAction.Create (takePhotoButton, UIAlertActionStyle.Default, action => {
				var imagePicker = new UIImagePickerController();
				imagePicker.WeakDelegate = this;
				imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
				PresentViewController(imagePicker, true, null);
			});

			string uploadButton = "Upload Photo";
			UIAlertAction uploadPhoto = UIAlertAction.Create (uploadButton, UIAlertActionStyle.Default, action => {
				var imagePicker = new UIImagePickerController();
				imagePicker.WeakDelegate = this;
				imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
				PresentViewController(imagePicker, true, null);
			});

			string selectButton = "Select from CloudKit";
			UIAlertAction selectExisting = UIAlertAction.Create (selectButton, UIAlertActionStyle.Default, action => {
				PerformSegue("selectExisting", null);
			});

			UIAlertAction cancel = UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null);

			picMethod.AddAction (takePhoto);
			picMethod.AddAction (uploadPhoto);
			picMethod.AddAction (selectExisting);
			picMethod.AddAction (cancel);
			PresentViewController (picMethod, true, null);
		}

		#region UITableViewDelegate

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return postManager.PostCells.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = (PostCell)TableView.DequeueReusableCell (cellReuseIdentifier, indexPath);
			cell = cell ?? new PostCell ();
			Post post = postManager.PostCells[indexPath.Row];
			cell.DisplayInfoForPost (post);
			return cell;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			// Programatically sets cell height to equal the device width, makes all cells square
			return View.Bounds.Size.Width;
		}

		[Export ("scrollViewDidScroll:")]
		public void Scrolled (UIScrollView scrollView)
		{
			// Checks to see if the user has scrolled five posts from the bottom and if we want to update
			var tableBottom = new CGPoint(scrollView.ContentOffset.X, scrollView.ContentOffset.Y + scrollView.Bounds.Size.Height);
			int count = postManager.PostCells.Count;

			NSIndexPath index = TableView.IndexPathForRowAtPoint (tableBottom);
			if( count > 0 && index != null && index.Row + 5 > count)
				postManager.LoadBatch ();
		}

		#endregion

		#region UISearchBarDelegate

		[Export ("searchBarTextDidBeginEditing:")]
		public void OnEditingStarted (UISearchBar searchBar)
		{
			searchBar.SetShowsCancelButton (true, true);
		}

		[Export ("searchBarSearchButtonClicked:")]
		public virtual void SearchButtonClicked (UISearchBar searchBar)
		{
			// Tells the postManager to reset the tag string with the new tag string
			postManager.ResetWithTagString (searchBar.Text);
			searchBar.SetShowsCancelButton (false, true);
			searchBar.ResignFirstResponder ();
		}

		[Export ("searchBarCancelButtonClicked:")]
		public virtual void CancelButtonClicked (UISearchBar searchBar)
		{
			searchBar.Text = string.Empty;
			postManager.ResetWithTagString (string.Empty);
			searchBar.ShowsCancelButton = false;
			searchBar.ResignFirstResponder ();
		}

		#endregion

		#region UIImagePickerControllerDelegate

		[Export ("imagePickerController:didFinishPickingMediaWithInfo:")]
		public void FinishedPickingMedia (UIImagePickerController picker, NSDictionary info)
		{
			var eArg = new UIImagePickerMediaPickedEventArgs (info);
			// UIImagePickerController media types default to kUTTypeImage so we should only get images here
			// Dismisses imagePicker and pulls up SubmitPostViewController
			var imageRecord = new Image (eArg.OriginalImage);
			picker.DismissViewController (true, () => {
				PerformSegue("newPost", imageRecord);
			});
		}

		#endregion

		public void GoTo(ExistingImageViewController controller, Image image)
		{
			// Gets called when the user taps on an image in the collection view
			// Dismisses collection view and pulls up a SubmitPostViewController
			controller.DismissViewController (true, () => {
				PerformSegue("newPost", image);
			});
		}

		public void Submit(Post post)
		{
			postManager.LoadNewPosts (post);
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == "selectExisting") {
				((ExistingImageViewController)segue.DestinationViewController).MasterController = this;
			} else if (segue.Identifier == "newPost") {
				var controller = ((SubmitPostViewController)segue.DestinationViewController);
				controller.ImageRecord = (Image)sender;
				controller.MainController = this;
			} else {
				throw new NotImplementedException ("Unhandled segue");
			}
		}
	}
}

