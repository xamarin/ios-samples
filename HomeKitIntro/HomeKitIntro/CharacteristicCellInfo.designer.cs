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
	[Register ("CharacteristicCellInfo")]
	partial class CharacteristicCellInfo
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel SubTItle { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel Title { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (SubTItle != null) {
				SubTItle.Dispose ();
				SubTItle = null;
			}
			if (Title != null) {
				Title.Dispose ();
				Title = null;
			}
		}
	}
}
