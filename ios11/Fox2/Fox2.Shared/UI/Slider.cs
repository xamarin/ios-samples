
namespace Fox2.UI
{
    using CoreGraphics;
    using Foundation;
    using SpriteKit;
    using System;

#if __IOS__
    using UIKit;
    using SKColor = UIKit.UIColor;
#elif __OSX__
    using AppKit;
    using SKColor = AppKit.NSColor;
#else
    using SKColor = UIKit.UIColor;
#endif

    /// <summary>
    /// A basic `SKNode` based slider.
    /// </summary>
    public class Slider : SKNode
    {
        private Action<Slider> actionClicked;

        private SKSpriteNode background;

        private SKLabelNode label;

        private SKShapeNode slider;

        private float value = 0f;

        public Slider(int width, int height, string text) : base()
        {
            // create a label
            var fontName = "Optima-ExtraBlack";
            this.label = SKLabelNode.FromFont(fontName);
            this.label.Text = text;
            this.label.FontSize = 18;
            this.label.FontColor = SKColor.White;
            this.label.Position = new CGPoint(0f, -8f);

            // create background & slider
            this.background = new SKSpriteNode(SKColor.White, new CGSize(width, 2f));
            this.slider = SKShapeNode.FromCircle(height);
            this.slider.FillColor = SKColor.White;
            this.background.AnchorPoint = new CGPoint(0f, 0.5f);

            this.slider.Position = new CGPoint(this.label.Frame.Size.Width / 2f + 15f, 0f);
            this.background.Position = new CGPoint(this.label.Frame.Size.Width / 2f + 15f, 0f);

            // add to the root node
            this.AddChild(this.label);
            this.AddChild(this.background);
            this.AddChild(this.slider);

            // track mouse event
            base.UserInteractionEnabled = true;
            this.Value = 0f;
        }

        public Slider(NSCoder coder)
        {
            throw new NotImplementedException("init(coder:) has not been implemented");
        }

        public nfloat Width => this.background.Frame.Size.Width;

        public nfloat Height => this.slider.Frame.Size.Height;

        public float Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
                this.slider.Position = new CGPoint(this.background.Position.X + this.value * this.Width, 0f);
            }
        }

        private void SetBackgroundColor(SKColor color)
        {
            if (this.background != null)
            {
                background.Color = color;
            }
        }

        public void SetClickedTarget(Action<Slider> action)
        {
            this.actionClicked = action;
        }

#if __OSX__

        public override void MouseDown(NSEvent @event)
        {
            this.MouseDragged(@event);
        }

        public override void MouseUp(NSEvent @event)
        {
            this.SetBackgroundColor(SKColor.White);
        }

        public override void MouseDragged(NSEvent @event)
        {
            this.SetBackgroundColor(SKColor.Gray);

            var posInView = this.Scene.ConvertPointFromNode(this.Position, this.Parent);

            var x = @event.LocationInWindow.X - posInView.X - this.background.Position.X;
            var pos = NMath.Min(NMath.Min(x, this.Width), 0f);
            this.slider.Position = new CGPoint(this.background.Position.X + pos, 0f);
            this.Value = (float)(pos / this.Width);
            this.actionClicked.Invoke(this);
        }

#endif

#if __IOS__

        public override void TouchesMoved(NSSet touches, UIEvent @event)
        {
            this.SetBackgroundColor(UIColor.Gray);

            var first = touches.ToArray<UITouch>()[0];
            var x = first.LocationInNode(this).X - this.background.Position.X;
            var position = Math.Max(NMath.Min(x, this.Width), 0f);

            this.slider.Position = new CGPoint(this.background.Position.X + position, 0f);
            this.Value = (float)(position / this.Width);

            this.actionClicked.Invoke(this);
        }

#endif
    }
}
