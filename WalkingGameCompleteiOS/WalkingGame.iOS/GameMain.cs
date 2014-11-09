using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;


namespace WalkingGame.iOS
{
	[Register ("AppDelegate")]
	class Program : UIApplicationDelegate 
	{
		private WalkingGame.Game1 game;

		public override void FinishedLaunching (UIApplication app)
		{
			// Fun begins..
			game = new WalkingGame.Game1();
			game.Run();
		}

		static void Main (string [] args)
		{
			UIApplication.Main (args,null,"AppDelegate");
		}
	}
}
