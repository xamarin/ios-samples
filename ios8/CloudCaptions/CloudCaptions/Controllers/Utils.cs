using System;
using System.Threading.Tasks;

using Foundation;

namespace CloudCaptions
{
	public static class Utils
	{
		public static void Retry(Action handler, NSError error)
		{
			int delayInSec = FetchRetryDelay (error);
			PrintLog (error, delayInSec);
			Retry (handler, delayInSec);
		}

		static int FetchRetryDelay(NSError error)
		{
			CKErrorInfo errorInfo = new CKErrorInfo (error.UserInfo);
			return (int)(errorInfo.RetryAfter.HasValue ? errorInfo.RetryAfter.Value : 3);
		}

		static void PrintLog(NSError error, int retryDelay)
		{
			Console.WriteLine ("Error: {0}. Recoverable, retry after {1} seconds", error.Description, retryDelay);
		}

		static void Retry(Action action, int retryDelayInSec)
		{
			Task.Delay ((int)retryDelayInSec * 1000).ContinueWith (_ => action ());
		}

		public static NSPredicate CreateTagPredicate(string tags)
		{
			return NSPredicate.FromFormat ("Tags CONTAINS %@", (NSString)tags);
		}

		public static NSPredicate CreateAfterPredicate(NSDate date)
		{
			return NSPredicate.FromFormat ("creationDate > %@", date);
		}

		public static NSSortDescriptor[] CreateCreationDateDescriptor(bool ascending)
		{
			return new NSSortDescriptor[] {
				new NSSortDescriptor ("creationDate", ascending)
			};
		}
	}
}

