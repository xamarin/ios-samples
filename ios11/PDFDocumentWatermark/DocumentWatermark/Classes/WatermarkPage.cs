using System;
using Foundation;
using UIKit;
using PdfKit;
using CoreGraphics;

namespace DocumentWatermark
{
	/**
	 WatermarkPage subclasses PDFPage so that it can override the draw(with box: to context:) method.
	 This method is called by PDFDocument to draw the page into a PDFView. All custom drawing for a PDF
	 page should be done through this mechanism.
	 
	 Custom drawing methods should always be thread-safe and call the super-class method. This is needed
	 to draw the original PDFPage content. Custom drawing code can execute before or after this super-class
	 call, though order matters! If your graphics run before the super-class call, they are drawn below the
	 PDFPage content. Conversely, if your graphics run after the super-class call, they are drawn above the
	 PDFPage.
	*/
	[Register("WatermarkPage")]
	public class WatermarkPage : PdfPage
	{

		#region Constructors
		protected WatermarkPage(IntPtr handle) : base(handle)
        {
			// Note: this .ctor should not contain any initialization logic.
		}
        #endregion

        public override void Draw(PdfDisplayBox box, CoreGraphics.CGContext context)
        {
			// Draw original content
			base.Draw(box, context);

            using (context)
            {
                // Draw watermark underlay
                UIGraphics.PushContext(context);
                context.SaveState();

                var pageBounds = this.GetBoundsForBox(box);
                context.TranslateCTM(0.0f, pageBounds.Size.Height);
                context.ScaleCTM(1.0f, -1.0f);
                context.RotateCTM((float)(Math.PI / 4.0f));


                Console.WriteLine($"{pageBounds}");

                var attributes = new UIStringAttributes()
                {
                    ForegroundColor = UIColor.FromRGBA(255, 0, 0, 125),
                    Font = UIFont.BoldSystemFontOfSize(84)
                };

                var text = new NSAttributedString("WATERMARK", attributes);

                text.DrawString(new CGPoint(250, 40));

                context.RestoreState();
                UIGraphics.PopContext();
            }
		}
	}
}
