using System;
using CoreGraphics;

using Foundation;
using SpriteKit;

namespace SpriteTour {

	public partial class AnimatedSpritesViewController : CommonSpritesViewController {

		public AnimatedSpritesViewController (IntPtr handle) : base (handle)
		{
		}

		protected override void CreateSceneContents ()
		{
			base.CreateSceneContents ();

			SKTexture[] walkFrames = LoadFrames ("Art/warrior_walk_", 28);

			SKSpriteNode sprite = new SKSpriteNode (walkFrames [0]) {
				Position = new CGPoint (Scene.Frame.GetMidX (), Scene.Frame.GetMidY ())
			};
			Scene.AddChild (sprite);

			//Cycle through the frame
			var animation = SKAction.AnimateWithTextures (walkFrames, 1.0f / 28, true, false);
			sprite.RunAction (SKAction.RepeatActionForever (animation));

			Scene.AddDescription ("This sprite is animating through a series of texture images.",
				new CGPoint (Scene.Frame.GetMidX (), 100));
		}

		SKTexture [] LoadFrames (string baseImageName, int frameCount)
		{
			var frames = new SKTexture [frameCount];
			for (int i = 1; i <= frameCount; i++)
				frames [i - 1] = SKTexture.FromImageNamed (baseImageName + i.ToString ("0000"));
			return frames;
		}
	}
}