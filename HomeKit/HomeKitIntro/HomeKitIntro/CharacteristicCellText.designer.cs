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
	[Register ("CharacteristicCellText")]
	partial class CharacteristicCellText
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel SubTitle { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField Text { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel TItle { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (SubTitle != null) {
				SubTitle.Dispose ();
				SubTitle = null;
			}
			if (Text != null) {
				Text.Dispose ();
				Text = null;
			}
			if (TItle != null) {
				TItle.Dispose ();
				TItle = null;
			}
		}
	}
}
