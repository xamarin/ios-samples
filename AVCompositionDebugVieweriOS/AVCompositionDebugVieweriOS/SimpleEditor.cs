using System;
using System.Collections.Generic;

using Foundation;
using CoreMedia;
using AVFoundation;
using CoreGraphics;

namespace AVCompositionDebugVieweriOS
{
	public class SimpleEditor : NSObject
	{
		public List<AVAsset> Clips { get; set; }
		public List<NSValue> ClipTimeRanges { get; set; }

		public int TransitionType { get; set; }
		public CMTime TransitionDuration { get; set; }
		public AVMutableComposition Composition { get; set; }
		public AVMutableVideoComposition VideoComposition { get; set; }
		public AVMutableAudioMix AudioMix { get; set; }

		public AVPlayerItem PlayerItem
		{
			get {
				AVPlayerItem playerItem = AVPlayerItem.FromAsset (Composition);
 				playerItem.VideoComposition = VideoComposition;
				playerItem.AudioMix = AudioMix;
		
				return playerItem;
			}
		}

		public SimpleEditor () : base()
		{
		}

		private void BuildTransitionComposition(AVMutableComposition composition, AVMutableVideoComposition videoComposition, AVMutableAudioMix audioMix)
		{
			CMTime nextClipStartTime = CMTime.Zero;
			int clipsCount = Clips.Count;

			// Make transitionDuration no greater than half the shortest clip duration.
			CMTime transitionDuration = TransitionDuration;
			Console.WriteLine ("Clips Count:" + clipsCount);
			Console.WriteLine ("Clips Range Count:" + ClipTimeRanges.Count);

			for (int i = 0; i < clipsCount; i++) {
				NSValue clipTimeRange = ClipTimeRanges [i];
				if(clipTimeRange != null) {
					CMTime halfClipDuration = clipTimeRange.CMTimeRangeValue.Duration;
					halfClipDuration.TimeScale *= 2;
					transitionDuration = CMTime.GetMinimum(transitionDuration,halfClipDuration);
				}
			}
		
			// Add two video tracks and two audio tracks.
			var compositionVideoTracks = new AVMutableCompositionTrack [] {
				composition.AddMutableTrack (AVMediaType.Video, 0),
				composition.AddMutableTrack (AVMediaType.Video, 0)
			};
			var compositionAudioTracks = new AVMutableCompositionTrack [] {
				composition.AddMutableTrack (AVMediaType.Audio, 0),
				composition.AddMutableTrack (AVMediaType.Audio, 0)
			};

			var passThroughTimeRanges = new CMTimeRange[clipsCount];
			var transitionTimeRanges = new CMTimeRange[clipsCount];

			// Place clips into alternating video & audio tracks in composition, overlapped by transitionDuration.
			for(int i = 0; i < clipsCount; i++) {
				int alternatingIndex = i % 2;
				AVAsset asset = Clips [i];
				NSValue clipTimeRange = ClipTimeRanges [i];
				CMTimeRange timeRangeInAsset;
				if (clipTimeRange != null)
					timeRangeInAsset = clipTimeRange.CMTimeRangeValue;
				else {
					timeRangeInAsset = new CMTimeRange ();
					timeRangeInAsset.Start = CMTime.Zero;
					timeRangeInAsset.Duration = asset.Duration;
				}
				NSError error;
				AVAssetTrack clipVideoTrack = asset.TracksWithMediaType (AVMediaType.Video) [0];
				compositionVideoTracks [alternatingIndex].InsertTimeRange (timeRangeInAsset, clipVideoTrack, nextClipStartTime,out error);

				AVAssetTrack clipAudioTrack = asset.TracksWithMediaType (AVMediaType.Audio) [0];
				compositionAudioTracks [alternatingIndex].InsertTimeRange (timeRangeInAsset, clipAudioTrack, nextClipStartTime,out error);

				// Remember the time range in which this clip should pass through.
				// First clip ends with a transition.
				// Second clip begins with a transition.
				// Exclude that transition from the pass through time ranges
				CMTimeRange timeRange = new CMTimeRange();
				timeRange.Start = nextClipStartTime;
				timeRange.Duration = timeRangeInAsset.Duration;
				passThroughTimeRanges [i] = timeRange;

				if (i > 0) 
				{
					passThroughTimeRanges[i].Start = CMTime.Add(passThroughTimeRanges[i].Start,transitionDuration);
					passThroughTimeRanges[i].Duration = CMTime.Subtract(passThroughTimeRanges[i].Duration,transitionDuration);
				}

				if(i + 1 < clipsCount)
				{
					passThroughTimeRanges[i].Duration = CMTime.Subtract(passThroughTimeRanges[i].Duration,transitionDuration);
				}

				// The end of this clip will overlap the start of the next by transitionDuration.
				// (Note: this arithmetic falls apart if timeRangeInAsset.duration < 2 * transitionDuration.)
				nextClipStartTime = CMTime.Add (nextClipStartTime, timeRangeInAsset.Duration);
				nextClipStartTime = CMTime.Subtract (nextClipStartTime, transitionDuration);
			
				// Remember the time range for the transition to the next item
				if(i + 1 < clipsCount)
				{
					transitionTimeRanges [i] = new CMTimeRange () 
					{
						Start  = nextClipStartTime,
						Duration = transitionDuration
					};
			
				}
			}

			List<AVVideoCompositionInstruction> instructions = new List<AVVideoCompositionInstruction> ();
			List<AVMutableAudioMixInputParameters> trackMixArray = new List<AVMutableAudioMixInputParameters> ();

			// Set up the video composition if we are to perform crossfade transitions between clips.
			for (int i = 0; i < clipsCount; i++) 
			{
				int alternatingIndex = i % 2;
				AVMutableVideoCompositionInstruction passThroughInstructions = AVMutableVideoCompositionInstruction.Create () as AVMutableVideoCompositionInstruction;
				passThroughInstructions.TimeRange = passThroughTimeRanges [i];

				AVMutableVideoCompositionLayerInstruction passThroughLayerInstructions = AVMutableVideoCompositionLayerInstruction.FromAssetTrack (compositionVideoTracks [alternatingIndex]);

				passThroughInstructions.LayerInstructions = new AVVideoCompositionLayerInstruction[] { passThroughLayerInstructions };
				instructions.Add (passThroughInstructions);

				if (i + 1 < clipsCount) 
				{
					var transitionInstruction = AVMutableVideoCompositionInstruction.Create () as AVMutableVideoCompositionInstruction;
					transitionInstruction.TimeRange = transitionTimeRanges [i];
					var fromLayer = AVMutableVideoCompositionLayerInstruction.FromAssetTrack (compositionVideoTracks [alternatingIndex]);
					var toLayer = AVMutableVideoCompositionLayerInstruction.FromAssetTrack (compositionVideoTracks [1 - alternatingIndex]);


					// Fade in the toLayer by setting a ramp from 0.0 to 1.0.
					toLayer.SetOpacityRamp (0.0f, 1.0f, transitionTimeRanges [i]);
					transitionInstruction.LayerInstructions = new AVVideoCompositionLayerInstruction[] 
					{
						toLayer,
						fromLayer,
					};
					instructions.Add(transitionInstruction); 

					// Add AudioMix to fade in the volume ramps
					var trackMix = AVMutableAudioMixInputParameters.FromTrack(compositionAudioTracks[0]);
					trackMix.SetVolumeRamp (1f, 0f, transitionTimeRanges[0]);
					trackMixArray.Add (trackMix);

					trackMix = AVMutableAudioMixInputParameters.FromTrack (compositionAudioTracks[1]);
					trackMix.SetVolumeRamp (0f, 1f, transitionTimeRanges[0]);
					trackMix.SetVolumeRamp (1f, 1f, passThroughTimeRanges[1]);
					trackMixArray.Add (trackMix);
				}
			}

			videoComposition.Instructions = instructions.ToArray ();
			audioMix.InputParameters = trackMixArray.ToArray();
		}

		public void BuildCompositionObjects(Boolean playBack)
		{
			if (Clips == null || Clips.Count == 0) 
			{
				Composition = null;
				VideoComposition = null;
				AudioMix = null;
				return;
			}

			CGSize videoSize = Clips [0].NaturalSize;
			var composition1 = AVMutableComposition.Create ();
			var videoComposition1 = AVMutableVideoComposition.Create ();
			var audioMix = AVMutableAudioMix.Create ();

			composition1.NaturalSize = videoSize;

			BuildTransitionComposition (composition1, videoComposition1, audioMix);
			if (videoComposition1 != null) 
			{
				videoComposition1.FrameDuration = new CMTime (1, 30);
				videoComposition1.RenderSize = videoSize;
			}

			Composition = composition1;
			VideoComposition = videoComposition1;
			AudioMix = audioMix;
		}

		public AVAssetExportSession AssetExportSession (string presetName)
		{
			var session = new AVAssetExportSession (Composition, presetName);
			session.VideoComposition = VideoComposition;
			session.AudioMix = AudioMix;
			return session;
		}
	}

}

