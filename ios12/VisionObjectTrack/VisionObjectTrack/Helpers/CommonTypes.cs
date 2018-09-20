
namespace VisionObjectTrack
{
    using System.Collections.Generic;
    using CoreGraphics;
    using System;
    using UIKit;
    using Vision;
    using VisionObjectTrack.Enums;

    public static class TrackedObjectsPalette
    {
        private static readonly Random Random = new Random();

        private static List<UIColor> Palette = new List<UIColor>
        {
            UIColor.Green,
            UIColor.Cyan,
            UIColor.Orange,
            UIColor.Brown,
            UIColor.DarkGray,
            UIColor.Red,
            UIColor.Yellow,
            UIColor.Magenta,
            UIColor.Gray,
            UIColor.Purple,
            UIColor.Clear,
            UIColor.LightGray,
            UIColor.Black,
            UIColor.Blue
        };

        public static UIColor Color(int index)
        {
            return index < Palette.Count ? Palette[index] : RandomColor();
        }

        private static UIColor RandomColor()
        {
            return UIColor.FromRGBA(RandomComponent(), RandomComponent(), RandomComponent(), 1f);

            float RandomComponent()
            {
                return Random.Next(256) / 255f;
            }
        }
    }

    public class TrackedPolyRect
    {
        private readonly CGPoint topLeft;
        private readonly CGPoint topRight;
        private readonly CGPoint bottomLeft;
        private readonly CGPoint bottomRight;

        public TrackedPolyRect(VNDetectedObjectObservation observation, UIColor color, TrackedPolyRectStyle style = TrackedPolyRectStyle.Solid)
            : this(observation.BoundingBox, color, style) { }

        public TrackedPolyRect(VNRectangleObservation observation, UIColor color, TrackedPolyRectStyle style = TrackedPolyRectStyle.Solid)
        {
            this.topLeft = observation.TopLeft;
            this.topRight = observation.TopRight;
            this.bottomLeft = observation.BottomLeft;
            this.bottomRight = observation.BottomRight;
            this.Color = color;
            this.Style = style;
        }

        public TrackedPolyRect(CGRect cgRect, UIColor color, TrackedPolyRectStyle style = TrackedPolyRectStyle.Solid)
        {
            this.topLeft = new CGPoint(cgRect.GetMinX(), cgRect.GetMaxY());
            this.topRight = new CGPoint(cgRect.GetMaxX(), cgRect.GetMaxY());
            this.bottomLeft = new CGPoint(cgRect.GetMinX(), cgRect.GetMinY());
            this.bottomRight = new CGPoint(cgRect.GetMaxX(), cgRect.GetMinY());
            this.Color = color;
            this.Style = style;
        }

        public UIColor Color { get; private set; }

        public TrackedPolyRectStyle Style { get; private set; }

        public IList<CGPoint> CornerPoints => new List<CGPoint> { this.topLeft, this.topRight, this.bottomRight, this.bottomLeft };

        public CGRect BoundingBox
        {
            get
            {
                var topLeftRect = new CGRect(this.topLeft, CGSize.Empty);
                var topRightRect = new CGRect(this.topRight, CGSize.Empty);
                var bottomLeftRect = new CGRect(this.bottomLeft, CGSize.Empty);
                var bottomRightRect = new CGRect(this.bottomRight, CGSize.Empty);

                return topLeftRect.UnionWith(topRightRect).UnionWith(bottomLeftRect).UnionWith(bottomRightRect);
            }
        }
    }
}