
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
    /// Exposes D-Pad game controller type functionality with screen-rendered buttons.
    /// </summary>
    public class PadOverlay : SKNode
    {
        // Range [-1, 1]
        private CGPoint stickPosition = CGPoint.Empty;

        private CGSize size = CGSize.Empty;

        private CGPoint startLocation = CGPoint.Empty;
        private SKShapeNode background;
        private UITouch trackingTouch;
        private SKShapeNode stick;

        public PadOverlay() : base()
        {
            this.Alpha = 0.7f;
            this.Size = new CGSize(150f, 150f);
            base.UserInteractionEnabled = true;

            this.BuildPad();
        }

        public PadOverlay(NSCoder coder)
        {
            throw new NotImplementedException("init(coder:) has not been implemented");
        }

        public IPadOverlayDelegate Delegate { get; set; }

        public CGSize StickSize => new CGSize(this.Size.Width / 3f, this.Size.Height / 3f);

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

        public CGPoint StickPosition
        {
            get
            {
                return this.stickPosition;
            }

            set
            {
                if (this.stickPosition != value)
                {
                    this.stickPosition = value;
                    this.UpdateStickPosition();
                }
            }
        }

        private void BuildPad()
        {
            var backgroundRect = new CGRect(0f, 0f, this.Size.Width, this.Size.Height);
            this.background = new SKShapeNode();
            this.background.Path = CGPath.EllipseFromRect(backgroundRect);
            this.background.StrokeColor = UIColor.Black;
            this.background.LineWidth = 3f;
            this.AddChild(this.background);

            var stickRect = CGRect.Empty;
            stickRect.Size = this.StickSize;
            this.stick = new SKShapeNode();
            this.stick.Path = CGPath.EllipseFromRect(stickRect);
            this.stick.LineWidth = 2f;
            this.stick.FillColor = UIColor.White;
            this.stick.StrokeColor = UIColor.Black;
            this.AddChild(this.stick);

            this.UpdateStickPosition();
        }

        private void UpdateForSizeChange()
        {
            if (this.background != null)
            {
                var backgroundRect = new CGRect(0f, 0f, this.Size.Width, this.Size.Height);
                this.background.Path = CGPath.EllipseFromRect(backgroundRect);

                var stickRect = new CGRect(0f, 0f, this.Size.Width / 3f, this.Size.Height / 3f);
                this.stick.Path = CGPath.EllipseFromRect(stickRect);
            }
        }

        private void UpdateStickPosition()
        {
            var tempStickSize = this.StickSize;
            var stickX = this.Size.Width / 2f - tempStickSize.Width / 2f + this.Size.Width / 2f * this.StickPosition.X;
            var stickY = this.Size.Height / 2f - tempStickSize.Height / 2f + this.Size.Width / 2f * this.StickPosition.Y;

            this.stick.Position = new CGPoint(stickX, stickY);
        }

        private void UpdateStickPosition(CGPoint location)
        {
            var l_vec = new OpenTK.Vector2((float)(location.X - this.startLocation.X), (float)(location.Y - this.startLocation.Y));
            l_vec.X = (l_vec.X / (float)this.Size.Width - 0.5f) * 2f;
            l_vec.Y = (l_vec.Y / (float)this.Size.Height - 0.5f) * 2f;

            if (l_vec.LengthSquared > 1)
            {
                l_vec = OpenTK.Vector2.Normalize(l_vec);
            }

            this.StickPosition = new CGPoint(l_vec.X, l_vec.Y);
        }

        private void ResetInteraction()
        {
            this.StickPosition = CGPoint.Empty;
            this.trackingTouch = null;
            this.startLocation = CGPoint.Empty;
            this.Delegate.PadOverlayVirtualStickInteractionDidEnd(this);
        }

        public override void TouchesBegan(NSSet touches, UIEvent @event)
        {
            this.trackingTouch = touches.ToArray<UITouch>().FirstOrDefault();
            this.startLocation = this.trackingTouch.LocationInNode(this);
            // Center start location
            this.startLocation.X -= this.Size.Width / 2f;
            this.startLocation.Y -= this.Size.Height / 2f;

            this.UpdateStickPosition(this.trackingTouch.LocationInNode(this));
            this.Delegate.PadOverlayVirtualStickInteractionDidStart(this);
        }

        public override void TouchesMoved(NSSet touches, UIEvent @event)
        {
            if (touches.Contains(this.trackingTouch))
            {
                this.UpdateStickPosition(this.trackingTouch.LocationInNode(this));
                this.Delegate.PadOverlayVirtualStickInteractionDidChange(this);
            }
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