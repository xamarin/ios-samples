
namespace XamarinShot.Models
{
    using CoreGraphics;
    using Foundation;
    using SceneKit;
    using System;
    using UIKit;

    public enum Corner 
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
    }

    public enum Alignment 
    {
        Horizontal,
        Vertical
    }

    public static class CornerExtensions
    {
        public static float U(this Corner corner)
        {
            switch (corner) 
            {
                case Corner.TopLeft:     return -1;
                case Corner.TopRight:    return 1;
                case Corner.BottomLeft:  return -1;
                case Corner.BottomRight: return 1;
            }

            throw new NotImplementedException();
        }

        public static float V(this Corner corner)
        {
            switch (corner)
            {
                case Corner.TopLeft: return -1;
                case Corner.TopRight: return -1;
                case Corner.BottomLeft: return 1;
                case Corner.BottomRight: return 1;
            }

            throw new NotImplementedException();
        }
    }

    public static class AlignmentExtensions
    {
        public static float xOffset(this Alignment alignment, CGSize size)
        {
            switch (alignment) 
            {
                case Alignment.Horizontal:
                    return (float)(size.Width / 2f - BorderSegment.Thickness) / 2f;
                case Alignment.Vertical:
                    return (float)(size.Width / 2f);
            }

            throw new NotImplementedException();
        }

        public static float yOffset(this Alignment alignment, CGSize size)
        {
            switch (alignment)
            {
                case Alignment.Horizontal:
                    return (float)(size.Height / 2f - BorderSegment.Thickness / 2f);
                case Alignment.Vertical:
                    return (float)(size.Height / 2f) / 2f;
            }

            throw new NotImplementedException();
        }
    }

    public class BorderSegment : SCNNode
    {
        /// Thickness of the border lines.
        public const float Thickness = 0.012f;

        /// The scale of segment's length when in the open state
        private const float OpenScale = 0.4f;

        private readonly Corner corner;
        private readonly Alignment alignment;
        private readonly SCNPlane plane;

        private CGSize borderSize;

        public BorderSegment(Corner corner, Alignment alignment, CGSize borderSize) : base()
        {
            this.corner = corner;
            this.alignment = alignment;

            this.plane = SCNPlane.Create(BorderSegment.Thickness, BorderSegment.Thickness);
            this.borderSize = borderSize;

            var material = this.plane.FirstMaterial;
            material.Diffuse.Contents = GameBoard.BorderColor;
            material.Emission.Contents = GameBoard.BorderColor;
            material.DoubleSided = true;
            material.Ambient.Contents = UIColor.Black;
            material.LightingModelName = SCNLightingModel.Constant;

            this.Geometry = this.plane;
            this.Opacity = 0.8f;
        }

        public BorderSegment(NSCoder coder) => throw new NotImplementedException("it has not been implemented");

        public CGSize BorderSize
        {
            get
            {
                return this.borderSize;
            }

            set
            {
                this.borderSize = value;
                switch (this.alignment)
                {
                    case Alignment.Horizontal:
                        this.plane.Width = this.borderSize.Width / 2f;
                        break;
                    case Alignment.Vertical:
                        this.plane.Height = this.borderSize.Height / 2f;
                        break;
                }

                this.Scale = SCNVector3.One;
                this.Position = new SCNVector3(this.corner.U() * this.alignment.xOffset(this.borderSize),
                                               this.corner.V() * this.alignment.yOffset(this.borderSize),
                                               0f);
            }
        }

        #region Animating Open/Closed

        public void Open()
        {
            var offset = new OpenTK.Vector2();
            if (this.alignment == Alignment.Horizontal)
            {
                this.Scale = new SCNVector3(BorderSegment.OpenScale, 1f, 1f);
                offset.X = (1f - BorderSegment.OpenScale) * (float)(this.borderSize.Width) / 4f;
            }
            else
            {
                this.Scale = new SCNVector3(1f, BorderSegment.OpenScale, 1f);
                offset.Y = (1f - BorderSegment.OpenScale) * (float)(this.borderSize.Height) / 4f;
            }

            this.Position = new SCNVector3(this.corner.U() * this.alignment.xOffset(this.borderSize) + this.corner.U() * offset.X,
                                           this.corner.V() * this.alignment.yOffset(this.borderSize) + this.corner.V() * offset.Y,
                                           0f);
        }

        public void Close()
        {
            this.Scale = SCNVector3.One;
            this.Position = new SCNVector3(this.corner.U() * this.alignment.xOffset(this.borderSize),
                                           this.corner.V() * this.alignment.yOffset(this.borderSize),
                                           0f);
        }

        #endregion
    }
}