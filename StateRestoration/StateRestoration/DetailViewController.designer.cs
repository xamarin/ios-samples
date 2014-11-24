// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace StateRestoration
{
	[Register ("DetailViewController")]
	partial class DetailViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView imageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem shareButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIToolbar toolBar { get; set; }

		[Action ("share:")]
		partial void Share (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (imageView != null) {
				imageView.Dispose ();
				imageView = null;
			}

			if (shareButton != null) {
				shareButton.Dispose ();
				shareButton = null;
			}

			if (toolBar != null) {
				toolBar.Dispose ();
				toolBar = null;
			}
		}
	}
}
