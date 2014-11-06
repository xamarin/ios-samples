using System;
using CoreGraphics;
using UIKit;
using SpriteKit;

namespace SpriteTour {

	public class CommonSpritesViewController : UIViewController {

		public CommonSpritesViewController (IntPtr handle) : base (handle)
		{
		}

		protected SKScene Scene { get; private set; }

		protected SKView SpriteView {
			get { return View as SKView; }
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			SpriteView.ShowsDrawCount = true;
			SpriteView.ShowsNodeCount = true;
			SpriteView.ShowsFPS = true;
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			CreateSceneContents ();
			SpriteView.PresentScene (Scene);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			Scene.RemoveAllChildren ();
			Scene.RemoveAllActions ();
		}

		protected virtual void CreateSceneContents ()
		{
			Scene = new SKScene (new CGSize (1024, 768)) {
				BackgroundColor = UIColor.Black,
				ScaleMode = SKSceneScaleMode.AspectFit
			};
		}
	}
}