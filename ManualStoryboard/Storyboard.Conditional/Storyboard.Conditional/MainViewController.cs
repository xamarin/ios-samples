using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Storyboard.Conditional
{
	partial class MainViewController : UIViewController
	{
		

		public MainViewController (IntPtr handle) : base (handle)
		{
		}

		public override bool ShouldPerformSegue (string segueIdentifier, NSObject sender)
		{
			
			if(segueIdentifier == "SegueToPink"){
				if (PasswordTextField.Text == "password") {
					PasswordTextField.ResignFirstResponder ();
					return true;
				}
				else{
					ErrorLabel.Hidden = false;
					return false;
				}
			}
			return base.ShouldPerformSegue (segueIdentifier, sender);
		}
			
	}
}
