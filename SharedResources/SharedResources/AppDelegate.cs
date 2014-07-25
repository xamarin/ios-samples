using System;
using UIKit;
using Foundation;

namespace Example_SharedResources
{
	//========================================================================
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		//========================================================================
		#region -= declarations and properties =-
		
		protected UIWindow window;
		protected UINavigationController mainNavController;
		protected Example_SharedResources.Screens.iPhone.Home.HomeNavController iPhoneHome;
		protected int networkActivityCount = 0;
		
		#endregion
		//========================================================================
		
		//========================================================================
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			//---- create our window
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.MakeKeyAndVisible ();
		
			//---- instantiate our main navigatin controller and add it's view to the window
			mainNavController = new UINavigationController ();
			
			iPhoneHome = new Example_SharedResources.Screens.iPhone.Home.HomeNavController ();
			mainNavController.PushViewController (iPhoneHome, false);
			
			
			window.RootViewController = mainNavController;
			
			//----
			return true;
		}
		//========================================================================
		
		//========================================================================
		/// <summary>
		/// Keeps a running reference of items that want to turn the network activity on or off 
		/// so it doesn't get turned off by one activity if another is still active
		/// </summary>
		public void SetNetworkActivityIndicator(bool onOrOff)
		{
			//---- increment or decrement our reference count
			if(onOrOff)
			{ networkActivityCount++; }
			else { networkActivityCount--; }
			
			//---- set it's visibility based on whether or not there is still activity
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = (networkActivityCount > 0);
		}
		//========================================================================
		
	}
	//========================================================================
}
