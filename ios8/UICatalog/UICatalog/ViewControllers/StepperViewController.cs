using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace UICatalog
{
	public partial class StepperViewController : UITableViewController
	{
		[Outlet]
		private UIStepper DefaultStepper { get; set; }
		[Outlet]
		private UIStepper TintedStepper { get; set; }
		[Outlet]
		private UIStepper CustomStepper { get; set; }

		[Outlet]
		private UILabel DefaultStepperLabel { get; set; }
		[Outlet]
		private UILabel TintedStepperLabel { get; set; }
		[Outlet]
		private UILabel CustomStepperLabel { get; set; }

		public StepperViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ConfigureDefaultStepper ();
			ConfigureTintedStepper ();
			ConfigureCustomStepper ();
		}

		private void ConfigureDefaultStepper()
		{
			DefaultStepper.Value = 0;
			DefaultStepper.MinimumValue = 0;
			DefaultStepper.MaximumValue = 10;
			DefaultStepper.StepValue = 1;

			DefaultStepperLabel.Text = GetStepperLabelText (DefaultStepper);
			DefaultStepper.ValueChanged += OnStepperValueChange;
		}

		private void ConfigureTintedStepper()
		{
			TintedStepper.TintColor = ApplicationColors.Blue;
			TintedStepperLabel.Text = GetStepperLabelText (TintedStepper);
			TintedStepper.ValueChanged += OnStepperValueChange;
		}

		private void ConfigureCustomStepper()
		{
			// Set the background image.
			CustomStepper.SetBackgroundImage (UIImage.FromBundle ("stepper_and_segment_background"), UIControlState.Normal);
			CustomStepper.SetBackgroundImage (UIImage.FromBundle ("stepper_and_segment_background_highlighted"), UIControlState.Highlighted);
			CustomStepper.SetBackgroundImage (UIImage.FromBundle ("stepper_and_segment_background_disabled"), UIControlState.Disabled);

			// Set the image which will be painted in between the two stepper segments (depends on the states of both segments).
			CustomStepper.SetDividerImage (UIImage.FromBundle ("stepper_and_segment_divider"), UIControlState.Normal, UIControlState.Normal);

			// Set the image for the + button.
			CustomStepper.SetIncrementImage (UIImage.FromBundle ("stepper_increment"), UIControlState.Normal);

			// Set the image for the - button.
			CustomStepper.SetDecrementImage (UIImage.FromBundle ("stepper_decrement"), UIControlState.Normal);

			CustomStepperLabel.Text = GetStepperLabelText (CustomStepper);
			CustomStepper.ValueChanged += OnStepperValueChange;
		}

		private void OnStepperValueChange(object sender, EventArgs e)
		{
			Console.WriteLine ("A stepper changed its value: {0}.", sender);

			var stepper = (UIStepper)sender;

			// A mapping from a stepper to its associated label.
			var stepperMapping = new Dictionary<UIStepper, UILabel> { 
				{ DefaultStepper, DefaultStepperLabel },
				{ TintedStepper, TintedStepperLabel },
				{ CustomStepper, CustomStepperLabel }
			};

			stepperMapping [stepper].Text = GetStepperLabelText (stepper);
		}

		private string GetStepperLabelText(UIStepper stepper)
		{
			return ((int)stepper.Value).ToString ();
		}
	}
}
