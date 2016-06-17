// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WatchkitExtension
{
	[Register ("MapDetailController")]
	partial class MapDetailController
	{
		[Outlet]
		WatchKit.WKInterfaceButton appleButton { get; set; }

		[Outlet]
		WatchKit.WKInterfaceButton imagesButton { get; set; }

		[Outlet]
		WatchKit.WKInterfaceButton inButton { get; set; }

		[Outlet]
		WatchKit.WKInterfaceMap map { get; set; }

		[Outlet]
		WatchKit.WKInterfaceButton outButton { get; set; }

		[Outlet]
		WatchKit.WKInterfaceButton pinsButton { get; set; }

		[Outlet]
		WatchKit.WKInterfaceButton removeAllButton { get; set; }

		[Outlet]
		WatchKit.WKInterfaceButton tokyoButton { get; set; }

		[Action ("addImageAnnotations:")]
		partial void AddImageAnnotations (Foundation.NSObject obj);

		[Action ("addPinAnnotations:")]
		partial void AddPinAnnotations (Foundation.NSObject obj);

		[Action ("goToApple:")]
		partial void GoToApple (Foundation.NSObject obj);

		[Action ("goToTokyo:")]
		partial void GoToTokyo (Foundation.NSObject obj);

		[Action ("removeAll:")]
		partial void RemoveAll (Foundation.NSObject obj);

		[Action ("zoomIn:")]
		partial void ZoomIn (Foundation.NSObject obj);

		[Action ("zoomOut:")]
		partial void ZoomOut (Foundation.NSObject obj);
		
		void ReleaseDesignerOutlets ()
		{
			if (appleButton != null) {
				appleButton.Dispose ();
				appleButton = null;
			}

			if (tokyoButton != null) {
				tokyoButton.Dispose ();
				tokyoButton = null;
			}

			if (inButton != null) {
				inButton.Dispose ();
				inButton = null;
			}

			if (outButton != null) {
				outButton.Dispose ();
				outButton = null;
			}

			if (pinsButton != null) {
				pinsButton.Dispose ();
				pinsButton = null;
			}

			if (imagesButton != null) {
				imagesButton.Dispose ();
				imagesButton = null;
			}

			if (removeAllButton != null) {
				removeAllButton.Dispose ();
				removeAllButton = null;
			}

			if (map != null) {
				map.Dispose ();
				map = null;
			}
		}
	}
}
