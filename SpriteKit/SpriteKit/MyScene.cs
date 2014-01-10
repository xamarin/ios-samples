using System;
using MonoTouch.SpriteKit;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;

namespace SpriteKit
{
	public class MyScene : SKScene
	{
		public MyScene (SizeF size) : base(size)
		{
			BackgroundColor = new MonoTouch.UIKit.UIColor (.15f, .15f, .3f, 1);

			AddChild( new SKLabelNode ("Chalkduster") {
				Text = "Hello World",
				FontSize = 30,
				Position = new PointF(Frame.GetMidX(),Frame.GetMidY()),
			});

		}
		public override void TouchesBegan (MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent evt)
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

