using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;

public partial class QuartzBlendingViewController {
        [Connect]                                                                                                                            
        public UIPickerView picker {
                get { return (UIPickerView) GetNativeField ("picker"); }
                set { SetNativeField ("picker", value); }
        }
}
