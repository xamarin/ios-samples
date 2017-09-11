using UIKit;
using Foundation;

namespace SamplePhotoApp
{
	[Register ("AssetGridViewController")]
	partial class AssetGridViewController
	{
		[Outlet ("addButtonItem")]
		UIBarButtonItem AddButtonItem { get; set; }

		[Action ("addAsset:")]
		partial void AddAsset (NSObject sender);

		void ReleaseDesignerOutlets ()
		{
			if (AddButtonItem != null)
			{
				AddButtonItem.Dispose ();
				AddButtonItem = null;
			}
		}
	}
}
