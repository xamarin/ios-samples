using AVFoundation;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using UIKit;

namespace AVTouch {
	[Register ("CALevelMeter")]
	public class CALevelMeter : UIView {
		private const float PeakFalloffPerSec = 0.7f;
		private const float LevelFalloffPerSec = 0.8f;
		private const float MinDbValue = -80f;

		// The AVAudioPlayer object
		private AVAudioPlayer player;

		// Array of NSNumber objects: The indices of the channels to display in this meter
		private int [] channelNumbers = new int [1] { 0 };

		private bool showsPeaks; // Whether or not we show peak levels
		private bool vertical; // Whether the view is oriented V or H
		private bool useGL; // Whether or not to use OpenGL for drawing

		private MeterTable meterTable = new MeterTable (MinDbValue);

		private CADisplayLink updateTimer;

		private DateTime peakFalloffLastFire;

		private List<LevelMeter> subLevelMeters = new List<LevelMeter> ();

		public CALevelMeter (IntPtr handle) : base (handle)
		{
			this.showsPeaks = true;
			this.vertical = this.Frame.Size.Width < this.Frame.Size.Height;
			//useGL = true;

			this.LayoutSublevelMeters ();
			this.RegisterForBackgroundNotifications ();
		}

		[Export ("initWithCoder:")]
		public CALevelMeter (NSCoder coder) : base (coder)
		{
			this.showsPeaks = true;
			this.vertical = this.Frame.Size.Width < this.Frame.Size.Height;
			//useGL = true;

			this.LayoutSublevelMeters ();
			this.RegisterForBackgroundNotifications ();
		}

		public CALevelMeter (CGRect frame) : base (frame)
		{
			this.showsPeaks = true;
			this.vertical = this.Frame.Size.Width < this.Frame.Size.Height;
			//useGL = true;

			this.LayoutSublevelMeters ();
			this.RegisterForBackgroundNotifications ();
		}



		public AVAudioPlayer Player {
			get {
				return this.player;
			}

			set {
				if (this.player == null && value != null) {
					this.SetupTimer ();
				} else if (this.player != null && value == null) {
					this.peakFalloffLastFire = DateTime.Now;
				}

				this.player = value;
				if (this.player != null) {
					this.player.MeteringEnabled = true;
					// now check the number of channels in the new queue, we will need to reallocate if this has changed
					if ((int) this.player.NumberOfChannels != this.channelNumbers.Length) {
						this.ChannelNumbers = this.player.NumberOfChannels < 2 ? new int [] { 0 } : new int [] { 0, 1 };
					}
				} else {
					foreach (var thisMeter in this.subLevelMeters) {
						thisMeter.SetNeedsDisplay ();
					}
				}
			}
		}

		public int [] ChannelNumbers {
			get {
				return this.channelNumbers;
			}

			set {
				this.channelNumbers = value;
				this.LayoutSublevelMeters ();
			}
		}

		public bool UseGL {
			get {
				return this.useGL;
			}

			set {
				this.useGL = value;
				this.LayoutSublevelMeters ();
			}
		}

		private void RegisterForBackgroundNotifications ()
		{
			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.WillResignActiveNotification, this.PauseTimer);
			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.WillEnterForegroundNotification, this.ResumeTimer);
		}

		private void LayoutSublevelMeters ()
		{
			foreach (var meter in this.subLevelMeters) {
				meter.RemoveFromSuperview ();
			}

			this.subLevelMeters.Clear ();

			var totalRect = vertical ? new CGRect (0, 0, this.Frame.Size.Width + 2, this.Frame.Size.Height) :
									   new CGRect (0, 0, this.Frame.Size.Width, this.Frame.Size.Height + 2);

			for (int i = 0; i < this.channelNumbers.Length; i++) {
				CGRect fr;
				if (vertical) {
					fr = new CGRect ((float) totalRect.X + ((float) i) / this.channelNumbers.Length * totalRect.Width,
									totalRect.Y,
									(1f / this.channelNumbers.Length) * totalRect.Width - 2f,
									totalRect.Height);
				} else {
					fr = new CGRect (totalRect.X,
									(float) totalRect.Y + ((float) i) / this.channelNumbers.Length * totalRect.Height,
									totalRect.Width,
									(1f / this.channelNumbers.Length) * totalRect.Height - 2);
				}

				LevelMeter newMeter = null;
				if (useGL) {
					//newMeter = new GLLevelMeter(fr) { NumLights = 30, Vertical = vertical };
				} else {
					newMeter = new LevelMeter (fr) { NumLights = 30, Vertical = this.vertical };
				}

				this.subLevelMeters.Add (newMeter);
				this.AddSubview (newMeter);
				newMeter.Dispose ();
				newMeter = null;
			}
		}

		private void Refresh ()
		{
			var success = false;

			// if we have no queue, but still have levels, gradually bring them down
			if (player == null) {
				float maxLvl = -1f;
				var thisFire = DateTime.Now;
				// calculate how much time passed since the last draw
				var timePassed = (thisFire - this.peakFalloffLastFire).TotalSeconds;
				foreach (var thisMeter in this.subLevelMeters) {
					float newPeak, newLevel;
					newLevel = (float) (thisMeter.Level - timePassed * LevelFalloffPerSec);
					if (newLevel < 0) {
						newLevel = 0;
					}

					thisMeter.Level = newLevel;
					if (this.showsPeaks) {
						newPeak = (float) (thisMeter.PeakLevel - timePassed * PeakFalloffPerSec);
						if (newPeak < 0) {
							newPeak = 0;
						}

						thisMeter.PeakLevel = newPeak;
						if (newPeak > maxLvl) {
							maxLvl = newPeak;
						}
					} else if (newLevel > maxLvl) {
						maxLvl = newLevel;
					}

					thisMeter.SetNeedsDisplay ();
				}

				// stop the timer when the last level has hit 0
				if (maxLvl <= 0) {
					this.updateTimer.Invalidate ();
					this.updateTimer.Dispose ();
					this.updateTimer = null;
				}

				this.peakFalloffLastFire = thisFire;
				success = true;
			} else {
				this.player.UpdateMeters ();

				for (uint i = 0; i < this.channelNumbers.Length; i++) {
					int channelIdx = this.channelNumbers [i];
					var channelView = this.subLevelMeters [channelIdx];

					if (channelIdx >= this.channelNumbers.Length)
						goto bail;
					if (channelIdx > 127)
						goto bail;

					channelView.Level = this.meterTable.ValueAt (this.player.AveragePower (i));
					channelView.PeakLevel = this.showsPeaks ? this.meterTable.ValueAt (this.player.PeakPower (i)) : 0;
					channelView.SetNeedsDisplay ();
					success = true;
				}
			}

		bail:

			if (!success) {
				foreach (var thisMeter in this.subLevelMeters) {
					thisMeter.Level = 0;
					thisMeter.SetNeedsDisplay ();
				}

				Console.WriteLine ("ERROR: metering failed\n");
			}
		}

		private void SetupTimer ()
		{
			if (this.updateTimer != null) {
				this.updateTimer.Invalidate ();
				this.updateTimer.Dispose ();
				this.updateTimer = null;
			}

			this.updateTimer = CADisplayLink.Create (this.Refresh);
			this.updateTimer.AddToRunLoop (NSRunLoop.Current, NSRunLoopMode.Default);
		}

		private void PauseTimer (NSNotification notification)
		{
			if (this.updateTimer != null) {
				this.updateTimer.Invalidate ();
				this.updateTimer.Dispose ();
				this.updateTimer = null;
			}
		}

		private void ResumeTimer (NSNotification notification)
		{
			if (this.player != null) {
				this.SetupTimer ();
			}
		}
	}
}
