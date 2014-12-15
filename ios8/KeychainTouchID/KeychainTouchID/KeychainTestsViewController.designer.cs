// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace KeychainTouchID
{
	[Register ("KeychainTestsViewController")]
	partial class KeychainTestsViewController
	{
		[Outlet]
		UIKit.NSLayoutConstraint dynamicViewHeight { get; set; }

		[Outlet]
		UIKit.UITableView tableView { get; set; }

		[Outlet]
		UIKit.UITextView textView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (textView != null) {
				textView.Dispose ();
				textView = null;
			}

			if (dynamicViewHeight != null) {
				dynamicViewHeight.Dispose ();
				dynamicViewHeight = null;
			}
		}
	}
}
