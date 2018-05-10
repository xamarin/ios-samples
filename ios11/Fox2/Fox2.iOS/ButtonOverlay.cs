
namespace Fox2
{
    using CoreGraphics;
    using Foundation;
    using Fox2.Interfaces;
    using SpriteKit;
    using System;
    using System.Linq;
    using UIKit;

    /// <summary>
    /// Support class for action buttons.
    /// </summary>
    public class ButtonOverlay : SKNode
    {
        // Default 25, 25
        private CGSize size = CGSize.Empty;

        private UITouch trackingTouch;
        private SKShapeNode background;
        private SKShapeNode inner;
        private SKLabelNode label;

        public ButtonOverlay(string text) : base()
        {
            this.Size = new CGSize(40f, 40f);
            this.Alpha = 0.7f;
            base.UserInteractionEnabled = true;

            this.BuildPad(text);
        }

        public ButtonOverlay(NSCoder coder)
        {
            throw new NotImplementedException("init(coder:) has not been implemented");
        }

        public IButtonOverlayDelegate Delegate { get; set; }

        public CGSize Size
        {
            get
            {
                return this.size;
            }
            set
            {
                if (this.size != value)
                {
                    this.size = value;
                    this.UpdateForSizeChange();
                }
            }
        }

        private void BuildPad(string text)
        {
            var backgroundRect = new CGRect(0f, 0f, this.Size.Width, this.Size.Height);
            this.background = new SKShapeNode
            {
                Path = CGPath.EllipseFromRect(backgroundRect),
                StrokeColor = UIColor.Black,
                LineWidth = 3f
            };

            this.AddChild(this.background);

            this.inner = new SKShapeNode
            {
                LineWidth = 1f,
                FillColor = UIColor.White,
                StrokeColor = UIColor.Gray,
                Path = CGPath.EllipseFromRect(new CGRect(0f, 0f, this.Size.Width, this.Size.Height)),
            };

            this.AddChild(this.inner);

            this.label = new SKLabelNode
            {
                Text = text,
                FontSize = 24f,
                FontColor = UIColor.Black,
                FontName = UIFont.BoldSystemFontOfSize(24f).Name,
                VerticalAlignmentMode = SKLabelVerticalAlignmentMode.Center,
                HorizontalAlignmentMode = SKLabelHorizontalAlignmentMode.Center,
                Position = new CGPoint(this.Size.Width / 2f, this.Size.Height / 2f + 1f),
            };

            this.AddChild(this.label);
        }

        private void UpdateForSizeChange()
        {
            if (this.background != null)
            {
                var backgroundRect = new CGRect(0f, 0f, this.Size.Width, this.Size.Height);
                this.background.Path = CGPath.EllipseFromRect(backgroundRect);

                var innerRect = new CGRect(0f, 0f, this.Size.Width / 3f, this.Size.Height / 3f);
                this.inner.Path = CGPath.EllipseFromRect(innerRect);

                this.label.Position = new CGPoint(this.Size.Width / 2f - this.label.Frame.Size.Width / 2f,
                                                  this.Size.Height / 2f - this.label.Frame.Size.Height / 2f);
            }
        }

        private void ResetInteraction()
        {
            this.trackingTouch = null;
            this.inner.FillColor = UIColor.White;
            this.Delegate.DidPress(this);
        }

        public override void TouchesBegan(NSSet touches, UIEvent @event)
        {
            this.trackingTouch = touches.ToArray<UITouch>().FirstOrDefault();
            this.inner.FillColor = UIColor.Black;
            this.Delegate.WillPress(this);
        }

        public override void TouchesEnded(NSSet touches, UIEvent @event)
        {
            if (touches.Contains(this.trackingTouch))
            {
                this.ResetInteraction();
            }
        }

        public override void TouchesCancelled(NSSet touches, UIEvent @event)
        {
            if (touches.Contains(this.trackingTouch))
            {
                this.ResetInteraction();
            }
        }
    }
}