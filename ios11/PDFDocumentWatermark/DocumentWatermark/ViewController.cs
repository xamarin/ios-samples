using System;
using Foundation;
using UIKit;
using PdfKit;

namespace DocumentWatermark {
	public partial class ViewController : UIViewController, IPdfDocumentDelegate {
		#region Constructors
		protected ViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var documentURL = NSBundle.MainBundle.GetUrlForResource ("Sample", "pdf");
			if (documentURL != null) {
				var document = new PdfDocument (documentURL);
				if (document != null) {
					// Center document on gray background
					PDFView.AutoScales = true;
					PDFView.BackgroundColor = UIColor.LightGray;

					// Set delegate
					document.Delegate = this;
					PDFView.Document = document;
				}
			}
		}
		#endregion

		#region PdfDocumentDelegate
		[Export ("classForPage")]
		public ObjCRuntime.Class GetClassForPage ()
		{
			return new ObjCRuntime.Class (typeof (WatermarkPage));
		}
		#endregion
	}
}
