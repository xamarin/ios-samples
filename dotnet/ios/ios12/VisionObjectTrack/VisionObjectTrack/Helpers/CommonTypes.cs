
namespace VisionObjectTrack;

using VisionObjectTrack.Enums;

public static class TrackedObjectsPalette
{
        static readonly Random Random = new Random ();

        static List<UIColor> Palette = new List<UIColor>
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

        public static UIColor Color (int index)
        {
                return index < Palette.Count ? Palette [index] : RandomColor ();
        }

        static UIColor RandomColor ()
        {
                return UIColor.FromRGBA (RandomComponent (), RandomComponent (), RandomComponent (), 1f);

                float RandomComponent ()
                {
                        return Random.Next (256) / 255f;
                }
        }
}

public class TrackedPolyRect
{
        readonly CGPoint topLeft;
        readonly CGPoint topRight;
        readonly CGPoint bottomLeft;
        readonly CGPoint bottomRight;

        public TrackedPolyRect (VNDetectedObjectObservation observation, UIColor color, TrackedPolyRectStyle style = TrackedPolyRectStyle.Solid)
            : this (observation.BoundingBox, color, style) { }

        public TrackedPolyRect (VNRectangleObservation observation, UIColor color, TrackedPolyRectStyle style = TrackedPolyRectStyle.Solid)
        {
                topLeft = observation.TopLeft;
                topRight = observation.TopRight;
                bottomLeft = observation.BottomLeft;
                bottomRight = observation.BottomRight;
                Color = color;
                Style = style;
        }

        public TrackedPolyRect (CGRect cgRect, UIColor color, TrackedPolyRectStyle style = TrackedPolyRectStyle.Solid)
        {
                topLeft = new CGPoint (cgRect.GetMinX (), cgRect.GetMaxY ());
                topRight = new CGPoint (cgRect.GetMaxX (), cgRect.GetMaxY ());
                bottomLeft = new CGPoint (cgRect.GetMinX (), cgRect.GetMinY ());
                bottomRight = new CGPoint (cgRect.GetMaxX (), cgRect.GetMinY ());
                Color = color;
                Style = style;
        }

        public UIColor Color { get; private set; }

        public TrackedPolyRectStyle Style { get; private set; }

        public IList<CGPoint> CornerPoints => new List<CGPoint> { topLeft, topRight, bottomRight, bottomLeft };

        public CGRect BoundingBox
        {
                get
                {
                        var topLeftRect = new CGRect (topLeft, CGSize.Empty);
                        var topRightRect = new CGRect (topRight, CGSize.Empty);
                        var bottomLeftRect = new CGRect (bottomLeft, CGSize.Empty);
                        var bottomRightRect = new CGRect (bottomRight, CGSize.Empty);

                        return topLeftRect.UnionWith (topRightRect).UnionWith (bottomLeftRect).UnionWith (bottomRightRect);
                }
        }
}
