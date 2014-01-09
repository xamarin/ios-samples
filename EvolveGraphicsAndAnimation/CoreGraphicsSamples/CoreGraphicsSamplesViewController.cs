using MonoTouch.UIKit;
using System.Drawing;
using System;
using MonoTouch.Foundation;

namespace CoreGraphicsSamples
{
	public partial class CoreGraphicsSamplesViewController : UIViewController
	{
		TriangleView triangleView;
//		DrawnImageView drawnImageView;
//		PDFView pdfView;
        
		public CoreGraphicsSamplesViewController ()
		{
		}
        
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			triangleView = new TriangleView (){Frame = UIScreen.MainScreen.Bounds};
			View.AddSubview (triangleView);

//			drawnImageView = new DrawnImageView (){Frame = UIScreen.MainScreen.Bounds};
//			View.AddSubview(drawnImageView);

//			pdfView = new PDFView (){Frame = UIScreen.MainScreen.Bounds};
//			View.AddSubview(pdfView);
		}
	}
}
