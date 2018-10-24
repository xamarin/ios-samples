// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace ZoomingPdfViewer
{
	[Register ("DataViewController")]
	partial class DataViewController
	{
		[Outlet]
		ZoomingPdfViewer.PdfScrollView ScrollView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ScrollView != null) {
				ScrollView.Dispose ();
				ScrollView = null;
			}
		}
	}
}
