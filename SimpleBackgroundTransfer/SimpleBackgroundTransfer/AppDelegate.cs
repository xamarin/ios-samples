using System;
using Foundation;
using UIKit;

namespace SimpleBackgroundTransfer {

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate {
	
		public NSAction BackgroundSessionCompletionHandler { get; set; }

		public override UIWindow Window { get; set; }

		public override void HandleEventsForBackgroundUrl (UIApplication application, string sessionIdentifier, NSAction completionHandler)
		{
			Console.WriteLine ("HandleEventsForBackgroundUrl");
			BackgroundSessionCompletionHandler = completionHandler;
		}

		public override void OnActivated (UIApplication application)
		{
			Console.WriteLine ("OnActivated");
		}

		public override void OnResignActivation (UIApplication application)
		{
			Console.WriteLine ("OnResignActivation");
		}
	}
}