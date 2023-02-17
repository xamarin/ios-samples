using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace ZoomingPdfViewer {
	/// <summary>
	/// UIScrollView subclass that handles the user input to zoom the PDF page.  This class handles swapping the TiledPDFViews when the zoom level changes.
	/// </summary>
	[Register ("PdfScrollView")]
	public class PdfScrollView : UIScrollView, IUIScrollViewDelegate {
		// The old TiledPDFView that we draw on top of when the zooming stops.
		private TiledPdfView oldPdfView;

		// a reference to the page being drawn
		private CGPDFPage page;

		// Frame of the PDF
		private CGRect pageRect;

		public PdfScrollView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		public PdfScrollView (CGRect frame) : base (frame)
		{
			Initialize ();
		}

		/// <summary>
		/// Current PDF zoom scale.
		/// </summary>
		public nfloat PdfScale { get; set; }

		/// <summary>
		/// The TiledPDFView that is currently front most.
		/// </summary>
		public TiledPdfView TiledPdfView { get; private set; }

		private void Initialize ()
		{
			DecelerationRate = UIScrollView.DecelerationRateFast;
			Delegate = this;

			Layer.BorderColor = UIColor.LightGray.CGColor;
			Layer.BorderWidth = 5f;

			MaximumZoomScale = 5f;
			MinimumZoomScale = 0.25f;

			oldPdfView = new TiledPdfView (pageRect, PdfScale);
		}

		public void SetPDFPage (CGPDFPage pdfPage)
		{
			page = pdfPage;

			// PDFPage is null if we're requested to draw a padded blank page by the parent UIPageViewController
			if (page == null) {
				pageRect = Bounds;
			} else {
				pageRect = page.GetBoxRect (CGPDFBox.Media);

				PdfScale = Frame.Size.Width / pageRect.Size.Width;
				pageRect = new CGRect (pageRect.X, pageRect.Y, pageRect.Width * PdfScale, pageRect.Height * PdfScale);
			}

			// Create the TiledPDFView based on the size of the PDF page and scale it to fit the view.
			ReplaceTiledPdfViewWithFrame (pageRect);
		}

		public override void LayoutSubviews ()
		{
			// Use layoutSubviews to center the PDF page in the view.
			base.LayoutSubviews ();

			// Center the image as it becomes smaller than the size of the screen.
			var boundsSize = Bounds.Size;
			var frameToCenter = TiledPdfView.Frame;

			// Center horizontally.
			frameToCenter.X = frameToCenter.Width < boundsSize.Width ? (boundsSize.Width - frameToCenter.Width) / 2f : 0f;
			frameToCenter.Y = frameToCenter.Height < boundsSize.Height ? (boundsSize.Height - frameToCenter.Height) / 2f : 0f;

			TiledPdfView.Frame = frameToCenter;

			/*
             * To handle the interaction between CATiledLayer and high resolution screens, set the tiling view's contentScaleFactor to 1.0.
             * If this step were omitted, the content scale factor would be 2.0 on high resolution screens, which would cause the CATiledLayer to ask for tiles of the wrong scale.
             */
			TiledPdfView.ContentScaleFactor = 1f;
		}

		[Export ("viewForZoomingInScrollView:")]
		public new UIView ViewForZoomingInScrollView (UIScrollView scrollView)
		{
			return TiledPdfView;
		}

		[Export ("scrollViewWillBeginZooming:withView:")]
		public new void ZoomingStarted (UIScrollView scrollView, UIView view)
		{
			// Remove back tiled view.
			oldPdfView.RemoveFromSuperview ();
			oldPdfView.Dispose ();

			// Set the current TiledPDFView to be the old view.
			oldPdfView = TiledPdfView;
		}

		/// <summary>
		/// A UIScrollView delegate callback, called when the user begins zooming.
		/// When the user begins zooming, remove the old TiledPDFView and set the current TiledPDFView to be the old view so we can create a new TiledPDFView when the zooming ends.
		/// </summary>
		[Export ("scrollViewDidEndZooming:withView:atScale:")]
		public new void ZoomingEnded (UIScrollView scrollView, UIView withView, nfloat atScale)
		{
			// Set the new scale factor for the TiledPDFView.
			PdfScale *= atScale;
			ReplaceTiledPdfViewWithFrame (oldPdfView.Frame);
		}

		private void ReplaceTiledPdfViewWithFrame (CGRect frame)
		{
			// Create a new tiled PDF View at the new scale
			var tiledPDFView = new TiledPdfView (frame, PdfScale) { PdfPage = page };

			// Add the new TiledPDFView to the PDFScrollView.
			AddSubview (tiledPDFView);
			TiledPdfView = tiledPDFView;
		}
	}
}
