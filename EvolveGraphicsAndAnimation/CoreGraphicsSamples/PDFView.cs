using System;
using UIKit;

using CoreGraphics;
using Foundation;

namespace CoreGraphicsSamples
{
	public class PDFView : UIView
	{
		CGPDFDocument pdfDoc;
			
		public PDFView ()
		{
			BackgroundColor = UIColor.White;

			//create a CGPDFDocument from file.pdf included in the main bundle
			pdfDoc = CGPDFDocument.FromFile ("file.pdf");
		}

		public override void Draw (CGRect rect)
		{
			base.Draw (rect);
				
			//flip the CTM so the PDF will be drawn upright
			using (CGContext g = UIGraphics.GetCurrentContext ()) {
				g.TranslateCTM (0, Bounds.Height);
				g.ScaleCTM (1, -1);
				
				// render the first page of the PDF
				using (CGPDFPage pdfPage = pdfDoc.GetPage (1)) {
					
					//get the affine transform that defines where the PDF is drawn
					CGAffineTransform t = pdfPage.GetDrawingTransform (CGPDFBox.Crop, rect, 0, true);
					//concatenate the pdf transform with the CTM for display in the view
					g.ConcatCTM (t);
					//draw the pdf page
					g.DrawPDFPage (pdfPage);
				}
			}
		}

		void DrawPDFInMemory ()
		{
			//data buffer to hold the PDF
			NSMutableData data = new NSMutableData ();
			//create a PDF with empty rectangle, which will configure it for 8.5x11 inches
			UIGraphics.BeginPDFContext (data, CGRect.Empty, null);
			//start a PDF page
			UIGraphics.BeginPDFPage ();       
			using (CGContext g = UIGraphics.GetCurrentContext ()) {
				g.ScaleCTM (1, -1);
				g.TranslateCTM (0, -25);      
				g.SelectFont ("Helvetica", 25, CGTextEncoding.MacRoman);
				g.ShowText ("Hello Evolve");
			}
			//complete a PDF page
			UIGraphics.EndPDFContent ();
		}
	}
}

