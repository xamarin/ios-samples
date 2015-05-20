// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace HomeKitIntro
{
	[Register ("AvailableViewController")]
	partial class AvailableViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITabBarItem AvailableTab { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AvailableTab != null) {
				AvailableTab.Dispose ();
				AvailableTab = null;
			}
		}
	}
}
