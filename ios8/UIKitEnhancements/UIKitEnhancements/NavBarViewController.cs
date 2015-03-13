using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace UIKitEnhancements
{
	partial class NavBarViewController : UIViewController
	{
		#region Constructors
		public NavBarViewController(IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Set the default mode
			this.NavigationController.HidesBarsOnTap = true;

			// Wireup segment controller
			NavBarMode.ValueChanged += (sender, e) => {
				// Take action based on the selected mode
				switch(NavBarMode.SelectedSegment) {
				case 0:
					this.NavigationController.HidesBarsOnTap = true;
					this.NavigationController.HidesBarsOnSwipe = false;
					break;
				case 1:
					this.NavigationController.HidesBarsOnTap = false;
					this.NavigationController.HidesBarsOnSwipe = true;
					break;
				}
			};
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			// Return navigation bar to normal
			this.NavigationController.HidesBarsOnTap = false;
			this.NavigationController.HidesBarsOnSwipe = false;
		}
		#endregion
	}
}
