using Foundation;
using System;
using UIKit;

namespace UICatalog
{
    public partial class StepperViewController : UITableViewController
    {
        public StepperViewController (IntPtr handle) : base (handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ConfigureCustomStepper();

            UpdateDefaultLabel();
            UpdateTintedLabel();
            UpdateCustomLabel();
        }

        private void ConfigureCustomStepper()
        {
            // Set the background image.
            customStepper.SetBackgroundImage(UIImage.FromBundle("stepper_and_segment_background"), UIControlState.Normal);
            customStepper.SetBackgroundImage(UIImage.FromBundle("stepper_and_segment_background_highlighted"), UIControlState.Highlighted);
            customStepper.SetBackgroundImage(UIImage.FromBundle("stepper_and_segment_background_disabled"), UIControlState.Disabled);

            // Set the image which will be painted in between the two stepper segments (depends on the states of both segments).
            customStepper.SetDividerImage(UIImage.FromBundle("stepper_and_segment_divider"), UIControlState.Normal, UIControlState.Normal);

            // Set the image for the + button.
            customStepper.SetIncrementImage(UIImage.FromBundle("stepper_increment"), UIControlState.Normal);

            // Set the image for the - button.
            customStepper.SetDecrementImage(UIImage.FromBundle("stepper_decrement"), UIControlState.Normal);
        }

        partial void CustomValueChanged(NSObject sender)
        {
            UpdateCustomLabel();
        }

        partial void DefaultValueChanged(NSObject sender)
        {
            UpdateDefaultLabel();
        }

        partial void TintedValueChanged(NSObject sender)
        {
            UpdateTintedLabel();
        }

        private void UpdateDefaultLabel()
        {
            defaultLabel.Text = defaultStepper.Value.ToString();
        }

        private void UpdateTintedLabel()
        {
            tintedLabel.Text = tintedStepper.Value.ToString();
        }

        private void UpdateCustomLabel()
        {
            customLabel.Text = customStepper.Value.ToString();
        }
    }
}