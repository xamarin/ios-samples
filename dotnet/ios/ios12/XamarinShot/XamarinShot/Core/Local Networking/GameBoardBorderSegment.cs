namespace XamarinShot.Models;

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
        public static float U (this Corner corner)
        {
                switch (corner)
                {
                        case Corner.TopLeft: return -1;
                        case Corner.TopRight: return 1;
                        case Corner.BottomLeft: return -1;
                        case Corner.BottomRight: return 1;
                }

                throw new NotImplementedException ();
        }

        public static float V (this Corner corner)
        {
                switch (corner)
                {
                        case Corner.TopLeft: return -1;
                        case Corner.TopRight: return -1;
                        case Corner.BottomLeft: return 1;
                        case Corner.BottomRight: return 1;
                }

                throw new NotImplementedException ();
        }
}

public static class AlignmentExtensions
{
        public static float xOffset (this Alignment alignment, CGSize size)
        {
                switch (alignment)
                {
                        case Alignment.Horizontal:
                                return (float)(size.Width / 2f - BorderSegment.Thickness) / 2f;
                        case Alignment.Vertical:
                                return (float)(size.Width / 2f);
                }

                throw new NotImplementedException ();
        }

        public static float yOffset (this Alignment alignment, CGSize size)
        {
                switch (alignment)
                {
                        case Alignment.Horizontal:
                                return (float)(size.Height / 2f - BorderSegment.Thickness / 2f);
                        case Alignment.Vertical:
                                return (float)(size.Height / 2f) / 2f;
                }

                throw new NotImplementedException ();
        }
}

public class BorderSegment : SCNNode
{
        /// Thickness of the border lines.
        public const float Thickness = 0.012f;

        /// The scale of segment's length when in the open state
        const float OpenScale = 0.4f;

        readonly Corner corner;
        readonly Alignment alignment;
        readonly SCNPlane plane;

        CGSize borderSize;

        public BorderSegment (Corner corner, Alignment alignment, CGSize borderSize) : base ()
        {
                this.corner = corner;
                this.alignment = alignment;

                plane = SCNPlane.Create (BorderSegment.Thickness, BorderSegment.Thickness);
                this.borderSize = borderSize;

                var material = plane.FirstMaterial;
                if (material is not null)
                {
                        material.Diffuse.Contents = GameBoard.BorderColor;
                        material.Emission.Contents = GameBoard.BorderColor;
                        material.DoubleSided = true;
                        material.Ambient.Contents = UIColor.Black;
                        material.LightingModelName = SCNLightingModel.Constant;
                }

                Geometry = plane;
                Opacity = 0.8f;
        }

        public BorderSegment (NSCoder coder) => throw new NotImplementedException ("it has not been implemented");

        public CGSize BorderSize
        {
                get
                {
                        return borderSize;
                }

                set
                {
                        borderSize = value;
                        switch (alignment)
                        {
                                case Alignment.Horizontal:
                                        plane.Width = borderSize.Width / 2f;
                                        break;
                                case Alignment.Vertical:
                                        plane.Height = borderSize.Height / 2f;
                                        break;
                        }

                        Scale = SCNVector3.One;
                        Position = new SCNVector3 (corner.U () * alignment.xOffset (borderSize),
                                corner.V () * alignment.yOffset (borderSize), 0f);
                }
        }

        #region Animating Open/Closed

        public void Open ()
        {
                var offset = new OpenTK.Vector2 ();
                if (alignment == Alignment.Horizontal)
                {
                        Scale = new SCNVector3 (BorderSegment.OpenScale, 1f, 1f);
                        offset.X = (1f - BorderSegment.OpenScale) * (float)(borderSize.Width) / 4f;
                } else {
                        Scale = new SCNVector3 (1f, BorderSegment.OpenScale, 1f);
                        offset.Y = (1f - BorderSegment.OpenScale) * (float)(borderSize.Height) / 4f;
                }

                Position = new SCNVector3 (corner.U () * alignment.xOffset (borderSize) + corner.U () * offset.X,
                        corner.V () * alignment.yOffset (borderSize) + corner.V () * offset.Y, 0f);
        }

        public void Close ()
        {
                Scale = SCNVector3.One;
                Position = new SCNVector3 (corner.U () * alignment.xOffset (borderSize),
                        corner.V () * alignment.yOffset (borderSize), 0f);
        }

        #endregion
}
