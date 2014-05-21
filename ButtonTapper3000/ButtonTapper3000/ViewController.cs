using System;
using SpriteKit;
using UIKit;

namespace ButtonTapper3000 {

	public class ViewController : UIViewController {

		public override void LoadView ()
		{
			View = new SKView ();

			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) { 
				this.EdgesForExtendedLayout = UIRectEdge.None;
			}
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

		public override UIStatusBarStyle PreferredStatusBarStyle ()
		{
			return UIStatusBarStyle.LightContent;
		}
	}
}