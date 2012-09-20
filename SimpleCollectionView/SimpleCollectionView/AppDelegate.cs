using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SimpleCollectionView
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		UIViewController simpleCollectionViewController;

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			// use UICollectionViewFlowLayout
			var layout = new UICollectionViewFlowLayout (){
				HeaderReferenceSize = new System.Drawing.SizeF (50, 50),
				SectionInset = new UIEdgeInsets (20,0,0,0)
			};

			// use CircleLayout
//			var layout = new CircleLayout ();

			// use LineLayout
//			var layout = new LineLayout (){
//				HeaderReferenceSize = new System.Drawing.SizeF (250, 50)
//			};
		
			simpleCollectionViewController = new SimpleCollectionViewController (layout);

			window.RootViewController = simpleCollectionViewController;

			// make the window visible
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}

