using System;
using UIKit;
using CoreGraphics;
using Foundation;

namespace ToastModern
{
	public class Layer : UIImageView
	{
		public Action<NSSet> OnTouchDown;
		public Action<NSSet> OnTouchMove;
		public Action<NSSet> OnTouchUp;

		public CGPoint Position {
			get {
				CGPoint center = Center;
				CGSize size = Size;
				return new CGPoint (center.X - size.Width * 0.5f, center.Y - size.Height * 0.5f);
			}

			set {
				CGSize size = Size;
				Center = new CGPoint (value.X + size.Width * 0.5f, value.Y + size.Height * 0.5f);
			}
		}

		public nfloat X {
			get {
				return Position.X;
			}

			set {
				Position = new CGPoint (value, Position.Y);
			}
		}

		public nfloat Y {
			get {
				return Position.Y;
			}

			set {
				Position = new CGPoint (Position.X, value);
			}
		}

		public CGSize Size {
			get {
				return Bounds.Size;
			}

			set {
				CGPoint position = Position;
				CGPoint origin = Bounds.Location;
				Bounds = new CGRect (origin.X, origin.Y, value.Width, value.Height);
				Position = position;
			}
		}

		public nfloat Width {
			get {
				return Bounds.Size.Width;
			}

			set {
				Size = new CGSize (value, Bounds.Size.Height);
			}
		}

		public nfloat Height {
			get {
				return Bounds.Size.Height;
			}

			set {
				Size = new CGSize (Bounds.Size.Width, value);
			}
		}

		public Layer (UIView parent) : base (CGRect.Empty)
		{
			parent.Add (this);
			// make all layers receive touches
			UserInteractionEnabled = true;
		}

		public void LoadImage (string filename)
		{
			Image = UIImage.FromFile (string.Format ("Images/{0}", filename));
			Size = new CGSize (Image.Size.Width * 0.5f, Image.Size.Height * 0.5f);
		}

		public static CGColor CreateColor (int red, int green, int blue, int alpha)
		{
			CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB ();
			var color = new CGColor (colorSpace, new nfloat[] { red / 255f, green / 255f, blue / 255f, alpha / 255f });
			colorSpace.Dispose ();
			return color;
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			if (OnTouchDown != null)
				OnTouchDown (touches);
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			if (OnTouchMove != null)
				OnTouchMove (touches);
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			if (OnTouchUp != null)
				OnTouchUp (touches);
		}

		public override void TouchesCancelled (NSSet touches, UIEvent evt)
		{
			if (OnTouchUp != null)
				OnTouchUp (touches);
		}
	}
}

