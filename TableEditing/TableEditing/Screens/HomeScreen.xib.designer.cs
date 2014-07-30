// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace TableEditing.Screens
{
	[Register ("HomeScreen")]
	partial class HomeScreen
	{
		[Outlet]
		UIKit.UIBarButtonItem btnEdit { get; set; }

		[Outlet]
		UIKit.UITableView tblMain { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem btnDone { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnEdit != null) {
				btnEdit.Dispose ();
				btnEdit = null;
			}

			if (tblMain != null) {
				tblMain.Dispose ();
				tblMain = null;
			}

			if (btnDone != null) {
				btnDone.Dispose ();
				btnDone = null;
			}
		}
	}
}
