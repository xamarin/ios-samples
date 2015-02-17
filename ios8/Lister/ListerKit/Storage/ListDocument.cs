using System;

using UIKit;
using Foundation;
using CoreFoundation;

namespace ListerKit
{
	public class ListDocument : UIDocument
	{
		public IListPresenter ListPresenter { get; private set; }
		public IListDocumentDelegate Delegate { get; set; }

		public ListDocument (NSUrl url, IListPresenter presenter)
			: base(url)
		{
			ListPresenter = presenter;
		}

		public override bool LoadFromContents (NSObject contents, string typeName, out NSError outError)
		{
			outError = null;

			List unarchivedList = (List)NSKeyedUnarchiver.UnarchiveObject ((NSData)contents);
			if (unarchivedList != null) {
				// This method is called on the queue that the openWithCompletionHandler: method was called
				// on (typically, the main queue). List presenter operations are main queue only, so explicitly
				// call on the main queue.
				DispatchQueue.MainQueue.DispatchAsync (() => {
					ListPresenter.SetList (unarchivedList);
				});

				return true;
			}

			outError = new NSError (NSError.CocoaErrorDomain, (int)NSCocoaError.FileReadCorruptFile, new NSDictionary (
				NSError.LocalizedDescriptionKey, "Could not read file",
				NSError.LocalizedFailureReasonErrorKey, "File was in an invalid format"
			));

			return false;
		}

		public override NSObject ContentsForType (string typeName, out NSError outError)
		{
			outError = null;
			List archiveableList = ListPresenter.ArchiveableList;
			return archiveableList != null ? NSKeyedArchiver.ArchivedDataWithRootObject (archiveableList) : null;
		}

		public override void AccommodatePresentedItemDeletion (Action<NSError> completionHandler)
		{
			Delegate.WasDeleted (this);
		}

		public override void UpdateUserActivityState (NSUserActivity userActivity)
		{
			base.UpdateUserActivityState (userActivity);
			var entries = new NSDictionary (AppConfig.UserActivityListColorUserInfoKey, AppColors.ColorFrom (ListPresenter.Color));
			userActivity.AddUserInfoEntries (entries);
		}
	}
}