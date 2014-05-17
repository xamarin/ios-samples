using System;
using SpriteKit;
using CoreGraphics;

using UIKit;

namespace SpriteKit
{
	public class MyScene : SKScene
	{
		public MyScene (CGSize size) : base(size)
		{
			BackgroundColor = new UIKit.UIColor (.15f, .15f, .3f, 1);

			AddChild( new SKLabelNode ("Chalkduster") {
				Text = "Hello World",
				FontSize = 30,
				Position = new CGPoint(Frame.GetMidX(),Frame.GetMidY()),
			});

		}
		public override void TouchesBegan (Foundation.NSSet touches, UIKit.UIEvent evt)
		{
			foreach (UITouch touch in touches) {
				var location = touch.LocationInNode (this);
				var sprite = new SKSpriteNode ("Spaceship") {
					Position =  location,
				};

				var action = SKAction.RotateByAngle ((float)Math.PI, 1);
				sprite.RunAction (SKAction.RepeatActionForever (action));

				AddChild (sprite);
			}
		}
		public override void Update (double currentTime)
		{

		}
	}
}

