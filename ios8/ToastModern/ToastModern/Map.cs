using System;
using UIKit;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using System.Timers;

namespace ToastModern
{
	public class Map : Layer
	{
		double lastTime;
		bool isInertiaing;
		CGPoint velocity;

		public Map (Layer parent) : base (parent)
		{
			var t = new Timer { Enabled = true, Interval = 10 };
			t.Elapsed += (sender, e) => Tick ();
			isInertiaing = false;
			velocity = CGPoint.Empty;
			lastTime = 0;
		}

		void Tick ()
		{
			if (isInertiaing) {
				UIView.PerformWithoutAnimation (() => {
					Y += velocity.Y / 60;
					X += velocity.X / 60;
					Clamp ();
				});

				velocity.X *= 0.9f;
				velocity.Y *= 0.9f;
			}
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			isInertiaing = false;
			velocity = CGPoint.Empty;
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			UITouch touch = (UITouch)touches.AnyObject;

			UIView.PerformWithoutAnimation (() => {
				X = X + touch.LocationInView (Screen.GlobalScreen).X - touch.PreviousLocationInView (Screen.GlobalScreen).X;
				Y = Y + touch.LocationInView (Screen.GlobalScreen).Y - touch.PreviousLocationInView (Screen.GlobalScreen).Y;
				Clamp ();
			});

			CGPoint newVelocity = CGPoint.Empty;
			newVelocity.X = touch.LocationInView (Screen.GlobalScreen).X - (nfloat)(touch.PreviousLocationInView (Screen.GlobalScreen).X / (touch.Timestamp - lastTime));
			newVelocity.Y = touch.LocationInView (Screen.GlobalScreen).Y - (nfloat)(touch.PreviousLocationInView (Screen.GlobalScreen).Y / (touch.Timestamp - lastTime));
			velocity.X = 0.25f * velocity.X + 0.75f * newVelocity.X;
			velocity.Y = 0.25f * velocity.Y + 0.75f * newVelocity.Y;

			lastTime = touch.Timestamp;
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			UITouch touch = (UITouch)touches.AnyObject;
			CGPoint newVelocity = CGPoint.Empty;

			newVelocity.X = touch.LocationInView (Screen.GlobalScreen).X - (nfloat)(touch.PreviousLocationInView (Screen.GlobalScreen).X / (touch.Timestamp - lastTime));
			newVelocity.Y = touch.LocationInView (Screen.GlobalScreen).Y - (nfloat)(touch.PreviousLocationInView (Screen.GlobalScreen).Y / (touch.Timestamp - lastTime));
			velocity.X = 0.25f * velocity.X + 0.75f * newVelocity.X;
			velocity.Y = 0.25f * velocity.Y + 0.75f * newVelocity.Y;

			if (Math.Abs (velocity.Y) > 100.0 || Math.Abs (velocity.X) > 100)
				isInertiaing = true;
		}

		void Clamp ()
		{
			if (Y > 64) {
				Y = 64;
				velocity.Y = 0;
			}

			if (Y < -200) {
				Y = -200;
				velocity.Y = 0;
			}

			if (X > 0) {
				X = 0;
				velocity.X = 0;
			}

			if (X < -Width + 320) {
				X = -Width + 320;
				velocity.X = 0;
			}
		}
	}
}

