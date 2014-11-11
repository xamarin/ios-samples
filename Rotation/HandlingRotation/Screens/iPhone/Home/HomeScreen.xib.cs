
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace HandlingRotation.Screens.iPhone.Home
{
	public partial class HomeScreen : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public HomeScreen (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public HomeScreen (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public HomeScreen () : base("HomeScreen", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		
		}
		
		#endregion
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			// we don't want the nav bar to appear on the home page, so we hide it
			// when the page appears. if it's an animated appearance, we likewise 
			// animate the disappearance of the nav bar
			NavigationController.SetNavigationBarHidden (true, animated);
		}
			
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			// when page is going away because we're showing another page, we
			// bring the nav bar back
			NavigationController.SetNavigationBarHidden (false, animated);
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// wire up our button TouchUpInside handlers
			btnMethod1.TouchUpInside += delegate {
				NavigationController.PushViewController (new Method1Autosize.AutosizeScreen (), true);
			};
			btnMethod2.TouchUpInside += delegate {
				NavigationController.PushViewController (new Method2MoveControls.Controller (), true);
			};
			btnMethod3.TouchUpInside += delegate {
				NavigationController.PushViewController (new Method3SwapViews.Controller (), true);
			};
		}
		
		/// <summary>
		/// When the device rotates, the OS calls this method to determine if it should try and rotate the
		/// application and then call WillAnimateRotation
		/// </summary>
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// we're passed to orientation that it will rotate to. in our case, we could
			// just return true, but this switch illustrates how you can test for the 
			// different cases
			switch (toInterfaceOrientation)
			{
				case UIInterfaceOrientation.LandscapeLeft:
				case UIInterfaceOrientation.LandscapeRight:
				case UIInterfaceOrientation.Portrait:
				case UIInterfaceOrientation.PortraitUpsideDown:
				default:
					return true;
			}
		}
	}
}