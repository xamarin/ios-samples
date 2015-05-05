using System;
using CoreAnimation;
using Foundation;
using ObjCRuntime;
using UIKit;
using CoreGraphics;

namespace ZoomingPdfViewer {

	public class TiledPdfView : UIView {

		public TiledPdfView (CGRect frame, float scale)
			: base (frame)
		{
			CATiledLayer tiledLayer = Layer as CATiledLayer;
			tiledLayer.LevelsOfDetail = 4;
			tiledLayer.LevelsOfDetailBias = 4;
			tiledLayer.TileSize = new CGSize (512, 512);
			// here we still need to implement the delegate
			tiledLayer.Delegate = new TiledLayerDelegate (this);
			Scale = scale;
		}

		public CGPDFPage Page { get; set; }

		public float Scale { get; set; }

		public override void Draw (CGRect rect)
		{
			// empty (on purpose so the delegate will draw)
		}

		[Export ("layerClass")]
		public static Class LayerClass ()
		{
			// instruct that we want a CATileLayer (not the default CALayer) for the Layer property
			return new Class (typeof (CATiledLayer));
		}
	}

	class TiledLayerDelegate : CALayerDelegate {

		TiledPdfView view;
		CGRect bounds;

		public TiledLayerDelegate (TiledPdfView view)
		{
			this.view = view;
			bounds = view.Bounds;
		}

		public override void DrawLayer (CALayer layer, CGContext context)
		{
			// fill with white background
			context.SetFillColor (1.0f, 1.0f, 1.0f, 1.0f);
			context.FillRect (bounds);
			context.SaveState ();

			// flip page so we render it as it's meant to be read
			context.TranslateCTM (0.0f, bounds.Height);
			context.ScaleCTM (1.0f, -1.0f);

			// scale page at the view-zoom level
			context.ScaleCTM (view.Scale, view.Scale);
			context.DrawPDFPage (view.Page);
			context.RestoreState ();
		}
	}
}
