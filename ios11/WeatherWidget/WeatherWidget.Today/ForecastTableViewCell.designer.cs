// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WeatherWidget
{
	[Register ("ForecastTableViewCell")]
	partial class ForecastTableViewCell
	{
		[Outlet]
		UIKit.UILabel dateLabel { get; set; }

		[Outlet]
		UIKit.UIImageView forecastImageView { get; set; }

		[Outlet]
		UIKit.UILabel forecastLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (dateLabel != null) {
				dateLabel.Dispose ();
				dateLabel = null;
			}

			if (forecastLabel != null) {
				forecastLabel.Dispose ();
				forecastLabel = null;
			}

			if (forecastImageView != null) {
				forecastImageView.Dispose ();
				forecastImageView = null;
			}
		}
	}
}
