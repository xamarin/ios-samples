using System;

using Foundation;
using UIKit;

namespace UICatalog {
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		public override UIWindow Window { get; set; }

		public override bool OpenUrl (UIApplication app, NSUrl url, NSDictionary options)
		{
			Console.WriteLine ("Application launched with URL: {0}", url.AbsoluteString);
			return true;
		}
	}
}


