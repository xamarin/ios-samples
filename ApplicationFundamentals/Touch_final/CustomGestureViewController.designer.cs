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

namespace Touch
{
	[Register ("CustomGestureViewController")]
	partial class CustomGestureViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView CheckboxImage { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (CheckboxImage != null) {
				CheckboxImage.Dispose ();
				CheckboxImage = null;
			}
		}
	}
}
