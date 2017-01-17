using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Drawing;

namespace FingerPaint
{
    class FingerPaintView : UIView
    {
        public FingerPaintView(CGRect frame) : base(frame)
        {

            SetNeedsDisplay();


            this.MultipleTouchEnabled = true;

        }

       

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            BackgroundColor = UIColor.FromRGB(1.0f, 0.8f, 0.8f);

        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                System.Diagnostics.Debug.WriteLine("Began: " + touch);
            }



        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                System.Diagnostics.Debug.WriteLine("Move: " + touch);
            }
            SetNeedsDisplay();
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                System.Diagnostics.Debug.WriteLine("Ended: " + touch);
            }
            SetNeedsDisplay();
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
        }
    }
}
