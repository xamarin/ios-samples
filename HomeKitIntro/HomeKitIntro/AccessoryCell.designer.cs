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
	[Register ("AccessoryCell")]
	partial class AccessoryCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView AccessoryImage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel AccessoryName { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AccessoryImage != null) {
				AccessoryImage.Dispose ();
				AccessoryImage = null;
			}
			if (AccessoryName != null) {
				AccessoryName.Dispose ();
				AccessoryName = null;
			}
		}
	}
}
