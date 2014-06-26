using System;
using CoreGraphics;

using UIKit;

namespace NSZombieApocalypse
{
	public class ButtonView: UIControl
	{
		UILabel labelView;
		UIImageView imageView;

		public event TrackingStartedEventHandler TrackingStartedEvent;
		public event TrackingContinuedEventHandler TrackingContinuedEvent;
		public event TrackingEndedEventHandler TrackingEndedEvent;

		public ButtonView (CGRect frame):base (frame)
		{
			imageView = new UIImageView (UIImage.FromBundle ("buttonView.png"));
			AddSubview (imageView);
			Layer.BorderWidth = 1;
			Layer.BorderColor = UIColor.Black.CGColor;
			Layer.CornerRadius = 8;
			MultipleTouchEnabled = true;
		}

		public static CGSize ButtonSize {
			get {
				CGSize size = UIImage.FromBundle ("buttonView.png").Size;
				size.Height += 20;
				size.Width += 60;
				return size;
			}
		}

		public override bool BeginTracking (UITouch touch, UIEvent uievent)
		{
			if (TrackingStartedEvent != null)
				TrackingStartedEvent (this);
			return true;
		}

		public override bool ContinueTracking (UITouch touch, UIEvent uievent)
		{
			if (TrackingContinuedEvent != null)
				TrackingContinuedEvent (this, touch);
			return true;
		}

		public override void EndTracking (UITouch touch, UIEvent uievent)
		{
			if (TrackingEndedEvent != null)
				TrackingEndedEvent (this, touch);
		}

		public override void CancelTracking (UIEvent uievent)
		{
			if (TrackingEndedEvent != null)
				TrackingEndedEvent (this, null);
		}

		public override bool IsAccessibilityElement {
			get {
				return true;
			}
			set {
				value = true;
				base.IsAccessibilityElement = value;
			}
		}

		public override string AccessibilityLabel {
			get {
				return labelView.Text;
			}
			set {
				value = labelView.Text;
				base.AccessibilityLabel = value;
			}
		}

		public override string AccessibilityHint {
			get {
				return "Drag live saving memory technique to the the zombies to decrease memory pressure.";
			}
			set {
				value = "Drag live saving memory technique to the the zombies to decrease memory pressure.";
				base.AccessibilityHint = value;

			}
		}

		public override UIAccessibilityTrait AccessibilityTraits {
			get {
				return UIAccessibilityTrait.Button;
			}
			set {
				base.AccessibilityTraits = value;
			}
		}

		public override void LayoutSubviews ()
		{
			CGRect frame = Bounds;
			var newFrame = new CGRect (Bounds.X, frame.Size.Height - 20, Bounds.Width, 20);
			labelView.Frame = newFrame.Integral ();
			CGRect imageFrame = imageView.Frame;
			imageFrame.X = (newFrame.Size.Width - imageFrame.Size.Width) / 2;
			imageView.Frame = imageFrame.Integral ();
		}

		public void SetLabel (String labelString)
		{
			if (labelView == null) {

				labelView = new UILabel (CGRect.Empty);
				labelView.BackgroundColor = UIColor.Clear;
				labelView.Font = UIFont.FromName ("HelveticaNeue", 18);
				labelView.TextColor = UIColor.Black;
				labelView.TextAlignment = UITextAlignment.Center;
				labelView.ClipsToBounds = false;
				AddSubview (labelView);
			}
			labelView.Text = labelString;
		}
	}
	public delegate void TrackingStartedEventHandler (ButtonView button);
	public delegate void TrackingContinuedEventHandler (ButtonView button, UITouch location);
	public delegate void TrackingEndedEventHandler (ButtonView button, UITouch location);
}

