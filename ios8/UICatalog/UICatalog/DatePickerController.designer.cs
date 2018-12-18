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
	[Register ("DatePickerController")]
	partial class DatePickerController
	{
		[Outlet]
		UIKit.UILabel dateLabel { get; set; }

		[Outlet]
		UIKit.UIDatePicker datePicker { get; set; }

		[Action ("PickerValurChanged:")]
		partial void PickerValurChanged (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (dateLabel != null) {
				dateLabel.Dispose ();
				dateLabel = null;
			}

			if (datePicker != null) {
				datePicker.Dispose ();
				datePicker = null;
			}
		}
	}
}
