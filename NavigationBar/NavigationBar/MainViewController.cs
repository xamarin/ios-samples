using Foundation;
using System;
using UIKit;

namespace NavigationBar {
	/// <summary>
	/// The application's main (initial) view controller.
	/// </summary>
	public partial class MainViewController : UITableViewController, IUIActionSheetDelegate {
		public MainViewController (IntPtr handle) : base (handle) { }

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait;
		}

		/// <summary>
		/// Action for the 'Style' bar button item.
		/// </summary>
		partial void StyleAction (NSObject sender)
		{
			var title = NSBundle.MainBundle.GetLocalizedString ("Choose a UIBarStyle:");
			var cancelButtonTitle = NSBundle.MainBundle.GetLocalizedString ("Cancel");
			var defaultButtonTitle = NSBundle.MainBundle.GetLocalizedString ("Default");
			var blackOpaqueTitle = NSBundle.MainBundle.GetLocalizedString ("Black Opaque");
			var blackTranslucentTitle = NSBundle.MainBundle.GetLocalizedString ("Black Translucent");

			var alertController = UIAlertController.Create (title, null, UIAlertControllerStyle.ActionSheet);

			alertController.AddAction (UIAlertAction.Create (cancelButtonTitle, UIAlertActionStyle.Cancel, null));
			alertController.AddAction (UIAlertAction.Create (defaultButtonTitle, UIAlertActionStyle.Default, _ => {
				base.NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
				// Bars are translucent by default.
				base.NavigationController.NavigationBar.Translucent = true;
				// Reset the bar's tint color to the system default.
				base.NavigationController.NavigationBar.TintColor = null;
			}));

			alertController.AddAction (UIAlertAction.Create (blackOpaqueTitle, UIAlertActionStyle.Default, (_) => {
				// Change to black-opaque.
				base.NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
				base.NavigationController.NavigationBar.Translucent = false;
				base.NavigationController.NavigationBar.TintColor = UIColor.FromRGBA (1f, 0.99997437f, 0.9999912977f, 1f);
			}));
			alertController.AddAction (UIAlertAction.Create (blackTranslucentTitle, UIAlertActionStyle.Default, (_) => {
				// Change to black-translucent.
				base.NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
				base.NavigationController.NavigationBar.Translucent = true;
				base.NavigationController.NavigationBar.TintColor = UIColor.FromRGBA (1f, 0.99997437f, 0.9999912977f, 1f);
			}));

			base.PresentViewController (alertController, true, null);
		}

		/// <summary>
		/// Unwind action that is targeted by the demos which present a modal view controller, to return to the main screen.
		/// </summary>
		[Action ("unwindToMainViewController:")]
		void UnwindToMainViewController (UIStoryboardSegue sender) { }
	}
}
