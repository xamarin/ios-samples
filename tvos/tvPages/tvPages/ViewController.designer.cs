// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MySingleView
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIImageView CatView { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem NextButton { get; set; }

		[Outlet]
		UIKit.UIPageControl PageView { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem PreviousButton { get; set; }

		[Action ("NextCat:")]
		partial void NextCat (Foundation.NSObject sender);

		[Action ("PreviousCat:")]
		partial void PreviousCat (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (CatView != null) {
				CatView.Dispose ();
				CatView = null;
			}

			if (PageView != null) {
				PageView.Dispose ();
				PageView = null;
			}

			if (PreviousButton != null) {
				PreviousButton.Dispose ();
				PreviousButton = null;
			}

			if (NextButton != null) {
				NextButton.Dispose ();
				NextButton = null;
			}
		}
	}
}
