// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace HttpClient
{
	[Register ("AppDelegate")]
	partial class AppDelegate
	{
		[Outlet]
		MonoTouch.UIKit.UIButton button1 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton cancelButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UINavigationController navigationController { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView stack { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIWindow window { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (window != null) {
				window.Dispose ();
				window = null;
			}

			if (button1 != null) {
				button1.Dispose ();
				button1 = null;
			}

			if (cancelButton != null) {
				cancelButton.Dispose ();
				cancelButton = null;
			}

			if (stack != null) {
				stack.Dispose ();
				stack = null;
			}

			if (navigationController != null) {
				navigationController.Dispose ();
				navigationController = null;
			}
		}
	}
}
