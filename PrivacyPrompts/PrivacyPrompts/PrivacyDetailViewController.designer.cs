// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace PrivacyPrompts
{
	[Register ("PrivacyDetailViewController")]
	partial class PrivacyDetailViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton checkAccessButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton requestAccessButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel titleLabel { get; set; }

		[Action ("tappedCheckAccessButton:")]
		partial void tappedCheckAccessButton (MonoTouch.Foundation.NSObject sender);

		[Action ("tappedRequestAccessButton:")]
		partial void tappedRequestAccessButton (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (titleLabel != null) {
				titleLabel.Dispose ();
				titleLabel = null;
			}

			if (checkAccessButton != null) {
				checkAccessButton.Dispose ();
				checkAccessButton = null;
			}

			if (requestAccessButton != null) {
				requestAccessButton.Dispose ();
				requestAccessButton = null;
			}
		}
	}
}
