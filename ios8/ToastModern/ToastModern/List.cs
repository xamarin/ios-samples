using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;
using System.Timers;

namespace ToastModern
{
	public class List : Layer
	{
		double lastTime;
		bool isInertiaing;
		double velocity;

		Spring spring;
		bool isSpringing;

		double min = -442;
		double max = 300;

		public List (Layer parent) : base (parent)
		{
			var t = new Timer { Enabled = true, Interval = 10 };
			t.Elapsed += (sender, e) => Tick ();
			isInertiaing = false;
			velocity = 0;
			lastTime = 0;

			spring = new Spring {
				Strength = 0.25,
				Damping = 0.4
			};

			isSpringing = false;
		}

		public void Tick ()
		{
			InvokeOnMainThread (() => {
				if (isSpringing) {
					spring.Tick ();
					UIView.PerformWithoutAnimation (() => {
						Y = (nfloat)spring.Position;
					});
				} else if (isInertiaing) {
					UIView.PerformWithoutAnimation (() => {
						Y += (nfloat)(velocity / 60);
					});

					velocity *= 0.98;

					if (Y < min) {
						spring.Length = min;

						spring.Position = Y;
						spring.Velocity = velocity / 60;

						isInertiaing = false;
						isSpringing = true;
					} else if (Y > max) {
						spring.Length = max;

						spring.Position = Y;
						spring.Velocity = velocity / 60;

						isInertiaing = false;
						isSpringing = true;
					}
				}
			});
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			isInertiaing = false;
			isSpringing = false;
			velocity = 0;
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			InvokeOnMainThread (() => {
				UITouch touch = (UITouch)touches.AnyObject;
				UIView.PerformWithoutAnimation (() => {
					if (Y < min || Y > max)
						Y += 0.5f * (touch.LocationInView (Screen.GlobalScreen).Y - touch.PreviousLocationInView (Screen.GlobalScreen).Y);
					else
						Y += touch.LocationInView (Screen.GlobalScreen).Y - touch.PreviousLocationInView (Screen.GlobalScreen).Y;
				});

				double dy = touch.LocationInView (Screen.GlobalScreen).Y - touch.PreviousLocationInView (Screen.GlobalScreen).Y;
				double dt = touch.Timestamp - lastTime;

				velocity = velocity * 0.25 + 0.75 * dy / dt;
				lastTime = touch.Timestamp;
			});
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			UITouch touch = (UITouch)touches.AnyObject;

			double dy = touch.LocationInView (Screen.GlobalScreen).Y - touch.PreviousLocationInView (Screen.GlobalScreen).Y;
			double dt = touch.Timestamp - lastTime;
			velocity = velocity * 0.5 + 0.5 * dy / dt;

			if (Y > max) {
				spring.Length = max;
				spring.Position = Y;
				spring.Velocity = velocity / 60;
				isSpringing = true;
			} else if (Y < min) {
				spring.Length = min;
				spring.Position = Y;
				spring.Velocity = velocity / 60;
				isSpringing = true;
			} else if (Math.Abs (velocity) > 100) {
				isInertiaing = true;
			}
		}

		public override void TouchesCancelled (NSSet touches, UIEvent evt)
		{
			TouchesEnded (touches, evt);
		}
	}
}

