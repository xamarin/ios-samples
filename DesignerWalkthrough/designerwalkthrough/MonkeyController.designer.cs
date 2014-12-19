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

namespace DesignerWalkthrough
{
	[Register ("MonkeyController")]
	partial class MonkeyController
	{
		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		UIKit.UIButton backButton { get; set; }

		[Action ("Clicked:")]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		partial void Clicked (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (backButton != null) {
				backButton.Dispose ();
				backButton = null;
			}
		}
	}
}
