// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace AddingTheSignInWithAppleFlowToYourApp
{
	[Register ("ResultViewController")]
	partial class ResultViewController
	{
		[Outlet]
		UIKit.UILabel emailLabel { get; set; }

		[Outlet]
		UIKit.UILabel familyNameLabel { get; set; }

		[Outlet]
		UIKit.UILabel givenNameLabel { get; set; }

		[Outlet]
		UIKit.UIButton signOutButton { get; set; }

		[Outlet]
		UIKit.UILabel userIdentifierLabel { get; set; }

		[Action ("SignOutButtonPressed:")]
		partial void SignOutButtonPressed (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (emailLabel != null) {
				emailLabel.Dispose ();
				emailLabel = null;
			}

			if (familyNameLabel != null) {
				familyNameLabel.Dispose ();
				familyNameLabel = null;
			}

			if (givenNameLabel != null) {
				givenNameLabel.Dispose ();
				givenNameLabel = null;
			}

			if (signOutButton != null) {
				signOutButton.Dispose ();
				signOutButton = null;
			}

			if (userIdentifierLabel != null) {
				userIdentifierLabel.Dispose ();
				userIdentifierLabel = null;
			}
		}
	}
}
