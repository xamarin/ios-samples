using AssetsLibrary;
using AVFoundation;
using CoreFoundation;
using CoreMedia;
using Foundation;
using Photos;
using System;
using System.Collections.Generic;
using System.IO;
using UIKit;

namespace AVCustomEdit
{
    public partial class ViewController : UIViewController, IUIGestureRecognizerDelegate, ITransitionTypePickerDelegate
    {
        private readonly NSString StatusObservationContext = new NSString("AVCustomEditPlayerViewControllerStatusObservationContext");
        private readonly NSString RateObservationContext = new NSString("AVCustomEditPlayerViewControllerRateObservationContext");

        private SimpleEditor editor;

        private List<AVAsset> clips;
        private List<NSValue> clipTimeRanges;

        private AVPlayer player;
        private AVPlayerItem playerItem;

        private bool isPlaying;
        private bool isScrubInFlight;
        private bool isSeekToZeroBeforePlaying;
        private float lastScrubSliderValue;
        private float playRateToRestore;
        private NSObject timeObserver;

        private float transitionDuration;
        private TransitionType transitionType;
        private bool isTransitionsEnabled;

        private NSTimer progressTimer;

        protected ViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.editor = new SimpleEditor();
            this.clips = new List<AVAsset>();
            this.clipTimeRanges = new List<NSValue>();

            // Defaults for the transition settings.
            this.transitionType = TransitionType.DiagonalWipeTransition;
            this.transitionDuration = 2f;
            this.isTransitionsEnabled = true;

            this.UpdateScrubber();
            this.UpdateTimeLabel();

            // Add the clips from the main bundle to create a composition using them
            this.SetupEditingAndPlayback();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (this.player == null)
            {
                this.isSeekToZeroBeforePlaying = false;

                this.player = new AVPlayer();
                this.player.AddObserver(this, "rate", NSKeyValueObservingOptions.Old | NSKeyValueObservingOptions.New, RateObservationContext.Handle);
                this.playerView.Player = this.player;
            }

            this.AddTimeObserverToPlayer();

            // Build AVComposition and AVVideoComposition objects for playback
            this.editor.BuildCompositionObjectsForPlayback(true);
            this.SynchronizePlayerWithEditor();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            this.player.Pause();
            this.RemoveTimeObserverFromPlayer();
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "Transition")
            {
                // Setup transition type picker controller before it is shown.
                if ((segue.DestinationViewController as UINavigationController)?.TopViewController is TransitionTypeController transitionTypePickerController)
                {
                    transitionTypePickerController.Delegate = this;
                    transitionTypePickerController.CurrentTransition = this.transitionType;
                }
            }
        }

        #region Simple Editor

        private void SetupEditingAndPlayback()
        {
            var path1 = NSBundle.MainBundle.PathForResource("sample_clip1", "m4v");
            var path2 = NSBundle.MainBundle.PathForResource("sample_clip2", "mov");
            var asset1 = AVAsset.FromUrl(new NSUrl(path1, false)) as AVUrlAsset;
            var asset2 = AVAsset.FromUrl(new NSUrl(path2, false)) as AVUrlAsset;

            var dispatchGroup = DispatchGroup.Create();
            string[] assetKeysToLoadAndTest = { "tracks", "duration", "composable" };

            this.LoadAsset(asset1, assetKeysToLoadAndTest, dispatchGroup);
            this.LoadAsset(asset2, assetKeysToLoadAndTest, dispatchGroup);

            // Wait until both assets are loaded
            dispatchGroup.Wait(DispatchTime.Forever);
            base.InvokeOnMainThread(() => this.SynchronizeWithEditor());
        }

        private void LoadAsset(AVAsset asset, string[] assetKeysToLoad, DispatchGroup dispatchGroup)
        {
            dispatchGroup.Enter();
            asset.LoadValuesAsynchronously(assetKeysToLoad, () =>
            {
                // First test whether the values of each of the keys we need have been successfully loaded.
                foreach (var key in assetKeysToLoad)
                {
                    if (asset.StatusOfValue(key, out NSError error) == AVKeyValueStatus.Failed)
                    {
                        Console.WriteLine($"Key value loading failed for key:{key} with error: {error?.LocalizedDescription ?? ""}");
                        goto bail;
                    }
                }

                if (!asset.Composable)
                {
                    Console.WriteLine("Asset is not composable");
                    goto bail;
                }

                this.clips.Add(asset);
                // This code assumes that both assets are atleast 5 seconds long.
                var value = NSValue.FromCMTimeRange(new CMTimeRange { Start = CMTime.FromSeconds(0, 1), Duration = CMTime.FromSeconds(5, 1) });
                this.clipTimeRanges.Add(value);
                if (this.clips.Count != this.clipTimeRanges.Count)
                { }
            bail:
                dispatchGroup.Leave();
            });
        }

        private void SynchronizePlayerWithEditor()
        {
            if (this.player != null)
            {
                var playerItem = this.editor.PlayerItem;
                if (this.playerItem != playerItem)
                {
                    if (this.playerItem != null)
                    {
                        this.playerItem.RemoveObserver(this, "status");
                        NSNotificationCenter.DefaultCenter.RemoveObserver(this, AVPlayerItem.DidPlayToEndTimeNotification, this.playerItem);
                    }

                    this.playerItem = playerItem;
                    if (this.playerItem != null)
                    {
                        this.playerItem.SeekingWaitsForVideoCompositionRendering = true;

                        // Observe the player item "status" key to determine when it is ready to play
                        this.playerItem.AddObserver(this, "status", NSKeyValueObservingOptions.New | NSKeyValueObservingOptions.Initial, StatusObservationContext.Handle);

                        // When the player item has played to its end time we'll set a flag
                        // so that the next time the play method is issued the player will
                        // be reset to time zero first.
                        NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, this.PlayerItemDidReachEnd);
                    }

                    this.player.ReplaceCurrentItemWithPlayerItem(this.playerItem);
                }
            }
        }

        private void SynchronizeWithEditor()
        {
            // Clips
            this.SynchronizeEditorClipsWithOurClips();
            this.SynchronizeEditorClipTimeRangesWithOurClipTimeRanges();

            // Transitions
            if (this.isTransitionsEnabled)
            {
                this.editor.TransitionDuration = CMTime.FromSeconds(this.transitionDuration, 600);
                this.editor.TransitionType = this.transitionType;
            }
            else
            {
                this.editor.TransitionDuration = CMTime.Invalid;
            }

            // Build AVComposition and AVVideoComposition objects for playback
            //this.editor.BuildCompositionObjectsForPlayback(true);
            //this.SynchronizePlayerWithEditor();
        }

        private void SynchronizeEditorClipsWithOurClips()
        {
            var validClips = new List<AVAsset>();
            foreach (var asset in this.clips)
            {
                if (asset != null)
                {
                    validClips.Add(asset);
                }
            }

            this.editor.Clips = validClips;
        }

        private void SynchronizeEditorClipTimeRangesWithOurClipTimeRanges()
        {
            var validClipTimeRanges = new List<NSValue>();
            foreach (var timeRange in this.clipTimeRanges)
            {
                if (timeRange != null)
                {
                    validClipTimeRanges.Add(timeRange);
                }
            }

            this.editor.ClipTimeRanges = validClipTimeRanges;
        }

        #endregion

        #region Utilities

        private const int NSEC_PER_SEC = 1000000000;

        /// <summary>
        /// Update the scrubber and time label periodically.
        /// </summary>
        private void AddTimeObserverToPlayer()
        {
            if (this.player?.CurrentItem != null &&
                this.player.CurrentItem.Status == AVPlayerItemStatus.ReadyToPlay)
            {
                var duration = this.GetPlayerItemDuration().Seconds;
                if (!double.IsInfinity(duration))
                {
                    var width = this.scrubber.Bounds.Width;
                    var interval = 0.5 * duration / width;

                    /* The time label needs to update at least once per second. */
                    if (interval > 1.0)
                    {
                        interval = 1.0;
                    }

                    timeObserver = this.player.AddPeriodicTimeObserver(CMTime.FromSeconds(interval, NSEC_PER_SEC),
                                                                        DispatchQueue.MainQueue,
                                                                        (time) =>
                                                                        {
                                                                            this.UpdateScrubber();
                                                                            this.UpdateTimeLabel();
                                                                        });
                }
            }
        }

        private void RemoveTimeObserverFromPlayer()
        {
            if (this.timeObserver != null)
            {
                this.player.RemoveTimeObserver(this.timeObserver);
                this.timeObserver.Dispose();
                this.timeObserver = null;
            }
        }

        private CMTime GetPlayerItemDuration()
        {
            var itemDuration = CMTime.Invalid;

            var playerItem = this.player?.CurrentItem;
            if (playerItem?.Status == AVPlayerItemStatus.ReadyToPlay)
            {
                itemDuration = playerItem.Duration;
            }

            // Will be kCMTimeInvalid if the item is not ready to play.
            return itemDuration;
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            if (context == this.RateObservationContext.Handle)
            {
                var changes = new NSObservedChange(change);
                if (changes.NewValue is NSNumber newValue &&
                    (changes.OldValue == null || (changes.OldValue is NSNumber oldRate && oldRate.FloatValue != newValue.FloatValue)))
                {
                    this.isPlaying = (newValue.FloatValue != 0f) || (playRateToRestore != 0f);
                    this.UpdatePlayPauseButton();
                    this.UpdateScrubber();
                    this.UpdateTimeLabel();

                    // clear
                    newValue.Dispose();
                    newValue = null;
                }
            }
            else if (context == this.StatusObservationContext.Handle)
            {
                if (ofObject is AVPlayerItem playerItem)
                {
                    if (playerItem.Status == AVPlayerItemStatus.ReadyToPlay)
                    {
                        /* Once the AVPlayerItem becomes ready to play, i.e.
                         [playerItem status] == AVPlayerItemStatusReadyToPlay,
                         its duration can be fetched from the item. */

                        this.AddTimeObserverToPlayer();
                    }
                    else if (playerItem.Status == AVPlayerItemStatus.Failed)
                    {
                        this.ReportError(this.playerItem.Error);
                    }
                }
            }
            else
            {
                base.ObserveValue(keyPath, ofObject, change, context);
            }
        }

        private void UpdatePlayPauseButton()
        {
            var style = this.isPlaying ? UIBarButtonSystemItem.Pause : UIBarButtonSystemItem.Play;
            using (var newPlayPauseButton = new UIBarButtonItem(style, (sender, e) => this.TogglePlayPause(sender as UIBarButtonItem)))
            {
                var items = this.toolbar.Items;
                items[0] = newPlayPauseButton;
                this.toolbar.SetItems(items, false);

                this.playPauseButton = newPlayPauseButton;
            }
        }

        private void UpdateTimeLabel()
        {
            var seconds = this.player != null ? this.player.CurrentTime.Seconds : 0d;
            if (double.IsNaN(seconds))
            {
                seconds = 0;
            }

            this.currentTimeLabel.TextColor = UIColor.White;
            this.currentTimeLabel.TextAlignment = UITextAlignment.Center;
            this.currentTimeLabel.Text = TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");
        }

        private void UpdateScrubber()
        {
            var duration = this.GetPlayerItemDuration().Seconds;
            if (!double.IsNaN(duration))
            {
                var time = this.player.CurrentTime.Seconds;
                this.scrubber.Value = (float)(time / duration);
            }
            else
            {
                this.scrubber.Value = 0f;
            }
        }

        private void UpdateProgress(AVAssetExportSession session)
        {
            if (session?.Status == AVAssetExportSessionStatus.Exporting)
            {
                this.exportProgressView.Progress = session.Progress;
            }
        }

        private void ReportError(NSError error)
        {
            if (error != null)
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    var alertView = UIAlertController.Create(error.LocalizedDescription, error.LocalizedRecoverySuggestion, UIAlertControllerStyle.Alert);
                    alertView.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    base.PresentViewController(alertView, true, null);
                });
            }
        }

        #endregion

        #region IBActions

        partial void TogglePlayPause(UIBarButtonItem sender)
        {
            this.isPlaying = !this.isPlaying;
            if (this.isPlaying)
            {
                if (this.isSeekToZeroBeforePlaying)
                {
                    this.player.Seek(CMTime.Zero);
                    this.isSeekToZeroBeforePlaying = false;
                }

                this.player.Play();
            }
            else
            {
                this.player.Pause();
            }
        }

        partial void BeginScrubbing(UISlider sender)
        {
            this.isSeekToZeroBeforePlaying = false;
            this.playRateToRestore = this.player.Rate;
            this.player.Rate = 0f;

            this.RemoveTimeObserverFromPlayer();
        }

        partial void Scrub(UISlider sender)
        {
            this.lastScrubSliderValue = this.scrubber.Value;
            if (!this.isScrubInFlight)
            {
                this.ScrubToSliderValue(this.lastScrubSliderValue);
            }
        }

        private void ScrubToSliderValue(float sliderValue)
        {
            var duration = this.GetPlayerItemDuration().Seconds;
            if (!double.IsInfinity(duration))
            {
                var width = this.scrubber.Bounds.Width;

                double time = duration * sliderValue;
                double tolerance = 1d * duration / width;

                this.isScrubInFlight = true;

                this.player.Seek(CMTime.FromSeconds(time, NSEC_PER_SEC),
                                 CMTime.FromSeconds(tolerance, NSEC_PER_SEC),
                                 CMTime.FromSeconds(tolerance, NSEC_PER_SEC),
                                 (finished) =>
                                 {
                                     this.isScrubInFlight = false;
                                     this.UpdateTimeLabel();
                                 });
            }
        }

        partial void EndScrubbing(UISlider sender)
        {
            if (this.isScrubInFlight)
            {
                this.ScrubToSliderValue(this.lastScrubSliderValue);
            }

            this.AddTimeObserverToPlayer();

            this.player.Rate = this.playRateToRestore;
            this.playRateToRestore = 0f;
        }

        /// <summary>
        /// Called when the player item has played to its end time
        /// </summary>
        private void PlayerItemDidReachEnd(NSNotification obj)
        {
            // After the movie has played to its end time, seek back to time zero to play it again. 
            this.isSeekToZeroBeforePlaying = true;
        }

        partial void HandleTapGesture(UITapGestureRecognizer sender)
        {
            this.toolbar.Hidden = !this.toolbar.Hidden;
            this.currentTimeLabel.Hidden = !this.currentTimeLabel.Hidden;
        }

        partial void ExportToMovie(UIBarButtonItem sender)
        {
            this.exportProgressView.Hidden = false;

            this.player.Pause();
            this.playPauseButton.Enabled = false;
            this.transitionButton.Enabled = false;
            this.scrubber.Enabled = false;
            this.exportButton.Enabled = false;

            this.editor.BuildCompositionObjectsForPlayback(false);

            // Get the export session from the editor
            var session = this.editor.AssetExportSessionWithPreset(AVAssetExportSession.PresetMediumQuality);

            var filePath = Path.Combine(Path.GetTempPath(), "ExportedProject.mov");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // If a preset that is not compatible with AVFileTypeQuickTimeMovie is used, one can use -[AVAssetExportSession supportedFileTypes] to obtain a supported file type for the output file and UTTypeCreatePreferredIdentifierForTag to obtain an appropriate path extension for the output file type.
            session.OutputUrl = NSUrl.FromFilename(filePath);
            session.OutputFileType = AVFileType.QuickTimeMovie;

            session.ExportAsynchronously(() => DispatchQueue.MainQueue.DispatchAsync(() => OnExportCompleted(session)));

            // Update progress view with export progress
            this.progressTimer = NSTimer.CreateRepeatingTimer(0.5, d => this.UpdateProgress(session));
            NSRunLoop.Current.AddTimer(this.progressTimer, NSRunLoopMode.Default);
        }

        private void OnExportCompleted(AVAssetExportSession session)
        {
            this.exportProgressView.Hidden = true;
            this.currentTimeLabel.Hidden = false;
            var outputURL = session.OutputUrl;

            this.progressTimer.Invalidate();
            this.progressTimer.Dispose();
            this.progressTimer = null;

            if (session.Status != AVAssetExportSessionStatus.Completed)
            {
                Console.WriteLine($"exportSession error:{session.Error}");
                this.ReportError(session.Error);
            }
            else
            {
                this.exportProgressView.Progress = 1f;

                // Save the exported movie to the camera roll
                PHPhotoLibrary.RequestAuthorization((status) =>
                {
                    if(status == PHAuthorizationStatus.Authorized)
                    {
                        PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() => PHAssetChangeRequest.FromVideo(outputURL), 
                        (successfully, error) =>
                        {
                            if (error != null)
                            {
                                Console.WriteLine($"writeVideoToAssestsLibrary failed: {error}");
                                this.ReportError(error);
                            }

                            base.InvokeOnMainThread(() =>
                            {
                                this.playPauseButton.Enabled = true;
                                this.transitionButton.Enabled = true;
                                this.scrubber.Enabled = true;
                                this.exportButton.Enabled = true;
                            });
                        });
                    }
                });
            }
        }

        #endregion

        #region Gesture recognizer delegate

        [Export("gestureRecognizer:shouldReceiveTouch:")]
        public bool ShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch)
        {
            return touch.View == this.playerView;
        }

        #endregion

        public void DidPickTransitionType(TransitionType transitionType)
        {
            this.transitionType = transitionType;

            // Let the editor know of the change in transition type.
            this.SynchronizeWithEditor();

            this.DismissViewController(true, null);
        }
    }
}