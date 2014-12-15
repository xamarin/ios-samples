// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;
using UIKit;

namespace QRchestra
{
	[Register ("ReceiveViewController")]
	partial class ReceiveViewController
	{
		[Outlet]
		UIKit.UIView previewView { get; set; }

		[Action ("handleTap:")]
		partial void handleTap (UIGestureRecognizer recognizer);

		[Action ("showInfo:")]
		partial void showInfo (Foundation.NSObject sender);

		void ReleaseDesignerOutlets ()
		{
			if (previewView != null) {
				previewView.Dispose ();
				previewView = null;
			}
		}
	}
}
