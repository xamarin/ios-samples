
using System;
using Foundation;
using AVFoundation;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;

namespace avTouch
{
	public partial class CALevelMeter : UIView 
	{
		const float kMinDbValue = -80f;
		const float kPeakFalloffPerSec =	 .7f;
		const float kLevelFalloffPerSec = .8f;

		AVAudioPlayer player;
		
		float refreshHz = (float) 1 / 60f;
		bool showPeaks = true, vertical;
		int [] channelNumbers = new int [1] { 0 };
		MeterTable meterTable = new MeterTable (kMinDbValue);
		List<LevelMeter> sublevelMeters = new List<LevelMeter> ();
		DateTime peakFalloffLastFire;
		NSTimer updateTimer;
		
		public CALevelMeter (IntPtr handle) : base (handle)
		{
			LayoutSublevelMeters ();
		}
		
		public CALevelMeter (NSCoder coder) : base (coder) 
		{
			LayoutSublevelMeters ();
		}
		
		public CALevelMeter (CGRect frame) : base (frame)
		{
			LayoutSublevelMeters ();
		}
	
		void LayoutSublevelMeters ()
		{
			foreach (var meter in sublevelMeters)
				meter.RemoveFromSuperview ();
		
			sublevelMeters.Clear ();
			vertical = Frame.Size.Width < Frame.Size.Height;
			
			CGRect totalRect;
			if (vertical)
				totalRect = new CGRect (0, 0, Frame.Size.Width + 2, Frame.Size.Height);
			else
				totalRect = new CGRect (0, 0, Frame.Size.Width, Frame.Size.Height + 2);
			
			for (int i = 0; i < channelNumbers.Length; i++){
				CGRect fr;
				
				if (vertical)
					fr = new CGRect (
				 		(float) totalRect.X + ((float) i) / channelNumbers.Length * totalRect.Width,
					    totalRect.Y,
						(float) (1f / channelNumbers.Length) * totalRect.Width - 2,
					    totalRect.Height);
				else
					fr = new CGRect (
					    totalRect.X,
					    (float) totalRect.Y + ((float) i) / channelNumbers.Length * totalRect.Height,
					    totalRect.Width,
					    (float) (1f / channelNumbers.Length) * totalRect.Height - 2);
					  
				LevelMeter newMeter;
				newMeter = new LevelMeter (fr) {
					NumLights = 30,
					Vertical = vertical
				};
				sublevelMeters.Add (newMeter);
				AddSubview (newMeter);
			}
		}
		
		void Refresh ()
		{
			bool success = false;
			
			// if we have no queue, but still have levels, gradually bring them down
			if (player == null)
			{
				float maxLvl = -1f;
				DateTime thisFire = DateTime.Now;
				// calculate how much time passed since the last draw
				var timePassed = (thisFire - peakFalloffLastFire).TotalSeconds;
				foreach (var thisMeter in sublevelMeters)
				{
					float newPeak, newLevel;
					newLevel = (float) (thisMeter.Level - timePassed * kLevelFalloffPerSec);
					if (newLevel < 0) 
						newLevel = 0;
					thisMeter.Level = newLevel;
					if (showPeaks){
						newPeak = (float) (thisMeter.PeakLevel - timePassed * kPeakFalloffPerSec);
						if (newPeak < 0) 
							newPeak = 0;
						thisMeter.PeakLevel = newPeak;
						if (newPeak > maxLvl) 
							maxLvl = newPeak;
					} else if (newLevel > maxLvl) 
						maxLvl = newLevel;
					
					thisMeter.SetNeedsDisplay ();
				}
				// stop the timer when the last level has hit 0
				if (maxLvl <= 0){
					updateTimer.Invalidate ();
					updateTimer = null;
				}
				
				peakFalloffLastFire = thisFire;
				success = true;
			} else {
				player.UpdateMeters ();
				
				for (int i = 0; i < channelNumbers.Length; i++){
					int channelIdx = channelNumbers [i];
					LevelMeter channelView = sublevelMeters [channelIdx];
					
					if (channelIdx >= channelNumbers.Length) 
						goto bail;
					if (channelIdx > 127) 
						goto bail;
					
					channelView.Level = meterTable.ValueAt (player.AveragePower ((uint) i));
					if (showPeaks) 
						channelView.PeakLevel = meterTable.ValueAt (player.PeakPower ((uint)i));
					else
						channelView.PeakLevel = 0;
					channelView.SetNeedsDisplay ();
					success = true;		
				}
			}
			
		bail:
			
			if (!success){
				foreach (var thisMeter in sublevelMeters) { 
					thisMeter.Level = 0; thisMeter.SetNeedsDisplay ();
				}
				Console.WriteLine ("ERROR: metering failed\n");
			}
		}
		
		void SetupTimer ()
		{
			if (updateTimer != null)
				updateTimer.Invalidate ();
			
			updateTimer = NSTimer.CreateRepeatingScheduledTimer (TimeSpan.FromSeconds (refreshHz), delegate {
				Refresh ();
			});
		}
		
		public AVAudioPlayer Player { 
			get { return player; }
		
			set {
				if (player == null && value != null){
					SetupTimer ();
				} else if (player != null && value == null){
					peakFalloffLastFire = DateTime.Now;
				}
				
				player = value;
				
				if (player != null){
					player.MeteringEnabled = true;
					if ((nint)player.NumberOfChannels != channelNumbers.Length){
						if (player.NumberOfChannels < 2)
							ChannelNumbers = new int [] {0};
						else
							ChannelNumbers = new int [] {0, 1};
					}
				} else {
					foreach (var thisMeter in sublevelMeters)
						thisMeter.SetNeedsDisplay ();
				}
			}
		}
		
		public float RefreshHz {
			get {
				return refreshHz;
			}
			
			set {
				refreshHz = value;
				if (updateTimer != null)
					SetupTimer ();
			}
		}
		
		public int [] ChannelNumbers {
			get {
				return channelNumbers;
			}
			
			set {
				channelNumbers = value;
				LayoutSublevelMeters ();
			}
		}
	}
}