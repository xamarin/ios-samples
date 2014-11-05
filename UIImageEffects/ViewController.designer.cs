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

namespace UIImageEffects
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel effectLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView imageView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (effectLabel != null) {
				effectLabel.Dispose ();
				effectLabel = null;
			}
			if (imageView != null) {
				imageView.Dispose ();
				imageView = null;
			}
		}
	}
}
