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
	[Register ("CustomAppearanceViewController")]
	partial class CustomAppearanceViewController
	{
		[Outlet]
		UIKit.UISegmentedControl backgroundSwitcher { get; set; }

		[Action ("ConfigureNewNavigationBarBackground:")]
		partial void ConfigureNewNavigationBarBackground (UIKit.UISegmentedControl sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (backgroundSwitcher != null) {
				backgroundSwitcher.Dispose ();
				backgroundSwitcher = null;
			}
		}
	}
}
