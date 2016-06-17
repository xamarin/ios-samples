using System;
using Foundation;
using UIKit;

namespace tvSplit
{
	public partial class MainSplitViewController : UISplitViewController
	{
		#region Constructors
		public MainSplitViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Gain access to master and detail view controllers
			var masterController = ViewControllers [0] as MasterViewController;
			var detailController = ViewControllers [1] as DetailViewController;

			// Wire-up views
			masterController.SplitViewController = this;
			masterController.DetailController = detailController;
			detailController.SplitViewController = this;
		}
		#endregion
	}
}
