// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace DesignerWalkthrough
{
	[Register ("MainViewController")]
	partial class MainViewController
	{
		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIButton monkeyButton { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (monkeyButton != null) {
				monkeyButton.Dispose ();
				monkeyButton = null;
			}
		}
	}
}
