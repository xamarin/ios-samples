using System;
using Foundation;
using UIKit;
using System.Linq;

namespace PhotoAlbum
{
    public partial class AlbumTableViewController
    {
		public UIDragItem[] GetItemsForBeginningDragSession(UITableView tableView, IUIDragSession session, NSIndexPath indexPath)
		{
            if (tableView.Editing)
            {
                return new UIDragItem[0];
            }
            var dragItems = DragItems(indexPath);
            return dragItems;
		}

        UIDragItem[] DragItems (NSIndexPath forAlbumAt)
        {
            var album = this.Album(forAlbumAt);
            var dragItems = album.Photos.Select((photo) => {
                var itemProvider = photo.ItemProvider;
                var dragItem = new UIDragItem(itemProvider);
                dragItem.LocalObject = photo;
                dragItem.PreviewProvider = photo.PreviewProvider;
                return dragItem;
            });
            return dragItems.ToArray();
        }

        [Export("tableView:dragSessionWillBegin:")]
        public void DragSessionWillBegin(UIKit.UITableView tableView, UIKit.IUIDragSession session)
        {
            NavigationItem.RightBarButtonItem.Enabled = false;
        }

        [Export("tableView:dragSessionDidEnd:")]
        public void DragSessionDidEnd(UIKit.UITableView tableView, UIKit.IUIDragSession session)
        {
            NavigationItem.RightBarButtonItem.Enabled = true;
        }
	}
}
