// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Fit
{
	[Register ("ProfileViewController")]
	partial class ProfileViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel ageHeightValueLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel ageUnitLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel heightUnitLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel heightValueLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel weightUnitLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel weightValueLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ageUnitLabel != null) {
				ageUnitLabel.Dispose ();
				ageUnitLabel = null;
			}

			if (ageHeightValueLabel != null) {
				ageHeightValueLabel.Dispose ();
				ageHeightValueLabel = null;
			}

			if (heightUnitLabel != null) {
				heightUnitLabel.Dispose ();
				heightUnitLabel = null;
			}

			if (heightValueLabel != null) {
				heightValueLabel.Dispose ();
				heightValueLabel = null;
			}

			if (weightUnitLabel != null) {
				weightUnitLabel.Dispose ();
				weightUnitLabel = null;
			}

			if (weightValueLabel != null) {
				weightValueLabel.Dispose ();
				weightValueLabel = null;
			}
		}
	}
}
