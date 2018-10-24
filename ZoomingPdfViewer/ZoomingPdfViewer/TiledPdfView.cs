using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using CoreAnimation;
using UIKit;

namespace ZoomingPdfViewer
{
    /// <summary>
    /// This view is backed by a CATiledLayer into which the PDF page is rendered into.
    /// </summary>
    public class TiledPdfView : UIView//, ICALayerDelegate
    {
        //private readonly CATiledLayer tiledLayer;

        public nfloat MyScale { get; set; }

        public CGPDFPage Page { get; set; }

        public TiledPdfView(CGRect frame, nfloat scale) : base(frame)
        {
            //var tiledLayer = CATiledLayer.Create();
            var tiledLayer = Layer as CATiledLayer;

            // levelsOfDetail and levelsOfDetailBias determine how the layer is rendered at different zoom levels.
            // This only matters while the view is zooming, because once the the view is done zooming a new TiledPDFView is created at the correct size and scale.
            tiledLayer.LevelsOfDetail = 4;
            tiledLayer.LevelsOfDetailBias = 3;
            tiledLayer.TileSize = new CGSize(512f, 512f);
            MyScale = scale;
            tiledLayer.BackgroundColor = UIColor.LightGray.CGColor;
            tiledLayer.BorderWidth = 5;
        }

        [Export("layerClass")]
        public static Class LayerClass()
        {
            // instruct that we want a CATileLayer (not the default CALayer) for the Layer property
            return new Class(typeof(CATiledLayer));
        }

        public override void DrawLayer(CALayer layer, CGContext context)
        {
            // Fill the background with white.
            context.SetFillColor(1f, 1f, 1f, 1f);
            context.FillRect(Bounds);

            // Print a blank page and return if our page is nil.
            if (Page != null)
            {
                context.SaveState();

                // Flip page so we render it as it's meant to be read
                context.TranslateCTM(0f, Bounds.Height);
                context.ScaleCTM(1f, -1f);

                // Scale page at the view-zoom level
                context.ScaleCTM(MyScale, MyScale);

                // draw the page, restore and exit
                context.DrawPDFPage(Page);
                context.RestoreState();
            }
        }

        protected override void Dispose(bool disposing)
        {
            Cleanup();
            base.Dispose(disposing);
        }

        void Cleanup()
        {
            //InvokeOnMainThread(() => {
            //    tiledLayer.Delegate = null;
            //    RemoveFromSuperview();
            //    tiledLayer.RemoveFromSuperLayer();
            //});
        }
    }
}
