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

namespace UIKitEnhancements
{
	[Register ("MainMenuTableCell")]
	partial class MainMenuTableCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel Subtitle { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel Title { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (Subtitle != null) {
				Subtitle.Dispose ();
				Subtitle = null;
			}
			if (Title != null) {
				Title.Dispose ();
				Title = null;
			}
		}
	}
}
