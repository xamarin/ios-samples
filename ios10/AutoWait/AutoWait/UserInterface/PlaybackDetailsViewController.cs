using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;
using AVFoundation;
using CoreFoundation;
using CoreMedia;

namespace AutoWait
{
	public partial class PlaybackDetailsViewController : UIViewController
	{
		[Outlet ("rateLabel")]
		UILabel RateLabel { get; set; }

		[Outlet ("timeControlStatusLabel")]
		UILabel TimeControlStatusLabel { get; set; }

		[Outlet ("reasonForWaitingLabel")]
		UILabel ReasonForWaitingLabel { get; set; }

		[Outlet ("likelyToKeepUpLabel")]
		UILabel LikelyToKeepUpLabel { get; set; }

		[Outlet ("loadedTimeRangesLabel")]
		UILabel LoadedTimeRangesLabel { get; set; }

		[Outlet ("currentTimeLabel")]
		UILabel CurrentTimeLabel { get; set; }

		[Outlet ("playbackBufferFullLabel")]
		UILabel PlaybackBufferFullLabel { get; set; }

		[Outlet ("playbackBufferEmptyLabel")]
		UILabel PlaybackBufferEmptyLabel { get; set; }

		[Outlet ("timebaseRateLabel")]
		UILabel TimebaseRateLabel { get; set; }

		public AVPlayer Player { get; set; }

		// AVPlayerItem.CurrentTime and the AVPlayerItem.Timebase's rate are not KVO observable. We check their values regularly using this timer.
		DispatchSource.Timer nonObservablePropertiesUpdateTimer = new DispatchSource.Timer (DispatchQueue.MainQueue);

		IDisposable rateToken;
		IDisposable timeControlStatusToken;
		IDisposable reasonForWaitingToPlayToken;
		IDisposable playbackLikelyToKeepUpToken;
		IDisposable loadedTimeRangesToken;
		IDisposable playbackBufferFullToken;
		IDisposable playbackBufferEmptyToken;

		public PlaybackDetailsViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			nonObservablePropertiesUpdateTimer.SetEventHandler (UpdateNonObservableProperties);
			nonObservablePropertiesUpdateTimer.SetTimer (DispatchTime.Now, 1000000, 0);
			nonObservablePropertiesUpdateTimer.Resume ();

			var options = NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New;
			rateToken = Player.AddObserver ("rate", options, RateChanged);
			timeControlStatusToken = Player.AddObserver ("timeControlStatus", options, timeControlStatusChanged);
			reasonForWaitingToPlayToken = Player.AddObserver ("reasonForWaitingToPlay", options, ReasonForWaitingToPlayChanged);
			playbackLikelyToKeepUpToken = Player.AddObserver ("currentItem.playbackLikelyToKeepUp", options, PlaybackLikelyToKeepUpChanged);
			loadedTimeRangesToken = Player.AddObserver ("currentItem.loadedTimeRanges", options, LoadedTimeRangesChanged);
			playbackBufferFullToken = Player.AddObserver ("currentItem.playbackBufferFull", options, PlaybackBufferFullChanged);
			playbackBufferEmptyToken = Player.AddObserver ("currentItem.playbackBufferEmpty", options, PlaybackBufferEmptyChanged);
		}

		protected override void Dispose (bool disposing)
		{
			rateToken?.Dispose ();
			timeControlStatusToken?.Dispose ();
			reasonForWaitingToPlayToken?.Dispose ();
			playbackLikelyToKeepUpToken?.Dispose ();
			loadedTimeRangesToken?.Dispose ();
			playbackBufferFullToken?.Dispose ();
			playbackBufferEmptyToken?.Dispose ();

			base.Dispose (disposing);
		}

		// Helper function to get a background color for the timeControlStatus label.
		UIColor LabelBackgroundColor (AVPlayerTimeControlStatus status)
		{
			switch (status) {
			case AVPlayerTimeControlStatus.Paused:
				return new UIColor (0.8196078538894653f, 0.2627451121807098f, 0.2823528945446014f, 1);

			case AVPlayerTimeControlStatus.Playing:
				return new UIColor (0.2881325483322144f, 0.6088829636573792f, 0.261575847864151f, 1);

			case AVPlayerTimeControlStatus.WaitingToPlayAtSpecifiedRate:
				return new UIColor (0.8679746985435486f, 0.4876297116279602f, 0.2578189671039581f, 1);

			default:
				throw new InvalidProgramException ();
			}
		}

		// Helper function to get an abbreviated description for the waiting reason.
		string AbbreviatedDescription (string reason)
		{
			if (reason == AVPlayer.WaitingToMinimizeStallsReason)
				return "Minimizing Stalls";

			if (reason == AVPlayer.WaitingWhileEvaluatingBufferingRateReason)
				return "Evaluating Buffering Rate";

			if (reason == AVPlayer.WaitingWithNoItemToPlayReason)
				return "No Item";

			return "UNKOWN";
		}

		void UpdateNonObservableProperties ()
		{
			CurrentTimeLabel.Text = Player.CurrentItem.CurrentTime.Description;
			TimebaseRateLabel.Text = Player.CurrentItem.Timebase?.Rate.ToString ();
		}

		void RateChanged (NSObservedChange obj)
		{
			RateLabel.Text = (Player != null) ? Player.Rate.ToString () : "-";
		}

		void timeControlStatusChanged (NSObservedChange obj)
		{
			TimeControlStatusLabel.Text = (Player != null) ? Player.TimeControlStatus.ToString () : "-";
			TimeControlStatusLabel.BackgroundColor = (Player != null)
				? LabelBackgroundColor (Player.TimeControlStatus)
				: new UIColor (1, 0.9999743700027466f, 0.9999912977218628f, 1);
		}

		void ReasonForWaitingToPlayChanged (NSObservedChange obj)
		{
			ReasonForWaitingLabel.Text = (Player != null) ? AbbreviatedDescription (Player.ReasonForWaitingToPlay) : "-";
		}

		void PlaybackLikelyToKeepUpChanged (NSObservedChange obj)
		{
			LikelyToKeepUpLabel.Text = (Player != null) ? Player.CurrentItem.PlaybackLikelyToKeepUp.ToString () : "-";
		}

		void LoadedTimeRangesChanged (NSObservedChange obj)
		{
			LoadedTimeRangesLabel.Text = (Player != null) ? Descr (TimeRanges(Player.CurrentItem.LoadedTimeRanges)) : "-";
		}

		void PlaybackBufferFullChanged (NSObservedChange obj)
		{
			PlaybackBufferFullLabel.Text = (Player != null) ? Player.CurrentItem.PlaybackBufferFull.ToString () : "-";
		}

		void PlaybackBufferEmptyChanged (NSObservedChange obj)
		{
			PlaybackBufferEmptyLabel.Text = (Player != null) ? Player.CurrentItem.PlaybackBufferEmpty.ToString () : "-";
		}

		IEnumerable<CMTimeRange> TimeRanges (NSValue [] values)
		{
			foreach (var v in values)
				yield return v.CMTimeRangeValue;
		}

		static string Descr (IEnumerable<CMTimeRange> ranges)
		{
			return string.Join (", ", ranges.Select (r => $"{r.Start}–{r.Start + r.Duration}"));
		}
	}
}
