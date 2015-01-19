using System;

using UIKit;
using CloudKit;
using CoreGraphics;
using Foundation;

namespace CloudCaptions
{
	[Register("ExistingImageViewController")]
	public class ExistingImageViewController : UIViewController
	{
		class CollectionViewDelegate : UICollectionViewDelegate
		{
			readonly ExistingImageViewController controller;

			public CollectionViewDelegate(ExistingImageViewController controller)
			{
				this.controller = controller;
			}

			public override void Scrolled (UIScrollView scrollView)
			{
				// Gets the point at the bottom of the scroll view
				var contentOffset = scrollView.ContentOffset;
				CGPoint bottomRowPoint = new CGPoint(contentOffset.X, contentOffset.Y + scrollView.Bounds.Size.Height);

				// Finds number of rows left (gets height of row and adds 5 px spacing between rows)
				var flowLayout = (UICollectionViewFlowLayout) controller.ImageCollection.CollectionViewLayout;
				nfloat rowHeight = flowLayout.ItemSize.Height;
				nfloat rowSpacing = flowLayout.MinimumLineSpacing;
				nfloat rowsLeft = (scrollView.ContentSize.Height - bottomRowPoint.Y) / (rowHeight + rowSpacing);

				// If we have less five rows left, load the next set
				if ((int)rowsLeft < 5)
					controller.LoadImages ();
			}

			// This method fetches the whole ImageRecord that a user taps on and then passes it to the delegate
			public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
			{
				// If the user has already tapped on a thumbnail, prevent them from tapping any others
				if (controller.lockSelectThumbnail)
					return;
				else
					controller.lockSelectThumbnail = true;

				// Starts animating the thumbnail to indicate it is loading
				controller.ImageCollection.SetLoadingFlag (indexPath, true);

				// Uses convenience API to fetch the whole image record associated with the thumbnail that was tapped
				CKRecordID userSelectedRecordID = controller.ImageCollection.GetRecordId (indexPath);
				controller.PublicCloudDatabase.FetchRecord (userSelectedRecordID, (record, error) => {
					// If we get a partial failure, we should unwrap it
					if(error != null && error.Code == (long)CKErrorCode.PartialFailure) {
						CKErrorInfo info = new CKErrorInfo(error);
						error = info[userSelectedRecordID];
					}

					Error errorResponse = controller.HandleError(error);

					switch(errorResponse) {
						case Error.Success:
							controller.ImageCollection.SetLoadingFlag(indexPath, false);
							Image selectedImage = new Image(record);
							InvokeOnMainThread(() => {
								controller.MasterController.GoTo(controller, selectedImage);
							});
							break;

						case Error.Retry:
							Utils.Retry(()=> {
								controller.lockSelectThumbnail = false;
								ItemSelected(collectionView, indexPath);
							}, error);
							break;

						case Error.Ignore:
							Console.WriteLine ("Error: {0}", error.Description);
							string errorTitle = "Error";
							string errorMessage = "We couldn't fetch the full size thumbnail you tried to select, try again";
							string dismissButton = "Okay";
							UIAlertController alert = UIAlertController.Create(errorTitle, errorMessage, UIAlertControllerStyle.Alert);
							alert.AddAction(UIAlertAction.Create(dismissButton, UIAlertActionStyle.Cancel, null));
							InvokeOnMainThread(()=> controller.PresentViewController(alert, true, null));
							controller.ImageCollection.SetLoadingFlag(indexPath, false);
							controller.lockSelectThumbnail = false;
							break;

						default:
							throw new NotImplementedException();
					}
				});
			}
		}

		// This constant determines the number of images to fetch at a time
		const int UpdateBy = 24;

		[Outlet("imageCollection")]
		ExistingImageCollectionView ImageCollection { get; set; }

		[Outlet("loadingImages")]
		UIActivityIndicatorView loadingImages { get; set; }

		CKQueryCursor imageCursor;

		bool isLoadingBatch; // Boolean value used to prevent multiple loadImages methods running
		bool firstThumbnailLoaded; // Boolean value used to permanently lock loadImages when we've grabbed the earliest image
		bool lockSelectThumbnail; // Only lets user select one image at a time

		CollectionViewDelegate collectionViewDelegate;
		readonly object locker = new Object();

		CKDatabase PublicCloudDatabase {
			get {
				return CKContainer.DefaultContainer.PublicCloudDatabase;
			}
		}

		// ExistingImageViewController is detailed controller. Need to store ref to master(parent) controller to perform callback
		public MainViewController MasterController { get; set; }

		public ExistingImageViewController(IntPtr handle)
			: base(handle)
		{

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			collectionViewDelegate = new CollectionViewDelegate(this);
			ImageCollection.Delegate = collectionViewDelegate;
			isLoadingBatch = false;
			firstThumbnailLoaded = false;
			lockSelectThumbnail = false;

			// This ensures that there are always three images per row whether it's an iPhone or an iPad (20 px subtracted to account for four 5 px spaces between thumbnails)
			var screenSize = UIScreen.MainScreen.Bounds.Size;
			nfloat smallerDimension = NMath.Min(screenSize.Width, screenSize.Height);   // Works even if iPad is rotated
			UICollectionViewFlowLayout flowLayout = (UICollectionViewFlowLayout)ImageCollection.CollectionViewLayout;
			nfloat imageWidth = (smallerDimension - 20) / 3;
			flowLayout.ItemSize = new CGSize(imageWidth, imageWidth);

			LoadImages ();
		}

		void LoadImages()
		{
			// If we're already loading a set of images or there are no images left to load, just return
			lock (locker) {
				if (isLoadingBatch || firstThumbnailLoaded)
					return;
				else
					isLoadingBatch = true;
			}

			// If we have a cursor, continue where we left off, otherwise set up new query
			CKQueryOperation queryOp = null;
			if (imageCursor != null) {
				queryOp = new CKQueryOperation (imageCursor);
			} else {
				CKQuery thumbnailQuery = new CKQuery (Image.RecordType, NSPredicate.FromValue (true));
				thumbnailQuery.SortDescriptors = Utils.CreateCreationDateDescriptor (ascending: false);
				queryOp = new CKQueryOperation (thumbnailQuery);
			}

			// We only want to download the thumbnails, not the full image
			queryOp.DesiredKeys = new string[] {
				Image.ThumbnailKey
			};
			queryOp.ResultsLimit = UpdateBy;
			queryOp.RecordFetched = (CKRecord record) => {
				ImageCollection.AddImageFromRecord (record);
				InvokeOnMainThread (() => {
					loadingImages.StopAnimating ();
					ImageCollection.ReloadData ();
				});
			};
			queryOp.Completed = (CKQueryCursor cursor, NSError error) => {
				Error errorResponse = HandleError (error);

				if (errorResponse == Error.Success) {
					imageCursor = cursor;
					isLoadingBatch = false;
					if (cursor == null)
						firstThumbnailLoaded = true; // If cursor is nil, lock this method indefinitely (all images have been loaded)
				} else if (errorResponse == Error.Retry) {
					// If there's no specific number of seconds we're told to wait, default to 3
					Utils.Retry (() => {
						// Resets so we can load images again and then goes to load
						isLoadingBatch = false;
						LoadImages ();
					}, error);
				} else if (errorResponse == Error.Ignore) {
					// If we get an ignore error they're not often recoverable. I'll leave loadImages locked indefinitely (this is up to the developer)
					Console.WriteLine ("Error: {0}", error.Description);
					string errorTitle = "Error";
					string dismissButton = "Okay";
					string errorMessage = "We couldn't fetch one or more of the thumbnails";

					UIAlertController alert = UIAlertController.Create (errorTitle, errorMessage, UIAlertControllerStyle.Alert);
					alert.AddAction (UIAlertAction.Create (dismissButton, UIAlertActionStyle.Cancel, null));
					InvokeOnMainThread (() => PresentViewController (alert, true, null));
				}
			};

			PublicCloudDatabase.AddOperation (queryOp);
		}

		Error HandleError (NSError error)
		{
			if (error == null)
				return Error.Success;

			switch ((CKErrorCode)(long)error.Code)
			{
				case CKErrorCode.NetworkUnavailable:
				case CKErrorCode.NetworkFailure:
					// A reachability check might be appropriate here so we don't just keep retrying if the user has no service
				case CKErrorCode.ServiceUnavailable:
				case CKErrorCode.RequestRateLimited:
					return Error.Retry;

				case CKErrorCode.UnknownItem:
					Console.WriteLine ("If an image has never been uploaded, CKErrorCode.UnknownItem will be returned in ExistingImageViewController because it has never seen the Image record type");
					return Error.Ignore;

				case CKErrorCode.InvalidArguments:
					Console.WriteLine ("If invalid arguments is returned in ExistingImageViewController with a message about not being marked indexable or sortable, go into CloudKit dashboard and set the Image record type as sortable on date created");
					return Error.Ignore;

				case CKErrorCode.IncompatibleVersion:
				case CKErrorCode.BadContainer:
				case CKErrorCode.MissingEntitlement:
				case CKErrorCode.PermissionFailure:
				case CKErrorCode.BadDatabase:
					// This app uses the publicDB with default world readable permissions
				case CKErrorCode.AssetFileNotFound:
				case CKErrorCode.QuotaExceeded:
					// We should not retry if it'll exceed our quota
				case CKErrorCode.OperationCancelled:
					// Nothing to do here, we intentionally cancelled
				case CKErrorCode.NotAuthenticated:
				case CKErrorCode.ResultsTruncated:
				case CKErrorCode.ServerRecordChanged:
				case CKErrorCode.AssetFileModified:
				case CKErrorCode.ChangeTokenExpired:
				case CKErrorCode.BatchRequestFailed:
				case CKErrorCode.ZoneBusy:
				case CKErrorCode.ZoneNotFound:
				case CKErrorCode.LimitExceeded:
				case CKErrorCode.UserDeletedZone:
					// All of these errors are irrelevant for this query operation
				case CKErrorCode.InternalError:
				case CKErrorCode.ServerRejectedRequest:
				case CKErrorCode.ConstraintViolation:
					//Non-recoverable, should not retry
				default:
					return Error.Ignore;
			}
		}

		[Export("cancelSelection:")]
		void CancelSelection(NSObject sender)
		{
			// If cancel is pressed, dismiss the selection view controller
			DismissViewController (true, null);
		}
	}
}
