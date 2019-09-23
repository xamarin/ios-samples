using System;

using AuthenticationServices;
using CoreImage;
using Foundation;
using UIKit;

namespace AddingTheSignInWithAppleFlowToYourApp {
	public partial class LoginViewController : UIViewController, IASAuthorizationControllerDelegate, IASAuthorizationControllerPresentationContextProviding {
		public LoginViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			SetupProviderLoginView ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			PerformExistingAccountSetupFlows ();
		}

		void SetupProviderLoginView ()
		{
			var authorizationButton = new ASAuthorizationAppleIdButton (ASAuthorizationAppleIdButtonType.Default, ASAuthorizationAppleIdButtonStyle.White);
			authorizationButton.TouchUpInside += HandleAuthorizationAppleIDButtonPress;
			loginProviderStackView.AddArrangedSubview (authorizationButton);
		}

		// Prompts the user if an existing iCloud Keychain credential or Apple ID credential is found.
		void PerformExistingAccountSetupFlows ()
		{
			// Prepare requests for both Apple ID and password providers.
			ASAuthorizationRequest [] requests = {
				new ASAuthorizationAppleIdProvider ().CreateRequest (),
				new ASAuthorizationPasswordProvider ().CreateRequest ()
			};

			// Create an authorization controller with the given requests.
			var authorizationController = new ASAuthorizationController (requests);
			authorizationController.Delegate = this;
			authorizationController.PresentationContextProvider = this;
			authorizationController.PerformRequests ();
		}

		private void HandleAuthorizationAppleIDButtonPress (object sender, EventArgs e)
		{
			var appleIdProvider = new ASAuthorizationAppleIdProvider ();
			var request = appleIdProvider.CreateRequest ();
			request.RequestedScopes = new [] { ASAuthorizationScope.Email, ASAuthorizationScope.FullName };

			var authorizationController = new ASAuthorizationController (new [] { request });
			authorizationController.Delegate = this;
			authorizationController.PresentationContextProvider = this;
			authorizationController.PerformRequests ();
		}

		#region IASAuthorizationController Delegate

		[Export ("authorizationController:didCompleteWithAuthorization:")]
		public void DidComplete (ASAuthorizationController controller, ASAuthorization authorization)
		{
			if (authorization.GetCredential<ASAuthorizationAppleIdCredential> () is ASAuthorizationAppleIdCredential appleIdCredential) {
				var userIdentifier = appleIdCredential.User;
				var fullName = appleIdCredential.FullName;
				var email = appleIdCredential.Email;

				// Create an account in your system.
				// For the purpose of this demo app, store the userIdentifier in the keychain.
				try {
					new KeychainItem ("com.xamarin.AddingTheSignInWithAppleFlowToYourApp", "userIdentifier").SaveItem (userIdentifier);
				} catch (Exception) {
					Console.WriteLine ("Unable to save userIdentifier to keychain.");
				}

				// For the purpose of this demo app, show the Apple ID credential information in the ResultViewController.
				if (!(PresentingViewController is ResultViewController viewController))
					return;

				InvokeOnMainThread (() => {
					viewController.UserIdentifierText = userIdentifier;
					viewController.GivenNameText = fullName?.GivenName ?? "";
					viewController.FamilyNameText = fullName?.FamilyName ?? "";
					viewController.EmailText = email ?? "";

					DismissViewController (true, null);
				});
			} else if (authorization.GetCredential<ASPasswordCredential> () is ASPasswordCredential passwordCredential) {
				// Sign in using an existing iCloud Keychain credential.
				var username = passwordCredential.User;
				var password = passwordCredential.Password;

				// For the purpose of this demo app, show the password credential as an alert.
				InvokeOnMainThread (() => {
					var message = $"The app has received your selected credential from the keychain. \n\n Username: {username}\n Password: {password}";
					var alertController = UIAlertController.Create ("Keychain Credential Received", message, UIAlertControllerStyle.Alert);
					alertController.AddAction (UIAlertAction.Create ("Dismiss", UIAlertActionStyle.Cancel, null));

					PresentViewController (alertController, true, null);
				});
			}
		}

		[Export ("authorizationController:didCompleteWithError:")]
		public void DidComplete (ASAuthorizationController controller, NSError error)
		{
			Console.WriteLine (error);
		}

		#endregion

		#region IASAuthorizationControllerPresentation Context Providing

		public UIWindow GetPresentationAnchor (ASAuthorizationController controller) => View.Window;

		#endregion
	}
}

