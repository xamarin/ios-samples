using AVFoundation;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using UIKit;

namespace AVTouch
{
    [Register("CALevelMeter")]
    public class CALevelMeter : UIView
    {
        private const float kPeakFalloffPerSec = 0.7f;
        private const float kLevelFalloffPerSec = 0.8f;
        private const float kMinDbValue = -80f;

        // The AVAudioPlayer object
        private AVAudioPlayer player;

        // Array of NSNumber objects: The indices of the channels to display in this meter
        private int[] channelNumbers = new int[1] { 0 };

        private bool showsPeaks; // Whether or not we show peak levels
        private bool vertical; // Whether the view is oriented V or H
        private bool useGL; // Whether or not to use OpenGL for drawing

        private MeterTable meterTable = new MeterTable(kMinDbValue);

        private CADisplayLink updateTimer;

        private DateTime peakFalloffLastFire;

        private List<LevelMeter> subLevelMeters = new List<LevelMeter>();

        public CALevelMeter(IntPtr handle) : base(handle)
        {
            showsPeaks = true;
            vertical = Frame.Size.Width < Frame.Size.Height;
            //useGL = true;

            LayoutSublevelMeters();
            RegisterForBackgroundNotifications();
        }

        [Export("initWithCoder:")]
        public CALevelMeter(NSCoder coder) : base(coder)
        {
            showsPeaks = true;
            vertical = Frame.Size.Width < Frame.Size.Height;
            //useGL = true;

            LayoutSublevelMeters();
            RegisterForBackgroundNotifications();
        }

        public CALevelMeter(CGRect frame) : base(frame)
        {
            showsPeaks = true;
            vertical = Frame.Size.Width < Frame.Size.Height;
            //useGL = true;

            LayoutSublevelMeters();
            RegisterForBackgroundNotifications();
        }

        private void RegisterForBackgroundNotifications()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillResignActiveNotification, PauseTimer);
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillEnterForegroundNotification, ResumeTimer);
        }

        private void LayoutSublevelMeters()
        {
            foreach (var meter in subLevelMeters)
            {
                meter.RemoveFromSuperview();
            }

            subLevelMeters.Clear();


            var totalRect = vertical ? new CGRect(0, 0, Frame.Size.Width + 2, Frame.Size.Height) :
                                       new CGRect(0, 0, Frame.Size.Width, Frame.Size.Height + 2);

            for (int i = 0; i < channelNumbers.Length; i++)
            {
                CGRect fr;

                if (vertical)
                {
                    fr = new CGRect((float)totalRect.X + ((float)i) / channelNumbers.Length * totalRect.Width,
                                    totalRect.Y,
                                    (1f / channelNumbers.Length) * totalRect.Width - 2f,
                                    totalRect.Height);
                }
                else
                {
                    fr = new CGRect(totalRect.X,
                                    (float)totalRect.Y + ((float)i) / channelNumbers.Length * totalRect.Height,
                                    totalRect.Width,
                                    (1f / channelNumbers.Length) * totalRect.Height - 2);
                }

                LevelMeter newMeter = null;
                if (useGL)
                {
                    //newMeter = new GLLevelMeter(fr) { NumLights = 30, Vertical = vertical };
                }
                else
                {
                    newMeter = new LevelMeter(fr) { NumLights = 30, Vertical = vertical };
                }

                subLevelMeters.Add(newMeter);
                AddSubview(newMeter);
                newMeter.Dispose();
                newMeter = null;
            }
        }

        private void Refresh()
        {
            var success = false;

            // if we have no queue, but still have levels, gradually bring them down
            if (player == null)
            {
                float maxLvl = -1f;
                var thisFire = DateTime.Now;
                // calculate how much time passed since the last draw
                var timePassed = (thisFire - peakFalloffLastFire).TotalSeconds;
                foreach (var thisMeter in subLevelMeters)
                {
                    float newPeak, newLevel;
                    newLevel = (float)(thisMeter.Level - timePassed * kLevelFalloffPerSec);
                    if (newLevel < 0)
                    {
                        newLevel = 0;
                    }

                    thisMeter.Level = newLevel;
                    if (showsPeaks)
                    {
                        newPeak = (float)(thisMeter.PeakLevel - timePassed * kPeakFalloffPerSec);
                        if (newPeak < 0)
                        {
                            newPeak = 0;
                        }

                        thisMeter.PeakLevel = newPeak;
                        if (newPeak > maxLvl)
                        {
                            maxLvl = newPeak;
                        }
                    }
                    else if (newLevel > maxLvl)
                    {
                        maxLvl = newLevel;
                    }

                    thisMeter.SetNeedsDisplay();
                }

                // stop the timer when the last level has hit 0
                if (maxLvl <= 0)
                {
                    updateTimer.Invalidate();
                    updateTimer = null;
                }

                peakFalloffLastFire = thisFire;
                success = true;
            }
            else
            {
                player.UpdateMeters();

                for (uint i = 0; i < channelNumbers.Length; i++)
                {
                    int channelIdx = channelNumbers[i];
                    var channelView = subLevelMeters[channelIdx];

                    if (channelIdx >= channelNumbers.Length)
                        goto bail;
                    if (channelIdx > 127)
                        goto bail;

                    channelView.Level = meterTable.ValueAt(player.AveragePower(i));
                    channelView.PeakLevel = showsPeaks ? meterTable.ValueAt(player.PeakPower(i)) : 0;
                    channelView.SetNeedsDisplay();
                    success = true;
                }
            }

        bail:

            if (!success)
            {
                foreach (var thisMeter in subLevelMeters)
                {
                    thisMeter.Level = 0;
                    thisMeter.SetNeedsDisplay();
                }

                Console.WriteLine("ERROR: metering failed\n");
            }
        }

        private void SetupTimer()
        {
            if (updateTimer != null)
            {
                updateTimer.Invalidate();
                updateTimer.Dispose();
                updateTimer = null;
            }

            updateTimer = CADisplayLink.Create(Refresh);
            updateTimer.AddToRunLoop(NSRunLoop.Current, NSRunLoopMode.Default);
        }

        public AVAudioPlayer Player
        {
            get
            {
                return player;
            }

            set
            {
                if (player == null && value != null)
                {
                    SetupTimer();
                }
                else if (player != null && value == null)
                {
                    peakFalloffLastFire = DateTime.Now;
                }

                player = value;
                if (player != null)
                {
                    player.MeteringEnabled = true;
                    // now check the number of channels in the new queue, we will need to reallocate if this has changed
                    if ((int)player.NumberOfChannels != channelNumbers.Length)
                    {
                        ChannelNumbers = player.NumberOfChannels < 2 ? new int[] { 0 } : new int[] { 0, 1 };
                    }
                }
                else
                {
                    foreach (var thisMeter in subLevelMeters)
                    {
                        thisMeter.SetNeedsDisplay();
                    }
                }
            }
        }

        public int[] ChannelNumbers
        {
            get
            {
                return channelNumbers;
            }

            set
            {
                channelNumbers = value;
                LayoutSublevelMeters();
            }
        }

        public bool UseGL
        {
            get => useGL;
            set
            {
                useGL = value;
                LayoutSublevelMeters();
            }
        }

        private void PauseTimer(NSNotification notification)
        {
            if (updateTimer != null)
            {
                updateTimer.Invalidate();
                updateTimer.Dispose();
                updateTimer = null;
            }
        }

        private void ResumeTimer(NSNotification notification)
        {
            if (player != null)
            {
                SetupTimer();
            }
        }
    }
}