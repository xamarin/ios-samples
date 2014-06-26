using System;
using CoreGraphics;

using CoreLocation;
using UIKit;

namespace NSZombieApocalypse
{
	public enum ButtonType
	{
		Free,
		DeAlloc,
		Release,
		AutoRelease,
		GC,
		ARC,
		Count
	}

	public class ButtonCollectionView : UIView
	{

		UIImageView trackingImageView;

		public event ButtonSelectedHandler ButtonSelectedEvent;
		public event ButtonDraggedHandler  ButtonDraggedEvent;
		public event ButtonFinishedHandler ButtonFinishedEvent;

		public ButtonCollectionView () : base()
		{
		
		}

		public ButtonCollectionView (CGRect frame) : base(frame)
		{
			Layer.BorderColor = UIColor.Black.CGColor;
			Layer.BorderWidth = 1;
			Layer.CornerRadius = 8;
			BackgroundColor = UIColor.White.ColorWithAlpha (0.75f);

			for (int k = 0; k < (int) ButtonType.Count; k++) {
				var button = new ButtonView (CGRect.Empty);
				AddSubview (button);			
				button.TrackingStartedEvent += TrackingStarted;
				button.TrackingContinuedEvent += TrackingContinued;
				button.TrackingEndedEvent += TrackingEnded;
				button.Tag = k;
				button.SetLabel (ButtonLabelForType ((ButtonType)k));
						
			}

		}

		public String ButtonLabelForType (ButtonType type)
		{
			switch (type) {
			case ButtonType.Free:
				return "Free()";

			case ButtonType.Release:
				return "[Release]";

			case ButtonType.AutoRelease:
				return "[AutoRelease]";

			case ButtonType.DeAlloc:
				return "[DeAlloc]";

			case ButtonType.GC:
				return "Garbage Collection";

			case ButtonType.ARC:
				return "ARC!";
			default:
				break;

			}
			return null;
		}

		public  void TrackingStarted (ButtonView button)
		{
			UIGraphics.BeginImageContext (button.Bounds.Size);
			button.Layer.RenderInContext (UIGraphics.GetCurrentContext ());
			UIImage image = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			if (trackingImageView == null) {
				trackingImageView = new UIImageView (CGRect.Empty);
				Superview.AddSubview (trackingImageView);
				trackingImageView.Alpha = 0.5f;
			}

			trackingImageView.Image = image;
			trackingImageView.SizeToFit ();
			CGRect frame = trackingImageView.Frame;
			var newFrame = new CGRect (Superview.ConvertPointFromView (button.Frame.Location, this), frame.Size);
			trackingImageView.Frame = newFrame;
			if (ButtonSelectedEvent != null)
				ButtonSelectedEvent (button);
		}

		public void TrackingContinued (ButtonView button, UITouch location)
		{
			CGPoint point = location.LocationInView (Superview);
			CGRect frame = trackingImageView.Frame;
			var newPoint = new CGPoint (point.X - button.Frame.Size.Width / 2, point.Y - button.Frame.Size.Height / 2);
			var newFrame = new CGRect (newPoint, frame.Size);
			trackingImageView.Frame = newFrame;
			if (ButtonDraggedEvent != null)
				ButtonDraggedEvent (button, location);
		
		}

		public void  TrackingEnded (ButtonView button, UITouch location)
		{
			if (ButtonFinishedEvent != null)
				ButtonFinishedEvent (button, trackingImageView, location);
			trackingImageView = null;
		}

		public override void LayoutSubviews ()
		{

			UIView[] subviews = this.Subviews;
			int count = 0;
			CGRect bounds = Bounds;
			CGSize buttonSize = ButtonView.ButtonSize;
			nfloat xPad = (bounds.Size.Width - (buttonSize.Width * 3)) / 4;
			nfloat yPad = (bounds.Size.Height - (buttonSize.Height * 2)) / 3;
			nfloat x = xPad, y = 5;
			foreach (var subview in subviews) {
				if (count > 0 && count % 3 == 0) {
					x = xPad;
					y += buttonSize.Height + yPad;
				}
				count++;
			
				var frame = new CGRect (x, y, buttonSize.Width, buttonSize.Height);
				subview.Frame = frame.Integral ();
				x += buttonSize.Width + xPad;

			}

		}
	}
	public delegate void ButtonSelectedHandler (ButtonView button);
	public delegate void ButtonDraggedHandler (ButtonView button, UITouch location);
	public delegate void ButtonFinishedHandler (ButtonView button, UIView trackingview, UITouch location);
}

