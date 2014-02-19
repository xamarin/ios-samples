// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;
using MonoTouch.UIKit;

namespace QRchestra
{
	[Register ("ReceiveViewController")]
	partial class ReceiveViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIView previewView { get; set; }

		[Action ("handleTap:")]
		partial void handleTap (UIGestureRecognizer recognizer);

		[Action ("showInfo:")]
		partial void showInfo (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (previewView != null) {
				previewView.Dispose ();
				previewView = null;
			}
		}
	}
}
