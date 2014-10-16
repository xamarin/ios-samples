using System;

using Foundation;
using CloudKit;

namespace CloudCaptions
{
	// TODO: https://trello.com/c/Yu3OnaUH
	public class CKErrorInfo : DictionaryContainer
	{
		public nint? RetryAfter {
			get {
				NSObject obj;
				if (!base.Dictionary.TryGetValue (CKErrorFields.ErrorRetryAfterKey, out obj))
					return (nint?)null;

				return ((NSNumber)obj).NIntValue;
			}
		}

		public NSError this[CKRecordID key] {
			get {
				NSDictionary dict = base.GetNSDictionary (CKErrorFields.PartialErrorsByItemIdKey);
				if (dict == null)
					return null;

				NSObject obj;
				if (!dict.TryGetValue (key, out obj))
					return null;

				return (NSError)obj;
			}
		}

		public CKErrorInfo (NSError error)
			: this(error.UserInfo)
		{
		}

		public CKErrorInfo (NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}

