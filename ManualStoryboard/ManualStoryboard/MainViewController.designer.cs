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

namespace ManualStoryboard
{
	[Register ("MainViewController")]
	partial class MainViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton PinkButton { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (PinkButton != null) {
				PinkButton.Dispose ();
				PinkButton = null;
			}
		}
	}
}
