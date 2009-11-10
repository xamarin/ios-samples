using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;

public partial class QuartzViewController {
        [Connect]                                                                                                                            
        public UIScrollView scrollView {
                get { return (UIScrollView) GetNativeField ("scrollView"); }
                set { SetNativeField ("scrollView", value); }
        }
}
