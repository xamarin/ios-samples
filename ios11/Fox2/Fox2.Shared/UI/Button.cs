
namespace Fox2.UI
{
    using CoreGraphics;
    using Foundation;
    using SpriteKit;
    using System;

#if !__OSX__
    using UIKit;
    using SKColor = UIKit.UIColor;
#else
    using AppKit;
    using SKColor = AppKit.NSColor;
#endif

    /// <summary>
    /// A basic `SKNode` based button.
    /// </summary>
    public class Button : SKNode
    {
        private CGSize size = CGSize.Empty;

        private Action<Button> actionClicked;

        private SKSpriteNode background;

        private SKLabelNode label;

        public Button(string text) : base()
        {
            // create a label
            var fontName = "Optima-ExtraBlack";
            this.label = SKLabelNode.FromFont(fontName);
            this.label.Text = text;
            this.label.FontSize = 18;
            this.label.FontColor = SKColor.White;
            this.label.Position = new CGPoint(0f, -8f);

            // create the background
            this.size = new CGSize(this.label.Frame.Size.Width + 10f, 30f);
            this.background = new SKSpriteNode(SKColor.FromCIColor(new CoreImage.CIColor(0f, 0f, 0f, 0.75f)), size);

            // add to the root node
            this.AddChild(this.background);
            this.AddChild(this.label);

            // Track mouse event
            base.UserInteractionEnabled = true;
        }

        public Button(SKNode node) : base()
        {
            // Track mouse event
            base.UserInteractionEnabled = true;
            this.size = node.Frame.Size;
            this.AddChild(node);
        }

        public Button(NSCoder coder)
        {
            throw new NotImplementedException("init(coder:) has not been implemented");
        }

        public nfloat Height => this.size.Height;

        public nfloat Width => this.size.Width;

        private void SetText(string text)
        {
            this.label.Text = text;
        }

        private void SetBackgroundColor(SKColor color)
        {
            if (this.background != null)
            {
                this.background.Color = color;
            }
        }

        public void SetClickedTarget(Action<Button> action)
        {
            this.actionClicked = action;
        }

#if __OSX__

        public override void MouseDown(NSEvent @event)
        {
            this.SetBackgroundColor(NSColor.FromRgba(0f, 0f, 0f, 1.0f));
        }

        public override void MouseUp(NSEvent @event)
        {
            this.SetBackgroundColor(NSColor.FromRgba(0f, 0f, 0f, 0.75f));

            var x = this.Position.X + ((this.Parent?.Position.X) ?? 0f);
            var y = this.Position.Y + ((this.Parent?.Position.Y) ?? 0f);
            var p = @event.LocationInWindow;

            if (Math.Abs(p.X - x) < this.Width / 2 * this.XScale && Math.Abs(p.Y - y) < this.Height / 2 * this.YScale)
            {
                this.actionClicked.Invoke(this);
            }
        }

#endif

#if __IOS__

        public override void TouchesEnded(NSSet touches, UIEvent @event)
        {
            this.actionClicked.Invoke(this);
        }

#endif
    }
}
