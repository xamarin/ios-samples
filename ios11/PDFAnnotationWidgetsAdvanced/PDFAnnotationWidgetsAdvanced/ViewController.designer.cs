// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace PDFAnnotationWidgetsAdvanced
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		PdfKit.PdfView PDFView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (PDFView != null) {
				PDFView.Dispose ();
				PDFView = null;
			}
		}
	}
}
