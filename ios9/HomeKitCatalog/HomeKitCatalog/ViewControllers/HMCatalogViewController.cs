using System;

using CoreFoundation;
using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	/// <summary>
	/// The `HMCatalogViewController` is a base class which mainly provides easy-access methods for shared HomeKit objects.
	///
	/// The base class for most table view controllers in this app. It manages home
	/// delegate registration and facilitates 'popping back' when it's discovered that
	/// a home has been deleted.
	/// </summary>
	[Register ("HMCatalogViewController")]
	public class HMCatalogViewController : UITableViewController, IHMHomeDelegate
	{
		public HomeStore HomeStore {
			get {
				return HomeStore.SharedStore;
			}
		}

		public HMHome Home {
			get {
				return HomeStore.Home;
			}
		}

		[Export ("initWithCoder:")]
		public HMCatalogViewController (NSCoder coder)
			: base (coder)
		{
		}

		public HMCatalogViewController (IntPtr handle)
			: base (handle)
		{
		}

		public HMCatalogViewController ()
		{
		}

		//  Pops the view controller, if required. Invokes the delegate registration method.
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (ShouldPopViewController () && NavigationController != null) {
				NavigationController.PopToRootViewController (true);
				return;
			}

			RegisterAsDelegate ();
		}

		/// <summary>
		/// Evaluates whether or not the view controller should pop to the list of homes.
		/// </summary>
		/// <returns><c>true</c>, if this instance is not the root view controller and the `Home` is null; <c>false</c> otherwise.</returns>
		bool ShouldPopViewController ()
		{
			UIViewController rootViewController = null;
			if (NavigationController != null)
				rootViewController = NavigationController.ViewControllers [0];

			return this != rootViewController && Home == null;
		}

		protected virtual void RegisterAsDelegate ()
		{
			var home = Home;
			if (home != null)
				home.Delegate = this;
		}

		protected void DisplayError (NSError error)
		{
			if (error != null) {
				var errorCode = (HMError)(int)error.Code;
				if (PresentedViewController != null || errorCode == HMError.OperationCancelled || errorCode == HMError.UserDeclinedAddingUser)
					Console.WriteLine (error.LocalizedDescription);
				else
					DisplayErrorMessage (error.LocalizedDescription);
			} else {
				DisplayErrorMessage (error.Description);
			}
		}

		void DisplayErrorMessage (string message)
		{
			DisplayMessage ("Error", message);
		}

		protected void DisplayMessage (string title, string message)
		{
			DispatchQueue.MainQueue.DispatchAsync (() => {
				var alert = Alert.Create (title, message);
				PresentViewController (alert, true, null);
			});
		}
	}
}