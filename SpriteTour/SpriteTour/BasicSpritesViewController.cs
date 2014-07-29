using System;
using CoreGraphics;

using Foundation;
using UIKit;
using SpriteKit;

namespace SpriteTour {

	public partial class BasicSpritesViewController : CommonSpritesViewController {

		public BasicSpritesViewController (IntPtr handle) : base (handle)
		{
		}

		protected override void CreateSceneContents ()
		{
			base.CreateSceneContents ();
			AddTexturedSprite ();
			AddColoredSprite ();
		}

		void AddTexturedSprite ()
		{
			using (SKSpriteNode sprite = new SKSpriteNode ("Art/rocket.png")) {
				sprite.Position = new CGPoint (Scene.Frame.GetMidX () - 200, Scene.Frame.GetMidY ());
				sprite.AddDescription ("Texture Sprite", new CGPoint (0, -sprite.Size.Height / 2 - 30));
				Scene.AddChild (sprite);
			}
		}

		void AddColoredSprite ()
		{
			using (SKSpriteNode sprite = new SKSpriteNode (UIColor.Red, new CGSize (128, 128))) {
				sprite.Position = new CGPoint (Scene.Frame.GetMidX () + 200, Scene.Frame.GetMidY ());
				sprite.AddDescription ("Color Sprite", new CGPoint (0, -sprite.Size.Height / 2 - 30));
				Scene.AddChild (sprite);
			}
		}
	}
}