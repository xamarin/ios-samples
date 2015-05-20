using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace HomeKitIntro
{
	partial class MasterViewController : UINavigationController
	{
		#region Computed Properties
		/// <summary>
		/// Returns the delegate of the current running application
		/// </summary>
		/// <value>The this app.</value>
		public AppDelegate ThisApp {
			get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
		}
		#endregion

		#region Constructors
		public MasterViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			// Wireup events
			ThisApp.HomeManager.DidUpdateHomes += (sender, e) => {

				// Was a primary home found?
				if (ThisApp.HomeManager.PrimaryHome == null) {
					// Ask user to add a home
					PerformSegue("AddHomeSegue",this);
				} else {

				}
			};
				
		}
		#endregion
	}
}
