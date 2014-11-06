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

namespace StoryboardTable
{
	[Register ("AuthenticationViewController")]
	partial class AuthenticationViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton AuthenticateButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel unAuthenticatedLabel { get; set; }

		[Action ("AuthenticateMe:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void AuthenticateMe (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (AuthenticateButton != null) {
				AuthenticateButton.Dispose ();
				AuthenticateButton = null;
			}
			if (unAuthenticatedLabel != null) {
				unAuthenticatedLabel.Dispose ();
				unAuthenticatedLabel = null;
			}
		}
	}
}
