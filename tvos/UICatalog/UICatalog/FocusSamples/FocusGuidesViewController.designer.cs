// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace UICatalog
{
	[Register ("FocusGuidesViewController")]
	partial class FocusGuidesViewController
	{
		[Outlet]
		UIKit.UIButton BottomLeftButton { get; set; }

		[Outlet]
		UIKit.UIButton TopRightButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TopRightButton != null) {
				TopRightButton.Dispose ();
				TopRightButton = null;
			}

			if (BottomLeftButton != null) {
				BottomLeftButton.Dispose ();
				BottomLeftButton = null;
			}
		}
	}
}
