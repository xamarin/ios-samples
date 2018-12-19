// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace UICatalog
{
	[Register ("ButtonsViewController")]
	partial class ButtonsViewController
	{
		[Outlet]
		UIKit.UIButton attributedButton { get; set; }

		[Action ("AttributedButtonTouched:")]
		partial void AttributedButtonTouched (Foundation.NSObject sender);

		[Action ("ImageButtonTouched:")]
		partial void ImageButtonTouched (Foundation.NSObject sender);

		[Action ("SystemButtonTouched:")]
		partial void SystemButtonTouched (Foundation.NSObject sender);

		[Action ("SystemContactButtonTouched:")]
		partial void SystemContactButtonTouched (Foundation.NSObject sender);

		[Action ("SystemDetailsButtonTouched:")]
		partial void SystemDetailsButtonTouched (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (attributedButton != null) {
				attributedButton.Dispose ();
				attributedButton = null;
			}
		}
	}
}
