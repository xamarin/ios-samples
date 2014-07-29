using System;
using CoreGraphics;

using Foundation;
using UIKit;
using SpriteKit;

namespace SpriteTour {

	public partial class ResizingSpritesViewController : CommonSpritesViewController {

		public ResizingSpritesViewController (IntPtr handle) : base (handle)
		{
		}

		protected override void CreateSceneContents ()
		{
			base.CreateSceneContents ();

			SKTexture texture = SKTexture.FromImageNamed ("Art/stretchable_image.png");
			SKAction resizeSpritesAction = GetResizeAction (texture);

			SKSpriteNode defaultSprite = new SKSpriteNode (texture);
			defaultSprite.Position = new CGPoint (Scene.Frame.GetMidX () - 192, Scene.Frame.GetMidY ());
			defaultSprite.AddDescription ("Resized with default stretching", new CGPoint (0, -128));
			Scene.AddChild (defaultSprite);
			defaultSprite.RunAction (resizeSpritesAction);

			SKSpriteNode customSprite = new SKSpriteNode (texture);
			customSprite.Position = new CGPoint (Scene.Frame.GetMidX () + 192, Scene.Frame.GetMidY ());
			customSprite.CenterRect = new CGRect (12.0f / 28.0f, 12.0f / 28.0f, 4.0f / 28.0f, 4.0f / 28.0f);
			customSprite.AddDescription ("Resized with custom stretching", new CGPoint (0, -128));
			Scene.AddChild (customSprite);
			customSprite.RunAction (resizeSpritesAction);
		}

		SKAction GetResizeAction (SKTexture texture)
		{
			SKAction sequence = SKAction.Sequence (
				SKAction.WaitForDuration (1.0f),
				SKAction.ResizeTo (192.0f, 192.0f, 1.0f),
				SKAction.WaitForDuration (1.0f),
				SKAction.ResizeTo (128.0f, 192.0f, 1.0f),
				SKAction.WaitForDuration (1.0f),
				SKAction.ResizeTo (256.0f, 128.0f, 1.0f),
				SKAction.WaitForDuration (1.0f),
				SKAction.ResizeTo (texture.Size, 1.0f)
			);
			return SKAction.RepeatActionForever (sequence);
		}
	}
}