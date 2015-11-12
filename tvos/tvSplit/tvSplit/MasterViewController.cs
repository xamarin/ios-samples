using System;
using Foundation;
using UIKit;

namespace tvSplit
{
	public partial class MasterViewController : UIViewController
	{
		#region Computed Properties
		public MainSplitViewController SplitViewController { get; set;}
		public DetailViewController DetailController { get; set;}
		#endregion

		#region Constructors
		public MasterViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Custom Actions
		partial void PlayPausePressed (Foundation.NSObject sender) {

			// Show hide split view
			if (SplitViewController.DisplayMode == UISplitViewControllerDisplayMode.PrimaryHidden) {
				SplitViewController.PreferredDisplayMode = UISplitViewControllerDisplayMode.AllVisible;
			} else {
				SplitViewController.PreferredDisplayMode = UISplitViewControllerDisplayMode.PrimaryHidden;
			}

		}

		partial void Page1Pressed (Foundation.NSObject sender) {

			// Update GUI
			DetailController.Title = "Hot or Cold?";
			DetailController.FirstChoice = "Fire";
			DetailController.SecondChoice = "Ice";

		}

		partial void Page2Pressed (Foundation.NSObject sender) {

			// Update GUI
			DetailController.Title = "Wet or Dry?";
			DetailController.FirstChoice = "Desert";
			DetailController.SecondChoice = "Ocean";
		}

		partial void Page3Pressed (Foundation.NSObject sender) {

			// Update GUI
			DetailController.Title = "Sweet or Sour?";
			DetailController.FirstChoice = "Candy";
			DetailController.SecondChoice = "Lemons";
		}
		#endregion
	}
}
