using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace MonkeyBrowse
{
	public class UserActivityDelegate : NSUserActivityDelegate
	{
		#region Constructors
		public UserActivityDelegate ()
		{
		}
		#endregion

		#region Override Methods
		public override void UserActivityReceivedData (NSUserActivity userActivity, NSInputStream inputStream, NSOutputStream outputStream)
		{
			// Log
			Console.WriteLine ("User Activity Received Data: {0}", userActivity.Title);
		}

		public override void UserActivityWasContinued (NSUserActivity userActivity)
		{
			Console.WriteLine ("User Activity Was Continued: {0}", userActivity.Title);
		}

		public override void UserActivityWillSave (NSUserActivity userActivity)
		{
			Console.WriteLine ("User Activity will be Saved: {0}", userActivity.Title);
		}
		#endregion
	}	
}

