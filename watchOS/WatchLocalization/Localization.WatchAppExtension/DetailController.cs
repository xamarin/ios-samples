using Foundation;
using System;
using UIKit;
using WatchKit;

namespace Localization.WatchAppExtension
{
    public partial class DetailController : WKInterfaceController
    {
        public DetailController (IntPtr handle) : base (handle)
        {
        }

		public override void WillActivate()
		{
			base.WillActivate();

			var hour = DateTime.Now.Hour;

			var display = "zzzz";
			if (hour < 6)
			{
				// zzz
			}
			else if (hour < 10)
			{
				display = "Breakfast time";
			}
			else if (hour < 16)
			{
				display = "Lunch time";
			}
			else if (hour < 21)
			{
				display = "Dinner time";
			}
			else if (hour < 23)
			{
				display = "Bed time";
			}
			var localizedDisplay = NSBundle.MainBundle.LocalizedString(display, comment: "greeting");
			DisplayText.SetText(localizedDisplay);

			try
			{
				// "language@2x.png" is located in the Watch Extension
				// multiple times: once for each language .lproj directory
				using (var image = UIImage.FromBundle("language"))
				{
				//	DisplayImage.SetImage(image);
				}
			}
			catch (Exception ex) { 
				DisplayText.SetText("Exception: " + ex.Message);
			}

			// easiest way to format date and/or time
			var localizedDateTime = NSDateFormatter.ToLocalizedString
						(NSDate.Now, NSDateFormatterStyle.Long, NSDateFormatterStyle.Short);

			DisplayTime.SetText(localizedDateTime);


			// long way of date or time formatting
			//			var formatter = new NSDateFormatter ();
			//			formatter.DateStyle = NSDateFormatterStyle.Medium;
			//			formatter.TimeStyle = NSDateFormatterStyle.Long;
			//			formatter.Locale = NSLocale.CurrentLocale;
			//			var localizedDateTime = formatter.StringFor (NSDate.Now);
		}
    }
}