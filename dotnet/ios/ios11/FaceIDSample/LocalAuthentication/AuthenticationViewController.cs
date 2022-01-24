using LocalAuthentication;

namespace StoryboardTable;

/*
	* https://developer.apple.com/ios/human-interface-guidelines/user-interaction/authentication/
	*
	* NSFaceIDUsageDescription
	* https://developer.apple.com/library/content/documentation/General/Reference/InfoPlistKeyReference/Articles/CocoaKeys.html#//apple_ref/doc/uid/TP40009251-SW75
	*
	* https://developer.apple.com/documentation/localauthentication/labiometrytype
	* https://developer.apple.com/documentation/localauthentication/lapolicy/1622327-deviceownerauthenticationwithbio
	*/
/// <remarks>
/// The code for dealing with biometrics (first TouchID, then FaceID) requires iOS 8 or later. 
/// New properties/methods were added in iOS 9 and iOS 11, so these must be gated with a version check.
///
/// *Remember* in iOS 11 to add the Info.plist key NSFaceIDUsageDescription
/// </remarks>
partial class AuthenticationViewController : UIViewController
{
	LAContextReplyHandler replyHandler;
	/// <summary>String to use for display</summary>
	string BiometryType = "";

	public AuthenticationViewController(IntPtr handle) : base(handle)
	{
	}

	public override void ViewWillAppear(bool animated)
	{
		base.ViewWillAppear(animated);

		// bind every time, to reflect deletion in the Detail view
		unAuthenticatedLabel.Text = "";

		var context = new LAContext();
		var buttonText = "";
		if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out var authError1))
		{ // has Biometrics (Touch or Face)
			if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
			{
				context.LocalizedReason = "Authorize for access to secrets"; // iOS 11
				BiometryType = context.BiometryType == LABiometryType.TouchId ? "Touch ID" : "Face ID";
				buttonText = $"Login with {BiometryType}";
			}
			else
			{   // no FaceID before iOS 11
				buttonText = $"Login with Touch ID";
			}
		}
		else if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthentication, out var authError2))
		{
			buttonText = $"Login"; // with device PIN
			BiometryType = "Device PIN";
		}
		else
		{
			// Application might choose to implement a custom username/password
			buttonText = "Use unsecured";
			BiometryType = "none";
		}
		AuthenticateButton.SetTitle(buttonText, UIControlState.Normal);
	}

	partial void AuthenticateMe(UIButton sender)
	{
		var context = new LAContext();
		NSError AuthError;
		var localizedReason = new NSString("To access secrets");

		// because LocalAuthentication APIs have been extended over time, need to check iOS version before setting some properties
		context.LocalizedFallbackTitle = "Fallback"; // iOS 8

		if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
		{
			context.LocalizedCancelTitle = "Cancel"; // iOS 10
		}
		if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
		{
			context.LocalizedReason = "Authorize for access to secrets"; // iOS 11
			BiometryType = context.BiometryType == LABiometryType.TouchId ? "TouchID" : "FaceID";
		}

		//Use canEvaluatePolicy method to test if device is TouchID or FaceID enabled
		//Use the LocalAuthentication Policy DeviceOwnerAuthenticationWithBiometrics
		if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out AuthError))
		{
			Console.WriteLine("TouchID/FaceID available/enrolled");
			replyHandler = new LAContextReplyHandler((success, error) =>
			{
				//Make sure it runs on MainThread, not in Background
				this.InvokeOnMainThread(() =>
				{
					if (success)
					{
						Console.WriteLine($"You logged in with {BiometryType}!");

						PerformSegue("AuthenticationSegue", this);
					}
					else
					{
						Console.WriteLine(error.LocalizedDescription);
						//Show fallback mechanism here
						unAuthenticatedLabel.Text = $"{BiometryType} Authentication Failed";
						//AuthenticateButton.Hidden = true;
					}
				});

			});
			//Use evaluatePolicy to start authentication operation and show the UI as an Alert view
			//Use the LocalAuthentication Policy DeviceOwnerAuthenticationWithBiometrics
			context.EvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, localizedReason, replyHandler);
		}
		else if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthentication, out AuthError))
		{
			Console.WriteLine("When TouchID/FaceID aren't available or enrolled, use the device PIN");
			replyHandler = new LAContextReplyHandler((success, error) =>
			{
				//Make sure it runs on MainThread, not in Background
				this.InvokeOnMainThread(() =>
				{
					if (success)
					{
						Console.WriteLine($"You logged in with {BiometryType}!");

						PerformSegue("AuthenticationSegue", this);
					}
					else
					{
						Console.WriteLine(error.LocalizedDescription);
						//Show fallback mechanism here
						unAuthenticatedLabel.Text = "Device PIN Authentication Failed";
						AuthenticateButton.Hidden = true;
					}
				});

			});
			//Use evaluatePolicy to start authentication operation and show the UI as an Alert view
			//Use the LocalAuthentication Policy DeviceOwnerAuthenticationWithBiometrics
			context.EvaluatePolicy(LAPolicy.DeviceOwnerAuthentication, localizedReason, replyHandler);
		}
		else
		{
			// User hasn't configured a PIN or any biometric auth.
			// App may implement its own login, or choose to allow open access
			unAuthenticatedLabel.Text = "No device auth configured";

			var okCancelAlertController = UIAlertController.Create("No authentication", "This device does't have authentication configured.", UIAlertControllerStyle.Alert);
			okCancelAlertController.AddAction(UIAlertAction.Create("Use unsecured", UIAlertActionStyle.Default, alert => PerformSegue("AuthenticationSegue", this)));
			okCancelAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));
			PresentViewController(okCancelAlertController, true, null);
		}
	}
}
