
namespace AVCamBarcode
{
    using AVCamBarcode.Extensions;
    using AVFoundation;
    using CoreAnimation;
    using CoreGraphics;
    using Foundation;
    using ObjCRuntime;
    using System;
    using UIKit;

    [Register("PreviewView")]
    public class PreviewView : UIView, IUIGestureRecognizerDelegate
    {
        private static readonly object layerClassLock = new object();

        private static Class layerClass;

        private const float RegionOfInterestCornerTouchThreshold = 50;

        private const float RegionOfInterestControlDiameter = 12;

        private readonly CAShapeLayer maskLayer = new CAShapeLayer();

        private readonly CAShapeLayer regionOfInterestOutline = new CAShapeLayer();

        /// <summary>
        /// White dot on the top left of the region of interest.
        /// </summary>
        private readonly CAShapeLayer topLeftControl = new CAShapeLayer();

        /// <summary>
        /// White dot on the top right of the region of interest.
        /// </summary>
        private readonly CAShapeLayer topRightControl = new CAShapeLayer();

        /// <summary>
        /// White dot on the bottom left of the region of interest.
        /// </summary>
        private readonly CAShapeLayer bottomLeftControl = new CAShapeLayer();

        /// <summary>
        /// White dot on the bottom right of the region of interest.
        /// </summary>
        private readonly CAShapeLayer bottomRightControl = new CAShapeLayer();

        /// <summary>
        /// Saves a reference to the control corner that the user is using to resize
        /// the region of interest in `resizeRegionOfInterestWithGestureRecognizer()`.
        /// </summary>
        private ControlCorner currentControlCorner = ControlCorner.None;

        private UIPanGestureRecognizer resizeRegionOfInterestGestureRecognizer;

        public event EventHandler RegionOfInterestChanged;

        public PreviewView(CGRect frame) : base(frame)
        {
            this.Initialize();
        }

        [Export("initWithCoder:")]
        public PreviewView(NSCoder coder) : base(coder)
        {
            this.Initialize();
        }

        public static Class LayerClass
        {
            [Export("layerClass")]
            get
            {
                if (layerClass == null)
                {
                    lock (layerClassLock)
                    {
                        if (layerClass == null)
                        {
                            layerClass = new Class(typeof(AVCaptureVideoPreviewLayer));
                        }
                    }
                }

                return layerClass;
            }
        }

        // Region of Interest

        /// <summary>
        /// The minimum region of interest's size cannot be smaller than the corner
        /// touch threshold as to avoid control selection conflicts when a user tries
        /// to resize the region of interest.
        /// </summary>
        protected float MinimumRegionOfInterestSize => RegionOfInterestCornerTouchThreshold;

        protected float RegionOfInterestControlRadius => RegionOfInterestControlDiameter / 2;

        public bool IsResizingRegionOfInterest => this.resizeRegionOfInterestGestureRecognizer.State == UIGestureRecognizerState.Changed;

        // This property is set only in `setRegionOfInterestWithProposedRegionOfInterest()`.
        // When a user is resizing the region of interest in `resizeRegionOfInterestWithGestureRecognizer()`,
        // the KVO notification will be triggered when the resizing is finished.
        public CGRect RegionOfInterest { get; private set; }

        // AV capture properties

        public AVCaptureVideoPreviewLayer VideoPreviewLayer => this.Layer as AVCaptureVideoPreviewLayer;

        public AVCaptureSession Session
        {
            get
            {
                return this.VideoPreviewLayer.Session;
            }

            set
            {
                this.VideoPreviewLayer.Session = value;
            }
        }

        protected UIPanGestureRecognizer ResizeRegionOfInterestGestureRecognizer
        {
            get
            {
                if (this.resizeRegionOfInterestGestureRecognizer == null)
                {
                    this.resizeRegionOfInterestGestureRecognizer = new UIPanGestureRecognizer(this.ResizeRegionOfInterestWithGestureRecognizer);
                }

                return this.resizeRegionOfInterestGestureRecognizer;
            }
        }

        private void Initialize()
        {
            this.maskLayer.FillRule = CAShapeLayer.FillRuleEvenOdd;
            this.maskLayer.FillColor = UIColor.Black.CGColor;
            this.maskLayer.Opacity = 0.6f;
            this.Layer.AddSublayer(this.maskLayer);

            this.regionOfInterestOutline.Path = UIBezierPath.FromRect(this.RegionOfInterest).CGPath;
            this.regionOfInterestOutline.FillColor = UIColor.Clear.CGColor;
            this.regionOfInterestOutline.StrokeColor = UIColor.Yellow.CGColor;
            this.Layer.AddSublayer(this.regionOfInterestOutline);

            var controlRect = new CGRect(0, 0, RegionOfInterestControlDiameter, RegionOfInterestControlDiameter);

            this.topLeftControl.Path = UIBezierPath.FromOval(controlRect).CGPath;
            this.topLeftControl.FillColor = UIColor.White.CGColor;
            this.Layer.AddSublayer(this.topLeftControl);

            this.topRightControl.Path = UIBezierPath.FromOval(controlRect).CGPath;
            this.topRightControl.FillColor = UIColor.White.CGColor;
            this.Layer.AddSublayer(this.topRightControl);

            this.bottomLeftControl.Path = UIBezierPath.FromOval(controlRect).CGPath;
            this.bottomLeftControl.FillColor = UIColor.White.CGColor;
            this.Layer.AddSublayer(this.bottomLeftControl);

            this.bottomRightControl.Path = UIBezierPath.FromOval(controlRect).CGPath;
            this.bottomRightControl.FillColor = UIColor.White.CGColor;
            this.Layer.AddSublayer(this.bottomRightControl);

            // Add the region of interest gesture recognizer to the region of interest
            // view so that the region of interest can be resized and moved. If you
            // would like to have a fixed region of interest that cannot be resized
            // or moved, do not add the following gesture recognizer. You will simply
            // need to set the region of interest once.
            this.ResizeRegionOfInterestGestureRecognizer.Delegate = this;
            this.AddGestureRecognizer(this.ResizeRegionOfInterestGestureRecognizer);
        }

        /// <summary>
        /// Updates the region of interest with a proposed region of interest ensuring
        /// the new region of interest is within the bounds of the video preview. When
        /// a new region of interest is set, the region of interest is redrawn.
        /// </summary>
        /// <param name="proposedRegionOfInterest"></param>
        public void SetRegionOfInterestWithProposedRegionOfInterest(CGRect proposedRegionOfInterest)
        {
            // We standardize to ensure we have positive widths and heights with an origin at the top left.
            var videoPreviewRect = this.VideoPreviewLayer.MapToLayerCoordinates(new CGRect(0, 0, 1, 1)).Standardize();

            // Intersect the video preview view with the view's frame to only get
            // the visible portions of the video preview view.

            var visibleVideoPreviewRect = CGRect.Intersect(videoPreviewRect, this.Frame);
            var oldRegionOfInterest = this.RegionOfInterest;
            var newRegionOfInterest = proposedRegionOfInterest.Standardize();

            // Move the region of interest in bounds.
            if (this.currentControlCorner == ControlCorner.None)
            {
                nfloat xOffset = 0;
                nfloat yOffset = 0;

                if (!visibleVideoPreviewRect.Contains(newRegionOfInterest.CornerTopLeft()))
                {
                    xOffset = NMath.Max(visibleVideoPreviewRect.GetMinX() - newRegionOfInterest.GetMinX(), 0);
                    yOffset = NMath.Max(visibleVideoPreviewRect.GetMinY() - newRegionOfInterest.GetMinY(), 0);
                }

                if (!visibleVideoPreviewRect.Contains(visibleVideoPreviewRect.CornerBottomRight()))
                {
                    xOffset = NMath.Min(visibleVideoPreviewRect.GetMaxX() - newRegionOfInterest.GetMaxX(), xOffset);
                    yOffset = NMath.Min(visibleVideoPreviewRect.GetMaxY() - newRegionOfInterest.GetMaxY(), yOffset);
                }

                newRegionOfInterest.Offset(xOffset, yOffset);
            }

            // Clamp the size when the region of interest is being resized.
            visibleVideoPreviewRect.Intersect(newRegionOfInterest);
            newRegionOfInterest = visibleVideoPreviewRect;

            // Fix a minimum width of the region of interest.
            if (proposedRegionOfInterest.Size.Width < this.MinimumRegionOfInterestSize)
            {
                switch (this.currentControlCorner)
                {
                    case ControlCorner.TopLeft:
                    case ControlCorner.BottomLeft:
                        var x = oldRegionOfInterest.Location.X + oldRegionOfInterest.Size.Width - this.MinimumRegionOfInterestSize;

                        newRegionOfInterest.X = x;
                        newRegionOfInterest.Width = this.MinimumRegionOfInterestSize;
                        break;

                    case ControlCorner.TopRight:
                        newRegionOfInterest.X = oldRegionOfInterest.Location.X;
                        newRegionOfInterest.Width = this.MinimumRegionOfInterestSize;
                        break;

                    default:
                        newRegionOfInterest.Location = oldRegionOfInterest.Location;
                        newRegionOfInterest.Width = this.MinimumRegionOfInterestSize;
                        break;
                }
            }

            // Fix a minimum height of the region of interest.
            if (proposedRegionOfInterest.Height < this.MinimumRegionOfInterestSize)
            {
                switch (currentControlCorner)
                {
                    case ControlCorner.TopLeft:
                    case ControlCorner.TopRight:
                        newRegionOfInterest.Y = oldRegionOfInterest.Y + oldRegionOfInterest.Height - this.MinimumRegionOfInterestSize;
                        newRegionOfInterest.Height = this.MinimumRegionOfInterestSize;
                        break;

                    case ControlCorner.BottomLeft:
                        newRegionOfInterest.Y = oldRegionOfInterest.Y;
                        newRegionOfInterest.Height = this.MinimumRegionOfInterestSize;
                        break;

                    default:
                        newRegionOfInterest.Location = oldRegionOfInterest.Location;
                        newRegionOfInterest.Height = this.MinimumRegionOfInterestSize;
                        break;
                }
            }

            this.RegionOfInterest = newRegionOfInterest;
            this.SetNeedsLayout();
        }

        private void ResizeRegionOfInterestWithGestureRecognizer(UIPanGestureRecognizer gestureRecognizer)
        {
            var touchLocation = gestureRecognizer.LocationInView(gestureRecognizer.View);
            var oldRegionOfInterest = this.RegionOfInterest;

            switch (gestureRecognizer.State)
            {
                case UIGestureRecognizerState.Began:
                    // When the gesture begins, save the corner that is closes to
                    // the resize region of interest gesture recognizer's touch location.
                    this.currentControlCorner = CornerOfRect(oldRegionOfInterest, touchLocation);
                    break;


                case UIGestureRecognizerState.Changed:
                    var newRegionOfInterest = oldRegionOfInterest;

                    switch (this.currentControlCorner)
                    {
                        case ControlCorner.None:
                            // Update the new region of interest with the gesture recognizer's translation.
                            var translation = gestureRecognizer.TranslationInView(gestureRecognizer.View);

                            // Move the region of interest with the gesture recognizer's translation.
                            if (this.RegionOfInterest.Contains(touchLocation))
                            {
                                newRegionOfInterest.X += translation.X;
                                newRegionOfInterest.Y += translation.Y;
                            }

                            // If the touch location goes outside the preview layer,
                            // we will only translate the region of interest in the
                            // plane that is not out of bounds.
                            var normalizedRect = new CGRect(0, 0, 1, 1);
                            if (!normalizedRect.Contains(this.VideoPreviewLayer.PointForCaptureDevicePointOfInterest(touchLocation)))
                            {
                                if (touchLocation.X < RegionOfInterest.GetMinX() || touchLocation.X > RegionOfInterest.GetMaxX())
                                {
                                    newRegionOfInterest.Y += translation.Y;
                                }
                                else if (touchLocation.Y < RegionOfInterest.GetMinY() || touchLocation.Y > RegionOfInterest.GetMaxY())
                                {
                                    newRegionOfInterest.X += translation.X;
                                }
                            }

                            // Set the translation to be zero so that the new gesture
                            // recognizer's translation is in respect to the region of
                            // interest's new position.
                            gestureRecognizer.SetTranslation(CGPoint.Empty, gestureRecognizer.View);
                            break;

                        case ControlCorner.TopLeft:
                            newRegionOfInterest = new CGRect(touchLocation.X, touchLocation.Y,
                                                             oldRegionOfInterest.Width + oldRegionOfInterest.X - touchLocation.X,
                                                             oldRegionOfInterest.Height + oldRegionOfInterest.Y - touchLocation.Y);
                            break;

                        case ControlCorner.TopRight:
                            newRegionOfInterest = new CGRect(newRegionOfInterest.X,
                                                             touchLocation.Y,
                                                             touchLocation.X - newRegionOfInterest.X,
                                                             oldRegionOfInterest.Height + newRegionOfInterest.Y - touchLocation.Y);
                            break;


                        case ControlCorner.BottomLeft:
                            newRegionOfInterest = new CGRect(touchLocation.X,
                                                             oldRegionOfInterest.Y,
                                                             oldRegionOfInterest.Width + oldRegionOfInterest.X - touchLocation.X,
                                                             touchLocation.Y - oldRegionOfInterest.Y);
                            break;

                        case ControlCorner.BottomRight:
                            newRegionOfInterest = new CGRect(oldRegionOfInterest.X,
                                                             oldRegionOfInterest.Y,
                                                             touchLocation.X - oldRegionOfInterest.X,
                                                             touchLocation.Y - oldRegionOfInterest.Y);
                            break;
                    }

                    // Update the region of interest with a valid CGRect.
                    this.SetRegionOfInterestWithProposedRegionOfInterest(newRegionOfInterest);
                    break;

                case UIGestureRecognizerState.Ended:
                    this.RegionOfInterestChanged?.Invoke(this, EventArgs.Empty);

                    // Reset the current corner reference to none now that the resize.
                    // gesture recognizer has ended.
                    this.currentControlCorner = ControlCorner.None;
                    break;

                default:
                    return;
            }
        }

        private ControlCorner CornerOfRect(CGRect rect, CGPoint point)
        {
            var closestDistance = nfloat.MaxValue;
            var closestCorner = ControlCorner.None;

            Tuple<ControlCorner, CGPoint>[] corners = {
                Tuple (ControlCorner.TopLeft, rect.CornerTopLeft()),
                Tuple (ControlCorner.TopRight, rect.CornerTopRight ()),
                Tuple (ControlCorner.BottomLeft, rect.CornerBottomLeft ()),
                Tuple (ControlCorner.BottomRight, rect.CornerBottomRight())
            };

            foreach (var tuple in corners)
            {
                var corner = tuple.Item1;
                var cornerPoint = tuple.Item2;

                var dX = point.X - cornerPoint.X;
                var dY = point.Y - cornerPoint.Y;
                var distance = NMath.Sqrt((dX * dX) + (dY * dY));

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCorner = corner;
                }
            }

            if (closestDistance > RegionOfInterestCornerTouchThreshold)
            {
                closestCorner = ControlCorner.None;
            }

            return closestCorner;
        }

        private static Tuple<ControlCorner, CGPoint> Tuple(ControlCorner corner, CGPoint point)
        {
            return new Tuple<ControlCorner, CGPoint>(corner, point);
        }

        #region UIView

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            // Disable CoreAnimation actions so that the positions of the sublayers immediately move to their new position.
            CATransaction.Begin();
            CATransaction.DisableActions = true;

            // Create the path for the mask layer. We use the even odd fill rule so that the region of interest does not have a fill color.
            var path = UIBezierPath.FromRect(new CGRect(0, 0, this.Frame.Width, this.Frame.Height));
            path.AppendPath(UIBezierPath.FromRect(this.RegionOfInterest));
            path.UsesEvenOddFillRule = true;
            this.maskLayer.Path = path.CGPath;

            this.regionOfInterestOutline.Path = CGPath.FromRect(this.RegionOfInterest);

            var left = this.RegionOfInterest.X - this.RegionOfInterestControlRadius;
            var right = this.RegionOfInterest.X + this.RegionOfInterest.Width - this.RegionOfInterestControlRadius;
            var top = this.RegionOfInterest.Y - this.RegionOfInterestControlRadius;
            var bottom = this.RegionOfInterest.Y + this.RegionOfInterest.Height - this.RegionOfInterestControlRadius;

            this.topLeftControl.Position = new CGPoint(left, top);
            this.topRightControl.Position = new CGPoint(right, top);
            this.bottomLeftControl.Position = new CGPoint(left, bottom);
            this.bottomRightControl.Position = new CGPoint(right, bottom);

            CATransaction.Commit();
        }

        #endregion

        #region IUIGestureRecognizerDelegate

        [Export("gestureRecognizer:shouldReceiveTouch:")]
        public bool ShouldReceiveTouch(UIGestureRecognizer gestureRecognizer, UITouch touch)
        {
            var result = true;

            // Ignore drags outside of the region of interest (plus some padding).
            if (gestureRecognizer == this.resizeRegionOfInterestGestureRecognizer)
            {
                var touchLocation = touch.LocationInView(gestureRecognizer.View);

                var paddedRegionOfInterest = this.RegionOfInterest.Inset(-RegionOfInterestCornerTouchThreshold, -RegionOfInterestCornerTouchThreshold);
                if (!paddedRegionOfInterest.Contains(touchLocation))
                {
                    result = false;
                }
            }

            return result;
        }

        [Export("gestureRecognizer:shouldRecognizeSimultaneouslyWithGestureRecognizer:")]
        public bool ShouldRecognizeSimultaneously(UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
        {
            var result = false;

            // Allow multiple gesture recognizers to be recognized simultaneously if and only if the touch location is not within the touch threshold.
            if (gestureRecognizer == this.resizeRegionOfInterestGestureRecognizer)
            {
                var touchLocation = gestureRecognizer.LocationInView(gestureRecognizer.View);
                result = this.CornerOfRect(this.RegionOfInterest, touchLocation) == ControlCorner.None;
            }

            return result;
        }

        #endregion
    }
}