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
	[Register ("AvailableCell")]
	partial class AvailableCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView AvailableImage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel AvailableName { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AvailableImage != null) {
				AvailableImage.Dispose ();
				AvailableImage = null;
			}
			if (AvailableName != null) {
				AvailableName.Dispose ();
				AvailableName = null;
			}
		}
	}
}
