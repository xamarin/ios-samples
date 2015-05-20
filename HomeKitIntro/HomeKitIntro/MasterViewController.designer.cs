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
	[Register ("MasterViewController")]
	partial class MasterViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITabBarItem HomeTab { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (HomeTab != null) {
				HomeTab.Dispose ();
				HomeTab = null;
			}
		}
	}
}
