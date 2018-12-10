using AVFoundation;
using Foundation;
using System;
using System.IO;
using UIKit;

namespace AVTouch
{
    public partial class AvTouchViewController : UIViewController, IAVAudioPlayerDelegate
    {
        // amount to skip on rewind or fast forward
        float SKIP_TIME = 1f;
        // amount to play between skips
        float SKIP_INTERVAL = .2f;

        private AVAudioPlayer player;
        private NSTimer rewTimer;
        private NSTimer ffwTimer;

        private bool inBackground;
        private NSTimer updateTimer;

        public AvTouchViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            RegisterForBackgroundNotifications();

            updateTimer = null;
            rewTimer = null;
            ffwTimer = null;

            // Load the the sample file, use mono or stero sample
            var fileUrl = NSBundle.MainBundle.PathForResource("sample", "m4a");
            player = AVAudioPlayer.FromUrl(new NSUrl(fileUrl, false));

            if (player != null)
            {
                fileNameLabel.Text = $"Mono {Path.GetFileName(player.Url.RelativePath)} ({player.NumberOfChannels} ch)";
                UpdateViewForPlayerInfo(player);
                UpdateViewForPlayerState(player);
                player.NumberOfLoops = 1;
                player.Delegate = this;
            }

            // we don't do anything special in the route change notification
            NSNotificationCenter.DefaultCenter.AddObserver(AVAudioSession.RouteChangeNotification, HandleRouteChange);
        }

        private void PausePlaybackForPlayer(AVAudioPlayer p)
        {
            p.Pause();
            UpdateViewForPlayerState(p);
        }

        private void StartPlaybackForPlayer(AVAudioPlayer p)
        {
            if (p.Play())
            {
                UpdateViewForPlayerState(p);
            }
            else
            {
                Console.WriteLine($"Could not play {p.Url}");
            }
        }

        partial void PlayButtonPressed(UIBarButtonItem sender)
        {
            if (player.Playing)
            {
                PausePlaybackForPlayer(player);
            }
            else
            {
                StartPlaybackForPlayer(player);
            }
        }

        partial void RewindButtonPressed(UIBarButtonItem sender)
        {
            if (rewTimer != null)
            {
                rewTimer.Invalidate();
                rewTimer.Dispose();
                rewTimer = null;
            }

            rewTimer = NSTimer.CreateRepeatingScheduledTimer(SKIP_INTERVAL, (timer) => Rewind(player));
        }

        partial void RewindButtonReleased(UIBarButtonItem sender)
        {
            if (rewTimer != null)
            {
                rewTimer.Invalidate();
                rewTimer.Dispose();
                rewTimer = null;
            }
        }

        partial void ForwardButtonPressed(UIBarButtonItem sender)
        {
            if (ffwTimer != null)
            {
                ffwTimer.Invalidate();
                ffwTimer.Dispose();
                ffwTimer = null;
            }

            ffwTimer = NSTimer.CreateRepeatingScheduledTimer(SKIP_INTERVAL, (timer) => Forward(player));
        }

        partial void ForwardButtonReleased(UIBarButtonItem sender)
        {
            if (ffwTimer != null)
            {
                ffwTimer.Invalidate();
                ffwTimer.Dispose();
                ffwTimer = null;
            }
        }

        partial void VolumeSliderMoved(UISlider sender)
        {
            player.Volume = sender.Value;
        }

        partial void ProgressSliderMoved(UISlider sender)
        {
            player.CurrentTime = sender.Value;
            UpdateCurrentTimeForPlayer(player);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private void UpdateCurrentTimeForPlayer(AVAudioPlayer p)
        {
            currentTimeLabel.Text = TimeSpan.FromSeconds(player.CurrentTime).ToString(@"mm\:ss");
            progressSlider.Value = (float)p.CurrentTime;
        }

        private void UpdateCurrentTime(AVAudioPlayer p)
        {
            UpdateCurrentTimeForPlayer(p);
        }

        private void UpdateViewForPlayerState(AVAudioPlayer p)
        {
            UpdateCurrentTimeForPlayer(p);

            if (updateTimer != null)
            {
                updateTimer.Invalidate();
            }

            UpdatePlayButtonState(p);

            if (p.Playing)
            {
                lvlMeter.Player = p;
                updateTimer = NSTimer.CreateRepeatingScheduledTimer(.01f, (timer) => UpdateCurrentTime(p));
            }
            else
            {
                lvlMeter.Player = null;
                updateTimer = null;
            }
        }

        private void UpdateViewForPlayerStateInBackground(AVAudioPlayer p)
        {
            UpdateCurrentTimeForPlayer(p);
            UpdatePlayButtonState(p);
        }

        private void UpdatePlayButtonState(AVAudioPlayer p)
        {
            var style = p.Playing ? UIBarButtonSystemItem.Pause : UIBarButtonSystemItem.Play;
            using (var playButton = new UIBarButtonItem(style, (sender, e) => PlayButtonPressed(sender as UIBarButtonItem)))
            {
                playButton.TintColor = UIColor.White;

                var items = toolbar.Items;
                items[3] = playButton;
                toolbar.Items = items;
            }
        }

        private void UpdateViewForPlayerInfo(AVAudioPlayer p)
        {
            durationLabel.Text = TimeSpan.FromSeconds(player.Duration).ToString(@"mm\:ss");
            progressSlider.MaxValue = (float)p.Duration;
            volumeSlider.Value = p.Volume;
        }

        private void Rewind(AVAudioPlayer p)
        {
            p.CurrentTime-= SKIP_TIME;
            UpdateCurrentTimeForPlayer(p);
        }

        private void Forward(AVAudioPlayer p)
        {
            p.CurrentTime+= SKIP_TIME;
            UpdateCurrentTimeForPlayer(p);
        }

        #region AVAudioSession notification handlers

        private void HandleRouteChange(NSNotification notification)
        {
            var reasonValue = ((NSNumber)notification.UserInfo.ValueForKey(new NSString("AVAudioSessionRouteChangeReason"))).Int32Value;
            var routeDescription = notification.UserInfo.ValueForKey(new NSString("AVAudioSessionRouteChangePreviousRouteKey"));
            Console.WriteLine("Route change:");

            // TODO:
        }

        #endregion

        #region AVAudioPlayer delegate methods

        [Export("audioPlayerDidFinishPlaying:successfully:")]
        public void FinishedPlaying(AVAudioPlayer p, bool flag)
        {
            if (!flag)
            {
                Console.WriteLine(@"Playback finished unsuccessfully");
            }

            p.CurrentTime = 0d;
            if (inBackground)
            {
                UpdateViewForPlayerStateInBackground(p);
            }
            else
            {
                UpdateViewForPlayerState(p);
            }
        }

        [Export("audioPlayerDecodeErrorDidOccur:error:")]
        public void DecoderError(AVAudioPlayer player, NSError error)
        {
            Console.WriteLine($"ERROR IN DECODE: {error}");
        }

        // We will only get these notifications if playback was interrupted
        [Export("audioPlayerBeginInterruption:")]
        public void BeginInterruption(AVAudioPlayer p)
        {
            Console.WriteLine("Interruption begin. Updating UI for new state");
            // the object has already been paused,  we just need to update UI
            if (inBackground)
            {
                UpdateViewForPlayerStateInBackground(p);
            } 
            else 
            {
                UpdateViewForPlayerState(p);
            }
        }

        [Export("audioPlayerEndInterruption:")]
        public void EndInterruption(AVAudioPlayer p)
        {
            Console.WriteLine("Interruption ended. Resuming playback");
            StartPlaybackForPlayer(p);
        }

        #endregion

        #region mark background notifications

        private void RegisterForBackgroundNotifications()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillResignActiveNotification, SetInBackgroundFlag);
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillEnterForegroundNotification, ClearInBackgroundFlag);
        }

        private void SetInBackgroundFlag(NSNotification notification)
        {
            inBackground = true;
        }

        private void ClearInBackgroundFlag(NSNotification notification)
        {
            inBackground = false;
        }

        #endregion
    }
}