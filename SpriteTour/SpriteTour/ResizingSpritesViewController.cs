using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.SpriteKit;

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
			defaultSprite.Position = new PointF (Scene.Frame.GetMidX () - 192, Scene.Frame.GetMidY ());
			defaultSprite.AddDescription ("Resized with default stretching", new PointF (0, -128));
			Scene.AddChild (defaultSprite);
			defaultSprite.RunAction (resizeSpritesAction);

			SKSpriteNode customSprite = new SKSpriteNode (texture);
			customSprite.Position = new PointF (Scene.Frame.GetMidX () + 192, Scene.Frame.GetMidY ());
			customSprite.CenterRect = new RectangleF (12.0f / 28.0f, 12.0f / 28.0f, 4.0f / 28.0f, 4.0f / 28.0f);
			customSprite.AddDescription ("Resized with custom stretching", new PointF (0, -128));
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