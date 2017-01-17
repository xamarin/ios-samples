using System;

using UIKit;

namespace FingerPaint
{
    public partial class FingerPaintViewController : UIViewController
    {
        public FingerPaintViewController() // base("FingerPaintViewController", null)
        {
            ;
        }

        public override void LoadView()
        {
            base.LoadView();

            View = new FingerPaintView(UIScreen.MainScreen.Bounds);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();



            // TK
            View.SetNeedsDisplay();



            // Perform any additional setup after loading the view, typically from a nib.
        }
    }
}