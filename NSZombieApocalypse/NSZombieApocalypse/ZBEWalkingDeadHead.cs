using System;
using MonoTouch.CoreGraphics;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace NSZombieApocalypse
{
	public class ZBEWalkingDeadHead : ZBEBodyPart
	{
		public ZBEWalkingDeadHead ()
		{
		}

		public override void Draw (RectangleF rect)
		{
			rect = RectangleF.Inflate (rect, 4, 4);
			rect = new RectangleF ((rect.Size.Width - rect.Size.Height) / 2 + 4, 8, rect.Size.Height, rect.Size.Height);
			UIBezierPath path = UIBezierPath.FromOval (rect);
			UIColor.Black.SetStroke ();
			UIColor.White.SetFill ();
			path.LineWidth = 2;
			path.Fill ();
			path.Stroke ();

			UIBezierPath rightEye, leftEye, mouth = new UIBezierPath ();
			if (MovingRight) {
				rightEye = UIBezierPath.FromArc (new PointF (rect.GetMidX () - 5, rect.Y + 15), 4, 0, 180, true);
				leftEye = UIBezierPath.FromArc (new PointF (rect.GetMidX () + 10, rect.Y + 15), 4, 0, 180, true);
				mouth.MoveTo (new PointF (rect.GetMidX (), rect.Y + 30));
				mouth.AddLineTo (new PointF (rect.GetMidX () + 13, rect.Y + 30));
			} else {

				rightEye = UIBezierPath.FromArc (new PointF (rect.GetMidX () - 10, rect.Y + 15), 4, 0, 180, true);
				leftEye = UIBezierPath.FromArc (new PointF (rect.GetMidX () - 10, rect.Y + 15), 4, 0, 180, true);
				mouth.MoveTo (new PointF (rect.GetMidX (), rect.Y + 30));
				mouth.MoveTo (new PointF (rect.GetMidX (), rect.Y + 30));

			}
			rightEye.LineWidth = 2;
			rightEye.Stroke ();

			leftEye.LineWidth = 2;
			leftEye.Stroke ();

			mouth.LineWidth = 2;
			mouth.Stroke ();

		}

	}
}

