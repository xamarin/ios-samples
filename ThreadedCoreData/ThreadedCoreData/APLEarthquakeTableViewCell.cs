using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ThreadedCoreData
{
	[Register ("APLEarthquakeTableViewCell")]
	public partial class APLEarthquakeTableViewCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("EarthquakeCellID");
		public static readonly UINib Nib;
		NSDateFormatter dateFormatter;

		NSDateFormatter DateFormatter {
			get {
				if (dateFormatter != null) {
					return dateFormatter;
				} else {
					dateFormatter = new NSDateFormatter () {
						TimeZone = NSTimeZone.LocalTimeZone,
						DateFormat = "MMM dd, yyyy, hh:mm:ss a"
					};
				}

				return dateFormatter;
			}
		}

		static APLEarthquakeTableViewCell ()
		{
			Nib = UINib.FromName ("EarthquakeCellID", NSBundle.MainBundle);
		}

		public APLEarthquakeTableViewCell (IntPtr handle) : base (handle)
		{
		}

		public static APLEarthquakeTableViewCell Create ()
		{
			return (APLEarthquakeTableViewCell)Nib.Instantiate (null, null) [0];
		}

		public void ConfigureWithEarthquake(ManagedEarthquake earthquake)
		{
			locationLabel.Text = earthquake.Location;
			dateLabel.Text = DateFormatter.StringFor (earthquake.Date);
			magnitudeLabel.Text = string.Format("{0:#.#}", earthquake.Magnitude);
			magnitudeImage.Image = ImageForMagnitude (earthquake.Magnitude.FloatValue);
		}

		UIImage ImageForMagnitude (float magnitude)
		{
			if (magnitude >= 5.0f) {
				return UIImage.FromFile ("Images/5.0.png");
			}
			if (magnitude >= 4.0f) {
				return UIImage.FromFile ("Images/4.0.png");
			}
			if (magnitude >= 3.0f) {
				return UIImage.FromFile ("Images/3.0.png");
			}
			if (magnitude >= 2.0f) {
				return UIImage.FromFile ("Images/2.0.png");
			}
			return null;
		}
	}
}

