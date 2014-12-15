using System;
using System.Runtime.InteropServices;
using System.Threading;

using CoreAnimation;
using CoreGraphics;
using Foundation;
using OpenTK;
using Metal;
using UIKit;

namespace MetalBasic3D
{
	[Register ("GameViewController")]
	public class GameViewController : UIViewController
	{
		CADisplayLink timer;
		double timeSinceLastDrawPreviousTime;
		bool lastDrawOccurred;
		bool gameLoopPaused;

		NSObject didEnterBackgroundObserver;
		NSObject willEnterForegroundObserver;

		public bool Running { get; private set; }

		public double TimeSinceLastDraw { get; set; }

		public bool Paused {
			get {
				return gameLoopPaused;
			}

			set {
				if (gameLoopPaused == value)
					return;

				if (timer != null) {
					Delegate.RenderViewController (this, value);

					if (value) {
						gameLoopPaused = value;
						timer.Paused = true;
					} else {
						gameLoopPaused = value;
						timer.Paused = false;
					}

				}
			}
		}

		public IGameViewController Delegate { get; set; }

		public GameViewController (IntPtr handle) : base (handle)
		{
		}

		public void DispatchGameLoop ()
		{
			timer = CADisplayLink.Create (Gameloop);
			timer.FrameInterval = 1;
			timer.AddToRunLoop (NSRunLoop.Main, NSRunLoop.NSDefaultRunLoopMode);
			Running = true;
		}

		public void Gameloop ()
		{
			if (!lastDrawOccurred) {
				TimeSinceLastDraw = 0.0;
				timeSinceLastDrawPreviousTime = CAAnimation.CurrentMediaTime ();
				lastDrawOccurred = true;
			} else {
				double currentTime = CAAnimation.CurrentMediaTime ();
				TimeSinceLastDraw = currentTime - timeSinceLastDrawPreviousTime;
				timeSinceLastDrawPreviousTime = currentTime;
			}

			((GameView)View).Display ();
			Delegate.RenderViewControllerUpdate (this);
		}

		public void Stop ()
		{
			if (timer != null) {
				Running = false;
				timer.Invalidate ();
			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			didEnterBackgroundObserver.Dispose ();
			willEnterForegroundObserver.Dispose ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			didEnterBackgroundObserver = UIApplication.Notifications.ObserveDidEnterBackground (DidEnterBackground);
			willEnterForegroundObserver = UIApplication.Notifications.ObserveWillEnterForeground (WillEnterForeground);
		}

		void DidEnterBackground (object sender, NSNotificationEventArgs notification)
		{
			Paused = true;
		}

		void WillEnterForeground (object sender, NSNotificationEventArgs notification)
		{
			Paused = false;
		}
	}
}
