using System;
using CoreAnimation;
using Foundation;
using ObjCRuntime;
using UIKit;
using CoreGraphics;

namespace ZoomingPdfViewer {

	public class PdfScrollView : UIScrollView {

		// main, visible, pdf view
		TiledPdfView pdfView;
		// temporary, while zooming, view
		TiledPdfView oldPdfView;
		// low resolution bitmap using until 'pdfView' is ready
		UIImageView backgroundImageView;

		// global reference to the PDF document/page
		CGPDFDocument pdf;
		CGPDFPage page;

		nfloat scale;

		public PdfScrollView (CGRect frame)
			: base (frame)
		{
			ShowsVerticalScrollIndicator = false;
			ShowsHorizontalScrollIndicator = false;
			BouncesZoom = true;
			DecelerationRate = UIScrollView.DecelerationRateFast;
			BackgroundColor = UIColor.Gray;
			MaximumZoomScale = 5.0f;
			MinimumZoomScale = 0.25f;

			// open the PDF file (default directory is the bundle path)
			pdf = CGPDFDocument.FromFile ("Tamarin.pdf");
			// select the first page (the only one we'll use)
			page = pdf.GetPage (1);

			// make the initial view 'fit to width'
			CGRect pageRect = page.GetBoxRect (CGPDFBox.Media);
			scale = Frame.Width / pageRect.Width;
			pageRect.Size = new CGSize (pageRect.Width * scale, pageRect.Height * scale);

			// create bitmap version of the PDF page, to be used (scaled)
			// when no other (tiled) view are visible
			UIGraphics.BeginImageContext (pageRect.Size);
			CGContext context = UIGraphics.GetCurrentContext ();

			// fill with white background
			context.SetFillColor (1.0f, 1.0f, 1.0f, 1.0f);
			context.FillRect (pageRect);
			context.SaveState ();

			// flip page so we render it as it's meant to be read
			context.TranslateCTM (0.0f, pageRect.Height);
			context.ScaleCTM (1.0f, -1.0f);

			// scale page at the view-zoom level
			context.ScaleCTM (scale, scale);
			context.DrawPDFPage (page);
			context.RestoreState ();

			UIImage backgroundImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			backgroundImageView = new UIImageView (backgroundImage);
			backgroundImageView.Frame = pageRect;
			backgroundImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			AddSubview (backgroundImageView);
			SendSubviewToBack (backgroundImageView);

			// Create the TiledPDFView based on the size of the PDF page and scale it to fit the view.
			pdfView = new TiledPdfView (pageRect, (float)scale);
			pdfView.Page = page;
			AddSubview (pdfView);

			// no need to have (or set) a UIScrollViewDelegate with MonoTouch

			this.ViewForZoomingInScrollView = delegate {
				// return the view we'll be using while zooming
				return pdfView;
			};

			// when zooming starts we remove (from view) and dispose any
			// oldPdfView and set pdfView as our 'new' oldPdfView, it will
			// stay there until a new view is available (when zooming ends)
			this.ZoomingStarted += delegate {
				if (oldPdfView != null) {
					oldPdfView.RemoveFromSuperview ();
					oldPdfView.Dispose ();
				}
				oldPdfView = pdfView;
				AddSubview (oldPdfView);
			};

			// when zooming ends a new TiledPdfView is created (and shown)
			// based on the updated 'scale' and 'frame'
			ZoomingEnded += delegate (object sender, ZoomingEndedEventArgs e) {
				scale *= e.AtScale;

				CGRect rect = page.GetBoxRect (CGPDFBox.Media);
				rect.Size = new CGSize (rect.Width * scale, rect.Height * scale);

				pdfView = new TiledPdfView (rect, (float)scale);
				pdfView.Page = page;
				AddSubview (pdfView);
			};
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			// if the page becomes smaller than the view's bounds then we
			// center it in the screen
			CGSize boundsSize = Bounds.Size;
			CGRect frameToCenter = pdfView.Frame;

			if (frameToCenter.Width < boundsSize.Width)
				frameToCenter.X = (boundsSize.Width - frameToCenter.Width) / 2;
			else
				frameToCenter.X = 0;

			if (frameToCenter.Height < boundsSize.Height)
				frameToCenter.Y = (boundsSize.Height - frameToCenter.Height) / 2;
			else
				frameToCenter.Y = 0;

			// adjust the pdf and the bitmap views to the new, centered, frame
			pdfView.Frame = frameToCenter;
			backgroundImageView.Frame = frameToCenter;

			// this is important wrt high resolution screen to ensure
			// CATiledLayer will ask proper tile sizes
			pdfView.ContentScaleFactor = 1.0f;
		}
	}
}
