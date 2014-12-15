// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace CloudKitAtlas
{
	[Register ("CKRecordViewController")]
	partial class CKRecordViewController
	{
		[Outlet]
		MapKit.MKMapView map { get; set; }

		[Outlet]
		UIKit.UITextField nameTextField { get; set; }

		[Action ("SaveRecord:")]
		partial void SaveRecord (UIKit.UIButton sender);

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
