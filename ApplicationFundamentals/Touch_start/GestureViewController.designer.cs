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
	[Register ("GestureViewController")]
	partial class GestureViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView DoubleTouchImage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView DragImage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel TouchStatus { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (DoubleTouchImage != null) {
				DoubleTouchImage.Dispose ();
				DoubleTouchImage = null;
			}
			if (DragImage != null) {
				DragImage.Dispose ();
				DragImage = null;
			}
			if (TouchStatus != null) {
				TouchStatus.Dispose ();
				TouchStatus = null;
			}
		}
	}
}
