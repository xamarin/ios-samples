// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace HelloWorld_iPhone
{
	[Register ("HelloWorld_iPhoneViewController")]
	partial class HelloWorld_iPhoneViewController
	{
		[Outlet]
		UIKit.UIButton btnClickMe { get; set; }

		[Outlet]
		UIKit.UILabel lblOutput { get; set; }

		[Action ("actnButtonClick:")]
		partial void actnButtonClick (Foundation.NSObject sender);
	}
}
