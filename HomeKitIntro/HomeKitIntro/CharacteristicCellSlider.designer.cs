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
	[Register ("CharacteristicCellSlider")]
	partial class CharacteristicCellSlider
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISlider Slider { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel SubTItle { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel Title { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (Slider != null) {
				Slider.Dispose ();
				Slider = null;
			}
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
