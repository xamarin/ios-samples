using System;
using UIKit;
using SpriteKit;

namespace SpriteKit
{
	public class ViewController : UIViewController
	{
		public ViewController ()
		{

		}
		SKView skView;
		public override void LoadView ()
		{
			base.LoadView ();
			View = skView = new SKView {
				ShowsFPS = true,
				ShowsNodeCount = true,
				Frame = View.Frame,
			};

			var scene = new MyScene (View.Bounds.Size) {
				ScaleMode = SKSceneScaleMode.AspectFill
			};

			skView.PresentScene (scene);
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone ? UIInterfaceOrientationMask.AllButUpsideDown : UIInterfaceOrientationMask.All;
		}
	}
}

