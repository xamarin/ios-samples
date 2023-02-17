using Foundation;
using System;
using System.Linq;
using UIKit;

namespace WeatherWidget {
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method

			return true;
		}

		public override bool OpenUrl (UIApplication app, NSUrl url, NSDictionary options)
		{
			if (this.Window?.RootViewController is UINavigationController navigationController &&
			   navigationController?.ViewControllers?.FirstOrDefault () is ForecastViewController initialViewController) {
				// Extract the daysFromNow query string parameter from the incoming URL.
				var components = NSUrlComponents.FromUrl (url, false);
				var daysFromNowValue = components?.QueryItems?.FirstOrDefault (item => item.Name == "daysFromNow")?.Value;
				if (string.IsNullOrEmpty (daysFromNowValue)) {
					throw new Exception ("Expected daysFromNow parameter to have a value");
				}

				if (int.TryParse (daysFromNowValue, out int daysFromNow)) {
					// Scroll the initial view controller to the correct index.
					initialViewController.ScrollToForecast (daysFromNow);
					return true;
				} else {
					throw new Exception ("Expected daysFromNow to be an integer");
				}
			} else {
				throw new Exception ("Expected the root view controller to be a UINavigationController");
			}
		}
	}
}
