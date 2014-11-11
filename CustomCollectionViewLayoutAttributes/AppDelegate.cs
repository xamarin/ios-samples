using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using CoreGraphics;

namespace SimpleCollectionView
{
    [Register ("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        UIWindow window;
        UICollectionViewController simpleCollectionViewController;
        CircleLayout circleLayout;

        public override bool FinishedLaunching (UIApplication app, NSDictionary options)
        {
            window = new UIWindow (UIScreen.MainScreen.Bounds);

            circleLayout = new CircleLayout ();
            simpleCollectionViewController = new SimpleCollectionViewController (circleLayout);

            window.RootViewController = simpleCollectionViewController;
            window.MakeKeyAndVisible ();
            
            return true;
        }

		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
    }
}

