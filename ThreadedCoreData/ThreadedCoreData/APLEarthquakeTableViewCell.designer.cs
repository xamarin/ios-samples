// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace ThreadedCoreData
{
	partial class APLEarthquakeTableViewCell
	{
		[Outlet]
		UIKit.UILabel dateLabel { get; set; }

		[Outlet]
		UIKit.UILabel locationLabel { get; set; }

		[Outlet]
		UIKit.UIImageView magnitudeImage { get; set; }

		[Outlet]
		UIKit.UILabel magnitudeLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (locationLabel != null) {
				locationLabel.Dispose ();
				locationLabel = null;
			}

			if (dateLabel != null) {
				dateLabel.Dispose ();
				dateLabel = null;
			}

			if (magnitudeLabel != null) {
				magnitudeLabel.Dispose ();
				magnitudeLabel = null;
			}

			if (magnitudeImage != null) {
				magnitudeImage.Dispose ();
				magnitudeImage = null;
			}
		}
	}
}
