// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using System.CodeDom.Compiler;

#if __UNIFIED__
using Foundation;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif

namespace CalabashSample.iOS
{
	[Register ("MainViewController")]
	partial class MainViewController
	{
		[Outlet]
		UILabel statusLabel { get; set; }

		[Action ("showInfo:")]
		partial void showInfo (NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (statusLabel != null) {
				statusLabel.Dispose ();
				statusLabel = null;
			}
		}
	}
}
