using System;
using System.Collections.Generic;
using System.IO;
using AssetsLibrary;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using CoreMedia;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace AVCustomEdit
{
    public partial class ViewController : UIViewController, IUIGestureRecognizerDelegate
    {
        private readonly NSString StatusObservationContext = new NSString("AVCustomEditPlayerViewControllerStatusObservationContext");
        private readonly NSString RateObservationContext = new NSString("AVCustomEditPlayerViewControllerRateObservationContext");

        private SimpleEditor editor;

        private List<AVAsset> clips;
        private List<NSValue> clipTimeRanges;

        private AVPlayer player;
        private AVPlayerItem playerItem;

        //private UIPopoverController popover;

        /*****/

        private bool playing;
        private bool scrubInFlight;
        private bool seekToZeroBeforePlaying;
        private float lastScrubSliderValue;
        private float playRateToRestore;
        private NSObject timeObserver;

        private float transitionDuration;
        private TransitionType transitionType;
        private bool transitionsEnabled;

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
            this.transitionsEnabled = true;

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
                seekToZeroBeforePlaying = false;
                this.player = new AVPlayer();
                this.player.AddObserver(this, "rate", NSKeyValueObservingOptions.Old | NSKeyValueObservingOptions.New, RateObservationContext.Handle);

                this.playerView.Player = this.player;
            }

            this.AddTimeObserverToPlayer();

            // Build AVComposition and AVVideoComposition objects for playback
            //this.SetupEditingAndPlayback();
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
                //APLTransitionTypeController* transitionTypePickerController = (APLTransitionTypeController*)((UINavigationController*)segue.destinationViewController).topViewController;
                //if ([[UIDevice currentDevice] userInterfaceIdiom] == UIUserInterfaceIdiomPad) {
                //    self.popover = [(UIStoryboardPopoverSegue*)segue popoverController];
                //}
                //transitionTypePickerController.delegate = self;
                //transitionTypePickerController.currentTransition = _transitionType;
                //if (_transitionType == kCrossDissolveTransition)
                //{
                //    // Make sure the view is loaded first
                //    if (!transitionTypePickerController.crossDissolveCell)
                //        [transitionTypePickerController loadView];
                //    [transitionTypePickerController.crossDissolveCell setAccessoryType:UITableViewCellAccessoryCheckmark];
                //} else {
                //    // Make sure the view is loaded first
                //    if (!transitionTypePickerController.diagonalWipeCell)
                //        [transitionTypePickerController loadView];
                //    [transitionTypePickerController.diagonalWipeCell setAccessoryType:UITableViewCellAccessoryCheckmark];
                //}
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
            InvokeOnMainThread(() => this.SynchronizeWithEditor());
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
                if(this.clips.Count != this.clipTimeRanges.Count)
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
                        //NSNotificationCenter.DefaultCenter.RemoveObserver(this, AVPlayerItem.DidPlayToEndTimeNotification, this.playerItem);
                    }

                    this.playerItem = playerItem;
                    if (this.playerItem != null)
                    {
                        //this.playerItem.SeekingWaitsForVideoCompositionRendering = true;

                        // Observe the player item "status" key to determine when it is ready to play
                        this.playerItem.AddObserver(this, "status", NSKeyValueObservingOptions.New | NSKeyValueObservingOptions.Initial, StatusObservationContext.Handle);

                        // When the player item has played to its end time we'll set a flag
                        // so that the next time the play method is issued the player will
                        // be reset to time zero first.
                        //NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, playerItemDidReachEnd);
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
            if (transitionsEnabled)
            {
                this.editor.TransitionDuration = CMTime.FromSeconds(transitionDuration, 600);
                this.editor.TransitionType = transitionType;
            }
            else
            {
                this.editor.TransitionDuration = CMTime.Invalid;
            }

            // Build AVComposition and AVVideoComposition objects for playback
            this.editor.BuildCompositionObjectsForPlayback(true);

            this.SynchronizePlayerWithEditor();
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
            if (timeObserver != null)
            {
                this.player.RemoveTimeObserver(timeObserver);
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

            /* Will be kCMTimeInvalid if the item is not ready to play. */
            return itemDuration;
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            if (context == RateObservationContext.Handle)
            {
                var ch = new NSObservedChange(change);
                var newValue = ch.NewValue as NSNumber;
                if ((ch.OldValue == null && newValue != null) ||
                    (ch.OldValue is NSNumber oldRate && newValue != null && oldRate.FloatValue != newValue.FloatValue))
                {
                    playing = (newValue.FloatValue != 0f) || (playRateToRestore != 0f);
                    this.UpdatePlayPauseButton();
                    this.UpdateScrubber();
                    this.UpdateTimeLabel();
                }
            }
            else if (context == StatusObservationContext.Handle)
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
            var style = playing ? UIBarButtonSystemItem.Pause : UIBarButtonSystemItem.Play;
            var newPlayPauseButton = new UIBarButtonItem(style, (sender, e) => togglePlayPause(sender as UIBarButtonItem));

            var items = toolbar.Items;
            items[0] = newPlayPauseButton;
            toolbar.SetItems(items, false);

            this.playPauseButton = newPlayPauseButton;
        }

        private void UpdateTimeLabel()
        {
            var seconds = this.player != null ? this.player.CurrentTime.Seconds : 0d;
            if (double.IsInfinity(seconds))
            {
                seconds = 0;
            }

            int secondsInt = (int)Math.Round(seconds);
            int minutes = secondsInt / 60;
            secondsInt -= minutes * 60;

            this.currentTimeLabel.TextColor = UIColor.White;
            this.currentTimeLabel.TextAlignment = UITextAlignment.Center;

            this.currentTimeLabel.Text = TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");
        }

        private void UpdateScrubber()
        {
            var duration = this.GetPlayerItemDuration().Seconds;
            if (!double.IsNaN(duration))
            {
                double time = this.player.CurrentTime.Seconds;
                this.scrubber.Value = (float)(time / duration);
            }
            else
            {
                this.scrubber.Value = 0f;
            }
        }

        private void UpdateProgress(NSTimer timer)
        {
            var session = timer.UserInfo as AVAssetExportSession;
            if (session.Status == AVAssetExportSessionStatus.Exporting)
            {
                exportProgressView.Progress = session.Progress;
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
                    PresentViewController(alertView, true, null);
                });
            }
        }

        #endregion

        #region IBActions

        partial void togglePlayPause(UIBarButtonItem sender)
        {
            this.playing = !this.playing;
            if (this.playing)
            {
                if (this.seekToZeroBeforePlaying)
                {
                    this.player.Seek(CMTime.Zero);
                    this.seekToZeroBeforePlaying = false;
                }

                this.player.Play();
            }
            else
            {
                this.player.Pause();
            }
        }

        partial void beginScrubbing(UISlider sender)
        {
            seekToZeroBeforePlaying = false;
            playRateToRestore = this.player.Rate;
            this.player.Rate = 0f;

            this.RemoveTimeObserverFromPlayer();
        }

        partial void scrub(UISlider sender)
        {
            lastScrubSliderValue = this.scrubber.Value;

            if (!scrubInFlight)
            {
                this.ScrubToSliderValue(lastScrubSliderValue);
            }
        }

        private void ScrubToSliderValue(float sliderValue)
        {
            var duration = this.GetPlayerItemDuration().Seconds;
            if (double.IsInfinity(duration))
            {
                var width = this.scrubber.Bounds.Width;

                double time = duration * sliderValue;
                double tolerance = 1.0f * duration / width;

                scrubInFlight = true;

                this.player.Seek(CMTime.FromSeconds(time, NSEC_PER_SEC),
                                 CMTime.FromSeconds(tolerance, NSEC_PER_SEC),
                                 CMTime.FromSeconds(tolerance, NSEC_PER_SEC),
                                 (finished) =>
                                 {
                                     scrubInFlight = false;
                                     this.UpdateTimeLabel();
                                 });
            }
        }

        partial void endScrubbing(UISlider sender)
        {
            if (scrubInFlight)
            {
                this.ScrubToSliderValue(lastScrubSliderValue);
            }

            this.AddTimeObserverToPlayer();

            this.player.Rate = playRateToRestore;
            playRateToRestore = 0f;
        }

        /// <summary>
        /// Called when the player item has played to its end time
        /// </summary>
        private void PlayerItemDidReachEnd(NSNotification obj)
        {
            /* After the movie has played to its end time, seek back to time zero to play it again. */
            seekToZeroBeforePlaying = true;
        }

        partial void handleTapGesture(UITapGestureRecognizer sender)
        {
            this.toolbar.Hidden = !this.toolbar.Hidden;
            this.currentTimeLabel.Hidden = !this.currentTimeLabel.Hidden;
        }

        partial void exportToMovie(UIBarButtonItem sender)
        {
            exportProgressView.Hidden = false;

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
                File.Delete(filePath);

            // If a preset that is not compatible with AVFileTypeQuickTimeMovie is used, one can use -[AVAssetExportSession supportedFileTypes] to obtain a supported file type for the output file and UTTypeCreatePreferredIdentifierForTag to obtain an appropriate path extension for the output file type.
            session.OutputUrl = NSUrl.FromFilename(filePath);
            session.OutputFileType = AVFileType.QuickTimeMovie;

            session.ExportAsynchronously(() => DispatchQueue.MainQueue.DispatchAsync(() => exportCompleted(session)));

            // Update progress view with export progress
            progressTimer = NSTimer.CreateRepeatingTimer(0.5, d => UpdateProgress(progressTimer));
            NSRunLoop.Current.AddTimer(progressTimer, NSRunLoopMode.Default);
        }

        private void exportCompleted(AVAssetExportSession session)
        {
            exportProgressView.Hidden = true;
            currentTimeLabel.Hidden = false;
            var outputURL = session.OutputUrl;

            progressTimer.Invalidate();
            progressTimer.Dispose();
            progressTimer = null;

            if (session.Status != AVAssetExportSessionStatus.Completed)
            {
                Console.WriteLine($"exportSession error:{session.Error}");
                this.ReportError(session.Error);
            }

            if (session.Status != AVAssetExportSessionStatus.Completed)
            {
                return;
            }

            exportProgressView.Progress = 1f;

            // Save the exported movie to the camera roll
            var library = new ALAssetsLibrary();
            library.WriteVideoToSavedPhotosAlbum(outputURL, (assetURL, error) =>
            {
                if (error != null)
                {
                    Console.WriteLine($"writeVideoToAssestsLibrary failed: {error}");
                    this.ReportError(error);
                }
            });

            this.player.Play();
            this.playPauseButton.Enabled = true;
            this.transitionButton.Enabled = true;
            this.scrubber.Enabled = true;
            this.exportButton.Enabled = true;
        }

        #endregion

        #region Gesture recognizer delegate

        [Export("gestureRecognizer:shouldReceiveTouch:")]
        public bool ShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch)
        {
            return touch.View == this.playerView;
        }

        #endregion
    }
}