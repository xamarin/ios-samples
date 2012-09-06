// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace RosyWriter
{
	[Register ("RosyWriterViewControllerUniversal")]
	partial class RosyWriterViewControllerUniversal
	{
		[Outlet]
		MonoTouch.UIKit.UIView previewView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btnRecord { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (previewView != null) {
				previewView.Dispose ();
				previewView = null;
			}

			if (btnRecord != null) {
				btnRecord.Dispose ();
				btnRecord = null;
			}
		}
	}
}
