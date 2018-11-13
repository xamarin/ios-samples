using CoreGraphics;
using UIKit;

namespace Appearance
{
    public class PlainView : UIView
    {
        private UIProgressView progress;
        private UIButton plainButton;
        private UISlider slider;

        public PlainView()
        {
            slider = new UISlider();
            progress = new UIProgressView();
            plainButton = UIButton.FromType(UIButtonType.RoundedRect);
            plainButton.SetTitle("Plain Button", UIControlState.Normal);

            plainButton.Frame = new CGRect(20, 150, 130, 40);
            slider.Frame = new CGRect(20, 190, 250, 20);
            progress.Frame = new CGRect(20, 230, 250, 20);

            slider.Value = 0.75f;
            progress.Progress = 0.35f;

            AddSubview(plainButton);
            AddSubview(slider);
            AddSubview(progress);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (plainButton != null)
            {
                plainButton.Dispose();
                plainButton = null;
            }

            if (slider != null)
            {
                slider.Dispose();
                slider = null;
            }

            if (progress != null)
            {
                progress.Dispose();
                progress = null;
            }
        }
    }
}