using System;
using UIKit;
using Foundation;
using CoreGraphics;

using System.IO;
using QuartzSample;

public class ImageDrawingView : QuartzView {
	UIImage uiimage;
	CGImage image;
	
	public ImageDrawingView () : base () {
		uiimage = UIImage.FromFile ("Images/Demo.png");
		image = uiimage.CGImage;
	}
	
	public override void DrawInContext (CGContext context)
	{
		var imageRect = new CGRect (8, 8, 64, 64);
		
		// Note: The images are actually drawn upside down because Quartz image drawing expects
		// the coordinate system to have the origin in the lower-left corner, but a UIView
		// puts the origin in the upper-left corner. For the sake of brevity (and because
		// it likely would go unnoticed for the image used) this is not addressed here.
		// For the demonstration of PDF drawing however, it is addressed, as it would definately
		// be noticed, and one method of addressing it is shown there.
	
		// Draw the image in the upper left corner (0,0) with size 64x64
		context.DrawImage (imageRect, image);
		
		// Tile the same image across the bottom of the view
		// CGContextDrawTiledImage() will fill the entire clipping area with the image, so to avoid
		// filling the entire view, we'll clip the view to the rect below. This rect extends
		// past the region of the view, but since the view's rectangle has already been applied as a clip
		// to our drawing area, it will be intersected with this rect to form the final clipping area
		context.ClipToRect(new CGRect (0, 80, Bounds.Width, Bounds.Height));
		
		// The origin of the image rect works similarly to the phase parameter for SetLineDash and
		// SetPatternPhase and specifies where in the coordinate system the "first" image is drawn.
		// The size (previously set to 64x64) specifies the size the image is scaled to before being tiled.
		imageRect.X = 32;
		imageRect.Y = 112;
		context.DrawTiledImage (imageRect, image);
		
		// Highlight the "first" image from the DrawTiledImage call.
		context.SetFillColor (1, 0, 0, 0.5f);
		context.FillRect (imageRect);
		
		// And stroke the clipped area
		context.SetLineWidth (3);
		context.SetStrokeColor (1, 0, 0, 1);
		context.StrokeRect (context.GetClipBoundingBox ());
	}
}

public class PDFDrawingView : QuartzView {
	CGPDFDocument doc;
	
	public PDFDrawingView () : base () {
		doc = CGPDFDocument.FromFile (Path.Combine (NSBundle.MainBundle.BundlePath, "Images/QuartzImageDrawing.pdf"));
		if (doc == null)
			throw new Exception ("Could not load document");
	}
	
	protected override void Dispose (bool disposing)
	{
		doc.Dispose ();
		
		base.Dispose (disposing);
	}
	
	public override void DrawInContext (CGContext context)
	{
		// PDF page drawing expects a Lower-Left coordinate system, so we flip the coordinate system
		// before we start drawing.
		context.TranslateCTM (0, Bounds.Height);
		context.ScaleCTM (1, -1);
		
		// Grab the first PDF page
		using (CGPDFPage page = doc.GetPage (1)){
			// We're about to modify the context CTM to draw the PDF page where we want it, so save the graphics state in case we want to do more drawing
			context.SaveState ();
			
			// CGPDFPageGetDrawingTransform provides an easy way to get the transform for a PDF page. It will scale down to fit, including any
			// base rotations necessary to display the PDF page correctly.
			
			CGAffineTransform pdfTransform = page.GetDrawingTransform (CGPDFBox.Crop, Bounds, 0, true);

			// And apply the transform.
			context.ConcatCTM (pdfTransform);
			// Finally, we draw the page and restore the graphics state for further manipulations!
			context.DrawPDFPage (page);
			context.RestoreState();
		}
	}	
}


