// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using UIKit;
using System;
using System.CodeDom.Compiler;

namespace HKWork
{
	[Register ("HKPermissions2ViewController")]
	partial class HKWorkViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField heartRate { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel PermissionsLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton StoreData { get; set; }

		[Action ("StoreData_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void StoreData_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (heartRate != null) {
				heartRate.Dispose ();
				heartRate = null;
			}
			if (PermissionsLabel != null) {
				PermissionsLabel.Dispose ();
				PermissionsLabel = null;
			}
			if (StoreData != null) {
				StoreData.Dispose ();
				StoreData = null;
			}
		}
	}
}
