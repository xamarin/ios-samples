using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace NavigationBar {
	/// <summary>
	/// Demonstrates configuring various types of controls as the right bar item of the navigation bar.
	/// </summary>
	public partial class CustomRightViewController : UIViewController {
		public CustomRightViewController (IntPtr handle) : base (handle) { }

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait;
		}

		/// <summary>
		/// Action for the segemented control.
		/// </summary>
		partial void ChangeRightBarItem (UISegmentedControl sender)
		{
			switch (sender.SelectedSegment) {
			case 0:
				// Add a custom add button as the nav bar's custom right view
				var addButton = new UIBarButtonItem (NSBundle.MainBundle.GetLocalizedString ("AddTitle"),
													UIBarButtonItemStyle.Plain,
													this.Action);

				base.NavigationItem.RightBarButtonItem = addButton;
				break;

			case 1:
				// Add a custom add button as the nav bar's custom right view
				var emailButton = new UIBarButtonItem (UIImage.FromBundle ("Email"),
													  UIBarButtonItemStyle.Plain,
													  this.Action);

				base.NavigationItem.RightBarButtonItem = emailButton;
				break;

			case 2:
				// "Segmented" control to the right
				var segmentedControl = new UISegmentedControl (new UIImage [] { UIImage.FromBundle ("UpArrow"), UIImage.FromBundle ("DownArrow") });

				segmentedControl.AddTarget (this.Action, UIControlEvent.ValueChanged);
				segmentedControl.Frame = new CGRect (0f, 0f, 90f, 30f);
				segmentedControl.Momentary = true;

				// Add a custom add button as the nav bar's custom right view
				var segmentBarItem = new UIBarButtonItem (segmentedControl);

				base.NavigationItem.RightBarButtonItem = segmentBarItem;
				break;
			}
		}

		private void Action (object sender, EventArgs e)
		{
			Console.WriteLine ("Custom Action was invoked");
		}

		partial void RightAction (UIBarButtonItem sender)
		{
			Console.WriteLine ("RightAction was invoked");
		}
	}
}
