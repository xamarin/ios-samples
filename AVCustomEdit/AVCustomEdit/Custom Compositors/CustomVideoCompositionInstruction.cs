using System;
using AVFoundation;
using CoreMedia;
using Foundation;


namespace AVCustomEdit
{
	public class CustomVideoCompositionInstruction : AVVideoCompositionInstruction
	{
		public int ForegroundTrackID;
		public int BackgroundTrackID;

		int passthroughTrackID;
		NSNumber [] requiredSourceTrackIDs;
		CMTimeRange timeRange;
		bool enablePostProcessing;
		bool containsTweening;

		public override int PassthroughTrackID {
			get {
				return passthroughTrackID;
			}
		}

		public override NSNumber [] RequiredSourceTrackIDs {
			get {
				return requiredSourceTrackIDs;
			}
		}

		public override CMTimeRange TimeRange {
			get {
				return timeRange;
			}
		}
		public override bool EnablePostProcessing {
			get {
				return enablePostProcessing;
			}
		}
		public override bool ContainsTweening {
			get {
				return containsTweening;
			}
		}

		public CustomVideoCompositionInstruction () : base ()
		{

		}

		public CustomVideoCompositionInstruction (int passthroughTrackID, CMTimeRange timeRange) : base()
		{
			this.passthroughTrackID = passthroughTrackID;
			requiredSourceTrackIDs = null;
			this.timeRange = timeRange;
			containsTweening = false;
			enablePostProcessing = false;
		}

		public CustomVideoCompositionInstruction(NSNumber [] sourceTracksIDS, CMTimeRange timeRange) : base()
		{
			requiredSourceTrackIDs = sourceTracksIDS;
			passthroughTrackID = 0;
			this.timeRange = timeRange;
			containsTweening = true;
			enablePostProcessing = false;
		}
	}
}

