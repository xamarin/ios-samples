// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace UICatalog
{
	[Register ("CustomSearchBarViewController")]
	partial class CustomSearchBarViewController
	{
		[Outlet]
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
