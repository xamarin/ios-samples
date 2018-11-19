// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

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
            if (btnDone != null) {
                btnDone.Dispose ();
                btnDone = null;
            }

            if (btnEdit != null) {
                btnEdit.Dispose ();
                btnEdit = null;
            }

            if (tblMain != null) {
                tblMain.Dispose ();
                tblMain = null;
            }
        }
    }
}