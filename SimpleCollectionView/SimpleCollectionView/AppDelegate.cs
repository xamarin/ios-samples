using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace SimpleCollectionView
{
    [Register ("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        UIWindow window;
        UICollectionViewController simpleCollectionViewController;
        CircleLayout circleLayout;
        LineLayout lineLayout;
        UICollectionViewFlowLayout flowLayout;

        public override bool FinishedLaunching (UIApplication app, NSDictionary options)
        {
            window = new UIWindow (UIScreen.MainScreen.Bounds);

            // Flow Layout
            flowLayout = new UICollectionViewFlowLayout (){
                HeaderReferenceSize = new System.Drawing.SizeF (100, 100),
                SectionInset = new UIEdgeInsets (20,20,20,20),
                ScrollDirection = UICollectionViewScrollDirection.Vertical
            };

            // Line Layout
            lineLayout = new LineLayout (){
                HeaderReferenceSize = new System.Drawing.SizeF (160, 100),
                ScrollDirection = UICollectionViewScrollDirection.Horizontal
            };

            // Circle Layout
            circleLayout = new CircleLayout ();

            simpleCollectionViewController = new SimpleCollectionViewController (flowLayout);
//            simpleCollectionViewController = new SimpleCollectionViewController (lineLayout);
//            simpleCollectionViewController = new SimpleCollectionViewController (circleLayout);

            simpleCollectionViewController.CollectionView.ContentInset = new UIEdgeInsets (50, 0, 0, 0);

            window.RootViewController = simpleCollectionViewController;
            window.MakeKeyAndVisible ();
            
            return true;
        }
    }
}

