using System;
using Foundation;
using UIKit;

namespace SpriteKitPhysicsCollisions {

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate {

		public override UIWindow Window {
			get;
			set;
		}

		static void Main (string [] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}
