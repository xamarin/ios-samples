using System;
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace AUSoundTriggeredPlayingSoundMemoryBased
{
    public partial class MainView : UIViewController
    {
        ExtAudioBufferPlayer _player;
        NSTimer _timer;
        bool _isTimerAvailable;

        public MainView() : base("MainView", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
     
            var url = CFUrl.FromFile("loop_stereo.aif");
            _player = new ExtAudioBufferPlayer(url);

            // setting audio session
            _slider.ValueChanged += new EventHandler(_slider_ValueChanged);

            _slider.MaxValue = _player.TotalFrames;

            _isTimerAvailable = true;
            _timer = NSTimer.CreateRepeatingTimer(TimeSpan.FromMilliseconds(100),
                delegate
                {
                    if (_isTimerAvailable)
                    {
                        long pos = _player.CurrentPosition;
                        _slider.Value = pos;
                        _signalLevelLabel.Text = _player.SignalLevel.ToString("0.00E0");
                    }                    
                }
            );

            NSRunLoop.Current.AddTimer(_timer, NSRunLoopMode.Default);            
        }

        void _slider_ValueChanged(object sender, EventArgs e)
        {
            _isTimerAvailable = false; 
            _player.CurrentPosition = (long)_slider.Value;     
            _isTimerAvailable = true;             
        }
    }
}

