using System;
using CoreGraphics;

using Foundation;
using SpriteKit;
using UIKit;

namespace SpriteTour {

	public partial class AnchorsSpritesViewController : CommonSpritesViewController {

		public AnchorsSpritesViewController (IntPtr handle) : base (handle)
		{
		}

		protected override void CreateSceneContents ()
		{
			base.CreateSceneContents ();
			AddAnchorGrid ();
			AddAnimatedAnchor ();
			Scene.AddDescription ("The dots mark the actual position of each sprite node", 
				new CGPoint (Scene.Frame.GetMidX (), 100));
		}

		void AddAnchorGrid ()
		{
			for (int x = 0; x <= 4; x++) {
				for (int y = 0; y <= 4; y++) {
					SKSpriteNode sprite = new SKSpriteNode ("Art/rocket.png") {
						Scale = 0.25f,
						AnchorPoint = new CGPoint (0.25f * x, 0.25f * y),
						Position = new CGPoint (Scene.Frame.GetMidX () - 400 + 100 * x, 
						                       Scene.Frame.GetMidY () - 200 + 100 * y)
					};
					Scene.AddChild (sprite);
					AddAnchorDotToSprite (sprite);
				}
			}
		}

		void AddAnchorDotToSprite (SKSpriteNode sprite)
		{
			CGPath myPath = new CGPath ();
			myPath.AddArc (0, 0, 10, 0, (float) Math.PI * 2, true);
			myPath.CloseSubpath ();

			SKShapeNode dot = new SKShapeNode () {
				Path = myPath,
				FillColor = UIColor.Green,
				LineWidth = 0.0f
			};
			sprite.AddChild (dot);
		}

		void AddAnimatedAnchor ()
		{
			SKSpriteNode animatedSprite = new SKSpriteNode ("Art/rocket.png") {
				Position = new CGPoint (Scene.Frame.GetMidX () + 200, Scene.Frame.GetMidY ()),
				AnchorPoint = CGPoint.Empty
			};
			Scene.AddChild (animatedSprite);
			AddAnchorDotToSprite (animatedSprite);

			animatedSprite.RunAction (NewAnimateAnchorAction ());
		}

		SKAction NewAnimateAnchorAction ()
		{
			SKAction moveAnchorRight = SKAction.CustomActionWithDuration (1.0f, (node, elapsedTime) => {
				(node as SKSpriteNode).AnchorPoint = new CGPoint (elapsedTime, 0.0f);
			});

			SKAction moveAnchorUp = SKAction.CustomActionWithDuration (1.0f, (node, elapsedTime) => {
				(node as SKSpriteNode).AnchorPoint = new CGPoint (1.0f, elapsedTime);
			});

			SKAction moveAnchorLeft = SKAction.CustomActionWithDuration (1.0f, (node, elapsedTime) => {
				(node as SKSpriteNode).AnchorPoint = new CGPoint(1.0f - elapsedTime, 1.0f);
			});

			SKAction moveAnchorDown = SKAction.CustomActionWithDuration (1.0f, (node, elapsedTime) => {
				(node as SKSpriteNode).AnchorPoint = new CGPoint (0.0f, 1.0f - elapsedTime);
			});

			var sequence = SKAction.Sequence (moveAnchorRight, moveAnchorUp, moveAnchorLeft, moveAnchorDown);
			return SKAction.RepeatActionForever (sequence);
		}
	}
}