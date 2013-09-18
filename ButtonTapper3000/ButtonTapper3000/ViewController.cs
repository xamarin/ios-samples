using System;
using MonoTouch.SpriteKit;
using MonoTouch.UIKit;

namespace ButtonTapper3000 {

	public class ViewController : UIViewController {

		public override void LoadView ()
		{
			View = new SKView ();
		}

		public override void ViewWillAppear (bool animated)
		{
			(View as SKView).PresentScene (new MainMenu (View.Bounds.Size));
		}

		public override bool ShouldAutorotate ()
		{
			return false;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait;
		}
	}
}