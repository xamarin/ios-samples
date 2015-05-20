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
	[Register ("CharacteristicCellStepper")]
	partial class CharacteristicCellStepper
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIStepper Stepper { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel SubTitle { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel Title { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (Stepper != null) {
				Stepper.Dispose ();
				Stepper = null;
			}
			if (SubTitle != null) {
				SubTitle.Dispose ();
				SubTitle = null;
			}
			if (Title != null) {
				Title.Dispose ();
				Title = null;
			}
		}
	}
}
