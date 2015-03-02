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

namespace WorkingWithImages
{
	[Register ("SecondViewController")]
	partial class SecondViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView MonkeyImage { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (MonkeyImage != null) {
				MonkeyImage.Dispose ();
				MonkeyImage = null;
			}
		}
	}
}
