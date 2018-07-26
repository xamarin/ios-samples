// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace iOSTenThree
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIButton AlternateIconButton { get; set; }

		[Outlet]
		UIKit.UIButton PrimaryIconButton { get; set; }

		[Outlet]
		UIKit.UIScrollView ScrollView { get; set; }

		[Outlet]
		UIKit.UIImageView XamarinImage { get; set; }

		[Action ("ToggleIndexDisplayMode:")]
		partial void ToggleIndexDisplayMode (Foundation.NSObject sender);

		[Action ("UseAlternateIcon:")]
		partial void UseAlternateIcon (Foundation.NSObject sender);

		[Action ("UsePrimaryIcon:")]
		partial void UsePrimaryIcon (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (ScrollView != null) {
				ScrollView.Dispose ();
				ScrollView = null;
			}

			if (XamarinImage != null) {
				XamarinImage.Dispose ();
				XamarinImage = null;
			}

			if (PrimaryIconButton != null) {
				PrimaryIconButton.Dispose ();
				PrimaryIconButton = null;
			}

			if (AlternateIconButton != null) {
				AlternateIconButton.Dispose ();
				AlternateIconButton = null;
			}
		}
	}
}
