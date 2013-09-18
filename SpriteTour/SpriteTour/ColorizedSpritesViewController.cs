using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.SpriteKit;
using MonoTouch.UIKit;

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
			var label = Scene.AddDescription ("Color blend factor:", new PointF (80, Scene.Frame.GetMaxY () - 70));
			label.HorizontalAlignmentMode = SKLabelHorizontalAlignmentMode.Left;

			for (int i = 0; i <= 10; i++) {
				string text = (i / 10.0f).ToString ("0.00");
				var pos = new PointF (85 + i * template.Size.Width + 10, Scene.Frame.GetMaxY () - 90);
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
				s.Position = new PointF (100 + i * s.Size.Width + 10, 100 + row * s.Size.Height + 10);
				Scene.AddChild (s);
			}

			SKSpriteNode colorSwash = new SKSpriteNode (color, new SizeF (64, 64));
			colorSwash.Position = new PointF (100 + 12 * template.Size.Width + 10, 100 + row * template.Size.Height + 10);
			Scene.AddChild (colorSwash);
		}

		void AddAnimatedSprite ()
		{
			SKSpriteNode animatedSprite = (SKSpriteNode) (template as NSObject).Copy ();
			animatedSprite.Position = new PointF (925, 384);

			Scene.AddChild (animatedSprite);
			Scene.AddDescription ("Animated Color Blending", 
			                      new PointF (animatedSprite.Position.X, animatedSprite.Position.Y - 90));

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