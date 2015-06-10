using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using CoreGraphics;

namespace LayoutWithNavController
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		ImagesCollectionViewController viewController;
		UICollectionViewFlowLayout layout;
		UINavigationController navController;

		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			// create and initialize a UICollectionViewFlowLayout
			layout = new UICollectionViewFlowLayout (){
				SectionInset = new UIEdgeInsets (10,5,10,5),
				MinimumInteritemSpacing = 5,
				MinimumLineSpacing = 5,
				ItemSize = new CGSize (100, 100)
			};

			viewController = new ImagesCollectionViewController (layout);
			viewController.UseLayoutToLayoutNavigationTransitions = false;

			navController = new UINavigationController (viewController);

			window.RootViewController = navController;
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}

