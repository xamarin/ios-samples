using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;
using CoreGraphics;

namespace DigitDetection
{
	// 2 points can give a line and this class is just for that purpose, it keeps a record of a lin
	public class Line
	{
		public CGPoint Start { get; }
		public CGPoint End { get; }

		public Line (CGPoint start, CGPoint end)
		{
			Start = start;
			End = end;
		}
	}

	[Register ("DrawView")]
	public class DrawView : UIView
	{
		// some parameters of how thick a line to draw 15 seems to work 
		// and we have white drawings on black background just like MNIST needs its input
		float lineWidth = 15f;
		float LineWidth {
			get {
				return lineWidth;
			}
			set {
				lineWidth = value;
				SetNeedsDisplay ();
			}
		}

		UIColor color = UIColor.White;
		UIColor Color {
			get {
				return color;
			}
			set {
				color = value;
				SetNeedsDisplay ();
			}
		}

		// we will keep touches made by user in view in these as a record so we can draw them
		public List<Line> Lines { get; } = new List<Line> ();
		CGPoint lastPoint;

		public DrawView (IntPtr handle)
			: base (handle)
		{
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			lastPoint = ((UITouch)touches.First ()).LocationInView (this);
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			var newPoint = ((UITouch)touches.First ()).LocationInView (this);
			// keep all lines drawn by user as touch in record so we can draw them in view
			Lines.Add (new Line (lastPoint, newPoint));

			lastPoint = newPoint;

			// make a draw call
			SetNeedsDisplay ();
		}

		public override void Draw (CGRect rect)
		{
			base.Draw (rect);

			var drawPath = new UIBezierPath ();

			drawPath.LineCapStyle = CGLineCap.Round;

			foreach (var line in Lines) {
				drawPath.MoveTo (line.Start);
				drawPath.AddLineTo (line.End);
			}

			drawPath.LineWidth = LineWidth;
			// TODO: request Set method (defined in Swift Headers
			Color.SetFill ();
			Color.SetStroke ();
		}

		public CGBitmapContext GetViewContext ()
		{
			// our network takes in only grayscale images as input
			var colorSpace = CGColorSpace.CreateDeviceGray ();
			// we have 3 channels no alpha value put in the network
			var bitmapInfo = CGImageAlphaInfo.None;

			// this is where our view pixel data will go in once we make the render call
			var context = new CGBitmapContext (null, 28, 28, 8, 28, colorSpace, bitmapInfo);

			// scale and translate so we have the full digit and in MNIST standard size 28x28
			context.TranslateCTM (0, 28);
			context.ScaleCTM (28 / Frame.Size.Width, -28 / Frame.Size.Height);

			// put view pixel data in context
			Layer.RenderInContext (context);

			return context;
		}
	}
}