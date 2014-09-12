// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace CloudKitAtlas
{
	[Register ("CKRecordViewController")]
	partial class CKRecordViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MapKit.MKMapView map { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField nameTextField { get; set; }

		[Action ("SaveRecord:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void SaveRecord (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (map != null) {
				map.Dispose ();
				map = null;
			}
			if (nameTextField != null) {
				nameTextField.Dispose ();
				nameTextField = null;
			}
		}
	}
}
