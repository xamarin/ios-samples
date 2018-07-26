using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using UIKit;

namespace ObjectTracker
{
	public class Overlay : UIView
	{
		List<CGPoint[]> quadrilaterals = new List<CGPoint[]>();

		public override void Draw(CGRect rect)
		{
			base.Draw(rect);

			using (var ctxt = UIGraphics.GetCurrentContext())
			{
				ctxt.SetStrokeColor(StrokeColor);
				using (var path = new CGPath())
				{
					var imageSize = this.Bounds.Size;
					lock (quadrilaterals)
					{
						var scaled = quadrilaterals.Select(q => q.Select(pt => pt.Scaled(imageSize)));
						foreach (var q in scaled)
						{
							path.AddLines(q.ToArray());
							path.CloseSubpath();
						}
					}
					ctxt.AddPath(path);
					ctxt.StrokePath();

					// Draw text
				}
				ctxt.SaveState();
				ctxt.ScaleCTM(1, -1);
				ctxt.SetFillColor(UIColor.Red.CGColor);
				ctxt.SetTextDrawingMode(CGTextDrawingMode.Fill);
				ctxt.SelectFont("Helvetica", 32, CGTextEncoding.MacRoman);

				//Draw the text at given coords.
				ctxt.ShowTextAtPoint(25, -25, Message);
				ctxt.RestoreState();
			}

		}

		private string message = "";

		public string Message
		{
			get => message;
			set
			{
				message = value;
				SetNeedsDisplay();
			}
		}

		public void Clear()
		{
			lock (quadrilaterals)
			{
				quadrilaterals.Clear();
			}
		}

		public void AddQuad(CGPoint[] quad)
		{
			lock (quadrilaterals)
			{
				quadrilaterals.Add(quad);
			}
		}

		public CGColor StrokeColor { get; internal set; }
	}
}
