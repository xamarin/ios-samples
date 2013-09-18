using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace PrintBanner {

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate {

		public override UIWindow Window { get; set; }

		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}