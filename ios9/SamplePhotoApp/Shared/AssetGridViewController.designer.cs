using UIKit;
using Foundation;

namespace SamplePhotoApp
{
	[Register ("AssetGridViewController")]
	partial class AssetGridViewController
	{
		[Outlet]
		UIBarButtonItem AddButton { get; set; }

		// TODO: fix selector
		[Action ("AddButtonClickHandler:")]
		partial void AddButtonClickHandler (NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AddButton != null) {
				AddButton.Dispose ();
				AddButton = null;
			}
		}
	}
}
