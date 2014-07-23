// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace PrivacyPrompts
{
	[Register ("PrivacyDetailViewController")]
	partial class PrivacyDetailViewController
	{
		[Outlet]
		UIKit.UIButton checkAccessButton { get; set; }

		[Outlet]
		UIKit.UIButton requestAccessButton { get; set; }

		[Outlet]
		UIKit.UILabel titleLabel { get; set; }

		[Action ("tappedCheckAccessButton:")]
		partial void tappedCheckAccessButton (Foundation.NSObject sender);

		[Action ("tappedRequestAccessButton:")]
		partial void tappedRequestAccessButton (Foundation.NSObject sender);
		
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
