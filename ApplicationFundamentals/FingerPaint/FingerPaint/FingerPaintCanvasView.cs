using System;
using System.Collections.Generic;
using System.Linq;

using CoreGraphics;
using Foundation;
using UIKit;

namespace FingerPaint
{
    class FingerPaintCanvasView : UIView
    {
        // Two collections for storing polylines
        Dictionary<IntPtr, FingerPaintPolyline> inProgressPolylines = new Dictionary<IntPtr, FingerPaintPolyline>();
        List<FingerPaintPolyline> completedPolylines = new List<FingerPaintPolyline>();

        public FingerPaintCanvasView()
        {
            BackgroundColor = UIColor.White;
            MultipleTouchEnabled = true;
        }

        public CGColor StrokeColor { set; get; } = new CGColor(1.0f, 0, 0);

        public float StrokeWidth { set; get; } = 2;

        public void Clear()
        {
            completedPolylines.Clear();
            SetNeedsDisplay();
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                // Create a FingerPaintPolyline, set the initial point, and store it
                FingerPaintPolyline polyline = new FingerPaintPolyline
                {
                    Color = StrokeColor,
                    StrokeWidth = StrokeWidth,
                };

                polyline.Path.MoveToPoint(touch.LocationInView(this));
                inProgressPolylines.Add(touch.Handle, polyline);
            }
            SetNeedsDisplay();
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                // Add point to path
                inProgressPolylines[touch.Handle].Path.AddLineToPoint(touch.LocationInView(this));
            }
            SetNeedsDisplay();
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                // Get polyline from dictionary and remove it from dictionary
                FingerPaintPolyline polyline = inProgressPolylines[touch.Handle];
                inProgressPolylines.Remove(touch.Handle);

                // Add final point to path and save with completed polylines
                polyline.Path.AddLineToPoint(touch.LocationInView(this));
                completedPolylines.Add(polyline);
            }
            SetNeedsDisplay();
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                inProgressPolylines.Remove(touch.Handle);
            }
            SetNeedsDisplay();
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            using (CGContext context = UIGraphics.GetCurrentContext())
            {
                // Stroke settings
                context.SetLineCap(CGLineCap.Round);
                context.SetLineJoin(CGLineJoin.Round);

                // Draw the completed polylines
                foreach (FingerPaintPolyline polyline in completedPolylines)
                {
                    context.SetStrokeColor(polyline.Color);
                    context.SetLineWidth(polyline.StrokeWidth);
                    context.AddPath(polyline.Path);
                    context.DrawPath(CGPathDrawingMode.Stroke);
                }

                // Draw the in-progress polylines
                foreach (FingerPaintPolyline polyline in inProgressPolylines.Values)
                {
                    context.SetStrokeColor(polyline.Color);
                    context.SetLineWidth(polyline.StrokeWidth);
                    context.AddPath(polyline.Path);
                    context.DrawPath(CGPathDrawingMode.Stroke);
                }
            }
        }
    }
}
