using Foundation;
using UIKit;

namespace WalkingGame.iOS {
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		MainGame game;

		public override void FinishedLaunching (UIApplication application)
		{
			game = new MainGame ();
			game.Run ();
		}
	}
}

