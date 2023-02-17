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
	[Register ("LoginViewController")]
	partial class LoginViewController
	{
		[Outlet]
		UIKit.UIStackView loginProviderStackView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (loginProviderStackView != null) {
				loginProviderStackView.Dispose ();
				loginProviderStackView = null;
			}
		}
	}
}
