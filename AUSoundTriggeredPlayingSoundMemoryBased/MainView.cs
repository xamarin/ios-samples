using System;
using CoreFoundation;
using Foundation;
using UIKit;

namespace AUSoundTriggeredPlayingSoundMemoryBased
{
    public partial class MainView : UIViewController
    {
        ExtAudioBufferPlayer player;
        NSTimer timer;
        bool isTimerAvailable;

        public MainView() : base("MainView", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var url = CFUrl.FromFile("loop_stereo.aif");
            player = new ExtAudioBufferPlayer(url);

            // setting audio session
            _slider.ValueChanged += OnSliderValueChanged;
            _slider.MaxValue = player.TotalFrames;

            isTimerAvailable = true;
			timer = NSTimer.CreateRepeatingTimer (TimeSpan.FromMilliseconds (100),
				_ => {
					if (isTimerAvailable) {
						long pos = player.CurrentFrame;
						_slider.Value = pos;
						_signalLevelLabel.Text = player.SignalLevel.ToString ("0.00E0");
					}
				}
			);

            NSRunLoop.Current.AddTimer(timer, NSRunLoopMode.Default);
        }

        void OnSliderValueChanged(object sender, EventArgs e)
        {
            isTimerAvailable = false;
            player.CurrentFrame = (long)_slider.Value;
            isTimerAvailable = true;
        }
    }
}

