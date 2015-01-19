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

			outError = new NSError (NSError.CocoaErrorDomain, (int)NSCocoaError.FileReadCorruptFile, new NSDictionary (
				NSError.LocalizedDescriptionKey, "Could not read file",
				NSError.LocalizedFailureReasonErrorKey, "File was in an invalid format"
			));

			return false;
		}

		public override NSObject ContentsForType (string typeName, out NSError outError)
		{
			outError = null;
			NSData serializedList = NSKeyedArchiver.ArchivedDataWithRootObject (List);

			if (serializedList != null)
				return serializedList;

			outError = new NSError (NSError.CocoaErrorDomain, (int)NSCocoaError.FileReadUnknown, new NSDictionary (
				NSError.LocalizedDescriptionKey, "Could not save file",
				NSError.LocalizedFailureReasonErrorKey, "An unexpected error occured"
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

