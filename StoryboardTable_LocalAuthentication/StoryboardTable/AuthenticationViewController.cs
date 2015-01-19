using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using LocalAuthentication;
using Security;

namespace StoryboardTable
{
	partial class AuthenticationViewController : UIViewController
	{
		LAContextReplyHandler replyHandler;

		public AuthenticationViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// bind every time, to reflect deletion in the Detail view
			unAuthenticatedLabel.Text="";
		}

		partial void AuthenticateMe (UIButton sender)
		{
			var context = new LAContext();
			NSError AuthError;
			var localizedReason = new NSString("To add a new chore");

			//Use canEvaluatePolicy method to test if device is TouchID enabled
			//Use the LocalAuthentication Policy DeviceOwnerAuthenticationWithBiometrics
			if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out AuthError)){
				replyHandler = new LAContextReplyHandler((success, error) => {
					//Make sure it runs on MainThread, not in Background
					this.InvokeOnMainThread(()=>{
						if(success){
							Console.WriteLine("You logged in!");
							PerformSegue("AuthenticationSegue", this);
						}
						else{
							//Show fallback mechanism here
							unAuthenticatedLabel.Text="Oh Noes";
							AuthenticateButton.Hidden= true;
						}
					});

				});
				//Use evaluatePolicy to start authentication operation and show the UI as an Alert view
				//Use the LocalAuthentication Policy DeviceOwnerAuthenticationWithBiometrics
				context.EvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, localizedReason, replyHandler);
			};
		}

	}

}

