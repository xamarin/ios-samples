// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace PhotoProgress
{
	[Register ("PhotosViewController")]
	partial class PhotosViewController
	{
		[Outlet]
		UIKit.UIBarButtonItem CancelToolbarItem { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem PauseToolbarItem { get; set; }

		[Outlet]
		UIKit.UIView ProgressContainerView { get; set; }

		[Outlet]
		UIKit.UIProgressView ProgressView { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem ResetToolbarItem { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem ResumeToolbarItem { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem StartToolbarItem { get; set; }

		[Action ("CanceltImport:")]
		partial void CanceltImport (Foundation.NSObject sender);

		[Action ("PauseImport:")]
		partial void PauseImport (Foundation.NSObject sender);

		[Action ("ResetImport:")]
		partial void ResetImport (Foundation.NSObject sender);

		[Action ("ResumeImport:")]
		partial void ResumeImport (Foundation.NSObject sender);

		[Action ("StartImport:")]
		partial void StartImport (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (CancelToolbarItem != null) {
				CancelToolbarItem.Dispose ();
				CancelToolbarItem = null;
			}

			if (PauseToolbarItem != null) {
				PauseToolbarItem.Dispose ();
				PauseToolbarItem = null;
			}

			if (ProgressContainerView != null) {
				ProgressContainerView.Dispose ();
				ProgressContainerView = null;
			}

			if (ProgressView != null) {
				ProgressView.Dispose ();
				ProgressView = null;
			}

			if (ResetToolbarItem != null) {
				ResetToolbarItem.Dispose ();
				ResetToolbarItem = null;
			}

			if (ResumeToolbarItem != null) {
				ResumeToolbarItem.Dispose ();
				ResumeToolbarItem = null;
			}

			if (StartToolbarItem != null) {
				StartToolbarItem.Dispose ();
				StartToolbarItem = null;
			}
		}
	}
}
