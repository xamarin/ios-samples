// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace UICatalog
{
    [Register ("CustomSearchBarViewController")]
    partial class CustomSearchBarViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISearchBar searchBar { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (searchBar != null) {
                searchBar.Dispose ();
                searchBar = null;
            }
        }
    }
}