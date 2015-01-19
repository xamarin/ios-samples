using System;

using UIKit;
using Foundation;
using System.Reflection;
using System.Globalization;
using CoreFoundation;

namespace PhotoHandoff
{
	[Register("CollectionViewController")]
	public class CollectionViewController : UICollectionViewController, INSUserActivityDelegate
	{
		const string DetailedViewControllerID = "DetailView";    // view controller storyboard id
		const string DetailSegueName = "showDetail";             // segue ID to navigate to the detail view controller
		const string DetailViewControllerKey = "DetailViewControllerKey";

		readonly NSString CellID = new NSString("cellID"); // UICollectionViewCell id

		DetailViewController detailViewController;

		public DataSource DataSource { get; set; }

		public CollectionViewController (IntPtr handle)
			: base(handle)
		{
		}
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			var selectedItems = CollectionView.GetIndexPathsForSelectedItems ();
			if (selectedItems.Length > 0) {
				UIView.Animate (0.3, 0, UIViewAnimationOptions.CurveLinear,
					() => CollectionView.DeselectItem (selectedItems [0], true)
					, null);
			}

			if (detailViewController == null)
				return;

			UserActivity = null;
			detailViewController = null;
		}

		#region NSUserActivity

		public override void UpdateUserActivityState (NSUserActivity activity)
		{
			base.UpdateUserActivityState (activity);

			string imageIdentifier = detailViewController.ImageIdentifier;
			if (string.IsNullOrEmpty (imageIdentifier))
				return;

			activity.Title = DataSource.GetTitleForIdentifier (imageIdentifier);

			var userInfo = new UserInfo {
				ImageId = imageIdentifier
			};
			activity.AddUserInfoEntries (userInfo.Dictionary);
		}

		public override void RestoreUserActivityState (NSUserActivity activity)
		{
			base.RestoreUserActivityState (activity);
		}

		[Export ("userActivityWasContinued:")]
		public void UserActivityWasContinued (NSUserActivity userActivity)
		{
			// we no longer are interested in our detail view controller since it was handed off to the other device
			DispatchQueue.MainQueue.DispatchAsync (() => {
				if (detailViewController == null)
					return;

				detailViewController.DismissFromUserActivity (() => NavigationController.PopViewController (true));
			});
		}

		void SaveActivity()
		{
			if (UserActivity == null) {
				UserActivity = new NSUserActivity (NSBundle.MainBundle.BundleIdentifier) {
					Delegate = this   // so we can be notified when another device takes over an activity
				};
			}
			UserActivity.NeedsSave = true;
			detailViewController.UserActivity = UserActivity;

			// so coming back to foreground will make activity current when something is presented
			if (PresentedViewController != null)
				PresentedViewController.UserActivity = UserActivity;
		}

		void InstantiateAndPushDetailViewController(bool animated)
		{
			// we use our bundle identifier to define the user activity
			detailViewController = (DetailViewController)Storyboard.InstantiateViewController ("DetailViewController");
			detailViewController.DataSource = DataSource;
			NavigationController.PushViewController (detailViewController, animated);
		}

		public bool HandleUserActivity(NSUserActivity userActivity)
		{
			var userInfo = new UserInfo (userActivity.UserInfo);
			bool rc = HandleActivityUserInfo (userInfo);
			ClearActivityContinuationInProgress ();

			return rc;
		}

		bool HandleActivityUserInfo(UserInfo userInfo)
		{
			string imageIdentifier = userInfo.ImageId;
			if (string.IsNullOrEmpty (imageIdentifier)) {
				HandleActivityFailure ();
				return false;
			}

			int row = int.Parse (imageIdentifier);
			NSIndexPath indexPath = NSIndexPath.FromRowSection (row, 0);

			CollectionView.SelectItem (indexPath, false, UICollectionViewScrollPosition.None);
			CollectionView.ScrollToItem (indexPath, UICollectionViewScrollPosition.None, false);
			if (detailViewController == null)
				InstantiateAndPushDetailViewController (true);

			DispatchQueue.MainQueue.DispatchAsync (() => {
				detailViewController.RestoreActivityForImageIdentifier (userInfo);
				SaveActivity ();
			});

			return true;
		}

		void ClearActivityContinuationInProgress ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}

		public void PrepareForActivity()
		{
			if (detailViewController == null)
				InstantiateAndPushDetailViewController (true);
			else
				detailViewController.PrepareForActivity ();

			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
		}

		public void HandleActivityFailure ()
		{
			// pop the current view controller since something failed
			if (detailViewController != null && string.IsNullOrEmpty(detailViewController.ImageIdentifier))
				NavigationController.PopToRootViewController (true);

			ClearActivityContinuationInProgress ();
		}

		#endregion

		#region UICollectionViewDataSource

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return DataSource.NumberOfItemsInSection (section);
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (Cell)collectionView.DequeueReusableCell (CellID, indexPath);

			string imageIdentifier = DataSource.IdentifierForIndexPath (indexPath);
			string text = DataSource.GetTitleForIdentifier (imageIdentifier);
			cell.Label.Text = text;
			cell.Image.Image = DataSource.ThumbnailForIdentifier (imageIdentifier);

			return cell;
		}

		#endregion

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier != DetailSegueName)
				return;

			NSIndexPath selectedIndexPath = CollectionView.GetIndexPathsForSelectedItems () [0];
			string imageIdentifier = DataSource.IdentifierForIndexPath (selectedIndexPath);
			detailViewController = (DetailViewController)segue.DestinationViewController;
			detailViewController.ImageIdentifier = imageIdentifier;
			detailViewController.DataSource = DataSource;
			SaveActivity ();    // create our new NSUserActivity handled us
			ClearActivityContinuationInProgress ();
		}

		public override void EncodeRestorableState (NSCoder coder)
		{
			base.EncodeRestorableState (coder);
			if (detailViewController != null)
				coder.Encode (detailViewController, DetailViewControllerKey);
		}

		public override void DecodeRestorableState (NSCoder coder)
		{
			base.DecodeRestorableState (coder);
			detailViewController = (DetailViewController)coder.DecodeObject (DetailViewControllerKey);
		}

		public override void ApplicationFinishedRestoringState ()
		{
			base.ApplicationFinishedRestoringState ();

			if (detailViewController != null && !string.IsNullOrEmpty (detailViewController.ImageIdentifier))
				SaveActivity ();
		}
	}
}