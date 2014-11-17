using System;
using System.Collections.Generic;

using Foundation;
using CoreMedia;
using AVFoundation;

namespace AVCustomEdit
{
	public class SimpleEditor : NSObject
	{
		public List<AVAsset> Clips;
		public List<NSValue> ClipTimeRanges;

		public int TransitionType;
		public CMTime TransitionDuration;
		AVMutableComposition Composition;
		AVMutableVideoComposition VideoComposition;

		public SimpleEditor () : base()
		{
		}

		void buildTransitionComposition(AVMutableComposition composition, AVMutableVideoComposition videoComposition)
		{
			CMTime nextClipStartTime = CMTime.Zero;
			int clipsCount = Clips.Count;

			// Make transitionDuration no greater than half the shortest clip duration.
			CMTime transitionDuration = TransitionDuration;

			foreach (var clipTimeRange in ClipTimeRanges) {
				if (clipTimeRange == null)
					continue;

				CMTime halfClipDuration = clipTimeRange.CMTimeRangeValue.Duration;
				halfClipDuration.TimeScale *= 2;
				transitionDuration = CMTime.GetMinimum(transitionDuration,halfClipDuration);
			}

			// Add two video tracks and two audio tracks.
			var compositionVideoTracks = new AVMutableCompositionTrack [2];
			var compositionAudioTracks = new AVMutableCompositionTrack [2];

			compositionVideoTracks [0] = composition.AddMutableTrack (AVMediaType.Video, 0);
			compositionVideoTracks [1] = composition.AddMutableTrack (AVMediaType.Video, 0);
			compositionAudioTracks [0] = composition.AddMutableTrack (AVMediaType.Audio, 0);
			compositionAudioTracks [1] = composition.AddMutableTrack (AVMediaType.Audio, 0);

			var passThroughTimeRanges = new CMTimeRange[clipsCount];
			var transitionTimeRanges = new CMTimeRange[clipsCount];

			// Place clips into alternating video & audio tracks in composition, overlapped by transitionDuration.
			for(int i = 0; i < clipsCount; i++)
			{
				int alternatingIndex = i % 2;
				AVAsset asset = Clips [i];
				NSValue clipTimeRange = ClipTimeRanges [i];
				CMTimeRange timeRangeInAsset;
				if (clipTimeRange != null)
					timeRangeInAsset = clipTimeRange.CMTimeRangeValue;
				else
				{
					timeRangeInAsset = new CMTimeRange {
						Start = CMTime.Zero,
						Duration = asset.Duration
					};
				}
				NSError error = new NSError ();
				AVAssetTrack clipVideoTrack = asset.TracksWithMediaType (AVMediaType.Video) [0];
				compositionVideoTracks [alternatingIndex].InsertTimeRange (timeRangeInAsset, clipVideoTrack, nextClipStartTime, out error);

				AVAssetTrack clipAudioTrack = asset.TracksWithMediaType (AVMediaType.Audio) [0];
				compositionAudioTracks [alternatingIndex].InsertTimeRange (timeRangeInAsset, clipAudioTrack, nextClipStartTime, out error);

				// Remember the time range in which this clip should pass through.
				// First clip ends with a transition.
				// Second clip begins with a transition.
				// Exclude that transition from the pass through time ranges
				passThroughTimeRanges [i] = new CMTimeRange {
					Start = nextClipStartTime,
					Duration = timeRangeInAsset.Duration
				};

				if (i > 0) {
					passThroughTimeRanges[i].Start = CMTime.Add(passThroughTimeRanges[i].Start, transitionDuration);
					passThroughTimeRanges[i].Duration = CMTime.Subtract(passThroughTimeRanges[i].Duration, transitionDuration);
				}
				if( i + 1 < clipsCount)
					passThroughTimeRanges[i].Duration = CMTime.Subtract(passThroughTimeRanges[i].Duration,transitionDuration);

				// The end of this clip will overlap the start of the next by transitionDuration.
				// (Note: this arithmetic falls apart if timeRangeInAsset.duration < 2 * transitionDuration.)
				nextClipStartTime = CMTime.Add (nextClipStartTime, timeRangeInAsset.Duration);
				nextClipStartTime = CMTime.Subtract (nextClipStartTime, transitionDuration);
			
				// Remember the time range for the transition to the next item.

				if(i + 1 < clipsCount)
				{
					transitionTimeRanges [i] = new CMTimeRange () {
						Start  = nextClipStartTime,
						Duration = transitionDuration
					};
			
				}
			}

			// Set up the video composition to perform cross dissolve or diagonal wipe transitions between clips.
			var instructions = new List<AVVideoCompositionInstruction> ();

			// Cycle between "pass through A", "transition from A to B", "pass through B"
			for(int i = 0;  i < clipsCount; i++)
			{
				int alternatingIndex = i % 2;

//				if (videoComposition.CustomVideoCompositorClass != null) {
//					var videoInstruction = new CustomVideoCompositionInstruction (compositionVideoTracks [alternatingIndex].TrackID, passThroughTimeRanges [i]);
//					instructions.Add (videoInstruction);
//				} else {
//					// Pass through clip i.
//					var passThroughInstruction = AVMutableVideoCompositionInstruction.Create () as AVMutableVideoCompositionInstruction;
//					passThroughInstruction.TimeRange = passThroughTimeRanges [i];
//					var passThroughLayer = AVMutableVideoCompositionLayerInstruction.FromAssetTrack (compositionVideoTracks [alternatingIndex]);
//					passThroughInstruction.LayerInstructions = new [] { passThroughLayer };
//					instructions.Add (passThroughInstruction);
//
//				}
				//TODO: Remove following call if previous works
 				if (videoComposition.CustomVideoCompositorClass.Name != "nil") {
					var videoInstruction = new CustomVideoCompositionInstruction (compositionVideoTracks [alternatingIndex].TrackID, passThroughTimeRanges [i]);
					instructions.Add (videoInstruction);

				} 
				else {
					// Pass through clip i.
					var passThroughInstruction = AVMutableVideoCompositionInstruction.Create () as AVMutableVideoCompositionInstruction;
					passThroughInstruction.TimeRange = passThroughTimeRanges [i];
					var passThroughLayer = AVMutableVideoCompositionLayerInstruction.FromAssetTrack (compositionVideoTracks [alternatingIndex]);
					passThroughInstruction.LayerInstructions = new [] { passThroughLayer };
					instructions.Add (passThroughInstruction);

				}

				if (i + 1 < clipsCount) {
					// Add transition from clip i to clip i+1.
//					if (videoComposition.CustomVideoCompositorClass != null) {
//						var videoInstruction = new CustomVideoCompositionInstruction (new NSNumber [] {
//							compositionVideoTracks [0].TrackID,
//							compositionVideoTracks [1].TrackID
//						}, transitionTimeRanges [1]);
//
//						if (alternatingIndex == 0) {
//							videoInstruction.ForegroundTrackID = compositionVideoTracks [alternatingIndex].TrackID;
//							videoInstruction.BackgroundTrackID = compositionVideoTracks [1 - alternatingIndex].TrackID;
//						}
//
//						instructions.Add (videoInstruction);
//					} else {
//						var transitionInstruction = AVMutableVideoCompositionInstruction.Create () as AVMutableVideoCompositionInstruction;
//						transitionInstruction.TimeRange = transitionTimeRanges [i];
//						var fromLayer = AVMutableVideoCompositionLayerInstruction.FromAssetTrack (compositionVideoTracks [alternatingIndex]);
//						var toLayer = AVMutableVideoCompositionLayerInstruction.FromAssetTrack (compositionVideoTracks [1 - alternatingIndex]);
//						transitionInstruction.LayerInstructions = new [] { toLayer, fromLayer };
//						instructions.Add (transitionInstruction);
//					}
					// TODO: remove following call if previous works
					if (videoComposition.CustomVideoCompositorClass.Name != "nil") {
						NSNumber[] sources = new NSNumber[] {
							new NSNumber (compositionVideoTracks [0].TrackID),
							new NSNumber (compositionVideoTracks [1].TrackID)
						};
						var videoInstructions = new CustomVideoCompositionInstruction (sources, transitionTimeRanges [i]);
						if (alternatingIndex == 0) {
							videoInstructions.ForegroundTrackID = compositionVideoTracks [alternatingIndex].TrackID;
							videoInstructions.BackgroundTrackID = compositionVideoTracks [1 - alternatingIndex].TrackID;
						}

						instructions.Add (videoInstructions);
						Console.WriteLine ("Add transition from clip i to clip i+1");
					} else {
						AVMutableVideoCompositionInstruction transitionInstruction = AVMutableVideoCompositionInstruction.Create () as AVMutableVideoCompositionInstruction;
						transitionInstruction.TimeRange = transitionTimeRanges [i];
						AVMutableVideoCompositionLayerInstruction fromLayer = AVMutableVideoCompositionLayerInstruction.FromAssetTrack (compositionVideoTracks [alternatingIndex]);
						AVMutableVideoCompositionLayerInstruction toLayer = AVMutableVideoCompositionLayerInstruction.FromAssetTrack (compositionVideoTracks [1 - alternatingIndex]);
						transitionInstruction.LayerInstructions = new AVVideoCompositionLayerInstruction[] {
							fromLayer,
							toLayer,
						};
						instructions.Add (transitionInstruction);
					}
				}
			}

			videoComposition.Instructions = instructions.ToArray ();
		}

		public void BuildCompositionObjects(bool playBack)
		{
			if (Clips == null || Clips.Count == 0) {
				Composition = null;
				VideoComposition = null;
				return;
			}

			var videoSize = Clips [0].NaturalSize;
			var composition = AVMutableComposition.Create ();
			AVMutableVideoComposition videoComposition = null;

			composition.NaturalSize = videoSize;

			// With transitions:
			// Place clips into alternating video & audio tracks in composition, overlapped by transitionDuration.
			// Set up the video composition to cycle between "pass through A", "transition from A to B",
			// "pass through B".

			videoComposition = AVMutableVideoComposition.Create ();


			//Set CustomVideoCompositorClass based on the Compositor user selected.

			if (TransitionType == TransitionTypeController.DiagonalWipeTransition) {
				videoComposition.CustomVideoCompositorClass = new ObjCRuntime.Class (typeof(DiagonalWipeCompositor));
			} else {
				videoComposition.CustomVideoCompositorClass = new ObjCRuntime.Class (typeof(CrossDissolveCompositor));
			}

			buildTransitionComposition (composition, videoComposition);
			if (videoComposition != null) {
				// Every videoComposition needs these properties to be set:
				videoComposition.FrameDuration = new CMTime (1, 30);
				videoComposition.RenderSize = videoSize;

			}
			Composition = composition;
			VideoComposition = videoComposition;
		}

		public AVAssetExportSession AssetExportSession (string presetName)
		{
			var session = new AVAssetExportSession (Composition, presetName);
			session.VideoComposition = VideoComposition;
			return session;
		}

		public AVPlayerItem PlayerItem {
			get {
				AVPlayerItem playerItem = AVPlayerItem.FromAsset (Composition);
				//TODO:Got an NullReferenceException here if this.videoComposition.CustomVideoCompositorClass is not a "Nil" Class
				playerItem.VideoComposition = VideoComposition;
				//If set the CustomVideoCompositorClass in playerItem.VideoComposition, will get a System.NotImplementedException
				//playerItem.VideoComposition.CustomVideoCompositorClass = new MonoTouch.ObjCRuntime.Class(typeof(CrossDissolveCompositor));
				return playerItem;
			}
		}
	}
}