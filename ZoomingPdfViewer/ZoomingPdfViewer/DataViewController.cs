using CoreGraphics;
using System;
using UIKit;

namespace ZoomingPdfViewer {
	public partial class DataViewController : UIViewController {
		private CGPDFPage page;

		private nfloat myScale;

		public DataViewController (IntPtr handle) : base (handle) { }

		public CGPDFDocument Pdf { get; set; }

		public nint PageNumber { get; set; }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Do any additional setup after loading the view, typically from a nib.
			page = Pdf.GetPage (PageNumber);
			ScrollView.SetPDFPage (page);

			// Disable zooming if our pages are currently shown in landscape, for new views
			ScrollView.UserInteractionEnabled = UIApplication.SharedApplication.StatusBarOrientation.IsPortrait ();
		}

		public override void ViewDidLayoutSubviews ()
		{
			RestoreScale ();
		}

		public override void ViewWillTransitionToSize (CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
		{
			base.ViewWillTransitionToSize (toSize, coordinator);
			coordinator.AnimateAlongsideTransition ((param) => { }, (context) => {
				// Disable zooming if our pages are currently shown in landscape after orientation changes
				ScrollView.UserInteractionEnabled = UIApplication.SharedApplication.StatusBarOrientation.IsPortrait ();
			});
		}

		private void RestoreScale ()
		{
			// Called on orientation change.
			// We need to zoom out and basically reset the scrollview to look right in two-page spline view.
			var pageRect = page.GetBoxRect (CGPDFBox.Media);
			var yScale = View.Frame.Height / pageRect.Height;
			var xScale = View.Frame.Width / pageRect.Width;
			myScale = NMath.Min (xScale, yScale);

			ScrollView.Bounds = View.Bounds;
			ScrollView.ZoomScale = 1f;
			ScrollView.PdfScale = myScale;

			ScrollView.TiledPdfView.Bounds = View.Bounds;
			ScrollView.TiledPdfView.MyScale = myScale;
			ScrollView.TiledPdfView.Layer.SetNeedsDisplay ();
		}
	}
}
