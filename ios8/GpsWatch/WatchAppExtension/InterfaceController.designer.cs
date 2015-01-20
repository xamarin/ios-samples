// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WatchAppExtension
{
	[Register ("InterfaceController")]
	partial class InterfaceController
	{
		[Outlet]
		WatchKit.WKInterfaceLabel LatitudeValueLbl { get; set; }

		[Outlet]
		WatchKit.WKInterfaceLabel LongitudeValueLbl { get; set; }

		[Outlet]
		WatchKit.WKInterfaceLabel WarningLbl { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (WarningLbl != null) {
				WarningLbl.Dispose ();
				WarningLbl = null;
			}

			if (LatitudeValueLbl != null) {
				LatitudeValueLbl.Dispose ();
				LatitudeValueLbl = null;
			}

			if (LongitudeValueLbl != null) {
				LongitudeValueLbl.Dispose ();
				LongitudeValueLbl = null;
			}
		}
	}
}
