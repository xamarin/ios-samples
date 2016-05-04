// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace iTravel
{
    [Register ("AlbumViewController")]
    partial class AlbumViewController
    {
        [Outlet]
        UIKit.UITableView CustomTableView { get; set; }


        [Outlet]
        UIKit.UILabel DetailsLabel { get; set; }


        [Outlet]
        UIKit.UIProgressView ProgressView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CustomTableView != null) {
                CustomTableView.Dispose ();
                CustomTableView = null;
            }

            if (DetailsLabel != null) {
                DetailsLabel.Dispose ();
                DetailsLabel = null;
            }

            if (ProgressView != null) {
                ProgressView.Dispose ();
                ProgressView = null;
            }
        }
    }
}