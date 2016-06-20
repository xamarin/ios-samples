using System;

using CoreGraphics;
using UIKit;

namespace ZoomingPdfViewer {
	public partial class DataViewController : UIViewController {
		CGPDFPage page;
		nfloat myScale;

		public CGPDFDocument Pdf { get; set; }

		public nint PageNumber { get; set; }

		public DataViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			page = Pdf.GetPage (PageNumber);
			ScrollView.SetPDFPage (page);
		}

		public override void ViewWillAppear (bool animated)
		{
			// Disable zooming if our pages are currently shown in landscape
			ScrollView.UserInteractionEnabled = InterfaceOrientation == UIInterfaceOrientation.Portrait ||
				InterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown;
		}

		public override void ViewDidLayoutSubviews ()
		{
			RestoreScale ();
		}

		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			ScrollView.UserInteractionEnabled = InterfaceOrientation != UIInterfaceOrientation.Portrait ||
				InterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown;
		}

		void RestoreScale ()
		{
			CGRect pageRect = page.GetBoxRect (CGPDFBox.Media);
			nfloat yScale = View.Frame.Height / pageRect.Height;
			nfloat xScale = View.Frame.Width / pageRect.Width;
			myScale = NMath.Min (xScale, yScale);

			ScrollView.Bounds = View.Bounds;
			ScrollView.ZoomScale = 1f;
			ScrollView.PdfScale = myScale;

			ScrollView.TiledPDFView.Bounds = View.Bounds;
			ScrollView.TiledPDFView.MyScale = myScale;
			ScrollView.TiledPDFView.Layer.SetNeedsDisplay ();
		}
	}
}
