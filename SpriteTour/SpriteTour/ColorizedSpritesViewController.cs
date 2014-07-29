using System;
using CoreGraphics;

using Foundation;
using SpriteKit;
using UIKit;

namespace SpriteTour {

	public partial class ColorizedSpritesViewController : CommonSpritesViewController {

		SKSpriteNode template;

		public ColorizedSpritesViewController (IntPtr handle) : base (handle)
		{
		}

		protected override void CreateSceneContents ()
		{
			base.CreateSceneContents ();

			template = new SKSpriteNode ("Art/rocket.png") {
				Scale = 0.5f
			};

			AddBlendFactorLabels ();

			AddColorRow (UIColor.Red, 0);
			AddColorRow (UIColor.Green, 1);
			AddColorRow (UIColor.Blue, 2);
			AddColorRow (UIColor.Yellow, 3);

			AddAnimatedSprite ();
		}

		void AddBlendFactorLabels ()
		{
			var label = Scene.AddDescription ("Color blend factor:", new CGPoint (80, Scene.Frame.GetMaxY () - 70));
			label.HorizontalAlignmentMode = SKLabelHorizontalAlignmentMode.Left;

			for (int i = 0; i <= 10; i++) {
				string text = (i / 10.0f).ToString ("0.00");
				var pos = new CGPoint (85 + i * template.Size.Width + 10, Scene.Frame.GetMaxY () - 90);
				var numberLabel = Scene.AddDescription (text, pos);
				numberLabel.HorizontalAlignmentMode = SKLabelHorizontalAlignmentMode.Left;
			}
		}

		void AddColorRow (UIColor color, int row)
		{
			for (int i = 0; i <= 10; i++) {
				SKSpriteNode s = (SKSpriteNode) (template as NSObject).Copy ();
				s.Color = color;
				s.ColorBlendFactor = 0.1f * i;
				s.Position = new CGPoint (100 + i * s.Size.Width + 10, 100 + row * s.Size.Height + 10);
				Scene.AddChild (s);
			}

			SKSpriteNode colorSwash = new SKSpriteNode (color, new CGSize (64, 64));
			colorSwash.Position = new CGPoint (100 + 12 * template.Size.Width + 10, 100 + row * template.Size.Height + 10);
			Scene.AddChild (colorSwash);
		}

		void AddAnimatedSprite ()
		{
			SKSpriteNode animatedSprite = (SKSpriteNode) (template as NSObject).Copy ();
			animatedSprite.Position = new CGPoint (925, 384);

			Scene.AddChild (animatedSprite);
			Scene.AddDescription ("Animated Color Blending", 
			                      new CGPoint (animatedSprite.Position.X, animatedSprite.Position.Y - 90));

			animatedSprite.RunAction (SKAction.RepeatActionForever (SKAction.Sequence (
				SKAction.WaitForDuration (1.0),
				SKAction.ColorizeWithColor (UIColor.Red, 1.0f, 1.0f),
				SKAction.WaitForDuration (1.0),
				SKAction.ColorizeWithColor (UIColor.Green, 1.0f, 1.0f),
				SKAction.WaitForDuration (1.0),
				SKAction.ColorizeWithColor (UIColor.Blue, 1.0f, 1.0f),
				SKAction.WaitForDuration (1.0),
				SKAction.ColorizeWithColor (UIColor.Yellow, 1.0f, 1.0f),
				SKAction.WaitForDuration (1.0),
				SKAction.ColorizeWithColorBlendFactor (0.0f, 1.0f)
			)));
		}
	}
}