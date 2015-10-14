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
	[Register ("TouchViewController")]
	partial class TouchViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIImageView DoubleTouchImage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIImageView DragImage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIImageView TouchImage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel TouchStatus { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView TouchView { get; set; }

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
			if (TouchImage != null) {
				TouchImage.Dispose ();
				TouchImage = null;
			}
			if (TouchStatus != null) {
				TouchStatus.Dispose ();
				TouchStatus = null;
			}
			if (TouchView != null) {
				TouchView.Dispose ();
				TouchView = null;
			}
		}
	}
}
