using System;
using System.Collections.Generic;

using Foundation;
using CoreFoundation;

using UIKit;
using CoreGraphics;
using AssetsLibrary;

namespace MediaNotes
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}

	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		static List<ALAsset> photoAssets;
		ALAssetsLibrary assetsLibrary;
		static ALAsset currentAsset;
		static UIImage currentPhotoImage;
		static int currentPhotoIndex;
		YYCommentContainerViewController ViewController;
		const int PHOTO_ASSETS_CAPACITY = 50;
		string PNALBUM_PREFIX = "WWDC2012";
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

			var pnViewController = new PhotoViewController ("PhotoViewController", null);
			pnViewController.Datasource = new MyDatasource ();
			InitializePhotos ();
			ViewController = new YYCommentContainerViewController (pnViewController);
			window.RootViewController = ViewController;
			// If you have defined a view, add it here:
			// window.AddSubview (navigationController.View);
			// make the window visible
			window.MakeKeyAndVisible ();

			return true;
		}

		//AlAssetsGroupSavedPhotos
		public void InitializePhotos ()
		{
			assetsLibrary = new ALAssetsLibrary ();
			currentPhotoIndex = -1;
			photoAssets = new List<ALAsset> (PHOTO_ASSETS_CAPACITY);
			int photoIndex = 0;
			bool syncContentController = true;
			assetsLibrary.Enumerate (ALAssetsGroupType.Album, ( ALAssetsGroup group, ref bool stop) => {

				if (group != null) {
				
					string groupName = group.Name;
					if (groupName.StartsWith (PNALBUM_PREFIX)) {
						group.SetAssetsFilter (ALAssetsFilter.AllPhotos);
						group.Enumerate ((ALAsset asset, int index, ref bool st) => {
							int notfound = Int32.MaxValue;
							if (asset != null && index != notfound) {
								photoAssets.Add (asset);
								photoIndex++;
								currentPhotoIndex = 0;
							}
							if (photoIndex == PHOTO_ASSETS_CAPACITY) {
								st = true;
							}
						});
					}
				}
				
				if (syncContentController) {
					syncContentController = false;
					DispatchQueue.MainQueue.DispatchAsync (() => {
						if (currentPhotoIndex == 0) {
							setCurrentPhotoToIndex (0);
						}
						((PhotoViewController)(ViewController.contentController)).Synchronize (currentPhotoIndex >= 0);
					});
				}
			},
			(NSError error) => {
				Console.WriteLine ("User denied access to photo Library... {0}", error);
			});
		}

		public static void setCurrentPhotoToIndex (int index)
		{
			currentAsset = photoAssets [index];
			ALAssetRepresentation rep = currentAsset.RepresentationForUti ("public.jpeg");
			// image might not be available as a JPEG
			if (rep == null)
				return;
			CGImage imageRef = rep.GetFullScreenImage ();
			if (imageRef == null)
				return;
			currentPhotoImage = UIImage.FromImage (imageRef);
			currentPhotoIndex = index;
		}
		  
		public class MyDatasource : PNDataSourceProtocol
		{
			public void ProceedToNextItem ()
			{
				if (photoAssets.Count > 0) {
					currentPhotoIndex++;
					setCurrentPhotoToIndex (currentPhotoIndex < (photoAssets.Count) ? currentPhotoIndex : 0);
				}
			}

			public void ProceedToPreviousItem ()
			{
				if (photoAssets.Count > 0) {
					currentPhotoIndex --;
					setCurrentPhotoToIndex ((currentPhotoIndex < 0) ? photoAssets.Count - 1 : currentPhotoIndex);
				}
			}

			public UIImage ImageForCurrentItem ()
			{
				return currentPhotoImage;
			}

			public NSUrl UrlForCurrentItem ()
			{
				NSDictionary map = currentAsset.UtiToUrlDictionary;
				return (NSUrl)map.ObjectForKey (new NSString ("public.jpeg"));
			}
		}
	}
}