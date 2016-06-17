using System;

using CoreAnimation;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace ZoomingPdfViewer {
	public class TiledPdfView : UIView, ICALayerDelegate {
		readonly CATiledLayer tiledLayer;

		public nfloat MyScale { get; set; }

		public CGPDFPage Page { get; set; }

		public TiledPdfView (CGRect frame, nfloat scale)
			: base (frame)
		{
			tiledLayer = Layer as CATiledLayer;
			tiledLayer.LevelsOfDetail = 4;
			tiledLayer.LevelsOfDetailBias = 3;
			tiledLayer.TileSize = new CGSize (512f, 512f);
			MyScale = scale;
			tiledLayer.BackgroundColor = UIColor.LightGray.CGColor;
			tiledLayer.BorderWidth = 5;
		}

		[Export ("layerClass")]
		public static Class LayerClass ()
		{
			// instruct that we want a CATileLayer (not the default CALayer) for the Layer property
			return new Class (typeof (CATiledLayer));
		}

		[Export ("drawLayer:inContext:")]
		public void DrawLayer(CALayer layer, CGContext context)
		{
			// fill with white background
			context.SetFillColor (1f, 1f, 1f, 1f);
			context.FillRect (Bounds);
			context.SaveState ();

			// flip page so we render it as it's meant to be read
			context.TranslateCTM (0f, Bounds.Height);
			context.ScaleCTM (1f, -1f);

			// scale page at the view-zoom level
			context.ScaleCTM (MyScale, MyScale);
			context.DrawPDFPage (Page);
			context.RestoreState ();
		}
		
		protected override void Dispose (bool disposing)
		{
			Cleanup ();
			base.Dispose (disposing);
		}

		void Cleanup ()
		{
			InvokeOnMainThread (() => {
				tiledLayer.Delegate = null;
				RemoveFromSuperview ();
				tiledLayer.RemoveFromSuperLayer ();
			});
		}
	}
}
