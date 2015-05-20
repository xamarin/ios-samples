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
	[Register ("AddHomeViewController")]
	partial class AddHomeViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton AddHome { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField HomeName { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AddHome != null) {
				AddHome.Dispose ();
				AddHome = null;
			}
			if (HomeName != null) {
				HomeName.Dispose ();
				HomeName = null;
			}
		}
	}
}
