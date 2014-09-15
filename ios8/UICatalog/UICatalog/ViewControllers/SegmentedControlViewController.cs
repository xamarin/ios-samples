using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace UICatalog
{
	[Register ("SegmentedControlViewController")]
	public class SegmentedControlViewController : UITableViewController
	{
		[Outlet]
		private UISegmentedControl DefaultSegmentedControl { get; set; }

		[Outlet]
		private UISegmentedControl TintedSegmentedControl { get; set; }

		[Outlet]
		private UISegmentedControl CustomSegmentsSegmentedControl { get; set; }

		[Outlet]
		private UISegmentedControl CustomBackgroundSegmentedControl { get; set; }

		public SegmentedControlViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ConfigureDefaultSegmentedControl ();
			ConfigureTintedSegmentedControl ();
			ConfigureCustomSegmentsSegmentedControl ();
			ConfigureCustomBackgroundSegmentedControl ();
		}

		private void ConfigureDefaultSegmentedControl()
		{
			DefaultSegmentedControl.Momentary = true;
			DefaultSegmentedControl.SetEnabled (false, 0);
			DefaultSegmentedControl.ValueChanged += SelectedSegmentDidChange;
		}

		private void ConfigureTintedSegmentedControl()
		{
			TintedSegmentedControl.TintColor = ApplicationColors.Blue;
			TintedSegmentedControl.SelectedSegment = 1;
			TintedSegmentedControl.ValueChanged += SelectedSegmentDidChange;
		}

		private void ConfigureCustomSegmentsSegmentedControl()
		{
			var imageToAccessibilityLabelMappings = new Dictionary<string, string> {
				{ "checkmark_icon",  "Done".Localize () },
				{ "search_icon", "Search".Localize () },
				{ "tools_icon", "Settings".Localize () }
			};

			// Guarantee that the segments show up in the same order.
			string[] sortedSegmentImageNames = imageToAccessibilityLabelMappings.Keys.ToArray ();
			Array.Sort (sortedSegmentImageNames);

			for (int i = 0; i < sortedSegmentImageNames.Length; i++) {
				string segmentImageName = sortedSegmentImageNames [i];
				var image = UIImage.FromBundle (segmentImageName);
				image.AccessibilityLabel = imageToAccessibilityLabelMappings [segmentImageName];
				CustomSegmentsSegmentedControl.SetImage (image, i);
			}

			CustomSegmentsSegmentedControl.SelectedSegment = 0;
			CustomSegmentsSegmentedControl.ValueChanged += SelectedSegmentDidChange;
		}

		private void ConfigureCustomBackgroundSegmentedControl()
		{
			CustomBackgroundSegmentedControl.SelectedSegment = 2;
			CustomBackgroundSegmentedControl.SetBackgroundImage (UIImage.FromBundle ("stepper_and_segment_background"), UIControlState.Normal, UIBarMetrics.Default);
			CustomBackgroundSegmentedControl.SetBackgroundImage (UIImage.FromBundle ("stepper_and_segment_background_disabled"), UIControlState.Disabled, UIBarMetrics.Default);
			CustomBackgroundSegmentedControl.SetBackgroundImage (UIImage.FromBundle ("stepper_and_segment_background_highlighted"), UIControlState.Highlighted, UIBarMetrics.Default);
			CustomBackgroundSegmentedControl.SetDividerImage (UIImage.FromBundle ("stepper_and_segment_divider"), UIControlState.Normal, UIControlState.Normal, UIBarMetrics.Default);

			var font = UIFont.FromDescriptor (UIFontDescriptor.PreferredCaption1, 0f);

			UITextAttributes normalTextAttributes = new UITextAttributes {
				TextColor = ApplicationColors.Purple,
				Font = font
			};
			CustomBackgroundSegmentedControl.SetTitleTextAttributes (normalTextAttributes, UIControlState.Normal);

			UITextAttributes highlightedTextAttributes = new UITextAttributes {
				TextColor = ApplicationColors.Green,
				Font = font
			};
			CustomBackgroundSegmentedControl.SetTitleTextAttributes (highlightedTextAttributes, UIControlState.Highlighted);

			CustomBackgroundSegmentedControl.ValueChanged += SelectedSegmentDidChange;
		}

		private void SelectedSegmentDidChange (object sender, EventArgs e)
		{
			Console.WriteLine ("The selected segment changed for: {0}", sender);
		}
	}
}
