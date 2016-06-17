// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MySingleView
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIImageView BackgroundView { get; set; }

		[Outlet]
		UIKit.UILabel CityTitle { get; set; }

		[Outlet]
		UIKit.UIImageView CityView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BackgroundView != null) {
				BackgroundView.Dispose ();
				BackgroundView = null;
			}

			if (CityView != null) {
				CityView.Dispose ();
				CityView = null;
			}

			if (CityTitle != null) {
				CityTitle.Dispose ();
				CityTitle = null;
			}
		}
	}
}
