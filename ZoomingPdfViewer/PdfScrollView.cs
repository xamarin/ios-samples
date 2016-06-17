using System;

using UIKit;
using CoreGraphics;
using Foundation;

namespace ZoomingPdfViewer {
	[Register ("PdfScrollView")]
	public class PdfScrollView : UIScrollView, IUIScrollViewDelegate {
		TiledPdfView oldPdfView;
		CGPDFPage page;
		CGRect pageRect;

		public nfloat PdfScale { get; set; }

		public TiledPdfView TiledPDFView { get; private set;}

		public PdfScrollView (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		public PdfScrollView (CGRect frame) : base (frame)
		{
			Initialize ();
		}

		void Initialize ()
		{
			DecelerationRate = DecelerationRateFast;

			Delegate = this;
			BackgroundColor = UIColor.LightGray;

			Layer.BorderWidth = 5f;
			MaximumZoomScale = 5f;
			MinimumZoomScale = .25f;
		}

		public void SetPDFPage (CGPDFPage pdfPage)
		{
			page = pdfPage;

			if (page == null) {
				pageRect = Bounds;
			} else {
				pageRect = page.GetBoxRect (CGPDFBox.Media);
				PdfScale = Frame.Size.Width / pageRect.Size.Width;
				pageRect = new CGRect (pageRect.X, pageRect.Y, pageRect.Width * PdfScale, pageRect.Height * PdfScale);
			}

			ReplaceTiledPDFView (pageRect);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			// if the page becomes smaller than the view's bounds then we
			// center it in the screen
			CGSize boundsSize = Bounds.Size;
			CGRect frameToCenter = TiledPDFView.Frame;

			frameToCenter.X = (frameToCenter.Width < boundsSize.Width) ?
				(boundsSize.Width - frameToCenter.Width) / 2f : 0f;
			frameToCenter.Y = (frameToCenter.Height < boundsSize.Height) ?
				(boundsSize.Height - frameToCenter.Height) / 2f : 0f;

			// adjust the pdf and the bitmap views to the new, centered, frame
			TiledPDFView.Frame = frameToCenter;

			// this is important wrt high resolution screen to ensure
			// CATiledLayer will ask proper tile sizes
			TiledPDFView.ContentScaleFactor = 1f;
		}

		[Export ("viewForZoomingInScrollView:")]
		public new UIView ViewForZoomingInScrollView (UIScrollView scrollView)
		{
			return TiledPDFView;
		}

		[Export ("scrollViewWillBeginZooming:withView:")]
		public new void ZoomingStarted (UIScrollView scrollView, UIView view)
		{
			// Remove back tiled view.
			oldPdfView?.RemoveFromSuperview ();
			oldPdfView = TiledPDFView;
		}

		[Export ("scrollViewDidEndZooming:withView:atScale:")]
		public new void ZoomingEnded (UIScrollView scrollView, UIView withView, nfloat atScale)
		{
			PdfScale *= atScale;
			ReplaceTiledPDFView (oldPdfView.Frame);
		}

		void ReplaceTiledPDFView (CGRect rect)
		{
			// Create a new tiled PDF View at the new scale
			var tiledPDFView = new TiledPdfView (Frame, PdfScale);
			tiledPDFView.Page = page;

			// Add the new TiledPDFView to the PDFScrollView.
			AddSubview (tiledPDFView);
			TiledPDFView = tiledPDFView;
		}
	}
}
