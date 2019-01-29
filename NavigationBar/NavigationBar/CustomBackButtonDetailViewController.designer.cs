// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace NavigationBar
{
	[Register ("CustomBackButtonDetailViewController")]
	partial class CustomBackButtonDetailViewController
	{
		[Outlet]
		UIKit.UILabel cityLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (cityLabel != null) {
				cityLabel.Dispose ();
				cityLabel = null;
			}
		}
	}
}
