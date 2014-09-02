using Foundation;
using UIKit;

namespace ImageInverterExt
{
	[Register ("ImageInverterViewController")]
	partial class ImageInverterViewController
	{
		[Outlet("imageView")]
		private UIImageView ImageView { get; set; }

		[Outlet("cancelButton")]
		private UIBarButtonItem CancelButton { get; set; }

		[Outlet("doneButton")]
		private UIBarButtonItem DoneButton { get; set; }

		[Action("cancel:")]
		partial void OnCancelClicked(UIButton sender);

		[Action("done:")]
		partial void OnDoneClicked(UIBarButtonItem sender);

		private void ReleaseDesignerOutlets ()
		{
		}
	}
}
