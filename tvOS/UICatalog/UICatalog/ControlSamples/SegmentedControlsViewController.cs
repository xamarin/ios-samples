using Foundation;
using UIKit;

namespace UICatalog {
	public partial class SegmentedControlsViewController : UIViewController {

		[Export ("initWithCoder:")]
		public SegmentedControlsViewController (NSCoder coder): base (coder)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			ConfigureCustomBackgroundSegmentedControl ();
		}

		void ConfigureCustomBackgroundSegmentedControl ()
		{
			var normalSegmentBackgroundImage = UIImage.FromBundle ("stepper_and_segment_background");
			CustomBackgroundSegmentControl.SetBackgroundImage (normalSegmentBackgroundImage, UIControlState.Normal, UIBarMetrics.Default);

			var disabledSegmentBackgroundImage = UIImage.FromBundle ("stepper_and_segment_background_disabled");
			CustomBackgroundSegmentControl.SetBackgroundImage (disabledSegmentBackgroundImage, UIControlState.Disabled, UIBarMetrics.Default);

			var highlightedSegmentBackgroundImage = UIImage.FromBundle ("stepper_and_segment_background_highlighted");
			CustomBackgroundSegmentControl.SetBackgroundImage (highlightedSegmentBackgroundImage, UIControlState.Highlighted, UIBarMetrics.Default);

			// Set the divider image.
			var segmentDividerImage = UIImage.FromFile ("stepper_and_segment_divider");
			CustomBackgroundSegmentControl.SetDividerImage (segmentDividerImage, UIControlState.Normal, UIControlState.Normal, UIBarMetrics.Default);

			//TODO fix it https://bugzilla.xamarin.com/show_bug.cgi?id=35338
			// Create a font to use for the attributed title (both normal and highlighted states).
//			var font = UIFont.FromName (".HelveticaNeueInterface-M3", 30f);

//			var normalTextAttributes = new UITextAttributes {
//				TextColor = UIColor.Purple,
//				Font = font
//			};

//			CustomBackgroundSegmentControl.SetTitleTextAttributes (normalTextAttributes, UIControlState.Normal);
//			CustomBackgroundSegmentControl.SetTitleTextAttributes (normalTextAttributes, UIControlState.Highlighted);
		}
	}
}
