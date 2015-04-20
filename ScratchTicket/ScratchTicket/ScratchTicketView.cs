using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ScratchTicket
{
	[Register("ScratchTicketView"), DesignTimeVisible(true)]
	public class ScratchTicketView : UIView
	{
		CGPath path;
		CGPoint initialPoint;
		CGPoint latestPoint;
		bool startNewPath = false;
		UIImage image;

		[Export("Image"), Browsable(true)]
		public UIImage Image
		{
			get { return image; }
			set
			{
				image = value;
				SetNeedsDisplay();
			}
		}

		public ScratchTicketView(IntPtr p)
			: base(p)
		{
			Initialize();
		}

		public ScratchTicketView()
		{
			Initialize();
		}

		void Initialize()
		{
			initialPoint = CGPoint.Empty;
			latestPoint = CGPoint.Empty;
			BackgroundColor = UIColor.Clear;
			Opaque = false;
			path = new CGPath();
			SetNeedsDisplay();
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);

			var touch = touches.AnyObject as UITouch;

			if (touch != null)
			{
				initialPoint = touch.LocationInView(this);
			}
		}

		public override void TouchesMoved(NSSet touches, UIEvent evt)
		{
			base.TouchesMoved(touches, evt);

			var touch = touches.AnyObject as UITouch;

			if (touch != null)
			{
				latestPoint = touch.LocationInView(this);
				SetNeedsDisplay();
			}
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);
			startNewPath = true;
		}

		public override void Draw(CGRect rect)
		{
			base.Draw(rect);

			using (var g = UIGraphics.GetCurrentContext())
			{
				if (image != null)
					g.SetFillColor((UIColor.FromPatternImage(image).CGColor));
				else
					g.SetFillColor(UIColor.LightGray.CGColor);
				g.FillRect(rect);

				if (!initialPoint.IsEmpty)
				{
					g.SetLineWidth(20);
					g.SetBlendMode(CGBlendMode.Clear);
					UIColor.Clear.SetColor();

					if (path.IsEmpty || startNewPath)
					{
						path.AddLines(new CGPoint[] { initialPoint, latestPoint });
						startNewPath = false;
					}
					else
					{
						path.AddLineToPoint(latestPoint);
					}

					g.SetLineCap(CGLineCap.Round);
					g.AddPath(path);		
					g.DrawPath(CGPathDrawingMode.Stroke);
				}
			}
		}
	}
}

