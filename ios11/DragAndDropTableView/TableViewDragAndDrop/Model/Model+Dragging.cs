using System;
using UIKit;
using Foundation;
using MobileCoreServices;

namespace TableViewDragAndDrop
{
	/// <summary>
	/// Helper methods for providing and consuming drag-and-drop data
	/// </summary>
	public partial class Model
    {
		/// <summary>
		/// A helper function that serves as an interface to the data model,
		/// called by the implementation of the `tableView(_ canHandle:)` method.
        /// </summary>
        public bool CanHandle(IUIDropSession session)
        {
            return session.CanLoadObjects(typeof(NSString));
        }

		/// <summary>
		/// A helper function that serves as an interface to the data mode, called
		/// by the `tableView(_:itemsForBeginning:at:)` method.
        /// </summary>
        public UIDragItem[] DragItems(NSIndexPath indexPath)
        {
            var placeName = PlaceNames[indexPath.Row];

            var data = NSData.FromString(placeName, NSStringEncoding.UTF8);
            var itemProvider = new NSItemProvider();

            itemProvider.RegisterDataRepresentation(UTType.PlainText, 
                                                    NSItemProviderRepresentationVisibility.All, 
                                                    (completion) =>
            {
                completion(data, null);
                return null;
            });

            return new UIDragItem[] { new UIDragItem(itemProvider) };
        }
    }
}
