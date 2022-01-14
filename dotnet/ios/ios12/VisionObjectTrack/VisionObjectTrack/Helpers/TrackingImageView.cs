
namespace VisionObjectTrack;

public partial class TrackingImageView : UIView
{
        CGRect imageAreaRect = CGRect.Empty;

        readonly nfloat [] dashedLinesLengths = { 4f, 2f };

        float dashedPhase = 0f;

        protected TrackingImageView (IntPtr handle) : base (handle) { }

        public UIImage? Image { get; set; }

        public List<TrackedPolyRect> PolyRects { get; set; } = new List<TrackedPolyRect> ();

        // Rubber-banding setup

        public CGPoint RubberbandingStart { get; set; } = CGPoint.Empty;

        public CGPoint RubberbandingVector { get; set; } = CGPoint.Empty;

        protected CGRect RubberbandingRect
        {
                get
                {
                        var pt1 = RubberbandingStart;
                        var pt2 = new CGPoint (RubberbandingStart.X + RubberbandingVector.X,
                                RubberbandingStart.Y + RubberbandingVector.Y);

                        return new CGRect (Math.Min (pt1.X, pt2.X),
                                Math.Min (pt1.Y, pt2.Y),
                                Math.Abs (pt1.X - pt2.X),
                                Math.Abs (pt1.Y - pt2.Y));
                }
        }

        public CGRect RubberbandingRectNormalized
        {
                get
                {
                        var result = CGRect.Empty;
                        if (imageAreaRect.Size.Width > 0 && imageAreaRect.Size.Height > 0)
                        {
                                result = RubberbandingRect;

                                // Make it relative to imageAreaRect
                                result.X = (result.X - imageAreaRect.X) / imageAreaRect.Width;
                                result.Y = (result.Y - imageAreaRect.Y) / imageAreaRect.Height;
                                result.Width /= imageAreaRect.Width;
                                result.Height /= imageAreaRect.Height;

                                // Adjust to Vision.framework input requrement - origin at LLC
                                result.Y = 1f - result.Y - result.Height;

                        }

                        return result;
                }
        }

        public bool IsPointWithinDrawingArea (CGPoint locationInView)
        {
                return imageAreaRect.Contains (locationInView);
        }

        public override void LayoutSubviews ()
        {
                base.LayoutSubviews ();
                SetNeedsDisplay ();
        }

        public override void Draw (CGRect rect)
        {
                var context = UIGraphics.GetCurrentContext ();

                context.SaveState ();

                context.ClearRect (rect);
                context.SetFillColor (0f, 0f, 0f, 0f);
                context.SetLineWidth (2f);

                // Draw a frame
                var newImage = ScaleImage (rect.Size);
                if (newImage is not null)
                {
                        newImage.Draw (imageAreaRect);

                        // Draw rubberbanding rectangle, if available
                        if (RubberbandingRect != CGRect.Empty)
                        {
                                context.SetStrokeColor (UIColor.Blue.CGColor);

                                // Switch to dashed lines for rubberbanding selection
                                context.SetLineDash (dashedPhase, dashedLinesLengths);
                                context.StrokeRect (RubberbandingRect);
                        }

                        // Draw rects
                        foreach (var polyRect in PolyRects)
                        {
                                context.SetStrokeColor (polyRect.Color.CGColor);
                                switch (polyRect.Style)
                                {
                                        case TrackedPolyRectStyle.Solid:
                                                context.SetLineDash (dashedPhase, new nfloat [] { });
                                                break;
                                        case TrackedPolyRectStyle.Dashed:
                                                context.SetLineDash (dashedPhase, dashedLinesLengths);
                                                break;
                                }

                                var cornerPoints = polyRect.CornerPoints;
                                var previous = Scale (cornerPoints [cornerPoints.Count - 1], rect);
                                foreach (var cornerPoint in cornerPoints)
                                {
                                        context.MoveTo (previous.X, previous.Y);
                                        var current = Scale (cornerPoint, rect);

                                        context.AddLineToPoint (current.X, current.Y);
                                        previous = current;
                                }

                                context.StrokePath ();
                        }

                        context.RestoreState ();
                }
        }

        UIImage? ScaleImage (CGSize viewSize)
        {
                UIImage? result = null;
                if (Image is not null && Image.Size != CGSize.Empty)
                {
                        imageAreaRect = CGRect.Empty;

                        // There are two possible cases to fully fit self.image into the the ImageTrackingView area:
                        // Option 1) image.width = view.width ==> image.height <= view.height
                        // Option 2) image.height = view.height ==> image.width <= view.width
                        var imageAspectRatio = Image.Size.Width / Image.Size.Height;

                        // Check if we're in Option 1) case and initialize self.imageAreaRect accordingly
                        var imageSizeOption1 = new CGSize (viewSize.Width, Math.Floor (viewSize.Width / imageAspectRatio));
                        if (imageSizeOption1.Height <= viewSize.Height)
                        {
                                var imageX = 0f;
                                var imageY = Math.Floor ((viewSize.Height - imageSizeOption1.Height) / 2f);
                                imageAreaRect = new CGRect (imageX, imageY,
                                        imageSizeOption1.Width, imageSizeOption1.Height);
                        }

                        if (imageAreaRect == CGRect.Empty)
                        {
                                // Check if we're in Option 2) case if Option 1) didn't work out and initialize imageAreaRect accordingly
                                var imageSizeOption2 = new CGSize (Math.Floor (viewSize.Height * imageAspectRatio), viewSize.Height);
                                if (imageSizeOption2.Width <= viewSize.Width)
                                {
                                        var imageX = Math.Floor ((viewSize.Width - imageSizeOption2.Width) / 2f);
                                        var imageY = 0f;
                                        imageAreaRect = new CGRect (imageX, imageY,
                                                imageSizeOption2.Width, imageSizeOption2.Height);
                                }
                        }

                        // In next line, pass 0.0 to use the current device's pixel scaling factor (and thus account for Retina resolution).
                        // Pass 1.0 to force exact pixel size.
                        UIGraphics.BeginImageContextWithOptions (imageAreaRect.Size, false, 0f);
                        Image.Draw (new CGRect (0f, 0f, imageAreaRect.Size.Width, imageAreaRect.Size.Height));

                        result = UIGraphics.GetImageFromCurrentImageContext ();
                        UIGraphics.EndImageContext ();
                }

                return result;
        }

        CGPoint Scale (CGPoint point, CGRect viewRect)
        {
                // Adjust bBox from Vision.framework coordinate system (origin at LLC) to imageView coordinate system (origin at ULC)
                var pointY = 1f - point.Y;
                var scaleFactor = imageAreaRect.Size;

                return new CGPoint (point.X * scaleFactor.Width + imageAreaRect.X,
                                   pointY * scaleFactor.Height + imageAreaRect.Y);
        }
}
