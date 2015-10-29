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
		UIKit.UIImageView HotelImage { get; set; }

		[Action ("ShowFirstHotel:")]
		partial void ShowFirstHotel (Foundation.NSObject sender);

		[Action ("ShowSecondHotel:")]
		partial void ShowSecondHotel (Foundation.NSObject sender);

		[Action ("ShowThirdHotel:")]
		partial void ShowThirdHotel (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (HotelImage != null) {
				HotelImage.Dispose ();
				HotelImage = null;
			}
		}
	}
}
