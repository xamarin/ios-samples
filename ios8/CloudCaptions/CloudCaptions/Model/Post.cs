using System;
using System.Threading.Tasks;

using CloudKit;
using Foundation;

namespace CloudCaptions
{
	public class Post
	{
		public static readonly string RecordType = "Post";
		public static readonly string TextKey = "ImageText";
		public static readonly string FontKey = "Font";
		public static readonly string ImageRefKey = "ImageRef";
		public static readonly string TagsKey = "Tags";

		public Image ImageRecord { get; set; }
		public CKRecord PostRecord { get; private set; }

		CKDatabase PublicDB {
			get {
				return CKContainer.DefaultContainer.PublicCloudDatabase;
			}
		}

		public Post (CKRecord postRecord)
		{
			PostRecord = postRecord;
		}

		public void LoadImage (string[] keys, Action updateBlock = null)
		{
			// Fetches the imageRecord this post record references in its ImageRefKey.
			// Only fetches the values associated with the keys passed in to the NSArray
			var imgRecordId = ((CKReference)PostRecord[ImageRefKey]).RecordId;
			CKFetchRecordsOperation imageOp = new CKFetchRecordsOperation (new CKRecordID[]{
				imgRecordId
			});
			imageOp.DesiredKeys = keys;
			imageOp.Completed = (NSDictionary recordDict, NSError error) => {
				if(error != null && error.Code == (long)CKErrorCode.PartialFailure) {
					CKErrorInfo info = new CKErrorInfo(error);
					error = info[imgRecordId];
				}

				Error errorResponse = HandleError(error);
				switch(errorResponse) {
					case Error.Success:
						CKRecord fetchedImageRecord = (CKRecord)recordDict[imgRecordId];
						ImageRecord = new Image(fetchedImageRecord);
						if(updateBlock != null)
							updateBlock();
						break;

					case Error.Retry:
						Utils.Retry(() => LoadImage(keys, updateBlock), error);
						ImageRecord = null;
						break;

					case Error.Ignore:
						Console.WriteLine ("Error: {0}", error.Description);
						ImageRecord = null;
						break;

					default:
						throw new NotImplementedException();
				}
			};
			PublicDB.AddOperation (imageOp);
		}

		Error HandleError(NSError error)
		{
			if (error == null)
				return Error.Success;

			switch ((CKErrorCode)(long)error.Code) {
				case CKErrorCode.NetworkUnavailable:
				case CKErrorCode.NetworkFailure:
					// A reachability check might be appropriate here so we don't just keep retrying if the user has no service
				case CKErrorCode.ServiceUnavailable:
				case CKErrorCode.RequestRateLimited:
					return Error.Retry;

				case CKErrorCode.BadContainer:
				case CKErrorCode.MissingEntitlement:
				case CKErrorCode.PermissionFailure:
				case CKErrorCode.BadDatabase:
					// This app uses the publicDB with default world readable permissions
				case CKErrorCode.UnknownItem:
				case CKErrorCode.AssetFileNotFound:
					// This shouldn't happen. If an Image record is deleted it should delete all Post records that reference it (CKReferenceActionDeleteSelf)
				case CKErrorCode.IncompatibleVersion:
				case CKErrorCode.QuotaExceeded:
					//App quota will be exceeded, cancelling operation
				case CKErrorCode.OperationCancelled:
					// Nothing to do here, we intentionally cancelled
				case CKErrorCode.NotAuthenticated:
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
					// These errors are pretty irrelevant here
					// We're fetching only one record by its recordID
					// These errors could be hit fetching multiple records, using zones, saving records, or fetching with predicates
				case CKErrorCode.InternalError:
				case CKErrorCode.ServerRejectedRequest:
				case CKErrorCode.ConstraintViolation:
					Console.WriteLine ("Nonrecoverable error, will not retry");
					return Error.Ignore;

				default:
					return Error.Ignore;
			}
		}
	}
}

