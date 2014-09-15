using System;

using UIKit;
using Foundation;

using Common;

namespace ListerKit
{
	public class ListDocument : UIDocument
	{
		public event EventHandler DocumentDeleted;

		public List List { get; set; }

		public ListDocument (NSUrl url)
			: base(url)
		{
			List = new List ();
		}

		public override bool LoadFromContents (NSObject contents, string typeName, out NSError outError)
		{
			outError = null;

			List deserializedList = (List)NSKeyedUnarchiver.UnarchiveObject ((NSData)contents);
			if (deserializedList!= null) {
				List = deserializedList;
				return true;
			}

			outError = new NSError (NSError.CocoaErrorDomain, (int)NSCocoaError.FileReadCorruptFile, NSDictionary.FromObjectsAndKeys (
				new NSObject[] {
					(NSString)"Could not read file",
					(NSString)"File was in an invalid format"
				},
				new NSObject[] {
					NSError.LocalizedDescriptionKey,
					NSError.LocalizedFailureReasonErrorKey
				}
			));

			return false;
		}

		public override NSObject ContentsForType (string typeName, out NSError outError)
		{
			outError = null;
			NSData serializedList = NSKeyedArchiver.ArchivedDataWithRootObject (List);

			if (serializedList != null)
				return serializedList;

			outError = new NSError (NSError.CocoaErrorDomain, (int)NSCocoaError.FileReadUnknown, NSDictionary.FromObjectsAndKeys (
				new NSObject[] {
					(NSString)"Could not save file",
					(NSString)"An unexpected error occured"
				},
				new NSObject[] {
					NSError.LocalizedDescriptionKey,
					NSError.LocalizedFailureReasonErrorKey
				}
			));

			return null;
		}

		public override void AccommodatePresentedItemDeletion (Action<NSError> completionHandler)
		{
			var handler = DocumentDeleted;
			if (handler != null)
				handler (this, EventArgs.Empty);
		}
	}
}

