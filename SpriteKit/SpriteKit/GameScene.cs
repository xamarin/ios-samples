using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace SpriteKit
{
    public class GameScene : SKScene
    {
        protected GameScene(IntPtr handle) : base(handle) { }

        public GameScene(CGSize size) : base(size) { }

        public override void DidMoveToView(SKView view)
        {
            // Setup your scene here
            BackgroundColor = new UIKit.UIColor (.15f, .15f, .3f, 1);
            var myLabel = new SKLabelNode("Chalkduster")
            {
                FontSize = 30,
                Text = "Hello, World!",
                Position = new CGPoint(Frame.GetMidX(),Frame.GetMidY())
            };

            AddChild(myLabel);
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            // Called when a touch begins
            foreach (UITouch touch in touches)
            {
                var location = touch.LocationInNode(this);
                var sprite = new SKSpriteNode("Spaceship")
                {
                    Position = location,
                    XScale = 0.5f,
                    YScale = 0.5f
                };

                var action = SKAction.RotateByAngle(NMath.PI, 1.0);
                sprite.RunAction(SKAction.RepeatActionForever(action));

                AddChild(sprite);
            }
        }

        public override void Update(double currentTime)
        {
            // Called before each frame is rendered
        }
    }
}