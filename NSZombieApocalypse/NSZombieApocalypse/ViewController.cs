using System;

using UIKit;
using Foundation;
using CoreGraphics;
using CoreFoundation;
using CoreAnimation;
using ObjCRuntime;

namespace NSZombieApocalypse
{
	public enum TimerEvent
	{
		AlmostInfiniteLoop,
		LogicBomb,
		Leak,
		BadProgramming,
		OverRetain,
		StagnantReleasePool
	}

	public partial class ViewController : UIViewController
	{
		const int NSEC_PER_SEC = 1000000000;

		bool buttonDraggedToPad;

		ZombieMeter meterView;
		MiniPadView miniPadView;
		StatusView statusView;
		ButtonCollectionView buttonsView;
		HelpView helpView;

		bool paused;
		bool isVoiceOverSpeaking;

		Random rand = new Random ();

		public override void ViewWillAppear (bool animated)
		{
			CGRect frame = View.Frame;

			var backGround = new UIImageView (UIImage.FromBundle ("background.png"));
			backGround.Alpha = 0.34f;
			View.AddSubview (backGround);

			var miniPadFrame = new CGRect (350, 50, 0, 0);
			miniPadView = new MiniPadView (miniPadFrame);
			View.AddSubview (miniPadView);

			var meterFrame = new CGRect (miniPadView.Frame.GetMaxX (), miniPadFrame.Y, 200, miniPadView.Frame.Size.Height);
			meterView = new ZombieMeter (meterFrame);
			View.AddSubview (meterView);

			var statusFrame = new CGRect (100, frame.Size.Height - 350, frame.Size.Width - 100, 100);
			statusView = new StatusView (statusFrame);
			View.AddSubview (statusView);
			statusView.Status = "Loading";

			var buttonsFrame = new CGRect (100, statusFrame.GetMaxY () + 20, frame.Size.Width - 100, 230);
			buttonsView = new ButtonCollectionView (buttonsFrame) {
				ShouldGroupAccessibilityChildren = true
			};
			buttonsView.ButtonSelectedEvent += ButtonSelected;
			buttonsView.ButtonDraggedEvent += ButtonDragged;
			buttonsView.ButtonFinishedEvent += ButtonFinished;
			View.AddSubview (buttonsView);

			var questionFrame = new CGRect (10, statusFrame.GetMaxY () + 110, 80, 80);
			var questionView = new SymbolMarkView (questionFrame) {
				AccessibilityLabel = "Help"
			};
			questionView.TouchUpInside += (s, e) => questionPressed ();
			View.AddSubview (questionView);
			questionView.Symbol = "?";

			meterView.ZombieLevel = 0;
			goForthZombies ();
			NSNotificationCenter.DefaultCenter.AddObserver (this, new Selector ("voiceOverFinished:"), null, null);
		}

		[Export("voiceOverFinished:")]
		void voiceOverFinished (NSObject notification)
		{
			isVoiceOverSpeaking = false;
		}

		void Pause ()
		{
			paused = true;
			miniPadView.PauseZombies ();
			UIAccessibility.PostNotification(UIAccessibilityPostNotification.Announcement, (NSString)"Apocalypse On Pause");
		}

		void Unpause ()
		{
			paused = false;
			ZombiesOnATimer ();
			miniPadView.UnpauseZombies ();
			UIAccessibility.PostNotification (UIAccessibilityPostNotification.Announcement, (NSString)"Apocalypse resumed");
		}

		public void HelpDidClose (HelpView view)
		{
			helpView.RemoveFromSuperview ();
			helpView = null;
			Unpause ();
			UIAccessibility.PostNotification (UIAccessibilityPostNotification.ScreenChanged, statusView);
		}

		public void TogglePause ()
		{
			if (paused)
				Unpause ();
			else
				Pause ();
		}

		void questionPressed ()
		{
			Pause ();
			helpView = new HelpView (View.Bounds);
			View.AddSubview (helpView);
			helpView.HelpDidClose += HelpDidClose;
			helpView.Show ();
			UIAccessibility.PostNotification (UIAccessibilityPostNotification.ScreenChanged, null);
		}

		TimerEvent nextZombieEvent ()
		{
			int random = rand.Next ();
			float point = (float)random / (float)Int32.MaxValue;
			if (point < .03)
				return TimerEvent.AlmostInfiniteLoop;
			else if (point < .10)
				return TimerEvent.LogicBomb;
			else if (point < .50)
				return TimerEvent.Leak;
			else if (point < .65)
				return TimerEvent.BadProgramming;
			else if (point < .80)
				return TimerEvent.OverRetain;
			else
				return TimerEvent.StagnantReleasePool;
		}

		static string StringForZombieEvent (TimerEvent timerEvent)
		{
			switch (timerEvent) {
			case TimerEvent.BadProgramming:
				return "Bad programming! Too much memory used! (8MB)";
			case TimerEvent.AlmostInfiniteLoop:
				return "An infinite loop broke out! (15MB)";
			case TimerEvent.Leak:
				return  "Memory leak! (3MB)";
			case TimerEvent.LogicBomb:
				return "A logic bomb went off in your code! (4MB)";
			case TimerEvent.OverRetain:
				return "An object was retained too many times (6MB)";
			case TimerEvent.StagnantReleasePool:
				return  "Your release pools stopped draining! (23MB)";
			}

			return string.Empty;
		}

		static float ZombieFactorForEvent (TimerEvent timerEvent)
		{
			switch (timerEvent) {
			case TimerEvent.BadProgramming:
				return .08f;
			case TimerEvent.AlmostInfiniteLoop:
				return .15f;
			case TimerEvent.Leak:
				return .03f;
			case TimerEvent.LogicBomb:
				return .04f;
			case TimerEvent.OverRetain:
				return .06f;
			case TimerEvent.StagnantReleasePool:
				return .23f;
			default:
				return 0.0f;
			}
		}

		void MonitorZombiePressure ()
		{
			if (meterView.ZombieLevel > 0.99f)
				Environment.Exit (0);
			else if (meterView.ZombieLevel > 0.95f)
				statusView.Status = "Your program is using 95% of available memory! Seek a new profession!";
			else if (meterView.ZombieLevel > .90f)
				statusView.Status = "Your program is using 90% of available memory! You're a goner!'";
			else if (meterView.ZombieLevel > 0.75f)
				statusView.Status = "Your program is using 75% of available memory! Fix your bugs faster";
			else if (meterView.ZombieLevel == 0)
				statusView.Status = "Just like the real zombie apocalypse, this game never ends. Keep it up, the best you can hope for is that you'll never stop playing!";
		}

		void ZombiesOnATimer ()
		{
			if (paused)
				return;

			TimerEvent eventType = nextZombieEvent ();
			statusView.Status = StringForZombieEvent (eventType);
			meterView.ZombieLevel = meterView.ZombieLevel + ZombieFactorForEvent (eventType);

			MonitorZombiePressure ();
			manageVisibleZombies ();

			var popTime = new DispatchTime (DispatchTime.Now, (long)6 * NSEC_PER_SEC);
			DispatchQueue.MainQueue.DispatchAfter (popTime, ZombiesOnATimer);
		}

		void manageVisibleZombies ()
		{
			float level = meterView.ZombieLevel;
			int zombieCount = (int)Math.Floor (level * 10);
			if (zombieCount < miniPadView.ZombieCount) {
				while (zombieCount < miniPadView.ZombieCount)
					miniPadView.RemoveZombie ();
			}
			if (zombieCount > miniPadView.ZombieCount) {
				while (zombieCount > miniPadView.ZombieCount)
					miniPadView.AddZombie ();
			}
		}

		void goForthZombies ()
		{
			manageVisibleZombies ();
			statusView.Status = "Your program has started. The zombies are massing";

			const double delayInSeconds = 2;
			var popTime = new DispatchTime (DispatchTime.Now, (long)(delayInSeconds * NSEC_PER_SEC));
			DispatchQueue.MainQueue.DispatchAfter (popTime, ZombiesOnATimer);
		}

		void UpdateScoreForDroppedButton (ButtonView button)
		{
			var buttonType = (ButtonType)(int)button.Tag;
			float change = 0;
			switch (buttonType) {
			case ButtonType.Free:
				change = -.02f;
				break;
			case ButtonType.DeAlloc:
				change = -.03f;
				break;
			case ButtonType.Release:
				change = -0.1f;
				break;
			case ButtonType.AutoRelease:
				change = -0.5f;
				break;
			case ButtonType.GC:
				change = .1f;
				break;
			case ButtonType.ARC:
				change = -.1f;
				break;
			}
			meterView.ZombieLevel = meterView.ZombieLevel + change;

			MonitorZombiePressure ();
			manageVisibleZombies ();
		}

		public void ButtonSelected (ButtonView button)
		{
			if (!isVoiceOverSpeaking)
				UIAccessibility.PostNotification (UIAccessibilityPostNotification.Announcement, (NSString)"Memory Selected, drag to zombies to play");
		}

		public void ButtonDragged (ButtonView button, UITouch location)
		{
			CGPoint point = location.LocationInView (miniPadView);
			if (miniPadView.PointInside (point, null)) {
				if (!buttonDraggedToPad) {
					CATransaction.Begin ();
					CATransaction.AnimationDuration = 1;
					miniPadView.Layer.BorderColor = UIColor.Yellow.CGColor;
					miniPadView.Layer.BorderWidth = 2;
					CATransaction.Commit ();

					buttonDraggedToPad = true;
					if (!isVoiceOverSpeaking) {
						isVoiceOverSpeaking = true;
						UIAccessibility.PostNotification (UIAccessibilityPostNotification.Announcement, (NSString)"Memory object near the zombies, Lift to deploy");
					}
				}
			} else {
				if (buttonDraggedToPad) {
					if (!isVoiceOverSpeaking) {
						isVoiceOverSpeaking = true;
						UIAccessibility.PostNotification (UIAccessibilityPostNotification.Announcement, (NSString)"Memory object outside iPad. Lift to Cancel");
					}
				}
				buttonDraggedToPad = false;
				miniPadView.Layer.BorderWidth = 0;
			}
		}

		public void ButtonFinished (ButtonView button, UIView trackingView, UITouch location)
		{
			double delayInSeconds;

			buttonDraggedToPad = false;
			miniPadView.Layer.BorderWidth = 0;

			CGPoint point = location.LocationInView (miniPadView);
			if (miniPadView.PointInside (point, null)) {
				UpdateScoreForDroppedButton (button);
				UIView.Animate (.1f, () => trackingView.Transform = CGAffineTransform.MakeRotation (10f * (float)Math.PI / 180), async () => {
					await UIView.AnimateAsync (.1f, () => trackingView.Transform = CGAffineTransform.MakeRotation (-10f * (float)Math.PI / 180));
					await UIView.AnimateAsync (.1f, () => trackingView.Transform = CGAffineTransform.MakeRotation (10f * (float)Math.PI / 180));
					await UIView.AnimateAsync (.1f, () => trackingView.Transform = CGAffineTransform.MakeRotation (-10f * (float)Math.PI / 180));
					await UIView.AnimateAsync (.1f, () => trackingView.Transform = CGAffineTransform.MakeRotation (0));
				});
			}

			delayInSeconds = 0.5;

			var popTime = new DispatchTime (DispatchTime.Now, (long)(delayInSeconds * NSEC_PER_SEC));
			DispatchQueue.MainQueue.DispatchAfter (popTime, async () => {
				await UIView.AnimateAsync (0.35f, () => {
					CGRect bounds = trackingView.Bounds;
					bounds.Size = new CGSize (10, 10);
					trackingView.Bounds = bounds;
				});
				trackingView.RemoveFromSuperview ();
			});
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return toInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || toInterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
		}
	}
}

