
namespace VisionObjectTrack
{
    using CoreGraphics;
    using System;
    using System.Collections.Generic;
    using UIKit;
    using VisionObjectTrack.Enums;

    public partial class TrackingImageView : UIView
    {
        private CGRect imageAreaRect = CGRect.Empty;

        private readonly nfloat[] dashedLinesLengths = { 4f, 2f };

        private float dashedPhase = 0f;

        public TrackingImageView(IntPtr handle) : base(handle) { }

        public UIImage Image { get; set; }

        public List<TrackedPolyRect> PolyRects { get; set; } = new List<TrackedPolyRect>();

        // Rubber-banding setup

        public CGPoint RubberbandingStart { get; set; } = CGPoint.Empty;

        public CGPoint RubberbandingVector { get; set; } = CGPoint.Empty;

        protected CGRect RubberbandingRect
        {
            get
            {
                var pt1 = this.RubberbandingStart;
                var pt2 = new CGPoint(this.RubberbandingStart.X + this.RubberbandingVector.X, 
                                      this.RubberbandingStart.Y + this.RubberbandingVector.Y);

                return new CGRect(Math.Min(pt1.X, pt2.X), 
                                  Math.Min(pt1.Y, pt2.Y), 
                                  Math.Abs(pt1.X - pt2.X), 
                                  Math.Abs(pt1.Y - pt2.Y));
            }
        }

        public CGRect RubberbandingRectNormalized
        {
            get
            {
                var result = CGRect.Empty;
                if (this.imageAreaRect.Size.Width > 0 && this.imageAreaRect.Size.Height > 0)
                {
                    result = this.RubberbandingRect;

                    // Make it relative to imageAreaRect
                    result.X = (result.X - this.imageAreaRect.X) / this.imageAreaRect.Width;
                    result.Y = (result.Y - this.imageAreaRect.Y) / this.imageAreaRect.Height;
                    result.Width /= this.imageAreaRect.Width;
                    result.Height /= this.imageAreaRect.Height;

                    // Adjust to Vision.framework input requrement - origin at LLC
                    result.Y = 1f - result.Y - result.Height;

                }

                return result;
            }
        }

        public bool IsPointWithinDrawingArea(CGPoint locationInView)
        {
            return this.imageAreaRect.Contains(locationInView);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            this.SetNeedsDisplay();
        }

        public override void Draw(CGRect rect)
        {
            var context = UIGraphics.GetCurrentContext();

            context.SaveState();

            context.ClearRect(rect);
            context.SetFillColor(0f, 0f, 0f, 0f);
            context.SetLineWidth(2f);

            // Draw a frame
            var newImage = this.ScaleImage(rect.Size);
            if (newImage != null)
            {
                newImage.Draw(this.imageAreaRect);

                // Draw rubberbanding rectangle, if available
                if (this.RubberbandingRect != CGRect.Empty)
                {
                    context.SetStrokeColor(UIColor.Blue.CGColor);

                    // Switch to dashed lines for rubberbanding selection
                    context.SetLineDash(this.dashedPhase, this.dashedLinesLengths);
                    context.StrokeRect(this.RubberbandingRect);
                }

                // Draw rects
                foreach(var polyRect in this.PolyRects)
                {
                    context.SetStrokeColor(polyRect.Color.CGColor);
                    switch (polyRect.Style)
                    {
                        case TrackedPolyRectStyle.Solid:
                            context.SetLineDash(this.dashedPhase, new nfloat[] { });
                            break;
                        case TrackedPolyRectStyle.Dashed:
                            context.SetLineDash(this.dashedPhase, this.dashedLinesLengths);
                            break;
                    }

                    var cornerPoints = polyRect.CornerPoints;
                    var previous = this.Scale(cornerPoints[cornerPoints.Count - 1], rect);
                    foreach(var cornerPoint in cornerPoints)
                    {
                        context.MoveTo(previous.X, previous.Y);
                        var current = this.Scale(cornerPoint, rect);

                        context.AddLineToPoint(current.X, current.Y);
                        previous = current;
                    }

                    context.StrokePath();
                }

                context.RestoreState();
            }
        }

        private UIImage ScaleImage(CGSize viewSize) 
        {
            UIImage result = null;
            if (this.Image != null && this.Image.Size != CGSize.Empty)
            {
                this.imageAreaRect = CGRect.Empty;

                // There are two possible cases to fully fit self.image into the the ImageTrackingView area:
                // Option 1) image.width = view.width ==> image.height <= view.height
                // Option 2) image.height = view.height ==> image.width <= view.width
                var imageAspectRatio = this.Image.Size.Width / this.Image.Size.Height;

                // Check if we're in Option 1) case and initialize self.imageAreaRect accordingly
                var imageSizeOption1 = new CGSize(viewSize.Width, Math.Floor(viewSize.Width / imageAspectRatio));
                if (imageSizeOption1.Height <= viewSize.Height)
                {
                    var imageX = 0f;
                    var imageY = Math.Floor((viewSize.Height - imageSizeOption1.Height) / 2f);
                    this.imageAreaRect = new CGRect(imageX,
                                                    imageY,
                                                    imageSizeOption1.Width,
                                                    imageSizeOption1.Height);
                }

                if (this.imageAreaRect == CGRect.Empty)
                {
                    // Check if we're in Option 2) case if Option 1) didn't work out and initialize imageAreaRect accordingly
                    var imageSizeOption2 = new CGSize(Math.Floor(viewSize.Height * imageAspectRatio), viewSize.Height);
                    if (imageSizeOption2.Width <= viewSize.Width)
                    {
                        var imageX = Math.Floor((viewSize.Width - imageSizeOption2.Width) / 2f);
                        var imageY = 0f;
                        this.imageAreaRect = new CGRect(imageX,
                                                        imageY,
                                                        imageSizeOption2.Width,
                                                        imageSizeOption2.Height);
                    }
                }

                // In next line, pass 0.0 to use the current device's pixel scaling factor (and thus account for Retina resolution).
                // Pass 1.0 to force exact pixel size.
                UIGraphics.BeginImageContextWithOptions(this.imageAreaRect.Size, false, 0f);
                this.Image.Draw(new CGRect(0f, 0f, this.imageAreaRect.Size.Width, this.imageAreaRect.Size.Height));

                result = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();
            }

            return result;
        }
    
        private CGPoint Scale(CGPoint point, CGRect viewRect)
        {
            // Adjust bBox from Vision.framework coordinate system (origin at LLC) to imageView coordinate system (origin at ULC)
            var pointY = 1f - point.Y;
            var scaleFactor = this.imageAreaRect.Size;

            return new CGPoint(point.X * scaleFactor.Width + this.imageAreaRect.X,
                               pointY * scaleFactor.Height + this.imageAreaRect.Y);
        }
    }
}