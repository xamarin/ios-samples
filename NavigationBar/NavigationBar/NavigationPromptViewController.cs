using System;
using UIKit;

namespace NavigationBar {
	/// <summary>
	/// Demonstrates displaying text above the navigation bar.
	/// </summary>
	public partial class NavigationPromptViewController : UIViewController {
		public NavigationPromptViewController (IntPtr handle) : base (handle) { }

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// There is a bug in iOS 7.x (fixed in iOS 8) which causes the
			// topLayoutGuide to not be properly resized if the prompt is set before
			// -viewDidAppear: is called. This may result in the navigation bar
			// improperly overlapping your content.  For this reason, you should
			// avoid configuring the prompt in your storyboard and instead configure
			// it programmatically in -viewDidAppear: if your application deploys to iOS 7.
			//
			base.NavigationItem.Prompt = "Navigation prompts appear at the top.";
		}
	}
}
