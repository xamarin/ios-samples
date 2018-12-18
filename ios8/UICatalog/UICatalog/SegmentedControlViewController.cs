using System;
using UIKit;

namespace UICatalog
{
    public partial class SegmentedControlViewController : UITableViewController
    {
        public SegmentedControlViewController (IntPtr handle) : base (handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ConfigureCustomBackgroundSegmentedControl();
        }

        private void ConfigureCustomBackgroundSegmentedControl()
        {
            customSegmentControl.SelectedSegment = 2;
            customSegmentControl.SetBackgroundImage(UIImage.FromBundle("stepper_and_segment_background"), UIControlState.Normal, UIBarMetrics.Default);
            customSegmentControl.SetBackgroundImage(UIImage.FromBundle("stepper_and_segment_background_disabled"), UIControlState.Disabled, UIBarMetrics.Default);
            customSegmentControl.SetBackgroundImage(UIImage.FromBundle("stepper_and_segment_background_highlighted"), UIControlState.Highlighted, UIBarMetrics.Default);
            customSegmentControl.SetDividerImage(UIImage.FromBundle("stepper_and_segment_divider"), UIControlState.Normal, UIControlState.Normal, UIBarMetrics.Default);

            var font = UIFont.FromDescriptor(UIFontDescriptor.PreferredCaption1, 0f);

            var normalTextAttributes = new UITextAttributes { TextColor = UIColor.Purple, Font = font };
            customSegmentControl.SetTitleTextAttributes(normalTextAttributes, UIControlState.Normal);

            var highlightedTextAttributes = new UITextAttributes { TextColor = UIColor.Green, Font = font };
            customSegmentControl.SetTitleTextAttributes(highlightedTextAttributes, UIControlState.Highlighted);
        }
    }
}