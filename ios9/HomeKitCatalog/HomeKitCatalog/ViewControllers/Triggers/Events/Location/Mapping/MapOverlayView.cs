using System;

using CoreGraphics;
using Foundation;
using UIKit;

namespace HomeKitCatalog
{
	// A simple `UIView` subclass to draw a selection circle over a MKMapView of the same size.
	public partial class MapOverlayView : UIView
	{
		#region ctors

		public MapOverlayView ()
		{
		}

		public MapOverlayView (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithFrame:")]
		public MapOverlayView (CGRect frame)
			: base (frame)
		{
		}

		[Export ("initWithCoder:")]
		public MapOverlayView (NSCoder coder)
			: base (coder)
		{
		}

		#endregion

		// Draws a dashed circle in the center of the `rect` with a radius 1/4th of the `rect`'s smallest side.
		public override void Draw (CGRect rect)
		{
			base.Draw (rect);

			var context = UIGraphics.GetCurrentContext ();

			var strokeColor = UIColor.Blue;

			nfloat circleDiameter = NMath.Min (rect.Width, rect.Height) / 2;
			var circleRadius = circleDiameter / 2;
			var cirlceRect = new CGRect (rect.GetMidX () - circleRadius, rect.GetMidY () - circleRadius, circleDiameter, circleDiameter);
			var circlePath = UIBezierPath.FromOval (cirlceRect);

			strokeColor.SetStroke ();
			circlePath.LineWidth = 3;
			context.SaveState ();
			context.SetLineDash (0, new nfloat[] { 6, 6 }, 2);
			circlePath.Stroke ();
			context.RestoreState ();
		}

		// returns:  `false` to accept no touches.
		public override bool PointInside (CGPoint point, UIEvent uievent)
		{
			return false;
		}
	}
}
