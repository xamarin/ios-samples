
namespace Fox2
{
    using CoreGraphics;
    using Foundation;
    using SpriteKit;
    using System;

    /// <summary>
    /// Exposes game controller action button type functionality with screen-rendered buttons.
    /// </summary>
    public class ControlOverlay : SKNode
    {
        private float buttonMargin = 25f;

        public ControlOverlay(CGRect frame) : base()
        {
            this.LeftPad.Position = new CGPoint(20f, 40f);
            this.AddChild(this.LeftPad);

            this.RightPad.Position = new CGPoint(frame.Size.Width - 20f - this.RightPad.Size.Width, 40f);
            this.AddChild(this.RightPad);

            var buttonDistance = this.RightPad.Size.Height / 2f + this.buttonMargin + this.ButtonA.Size.Height / 2f;
            var center = new CGPoint(this.RightPad.Position.X + this.RightPad.Size.Width / 2f, this.RightPad.Position.Y + this.RightPad.Size.Height / 2f);

            var buttonAx = center.X - buttonDistance * Math.Cos(Math.PI / 4f) - (this.ButtonB.Size.Width / 2f);
            var buttonAy = center.Y + buttonDistance * Math.Sin(Math.PI / 4f) - (this.ButtonB.Size.Height / 2f);
            this.ButtonA.Position = new CGPoint(buttonAx, buttonAy);
            this.AddChild(this.ButtonA);

            var buttonBx = center.X - buttonDistance * Math.Cos(Math.PI / 2f) - (this.ButtonB.Size.Width / 2f);
            var buttonBy = center.Y + buttonDistance * Math.Sin(Math.PI / 2f) - (this.ButtonB.Size.Height / 2f);
            this.ButtonB.Position = new CGPoint(buttonBx, buttonBy);
            this.AddChild(this.ButtonB);
        }

        public ControlOverlay() : base() { }

        public ControlOverlay(NSCoder coder) : base()
        {
            throw new NotImplementedException("init(coder:) has not been implemented");
        }

        public PadOverlay LeftPad { get; private set; } = new PadOverlay();

        public PadOverlay RightPad { get; private set; } = new PadOverlay();

        public ButtonOverlay ButtonA { get; private set; } = new ButtonOverlay("A");

        public ButtonOverlay ButtonB { get; private set; } = new ButtonOverlay("B");
    }
}
