using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using CoreGraphics;

namespace InteractiveTransitionLayout
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		ImagesCollectionViewController imagesController;
		UICollectionViewFlowLayout flowLayout;
		CircleLayout circleLayout;
		UIPinchGestureRecognizer pinch;

		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			// create and initialize a UICollectionViewFlowLayout
			flowLayout = new UICollectionViewFlowLayout (){
				SectionInset = new UIEdgeInsets (25,5,10,5),
				MinimumInteritemSpacing = 5,
				MinimumLineSpacing = 5,
				ItemSize = new CGSize (100, 100)
			};

			circleLayout = new CircleLayout (Monkeys.Instance.Count){
				ItemSize = new CGSize (100, 100)
			};
		
			imagesController = new ImagesCollectionViewController (flowLayout);

			nfloat sf = 0.4f;
			UICollectionViewTransitionLayout trLayout = null;
			UICollectionViewLayout nextLayout;

			pinch = new UIPinchGestureRecognizer (g => {

				var progress = Math.Abs(1.0f -  g.Scale)/sf;

				if(trLayout == null){
					if(imagesController.CollectionView.CollectionViewLayout is CircleLayout)
						nextLayout = flowLayout;
					else
						nextLayout = circleLayout;

					trLayout = imagesController.CollectionView.StartInteractiveTransition (nextLayout, (completed, finished) => {	
						Console.WriteLine ("transition completed");
						trLayout = null;
					});
				}

				trLayout.TransitionProgress = (nfloat)progress;

				imagesController.CollectionView.CollectionViewLayout.InvalidateLayout ();

				if(g.State == UIGestureRecognizerState.Ended){
					if (trLayout.TransitionProgress > 0.5f)
						imagesController.CollectionView.FinishInteractiveTransition ();
					else
						imagesController.CollectionView.CancelInteractiveTransition ();
				}

			});

			imagesController.CollectionView.AddGestureRecognizer (pinch);

			window.RootViewController = imagesController;
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}

