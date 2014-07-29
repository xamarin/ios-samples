using System;
//using Foundation;
using SpriteKit;
using UIKit;

namespace SpriteKitPhysicsCollisions {

	public partial class SpriteKitPhysicsCollisionsViewController : UIViewController {

		public SpriteKitPhysicsCollisionsViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			skView = View as SKView;
			skView.PresentScene (new SpaceScene (View.Bounds.Size));
			skView.ShowsFPS = true;
			skView.ShowsDrawCount = true;
			skView.ShowsNodeCount = true;
		}
	}
}