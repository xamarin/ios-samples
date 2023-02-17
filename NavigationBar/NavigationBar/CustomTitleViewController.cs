using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace NavigationBar {
	/// <summary>
	/// Demonstrates configuring the navigation bar to use a UIView as the title.
	/// </summary>
	public partial class CustomTitleViewController : UIViewController {
		public CustomTitleViewController (IntPtr handle) : base (handle) { }

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var segmentTextContent = new string []
			{
				NSBundle.MainBundle.GetLocalizedString("Image"),
				NSBundle.MainBundle.GetLocalizedString("Text"),
				NSBundle.MainBundle.GetLocalizedString("Video"),
			};

			// Segmented control as the custom title view
			var segmentedControl = new UISegmentedControl (segmentTextContent);
			segmentedControl.SelectedSegment = 0;
			segmentedControl.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			segmentedControl.Frame = new CGRect (0f, 0f, 400f, 30f);
			segmentedControl.AddTarget (this.Action, UIControlEvent.ValueChanged);

			base.NavigationItem.TitleView = segmentedControl;
		}

		private void Action (object sender, EventArgs e)
		{
			Console.WriteLine ("CustomTitleViewController IBAction invoked!");
		}
	}
}
