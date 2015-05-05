using System;

using UIKit;
using CloudKit;
using Foundation;

namespace CloudCaptions
{
	[Register("SubscriptionController")]
	public class SubscriptionController : UIBarButtonItem
	{
		const string SubscriptionID = "autoUpdate";

		CKDatabase PublicDB {
			get {
				return CKContainer.DefaultContainer.PublicCloudDatabase;
			}
		}

		public SubscriptionController(IntPtr handle)
			: base(handle)
		{

		}

		public SubscriptionController (UIView customView)
			: base(customView)
		{
			this.Clicked += OnToggleSubscribtion;
			CheckSubscription ();
		}

		#region ToggleSubscribtion

		void OnToggleSubscribtion (object sender, EventArgs e)
		{
			ToggleSubscription ();
		}

		void ToggleSubscription()
		{
			var indicator = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.Gray);
			indicator.StartAnimating ();
			CustomView = indicator;

			if(Title == "Subscribe")
				Subscribe ();
			else if(Title == "Unsubscribe")
				Unsubscribe ();
			else
				CheckSubscription ();
		}

		#region Subscribe

		async void Subscribe()
		{
			CKSubscription subscriptionToUpload = CreateSubscription ();

			try {
				await PublicDB.SaveSubscriptionAsync(subscriptionToUpload);
				ResetButton("Unsubscribe");
			} catch (NSErrorException ex) {
				HandleSubscribeError (ex);
			}
		}

		static CKSubscription CreateSubscription ()
		{
			var subNotification = new CKNotificationInfo {
				AlertBody = "New Post"
			};

			CKSubscription subscriptionToUpload = new CKSubscription (Post.RecordType, NSPredicate.FromValue (true), SubscriptionID, CKSubscriptionOptions.FiresOnRecordCreation);
			subscriptionToUpload.NotificationInfo = subNotification;

			return subscriptionToUpload;
		}

		void HandleSubscribeError (NSErrorException ex)
		{
			var error = ex.Error;
			Error errorResult = HandleError (error);

			switch (errorResult) {
				case Error.Success:
					if (error.Code == (long)CKErrorCode.UnknownItem)
						Console.WriteLine ("If you see this it's because you've tried to subscribe to new Post records when CloudKit hasn't seen the Post record type yet. Either manually create the record type in dashboard or upload a post");
					CheckSubscription ();
					break;

				case Error.Retry:
					Utils.Retry (ToggleSubscription, error);
					break;

				case Error.Ignore:
					Console.WriteLine ("Ignored error while saving subscription: {0}", error.Description);
					break;

				default:
					break;
			}
		}

		#endregion

		#region Unsubscribe

		async void Unsubscribe()
		{
			try {
				await PublicDB.DeleteSubscriptionAsync (SubscriptionID);
				CheckSubscription();
			} catch(NSErrorException ex) {
				HandleUnsubscribeError (ex);
			}
		}

		void HandleUnsubscribeError (NSErrorException ex)
		{
			var error = ex.Error;
			var errorResult = HandleError (error);

			switch (errorResult) {
				case Error.Retry:
					Utils.Retry (ToggleSubscription, error);
					break;

				case Error.Ignore:
					Console.WriteLine ("Ignored error while deleting subscription: {0}", error.Description);
					break;

				default:
					throw new NotImplementedException ();
			}
		}

		#endregion

		#endregion

		void OnCheckSubscriptionClicked(object sender, EventArgs e)
		{
			CheckSubscription ();
		}

		async void CheckSubscription()
		{
			try {
				var subscription = await PublicDB.FetchSubscriptionAsync(SubscriptionID);
				if (subscription != null)
					ResetButton("Unsubscribe");
			} catch(NSErrorException ex) {
				HandleCheckSubscriptionError (ex);
			}
		}

		void HandleCheckSubscriptionError(NSErrorException ex)
		{
			NSError error = ex.Error;
			Error errorResult = HandleError (error);

			switch (errorResult) {
				case Error.Success:
					if (ex.Error.Code == (long)CKErrorCode.UnknownItem)
						ResetButton ("Subscribe");
					break;

				case Error.Retry:
					Utils.Retry (CheckSubscription, error);
					break;

				case Error.Ignore:
					Console.WriteLine ("Ignored error while checking subscription: {0}", error.Description);
					ResetButton ("?");
					break;

				default:
					throw new NotImplementedException ();
			}
		}

		void ResetButton(string title)
		{
			InvokeOnMainThread (() => {
				Title = title;
				CustomView = null;
			});
		}

		Error HandleError(NSError error)
		{
			if (error == null) {
				return Error.Success;
			}
			switch ((CKErrorCode)(long)error.Code)
			{
				// This error occurs if it can't find the subscription named autoUpdate. (It tries to delete one that doesn't exits or it searches for one it can't find)
				// This is okay and expected behavior
				case CKErrorCode.UnknownItem:
					return Error.Success;

				case CKErrorCode.NetworkUnavailable:
				case CKErrorCode.NetworkFailure:
					// A reachability check might be appropriate here so we don't just keep retrying if the user has no service
				case CKErrorCode.ServiceUnavailable:
				case CKErrorCode.RequestRateLimited:
					return Error.Retry;

				case CKErrorCode.BadDatabase:
				case CKErrorCode.IncompatibleVersion:
				case CKErrorCode.BadContainer:
				case CKErrorCode.PermissionFailure:
				case CKErrorCode.MissingEntitlement:
					// This app uses the publicDB with default world readable permissions
				case CKErrorCode.AssetFileNotFound:
				case CKErrorCode.PartialFailure:
					// These shouldn't occur during a subscription operation
				case CKErrorCode.QuotaExceeded:
					// We should not retry if it'll exceed our quota
				case CKErrorCode.OperationCancelled:
					// Nothing to do here, we intentionally cancelled
				case CKErrorCode.NotAuthenticated:
					// User must be logged in
				case CKErrorCode.InvalidArguments:
				case CKErrorCode.ResultsTruncated:
				case CKErrorCode.ServerRecordChanged:
				case CKErrorCode.AssetFileModified:
				case CKErrorCode.ChangeTokenExpired:
				case CKErrorCode.BatchRequestFailed:
				case CKErrorCode.ZoneBusy:
				case CKErrorCode.ZoneNotFound:
				case CKErrorCode.LimitExceeded:
				case CKErrorCode.UserDeletedZone:
					// All of these errors are irrelevant for this subscription operation
				case CKErrorCode.InternalError:
				case CKErrorCode.ServerRejectedRequest:
				case CKErrorCode.ConstraintViolation:
					//Non-recoverable, should not retry
				default:
					return Error.Ignore;
			}
		}
	}
}

