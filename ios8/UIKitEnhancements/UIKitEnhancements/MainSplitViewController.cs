using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace UIKitEnhancements
{
	partial class MainSplitViewController : UISplitViewController
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

			// Set-up Split View Controller
			PreferredDisplayMode = UISplitViewControllerDisplayMode.AllVisible;


		}
		#endregion
	}
}
