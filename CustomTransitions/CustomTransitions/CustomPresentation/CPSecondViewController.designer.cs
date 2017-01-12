// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace CustomTransitions
{
    [Register ("CPSecondViewController")]
    partial class CPSecondViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider slider { get; set; }

        [Action ("UnwindToCustomPresentationSecondViewController:")]
        public void UnwindToCustomPresentationSecondViewController (UIStoryboardSegue segue)
        {
        }

        [Action ("SliderValueChange:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SliderValueChange (UIKit.UISlider sender);

        void ReleaseDesignerOutlets ()
        {
            if (slider != null) {
                slider.Dispose ();
                slider = null;
            }
        }
    }
}